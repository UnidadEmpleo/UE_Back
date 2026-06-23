using API.Seguridad.DTOs.Seguridad;
using Microsoft.Graph.Models;

namespace API.Seguridad.Application.Core
{
    public interface IGraphManager
    {
        Task UpdateUserGroups(string userId, string groupId, CancellationToken cancellationToken);
        Task UpdateUserExtension(string userId, OpenTypeExtension extensions, CancellationToken cancellationToken);
        Task UpdateUser(string userId, User user, CancellationToken cancellationToken);
        Task<User> CreateUser(User newUser, CancellationToken cancellationToken);
        Task<User?> GetUserById(string userId, CancellationToken cancellationToken);
        Task DeleteUser(string userId, CancellationToken cancellationToken);
        Task<string> GetMemberOfIdAsync<T>(string userId, CancellationToken cancellationToken) where T : DirectoryObject;
        Task RemoveUserFromGroup(string userId, string groupId, CancellationToken cancellationToken);
        Task UpdateGroupExtension(string groupId, OpenTypeExtension extension, CancellationToken cancellationToken);
        Task<Group> UpdateGroup(string groupId, Group group, CancellationToken cancellationToken);
        Task DeactivateGroup(string groupId, CancellationToken cancellationToken);
        Task ActivateGroup(string groupId, CancellationToken cancellationToken);
        Task<Group> CreateGroup(Group newGroup, CancellationToken cancellationToken);
        Task DeleteGroup(string groupId, CancellationToken cancellationToken);
        Task<bool> GetGroupActiveStatusAsync(string groupId, string extensionName, CancellationToken cancellationToken);
        Task<bool> GroupExistsAsync(string groupId, CancellationToken cancellationToken);
        Task<Group> GetGroupById(string groupId, CancellationToken cancellationToken);
        Task<List<User>> GetGroupMembersAsync(string groupId, CancellationToken cancellationToken);
        Task<List<Group>> GetActiveGroups(CancellationToken cancellationToken);
        bool GetGroupActiveExtensionValue(List<Extension> extensions, string extensionName);
        Task AddUserToGroup(string userId, string groupId, CancellationToken cancellationToken);
        Task<OpenTypeExtension> GetUserExtension(string userId, string extensionName, CancellationToken cancellationToken);
        Task<List<User>> GetUsers(CancellationToken cancellationToken);
        Task<DirectoryObjectCollectionResponse?> GetUserMemberOf(string userId, CancellationToken cancellationToken);
        Task<List<Group>> GetGroups(CancellationToken cancellationToken);
        UserDto GetUserAdditionalData(List<Extension> userExtensions, UserDto userDto);
        DateTime GetGroupFechaActualizacion(List<Extension> extensions, string extensionName);
        Task UpdateUserAppRole(string userId, string roleId, CancellationToken cancellationToken);
        Task<List<AppRoleAssignment>> GetUserAppRolesAsync(string userId, CancellationToken cancellationToken);
        Task UpdateGroupUsers(List<string> userIds, string groupId, CancellationToken cancellationToken);
    }
}