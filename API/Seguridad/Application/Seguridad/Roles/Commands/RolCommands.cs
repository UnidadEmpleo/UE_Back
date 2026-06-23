using API.Seguridad.Domain.Seguridad;
using API.Persistence;
using API.Seguridad.Application.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace API.Seguridad.Application.Seguridad.Roles.Commands
{
    public class CreateRol
    {
        public class Command : IRequest<Result<string>>
        {
            public string Nombre { get; set; }
            public string Descr { get; set; }
            public string Definicion { get; set; }
            public List<int> Procesos { get; set; }
            public int TipoRol { get; set; } = 0; // Tipo de rol, por defecto NoDefinido    
        }

        public class Handler(GraphServiceClient graphClient, 
            AppDbContext context,
            RoleManager<AppIdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<Handler> _logger) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Procesos == null || !request.Procesos.Any())
                {
                    return Result<string>.Failure("No se han proporcionado procesos", 400);
                }   

                var aspnetResult = await AddAspNetRole(request, cancellationToken); 
                if (!aspnetResult.IsSuccess)
                {
                    return Result<string>.Failure(aspnetResult.Error, aspnetResult.Code);
                }

                try
                {
                    var appId = configuration[AzureAD.Configuration.ApplicationId];

                    var application = await graphClient
                        .Applications[appId]
                        .GetAsync(cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    if (application == null)
                    {
                        _logger.LogError($"Error al crear rol {request.Nombre} en Azure AD, Aplicacion {appId} no encontrada");
                    }
                    else
                    {
                        // Crea un nuevo Application Role
                        var newAppRole = new AppRole
                        {
                            Id = aspnetResult.Value, // Genera un nuevo ID único para el rol
                            DisplayName = request.Nombre,
                            Description = request.Descr,
                            Value = request.Definicion, // Valor que se usará en los tokens
                            IsEnabled = true,
                            AllowedMemberTypes = new List<string> { "User" } // Puede ser "User" o "Application"                    
                        };

                        application.AppRoles.Add(newAppRole);

                        await graphClient
                            .Applications[appId]
                            .PatchAsync(application, cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al tratar de crear el Rol {request.Nombre} en Azure AD");
                }

                // Guarda los procesos asociados en la base de datos
                foreach (var proceso in request.Procesos)
                {
                    await context.RolesProcesos.AddAsync(new RolProceso
                    {
                        ProcesoId = proceso,
                        RolId = aspnetResult.Value.ToString()
                    }).ConfigureAwait(false);
                }

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<string>.Failure($"Error al tratar de asociar los procesos del rol {request.Nombre} {request.Definicion}", 500);
                }

                return Result<string>.Success(aspnetResult.Value.ToString());
            }

            private async Task<Result<Guid>> AddAspNetRole(Command request, CancellationToken cancellationToken)
            {
                var roleExists = await roleManager.RoleExistsAsync(request.Nombre);
                if (roleExists)
                {
                    return Result<Guid>.Failure($"El rol {request.Nombre} ya existe.", 400);
                }

                var newAppRole = new AppIdentityRole()
                {
                    Name = request.Nombre,
                    NormalizedName = request.Nombre.ToUpperInvariant(),
                    Descripcion = request.Descr,
                    Value = request.Definicion,
                    TipoRol = (TipoRoles)request.TipoRol,
                };

                var roleResult = await roleManager.CreateAsync(newAppRole);
                if (!roleResult.Succeeded)
                    Result<string>.Failure($"Error al crear el rol {request.Nombre}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}", 500);

                var role = await roleManager.FindByNameAsync(request.Nombre);
                if (role == null)
                {
                    return Result<Guid>.Failure($"Error al encontrar el rol {request.Nombre} después de crearlo.", 500);
                }

                return Result<Guid>.Success(new Guid(role.Id));
            }
        }
    }

    public class UpdateRol
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
            public string Descr { get; set; }
            public bool Activo { get; set; } = true;    
            public List<int> Procesos { get; set; }
            public int TipoRol { get; set; } = 0; // Tipo de rol, por defecto NoDefinido
        }

        public class Handler(GraphServiceClient graphClient, 
            AppDbContext context,
            RoleManager<AppIdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var aspNetRoleUpdateResult = await UpdateAspNetRole(request, cancellationToken);
                
                if (!aspNetRoleUpdateResult.IsSuccess)
                {
                    return Result<Unit>.Failure(aspNetRoleUpdateResult.Error, aspNetRoleUpdateResult.Code);
                }

                try
                {
                    var appId = configuration[AzureAD.Configuration.ApplicationId];

                    var application = await graphClient
                        .Applications[appId]
                        .GetAsync(cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    if (application == null)
                    {
                        _logger.LogError($"Error al actualizar los datos del rol {request.Id}-{request.Nombre} en Azure AD, Aplicacion {appId} no encontrada");
                    }
                    else
                    {
                        var appRole = application.AppRoles.FirstOrDefault(r => r.Id.ToString() == request.Id);
                        if (appRole == null)
                        {
                            _logger.LogError($"Error al actualizar los datos del rol {request.Id}-{request.Nombre} en Azure AD, Rol no encontrado");
                        }
                        else
                        {
                            appRole.DisplayName = request.Nombre;
                            appRole.Description = request.Descr;
                            appRole.IsEnabled = request.Activo; // Actualiza el estado del rol

                            await graphClient.Applications[appId]
                                .PatchAsync(application, cancellationToken: cancellationToken)
                                .ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al actualizar los datos del rol {request.Id}-{request.Nombre} en Azure AD");
                }

                // Actualiza los procesos asociados en la base de datos
                var existingRolProcesos = await context.RolesProcesos
                    .Where(rp => rp.RolId == request.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var procesosToRemove = existingRolProcesos
                    .Where(rp => !request.Procesos.Contains(rp.ProcesoId.Value))
                    .ToList();

                var procesosToAdd = request.Procesos
                    .Where(procesoId => !existingRolProcesos.Any(rp => rp.ProcesoId == procesoId))
                    .Select(procesoId => new RolProceso
                    {
                        RolId = request.Id,
                        ProcesoId = procesoId
                    })
                    .ToList();

                context.RolesProcesos.RemoveRange(procesosToRemove);
                await context.RolesProcesos.AddRangeAsync(procesosToAdd, cancellationToken);

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return Result<Unit>.Success(Unit.Value);
            }

            private async Task<Result<Unit>> UpdateAspNetRole(Command request, CancellationToken cancellationToken)
            {
                var role = await roleManager.FindByIdAsync(request.Id);
                if (role == null)
                {
                    return Result<Unit>.Failure($"Rol con ID {request.Id} no encontrado.", 404);
                }
                
                role.Name = request.Nombre;
                role.NormalizedName = request.Nombre.ToUpperInvariant();
                role.FechaUltimaActualizacion = DateTime.UtcNow;
                role.Descripcion = request.Descr;
                role.Activo = request.Activo;
                role.TipoRol = (TipoRoles)request.TipoRol;

                var updateResult = await roleManager.UpdateAsync(role);

                if (!updateResult.Succeeded)
                {
                    return Result<Unit>.Failure($"Error al actualizar el rol: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}", 500);
                }
                return Result<Unit>.Success(Unit.Value);
            }   
        }
    }

    public class DeleteRol
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler(GraphServiceClient graphClient, 
            RoleManager<AppIdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<Handler> logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var appId = configuration[AzureAD.Configuration.ApplicationId];

                    var application = await graphClient
                        .Applications[appId]
                        .GetAsync(cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    if (application == null)
                    {
                        logger.LogError($"Error al desactivar el rol {request.Id} en Azure AD, Aplicacion {appId} no encontrada");
                    }
                    else
                    {
                        var appRole = application.AppRoles?.FirstOrDefault(r => r.Id.ToString() == request.Id);
                        if (appRole == null)
                        {
                            return Result<Unit>.Failure("Rol no encontrado", 404);
                        }

                        appRole.IsEnabled = false;

                        // Actualiza la aplicación en Azure AD
                        await graphClient.Applications[appId]
                            .PatchAsync(application, cancellationToken: cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error al tratar de desactivar el rol {request.Id} en Azure AD");
                }

                // Actualiza los datos adicionales en la base de datos
                var appIdentityRole = await roleManager
                    .FindByIdAsync(request.Id)
                    .ConfigureAwait(false);

                if (appIdentityRole == null)
                {
                    return Result<Unit>.Failure($"Rol con ID {request.Id} no encontrado en ASP.NET Identity.", 404);
                }

                appIdentityRole.Activo = false; // Desactiva el rol en ASP.NET Identity
                appIdentityRole.FechaUltimaActualizacion = DateTime.UtcNow;
                var updateResult = await roleManager.UpdateAsync(appIdentityRole);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}