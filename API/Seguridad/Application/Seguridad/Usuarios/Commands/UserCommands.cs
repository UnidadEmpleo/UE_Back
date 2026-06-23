
using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ExternalConnectors;

namespace API.Seguridad.Application.Seguridad.Usuarios.Commands
{
    public class CreateUser
    {
        public class Command : IRequest<Result<string>>
        {
            public string EMail { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool ForceChangePasswordNextSignIn { get; set; } = true;
            public string Nombre { get; set; }
            public string PrimerApellido { get; set; }
            public string? SegundoApellido { get; set; }
            public string? Telefono { get; set; }
            public int TiempoInactividad { get; set; } = 30;
            public IList<string> Corporaciones { get; set; } = [];
            public string RolId { get; set; }
            public string GrupoId { get; set; }
            public bool? Activo { get; set; } = true;
            public List<PermisoDTO> Permisos { get; set; } = [];
            public bool EsUsuarioAD { get; set; } = false; // Indica si el usuario es de Directorio Activo (AD)
            public bool Requiere2FA { get; set; } = true; // Indica si el usuario requiere autenticación de dos factores (2FA)
            public string? CuerpoId { get; set; }
            public int RegionId { get; set; }
        }

        public class Handler(IGraphManager graphManager,
            AppDbContext context,
            IMapper mapper,
            IConfiguration configuration,
            UserManager<AppUserIdentity> userManager,
            RoleManager<AppIdentityRole> roleManager,
            ILogger<Handler> logger) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                string userId = string.Empty;

                if (request.EsUsuarioAD)
                {
                    var user = new User
                    {
                        DisplayName = $"{request.Nombre} {request.PrimerApellido} {request.SegundoApellido}",
                        GivenName = request.Nombre,
                        Surname = request.PrimerApellido,
                        MobilePhone = request.Telefono,
                        Mail = request.EMail,
                        MailNickname = request.UserName,
                        UserPrincipalName = $"{request.UserName}@{configuration[AzureAD.Configuration.Domain]}",
                        PasswordProfile = new PasswordProfile
                        {
                            Password = request.Password,
                            ForceChangePasswordNextSignIn = request.ForceChangePasswordNextSignIn
                        },
                        AccountEnabled = true
                    };

                    try
                    {
                        var result = await graphManager.CreateUser(user, cancellationToken);

                        await graphManager
                            .UpdateUserExtension(result.Id,
                                                 new OpenTypeExtension
                                                 {
                                                     Id = configuration[AzureAD.Configuration.Domain],
                                                     ExtensionName = configuration[AzureAD.Configuration.Domain],
                                                     AdditionalData = new Dictionary<string, object>
                                                     {
                                                        { AzureAD.AddtionalData.LastName, request.SegundoApellido ?? string.Empty },
                                                        { AzureAD.AddtionalData.InactiveTime, request.TiempoInactividad },
                                                        { AzureAD.AddtionalData.Corporaciones, request.Corporaciones },
                                                        { AzureAD.AddtionalData.Activo, true },
                                                        { AzureAD.AddtionalData.FechaActualizacion, DateTime.UtcNow },
                                                     }
                                                 }, cancellationToken);

                        await graphManager.UpdateUserGroups(result.Id, request.GrupoId, cancellationToken);
                        await graphManager.UpdateUserAppRole(result.Id, request.RolId, cancellationToken);

                        userId = result.Id;

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error al crear el usuario {request.UserName} en Azure AD.");
                        //return Result<string>.Failure($"Error al crear el usuario {request.UserName} en Azure AD", 500);
                    }                
                }

                var resultCreateIdentityUser = await CreateAppIdentityUser(userId, request, cancellationToken);

                if (resultCreateIdentityUser.IsSuccess == false)
                {
                    return Result<string>.Failure(resultCreateIdentityUser.Error, resultCreateIdentityUser.Code);
                }

                return Result<string>.Success(resultCreateIdentityUser.Value);
            }

            private async Task<Result<string>> CreateAppIdentityUser(string? userId, Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                {
                    return Result<string>.Failure("El nombre de usuario y la contraseña son obligatorios para usuarios externos.", 400);
                }

                if (await userManager.FindByNameAsync(request.UserName) != null)
                {
                    return Result<string>.Failure($"El usuario {request.UserName} ya existe.", 400);
                }

                var user = new AppUserIdentity
                {
                    UserName = request.UserName,
                    Email = request.EMail,
                    TwoFactorEnabled = request.Requiere2FA,
                    EmailConfirmed = true                    
                };

                var result = await userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    logger.LogError($"Error al crear el usuario {request.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}.");
                    Result<string>.Failure($"Error al crear el usuario {request.UserName}", 404);
                }

                user = await userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    logger.LogError($"No se pudo encontrar el usuario creado {request.UserName}.");
                    return Result<string>.Failure($"No se pudo encontrar el usuario creado {request.UserName}", 404);
                }

                var rol = await roleManager.FindByIdAsync(request.RolId);
                if (rol == null)
                {
                    await userManager.DeleteAsync(user);
                    logger.LogError($"No se encontró el rol {request.RolId} para asignar al usuario {user.UserName}.");
                    return Result<string>.Failure($"No se encontró el rol {request.RolId}", 404);
                }

                var addToRoleResult = await userManager.AddToRoleAsync(user, rol.NormalizedName);
                if (!addToRoleResult.Succeeded)
                {
                    await userManager.DeleteAsync(user);
                    logger.LogError($"Error al asignar el rol {request.RolId} al usuario {user.UserName}: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    return Result<string>.Failure($"Error al asignar el rol {request.RolId} al usuario {user.UserName}", 404);
                }

                try
                {
                    Usuario usuario = new Usuario
                    {
                        Id = userId.IsNullOrWhiteSpace() ? Guid.NewGuid().ToString() : userId,
                        Nombre = request.Nombre,
                        PrimerApellido = request.PrimerApellido,
                        Telefono = request.Telefono,
                        SegundoApellido = request.SegundoApellido,
                        TiempoInactividad = (short)request.TiempoInactividad,
                        Activo = request.Activo ?? true,
                        RolId = request.RolId,
                        GrupoId = request.GrupoId,
                        UsuarioCorporaciones = [.. request.Corporaciones
                            .Select(c => new UsuarioCorporacion
                            {
                                CorporacionId = c
                            })],
                        AppUserIdentityId = user.Id,
                        EsUsuarioAD = request.EsUsuarioAD,
                        CuerpoId = request.CuerpoId,
                        RegionId = request.RegionId
                    };

                    await context
                        .Usuarios
                        .AddAsync(usuario, cancellationToken);

                    request.Permisos.ForEach(p =>
                    {
                        p.UsuarioId = usuario.Id;
                        p.RolId = request.RolId;
                    });

                    await context
                        .Permisos
                        .AddRangeAsync(mapper.Map<List<Permiso>>(request.Permisos));

                    await context.SaveChangesAsync(cancellationToken);

                    return Result<string>.Success(usuario.Id);
                }
                catch (Exception ex)
                {
                    await userManager.DeleteAsync(user);

                    logger.LogError(ex, $"Error al crear el usuario {request.UserName}");

                    return Result<string>.Failure($"Error al tratara de crear al usuario {request.UserName}", 500);
                }
            }
        }
    }

    public class UpdateUser
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
            public string? EMail { get; set; }
            public string? UserName { get; set; }
            public bool Activo { get; set; } = true;
            public bool ForceChangePasswordNextSignIn { get; set; } = true;
            public string Nombre { get; set; }
            public string PrimerApellido { get; set; }
            public string? SegundoApellido { get; set; }
            public string? Telefono { get; set; }
            public int TiempoInactividad { get; set; }
            public bool EsUsuarioAD { get; set; } = false; // Indica si el usuario es de Directorio Activo (AD)
            public List<string> Corporaciones { get; set; } = new List<string>();
            public string RolId { get; set; }
            public string GrupoId { get; set; }
            public bool? Requiere2FA { get; set; }
            public List<PermisoDTO> Permisos { get; set; } = [];
            public string? CuerpoId { get; set; }
            public int RegionId { get; set; }
        }

        public class Handler(IGraphManager graphManager,
            AppDbContext context,
            UserManager<AppUserIdentity> userManager,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<Handler> logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                await UpdateUsuarioExterno(request, cancellationToken);

                if (request.EsUsuarioAD)
                {
                    try
                    {
                        var user = await graphManager
                    .GetUserById(request.Id, cancellationToken);

                        if (user == null)
                        {
                            return Result<Unit>.Failure("No se encontro usuario", 404);
                        }

                        user.DisplayName = $"{request.Nombre} {request.PrimerApellido} {request.SegundoApellido}";
                        user.GivenName = request.Nombre;
                        user.Surname = request.PrimerApellido;
                        user.MobilePhone = request.Telefono;
                        user.Mail = request.EMail;
                        user.MailNickname = request.UserName;
                        user.AccountEnabled = request.Activo;

                        await graphManager.UpdateUser(request.Id, user, cancellationToken);
                        await graphManager.UpdateUserGroups(request.Id, request.GrupoId, cancellationToken);
                        await graphManager.UpdateUserAppRole(request.Id, request.RolId, cancellationToken);
                        await graphManager.UpdateUserExtension(request.Id,
                            new OpenTypeExtension
                            {
                                Id = configuration[AzureAD.Configuration.Domain],
                                ExtensionName = configuration[AzureAD.Configuration.Domain],
                                AdditionalData = new Dictionary<string, object>
                                {
                            { AzureAD.AddtionalData.LastName, request.SegundoApellido ?? string.Empty },
                            { AzureAD.AddtionalData.InactiveTime, request.TiempoInactividad },
                            { AzureAD.AddtionalData.Corporaciones, request.Corporaciones },
                            { AzureAD.AddtionalData.Activo, request.Activo },
                            { AzureAD.AddtionalData.FechaActualizacion, DateTime.UtcNow },
                                }
                            }, cancellationToken);

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Ocurrio un error al tratar de actualizar el usuario {request.Id} {request.UserName} en Azure AD");
                    }                
                }

                return Result<Unit>.Success(Unit.Value);
            }

            private async Task<Result<Unit>> UpdateUsuarioExterno(Command request, CancellationToken cancellationToken)
            {
                var aspnetUser = await userManager.FindByNameAsync(request.UserName);

                if (aspnetUser == null)
                {
                    return Result<Unit>.Failure($"No se encontro el usuario {request.UserName}", 404);
                }

                if (request.EMail != null)
                {
                    aspnetUser.Email = request.EMail;
                }

                aspnetUser.TwoFactorEnabled = request.Requiere2FA.HasValue ? request.Requiere2FA.Value : false;

                await userManager.UpdateAsync(aspnetUser);
                
                // Buscar el usuario externo en la base de datos
                var usuario = await context.Usuarios
                    .Include(u => u.UsuarioCorporaciones)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

                if (usuario == null)
                    return Result<Unit>.Failure("No se encontró el usuario externo", 404);

                // Actualizar propiedades básicas
                usuario.Nombre = request.Nombre;
                usuario.PrimerApellido = request.PrimerApellido;
                usuario.SegundoApellido = request.SegundoApellido;
                usuario.Telefono = request.Telefono;
                usuario.TiempoInactividad = (short)request.TiempoInactividad;
                usuario.Activo = request.Activo;
                usuario.RolId = request.RolId;
                usuario.GrupoId = request.GrupoId;
                usuario.FechaUltimaActualizacion = DateTime.UtcNow;
                usuario.CuerpoId = request.CuerpoId;
                usuario.RegionId = request.RegionId;

                // Eliminar las corporaciones actuales que no están en la nueva lista
                var corporacionesToRemove = usuario.UsuarioCorporaciones
                    .Where(uc => !request.Corporaciones.Contains(uc.CorporacionId))
                    .ToList();
                context.UsuarioCorporaciones.RemoveRange(corporacionesToRemove);

                // Agregar nuevas corporaciones que no existían
                var corporacionesToAdd = request.Corporaciones
                    .Where(c => !usuario.UsuarioCorporaciones.Any(uc => uc.CorporacionId == c))
                    .Select(c => new UsuarioCorporacion
                    {
                        UsuarioId = usuario.Id,
                        CorporacionId = c
                    }).ToList();
                await context.UsuarioCorporaciones.AddRangeAsync(corporacionesToAdd, cancellationToken);

                // Actualizar permisos
                request.Permisos.ForEach(p =>
                {
                    p.UsuarioId = usuario.Id;
                    p.RolId = request.RolId;
                });

                var existingPermisos = await context.Permisos
                    .Where(p => p.UsuarioId == usuario.Id)
                    .ToListAsync(cancellationToken);

                var permisosToRemove = existingPermisos
                    .Where(ep => !request.Permisos.Any(rp => rp.RolId == ep.RolId && rp.ProcesoId == ep.ProcesoId))
                    .ToList();

                var permisosToAdd = request.Permisos
                    .Where(rp => !existingPermisos.Any(ep => ep.RolId == rp.RolId && ep.ProcesoId == rp.ProcesoId))
                    .Select(rp => new Permiso
                    {
                        UsuarioId = usuario.Id,
                        RolId = rp.RolId,
                        ProcesoId = rp.ProcesoId,
                        Acceso = rp.Acceso
                    })
                    .ToList();

                // Actualizar permisos existentes
                foreach (var existingPermiso in existingPermisos)
                {
                    var updatedPermiso = request.Permisos
                        .FirstOrDefault(rp => rp.RolId == existingPermiso.RolId && rp.ProcesoId == existingPermiso.ProcesoId);

                    if (updatedPermiso != null)
                    {
                        existingPermiso.Acceso = updatedPermiso.Acceso;
                    }
                }

                context.Permisos.RemoveRange(permisosToRemove);

                await context
                    .Permisos
                    .AddRangeAsync(permisosToAdd, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }

        }
    }

    public class ActiveInactiveUser
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
            public bool Activo { get; set; }
            public bool Externo { get; set; } = false; // Indica si el usuario es externo
        }

        public class Handler(IGraphManager graphManager, 
            IConfiguration configuration,
            AppDbContext context,
            UserManager<AppUserIdentity> userManager,
            ILogger<Handler> logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Externo)
                {
                    return await ActiveInactiveUsuarioExterno(request, cancellationToken);
                }

                try
                {
                    var userExtension = await graphManager
                        .GetUserExtension(request.Id,
                            configuration[AzureAD.Configuration.Domain],
                            cancellationToken);

                    if (userExtension == null)
                    {
                        return Result<Unit>.Failure("No se encontro usuario", 404);
                    }

                    userExtension.AdditionalData[AzureAD.AddtionalData.Activo] = request.Activo;

                    await graphManager.UpdateUserExtension(request.Id,
                        userExtension,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error al tratar de Activar/Inactivar al usuario {request.Id} en Azure AD");
                }

                return Result<Unit>.Success(Unit.Value);
            }

            private async Task<Result<Unit>> ActiveInactiveUsuarioExterno(Command request, CancellationToken cancellationToken)
            {
                // Buscar el usuario externo en la base de datos
                var usuario = await context.Usuarios
                    .Include(u => u.UsuarioCorporaciones)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

                if (usuario == null)
                    return Result<Unit>.Failure("No se encontró el usuario externo", 404);

                var userIdentity = await userManager.FindByIdAsync(usuario.AppUserIdentityId);
                if (userIdentity == null)
                    return Result<Unit>.Failure("No se encontró la identidad del usuario externo", 404);

                userIdentity.LockoutEnabled = !request.Activo; // Bloquear o desbloquear el usuario según el estado activo
                await userManager.UpdateAsync(userIdentity);

                // Actualizar el estado activo del usuario
                usuario.Activo = request.Activo;
                usuario.FechaUltimaActualizacion = DateTime.UtcNow;

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
