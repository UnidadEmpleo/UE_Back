using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Middleware;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace API.Seguridad.Application.Core
{
    public class GraphManager(GraphServiceClient graphClient,
            IConfiguration configuration,
            ILogger<ExceptionMiddleware> logger) : IGraphManager
    {        
        #region User management
        public async Task<User> CreateUser(User newUser, CancellationToken cancellationToken)
        {
            if (newUser == null)
            {
                throw new ArgumentNullException(nameof(newUser), "User no puede ser null");
            }

            if (await UserExistsAsync(newUser.Id, cancellationToken))
            {
                throw new ArgumentException($"El usuario con ID {newUser.Id} ya existe.");
            } 

            return await graphClient
                .Users
                .PostAsync(newUser, cancellationToken: cancellationToken);
        }

        public async Task DeleteUser(string userId, CancellationToken cancellationToken)
        {
            await graphClient
                .Users[userId]
                .DeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<User?> GetUserById(string userId, CancellationToken cancellationToken)
        {
            if (await UserExistsAsync(userId, cancellationToken) == false)
            {
                return null;
            }

            return await graphClient
                .Users[userId]
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = ["id", "displayName", "givenName", "surname", "mobilePhone", "mail", "oDataType", "mailNickname", "userPrincipalName", "accountEnabled", "createdDateTime"];
                    requestConfiguration.QueryParameters.Expand = new[] { "extensions", "appRoleAssignments" };
                },
                cancellationToken: cancellationToken);
        }

        public async Task<List<User>> GetUsers(CancellationToken cancellationToken)
        {
            var users = await graphClient
                .Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = ["id", "displayName", "givenName", "surname", "mobilePhone", "mail", "oDataType", "mailNickname", "userPrincipalName", "accountEnabled", "createdDateTime"];
                    requestConfiguration.QueryParameters.Expand = new[] { "extensions" };
                },
                cancellationToken: cancellationToken);

            return users?.Value?.ToList() ?? new List<User>();
        }

        public async Task UpdateUser(string userId, User user, CancellationToken cancellationToken)
        {
            await graphClient
                .Users[userId]
                .PatchAsync(user, cancellationToken: cancellationToken);
        }

        public async Task UpdateUserGroups(string userId, string groupId, CancellationToken cancellationToken)
        {
            string actualGroup = await GetMemberOfIdAsync<Group>(userId, cancellationToken);

            if (!actualGroup.IsNullOrWhiteSpace())
            {
                if (!actualGroup.Equals(groupId))
                {
                    await RemoveUserFromGroup(userId, actualGroup, cancellationToken);
                    await AddUserToGroup(userId, groupId, cancellationToken);
                }
            }
            else
            {
                await AddUserToGroup(userId, groupId, cancellationToken);
            }
        }

        public async Task AddUserToGroup(string userId, string groupId, CancellationToken cancellationToken)
        {
            var groupMember = new ReferenceCreate
            {
                OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userId}"
            };

            await graphClient
                .Groups[groupId]
                .Members
                .Ref
                .PostAsync(groupMember, null, cancellationToken);
        }

        public async Task UpdateGroupUsers(List<string> userIds, string groupId, CancellationToken cancellationToken)
        {
            if (userIds == null || !userIds.Any())
            {
                throw new ArgumentException("La lista de userIds no puede estar vacía.", nameof(userIds));
            }

            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentException("El groupId no puede estar vacío.", nameof(groupId));
            }

            // Obtener los miembros actuales del grupo
            var currentMembers = await GetGroupMembersAsync(groupId, cancellationToken);
            var currentMemberIds = currentMembers.Select(member => member.Id).ToList();

            // Identificar usuarios a eliminar (presentes en el grupo pero no en la nueva lista)
            var usersToRemove = currentMemberIds.Except(userIds).ToList();

            // Identificar usuarios a agregar (presentes en la nueva lista pero no en el grupo)
            var usersToAdd = userIds.Except(currentMemberIds).ToList();

            // Eliminar usuarios del grupo
            foreach (var userId in usersToRemove)
            {
                await RemoveUserFromGroup(userId, groupId, cancellationToken);
            }

            // Agregar usuarios al grupo
            foreach (var userId in usersToAdd)
            {
                await AddUserToGroup(userId, groupId, cancellationToken);
            }
        }

        public async Task UpdateUserAppRole(string userId, string roleId, CancellationToken cancellationToken)
        {
            var actualRoles = await GetUserAppRolesAsync(userId, cancellationToken);

            if (actualRoles != null && actualRoles.Count > 0)
            {
                if ((bool)!actualRoles.FirstOrDefault()?.AppRoleId.ToString().Equals(roleId))
                {
                    await RemoveUserAppRoleAsync(userId, actualRoles.FirstOrDefault().Id, cancellationToken);
                    await AddUserToAppRole(userId, roleId, cancellationToken);
                }
            }
            else
            {
                await AddUserToAppRole(userId, roleId, cancellationToken);
            }
        }

        public async Task<List<AppRoleAssignment>> GetUserAppRolesAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve AppRoleAssignments for the user
                var appRoleAssignments = await graphClient
                    .Users[userId]
                    .AppRoleAssignments
                    .GetAsync(cancellationToken: cancellationToken);

                var appId = configuration[AzureAD.Configuration.ServicePrincipalId];

                // Filter assignments by the specified ApplicationId
                return appRoleAssignments?.Value?
                    .Where(assignment => assignment.ResourceId.ToString() == appId)
                    .ToList() ?? new List<AppRoleAssignment>();
            }
            catch (ODataError odataError)
            {
                logger.LogError(odataError, odataError.Message); 
                return new List<AppRoleAssignment>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving AppRoleAssignments for the user.", ex);
            }
        }

        public async Task RemoveUserAppRoleAsync(string userId, string appRoleAssignmentId, CancellationToken cancellationToken)
        {
            try
            {
                // Remove the AppRole assignment for the user
                await graphClient
                    .Users[userId]
                    .AppRoleAssignments[appRoleAssignmentId]
                    .DeleteAsync(cancellationToken: cancellationToken);

                Console.WriteLine($"Successfully removed AppRoleAssignment with ID {appRoleAssignmentId} for user {userId}.");
            }
            catch (ODataError odataError)
            {
                // Handle specific OData errors
                Console.WriteLine($"ODataError: {odataError.Error?.Message}");
                throw new Exception($"Failed to remove AppRoleAssignment with ID {appRoleAssignmentId} for user {userId}.", odataError);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new Exception($"An error occurred while removing AppRoleAssignment with ID {appRoleAssignmentId} for user {userId}.", ex);
            }
        }

        public async Task<OpenTypeExtension?> GetUserExtension(string userId, string extensionName, CancellationToken cancellationToken)
        {
            var existingExtensions = await graphClient
                .Users[userId]
                .Extensions
                .GetAsync(cancellationToken: cancellationToken);

            var existingExtension = existingExtensions?
                .Value?
                .OfType<OpenTypeExtension>()?
                .FirstOrDefault(e => e.ExtensionName == extensionName);

            return existingExtension;
        }

        public async Task UpdateUserExtension(string userId, OpenTypeExtension extension, CancellationToken cancellationToken)
        {
            if (extension == null || 
                userId.IsNullOrWhiteSpace() || 
                extension.ExtensionName.IsNullOrWhiteSpace())
            {
                return;
            }

            var existingExtension = await GetUserExtension(userId, extension.ExtensionName, cancellationToken);

            if (existingExtension == null)
            {
                await graphClient
                    .Users[userId]
                    .Extensions
                    .PostAsync(extension, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await graphClient
                    .Users[userId]
                    .Extensions[extension.ExtensionName]
                    .PatchAsync(extension, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task<DirectoryObjectCollectionResponse?> GetUserMemberOf(string userId,
            CancellationToken cancellationToken)
        {
            var memberOf = await graphClient
                .Users[userId]
                .MemberOf
                .GetAsync(cancellationToken: cancellationToken);

            return memberOf;
        }

        public async Task<string> GetMemberOfIdAsync<T>(string userId, CancellationToken cancellationToken) where T : DirectoryObject
        {
            var memberOf = await GetUserMemberOf(userId, cancellationToken)
                .ConfigureAwait(false);

            // Check if memberOf is null or empty
            if (memberOf == null || memberOf.Value == null || !memberOf.Value.Any())
            {
                return null;
            }

            var id = memberOf?
                    .Value?
                    .OfType<T>()
                    .FirstOrDefault()?.Id;

            return id;
        }
        public async Task RemoveUserFromGroup(string userId, string groupId, CancellationToken cancellationToken)
        {
            await graphClient
                .Groups[groupId]
                .Members[userId]
                .Ref
                .DeleteAsync(cancellationToken: cancellationToken);
        }

        private async Task RemoveUserFromDirectoryRole(string userId, string roleId, CancellationToken cancellationToken)
        {
            await graphClient
                .DirectoryRoles[roleId]
                .Members[userId]
                .Ref
                .DeleteAsync(cancellationToken: cancellationToken);
        }
        private async Task AddUserToDirectoryRole(string userId, string roleId, CancellationToken cancellationToken)
        {
            var directoryRoleMember = new ReferenceCreate
            {
                OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userId}"
            };

            await graphClient
                .DirectoryRoles[roleId]
                .Members
                .Ref
                .PostAsync(directoryRoleMember, null, cancellationToken);
        }

        private async Task AddUserToAppRole(string userId, string roleId, CancellationToken cancellationToken)
        {
            var appId = configuration[AzureAD.Configuration.ApplicationId];

            string servicePrincipalId = configuration[AzureAD.Configuration.ServicePrincipalId];

            // Agregar al usuario al AppRole
            var appRoleAssignment = new AppRoleAssignment
            {
                PrincipalId = Guid.Parse(userId),
                ResourceId = Guid.Parse(servicePrincipalId), // ID del recurso (aplicación) al que pertenece el AppRole
                AppRoleId = Guid.Parse(roleId),                
            };

            await graphClient
                .ServicePrincipals[servicePrincipalId]
                .AppRoleAssignedTo
                .PostAsync(appRoleAssignment);
        }

        public UserDto GetUserAdditionalData(List<Extension> userExtensions, UserDto userDto)
        {
            if (userExtensions == null || userDto == null)
            {
                return userDto;
            }

            var extension = userExtensions?
                    .FirstOrDefault(x => x.Id == configuration[AzureAD.Configuration.Domain]);

            if (extension != null)
            {
                userDto.SegundoApellido = extension.AdditionalData.ContainsKey(AzureAD.AddtionalData.LastName)
                    ? Convert.ToString(extension.AdditionalData[AzureAD.AddtionalData.LastName])
                    : string.Empty;

                userDto.TiempoInactividad = extension.AdditionalData.ContainsKey(AzureAD.AddtionalData.InactiveTime)
                    ? Convert.ToInt32(extension.AdditionalData[AzureAD.AddtionalData.InactiveTime])
                    : AzureAD.User.DefaultInactiveTime;

                userDto.Activo = extension.AdditionalData.ContainsKey(AzureAD.AddtionalData.Activo)
                    ? Convert.ToBoolean(extension.AdditionalData[AzureAD.AddtionalData.Activo])
                    : true;

                userDto.FechaUltimaActualizacion = extension.AdditionalData.ContainsKey(AzureAD.AddtionalData.FechaActualizacion) ?
                    Convert.ToDateTime(extension.AdditionalData[AzureAD.AddtionalData.FechaActualizacion])
                    : DateTime.UtcNow;

                if (extension.AdditionalData?.ContainsKey(AzureAD.AddtionalData.Corporaciones) == true)
                {
                    var untypedArray = extension.AdditionalData[AzureAD.AddtionalData.Corporaciones] as Microsoft.Kiota.Abstractions.Serialization.UntypedArray;
                    userDto.Corporaciones = untypedArray?
                        .GetValue()
                        .OfType<Microsoft.Kiota.Abstractions.Serialization.UntypedString>()
                        .Select(node => node.GetValue())
                        .ToList() ?? [];
                }
            }

            return userDto;
        }

        public async Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                // Realiza una consulta con un filtro para buscar el usuario por su ID
                var users = await graphClient
                    .Users
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = $"id eq '{userId}'";
                        requestConfiguration.QueryParameters.Select = new[] { "id" }; // Selecciona solo el ID para optimizar
                    }, cancellationToken);
                // Verifica si el usuario existe
                return users?.Value?.Any() ?? false;
            }
            catch (ODataError)
            {
                // Maneja errores específicos de OData sin lanzar excepciones
                return false;
            }
            catch (Exception ex)
            {
                // Registra otros errores inesperados si es necesario
                throw new Exception("Error al verificar la existencia del usuario.", ex);
            }
        }
        #endregion

        #region Groups mamagment
        public async Task<List<Group>> GetGroups(CancellationToken cancellationToken)
        {
            var groups = await graphClient
                .Groups
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[] { "id", "displayName", "description", "securityEnabled", "CreatedDateTime" };
                    requestConfiguration.QueryParameters.Expand = new[] { "extensions" };
                }, cancellationToken: cancellationToken);

            return groups?.Value ?? [];
        }

        public async Task<List<Group>> GetActiveGroups(CancellationToken cancellationToken)
        {
            var activeGroups = new List<Group>();

            // Obtener todos los grupos de Azure AD
            var groups = await graphClient
                .Groups
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = new[] { "extensions" };
                }, cancellationToken: cancellationToken);

            if (groups?.Value != null)
            {
                foreach (var group in groups.Value)
                {
                    // Verificar si la extensión contiene la clave "Activo" con el valor "true"
                    if (GetGroupActiveExtensionValue(group.Extensions, configuration[AzureAD.Configuration.Domain]))
                    {
                        activeGroups.Add(group);
                    }
                }
            }

            return activeGroups;
        }

        public bool GetGroupActiveExtensionValue(List<Extension> extensions, string extensionName)
        {
            if (extensions == null || string.IsNullOrEmpty(extensionName))
            {
                return true;
            }

            var extension = extensions.FirstOrDefault(e => e.Id == extensionName) as OpenTypeExtension;

            // Verificar si la extensión y el campo AzureAD.AddtionalData.Activo existen
            if (extension != null &&
                extension.AdditionalData.ContainsKey(AzureAD.AddtionalData.Activo))
            {
                bool.TryParse(Convert.ToString(extension.AdditionalData[AzureAD.AddtionalData.Activo]), out var active);

                return active;
            }

            // Si no existe o no es válido, devolver false por defecto
            return true;
        }

        public DateTime GetGroupFechaActualizacion(List<Extension> extensions, string extensionName)
        {
            if (extensions == null || string.IsNullOrEmpty(extensionName))
            {
                return DateTime.UtcNow;
            }

            var extension = extensions.FirstOrDefault(e => e.Id == extensionName) as OpenTypeExtension;

            if (extension != null &&
                extension.AdditionalData.ContainsKey(AzureAD.AddtionalData.FechaActualizacion))
            {
                DateTime.TryParse(Convert.ToString(extension.AdditionalData[AzureAD.AddtionalData.FechaActualizacion]), out var fecha);
                return fecha;
            }

            return DateTime.UtcNow;
        }

        public async Task<Group> GetGroupById(string groupId, CancellationToken cancellationToken)
        {
            return await graphClient
                .Groups[groupId]
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = new[] { "extensions" };
                    requestConfiguration.QueryParameters.Select = new[] { "id", "displayName", "description", "securityEnabled", "CreatedDateTime" }; // Selecciona solo el ID para optimizar
                }, cancellationToken: cancellationToken);
        }
        public async Task<List<User>> GetGroupMembersAsync(string groupId, CancellationToken cancellationToken)
        {
            // Obtener los miembros del grupo
            var members = await graphClient
                .Groups[groupId]
                .Members
                .GetAsync(cancellationToken: cancellationToken);

            // Filtrar y devolver solo los usuarios
            return members?.Value?.OfType<User>().ToList() ?? [];
        }

        public async Task<Group> CreateGroup(Group newGroup, CancellationToken cancellationToken)
        {
            return await graphClient
                .Groups
                .PostAsync(newGroup, cancellationToken: cancellationToken);
        }

        public async Task<Group> UpdateGroup(string groupId, Group group, CancellationToken cancellationToken)
        {
            return await graphClient
                .Groups[groupId]
                .PatchAsync(group, cancellationToken: cancellationToken);
        }

        public async Task DeleteGroup(string groupId, CancellationToken cancellationToken)
        {
            await graphClient
                .Groups[groupId]
                .DeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task DeactivateGroup(string groupId, CancellationToken cancellationToken)
        {
            await UpdateGroupExtension(groupId,
                new OpenTypeExtension
                {
                    Id = configuration[AzureAD.Configuration.Domain],
                    ExtensionName = configuration[AzureAD.Configuration.Domain],
                    AdditionalData = new Dictionary<string, object>
                    {
                        { AzureAD.AddtionalData.Activo, "false" }
                    }
                },
                cancellationToken);
        }

        public async Task ActivateGroup(string groupId, CancellationToken cancellationToken)
        {
            await UpdateGroupExtension(groupId,
                new OpenTypeExtension
                {
                    Id = configuration[AzureAD.Configuration.Domain],
                    ExtensionName = configuration[AzureAD.Configuration.Domain],
                    AdditionalData = new Dictionary<string, object>
                    {
                        { AzureAD.AddtionalData.Activo, "true" }
                    }
                },
                cancellationToken);
        }

        public async Task UpdateGroupExtension(string groupId, OpenTypeExtension extension, CancellationToken cancellationToken)
        {
            if (extension == null ||
                groupId.IsNullOrWhiteSpace() ||
                extension.ExtensionName.IsNullOrWhiteSpace())
            {
                return;
            }

            // Verificar si la extensión ya existe
            var existingExtensions = await graphClient
                .Groups[groupId]
                .Extensions
                .GetAsync(cancellationToken: cancellationToken);

            var existingExtension = existingExtensions?.Value?.FirstOrDefault(e => e.Id == extension.Id);

            if (existingExtension == null)
            {
                await graphClient
                    .Groups[groupId]
                    .Extensions
                    .PostAsync(extension, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await graphClient
                    .Groups[groupId]
                    .Extensions[extension.ExtensionName]
                    .PatchAsync(extension, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task<bool> GetGroupActiveStatusAsync(string groupId, string extensionName, CancellationToken cancellationToken)
        {
            // Obtener la extensión del grupo
            var extensions = await graphClient
                .Groups[groupId]
                .Extensions
                .GetAsync(cancellationToken: cancellationToken);

            var extension = extensions?.Value?.FirstOrDefault(e => e.Id == extensionName) as OpenTypeExtension;

            // Verificar si la extensión y el campo "Activo" existen
            if (extension != null &&
                extension.AdditionalData.ContainsKey(AzureAD.AddtionalData.Activo) &&
                extension.AdditionalData[AzureAD.AddtionalData.Activo] is string activeString)
            {
                // Intentar convertir el valor de "Activo" a un booleano
                bool.TryParse(activeString, out var active);
                return active;
            }

            // Si no existe o no es válido, devolver false por defecto
            return true;
        }

        public async Task<bool> GroupExistsAsync(string groupId, CancellationToken cancellationToken)
        {
            try
            {
                // Realiza una consulta con un filtro para buscar el grupo por su ID
                var groups = await graphClient
                    .Groups
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = $"id eq '{groupId}'";
                        requestConfiguration.QueryParameters.Select = new[] { "id" }; // Selecciona solo el ID para optimizar
                    }, cancellationToken);

                // Verifica si el grupo existe
                return groups?.Value?.Any() ?? false;
            }
            catch (ODataError)
            {
                // Maneja errores específicos de OData sin lanzar excepciones
                return false;
            }
            catch (Exception ex)
            {
                // Registra otros errores inesperados si es necesario
                throw new Exception("Error al verificar la existencia del grupo.", ex);
            }
        }
        #endregion

        #region Roles

        #endregion
    }
}