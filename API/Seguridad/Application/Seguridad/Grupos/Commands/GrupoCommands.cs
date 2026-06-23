using API.Seguridad.Domain.Seguridad;
using API.Persistence;
using API.Seguridad.Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;

namespace API.Seguridad.Application.Seguridad.Grupos.Commands
{
    public class CreateGrupo
    {
        public class Command : IRequest<Result<string>>
        {
            public string Nombre { get; set; }
            public string Descr { get; set; }
            public List<string> Usuarios { get; set; } = [];
        }

        public class Handler(IGraphManager graphManager,
            AppDbContext context,
            IConfiguration configuration,
            ILogger<Handler> _logger) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                string grupoId = string.Empty;

                var group = new Group
                {
                    DisplayName = request.Nombre,
                    Description = request.Descr,
                    MailEnabled = false,
                    MailNickname = request.Nombre.Replace(" ", "").ToLower(),
                    SecurityEnabled = true 
                };

                try
                {
                    var result = await graphManager
                        .CreateGroup(group, cancellationToken)
                        .ConfigureAwait(false);

                    grupoId = result.Id;

                    await graphManager
                        .UpdateGroupExtension(result.Id,
                            new OpenTypeExtension
                            {
                                Id = configuration[AzureAD.Configuration.Domain],
                                ExtensionName = configuration[AzureAD.Configuration.Domain],
                                AdditionalData = new Dictionary<string, object>
                                {
                                { AzureAD.AddtionalData.Activo, true },
                                { AzureAD.AddtionalData.FechaActualizacion, DateTime.UtcNow },
                                }
                            }, cancellationToken)
                        .ConfigureAwait(false);

                    // Agregar miembros al grupo
                    foreach (var memberId in request.Usuarios)
                    {
                        var user = await graphManager
                            .GetUserById(memberId, cancellationToken)
                            .ConfigureAwait(false);

                        if (user != null)
                        {
                            await graphManager
                                .AddUserToGroup(user.Id, result.Id, cancellationToken)
                                .ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al tratar de crear el grupo {request.Nombre} en Azure AD");
                }

                var resultCreate = await CrearGrupoBD(request, grupoId, cancellationToken);
                
                if (!resultCreate.IsSuccess)
                {
                    await graphManager
                        .DeleteGroup(grupoId, cancellationToken)
                        .ConfigureAwait(false);

                    return Result<string>.Failure(resultCreate.Error, resultCreate.Code);
                }

                return Result<string>.Success(resultCreate.Value);
            }

            private async Task<Result<string>> CrearGrupoBD(Command request, string id, CancellationToken cancellationToken)
            {
                // Obtener los usuarios cuyos IDs est�n en request.Members
                var usuarios = await context.Usuarios
                    .Where(u => request.Usuarios.Contains(u.Id))
                    .ToListAsync(cancellationToken);

                var grupo = new Grupo
                {
                    Id = id.IsNullOrWhiteSpace() ? Guid.NewGuid().ToString() : id,
                    Nombre = request.Nombre,
                    Descr = request.Descr ?? string.Empty,
                    Usuarios = usuarios 
                };

                context.Grupos.Add(grupo);

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<string>.Failure("Error al crear el grupo", 400);
                }

                return Result<string>.Success(grupo.Id);

            }
        }
    }

    public class UpdateGrupo
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
            public string Descr { get; set; }
            public bool Activo { get; set; } = true;
            public List<string> Usuarios { get; set; } = [];
        }

        public class Handler(IGraphManager graphManager,
            AppDbContext context,
            IConfiguration configuration,
            ILogger<Handler> logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var resultUpdGrupoBD = await UpdateGrupoBD(request, cancellationToken);

                try
                {
                    var group = new Group
                    {
                        DisplayName = request.Nombre,
                        Description = request.Descr
                    };

                    await graphManager
                        .UpdateGroup(request.Id, group, cancellationToken)
                        .ConfigureAwait(false);

                    await graphManager.UpdateGroupExtension(request.Id,
                        new OpenTypeExtension
                        {
                            Id = configuration[AzureAD.Configuration.Domain],
                            ExtensionName = configuration[AzureAD.Configuration.Domain],
                            AdditionalData = new Dictionary<string, object>
                            {
                            { AzureAD.AddtionalData.Activo, request.Activo },
                            { AzureAD.AddtionalData.FechaActualizacion, DateTime.UtcNow },
                            }
                        }, cancellationToken).ConfigureAwait(false);

                    await graphManager
                        .UpdateGroupUsers(request.Usuarios, request.Id, cancellationToken)
                        .ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error al tratar de actualizar el grupo {request.Id}-{request.Nombre} en Azure AD");
                }

                return Result<Unit>.Success(Unit.Value);
            }

            private async Task<Result<Unit>> UpdateGrupoBD(Command request, CancellationToken cancellationToken)
            {
                var grupo = await context.Grupos
                    .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

                if (grupo == null)
                {
                    return Result<Unit>.Failure($"Grupo {request.Nombre} no existe", 404);
                }

                grupo.Nombre = request.Nombre;
                grupo.Descr = request.Descr;
                grupo.Activo = request.Activo;
                grupo.FechaUltimaActualizacion = DateTime.UtcNow;

                grupo.Usuarios.Clear();
                
                var usuarios = await context.Usuarios
                    .Where(u => request.Usuarios.Contains(u.Id))
                    .ToListAsync(cancellationToken);

                grupo.Usuarios = usuarios;  

                context.Grupos.Update(grupo);

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<Unit>.Failure($"Error al actualizar el grupo {request.Nombre}", 400);
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

    public class DeleteGrupo
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        /// <summary>
        /// Desactiva un grupo en Azure AD y actualiza su estado en la base de datos.
        /// </summary>
        /// <param name="graphManager"></param>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        public class Handler(IGraphManager graphManager,
            AppDbContext context,
            IConfiguration configuration,
            ILogger<Handler> logger) : IRequestHandler<Command, Result<Unit>>
        {
            /// <summary>
            /// Desactiva un grupo en Azure AD y actualiza su estado en la base de datos.
            /// </summary>
            /// <param name="request"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    await graphManager
                    .DeactivateGroup(request.Id, cancellationToken)
                    .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error al tratar de desactivar el grupo {request.Id} en Azure AD");
                }

                var grupo = await context
                    .Grupos
                    .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
                
                if (grupo == null)
                {
                    return Result<Unit>.Failure($"Grupo con ID {request.Id} no encontrado", 404);
                }

                grupo.Activo = false;
                context.Grupos.Update(grupo);
                
                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<Unit>.Failure($"Error al desactivar el grupo {grupo.Nombre}", 400);
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
