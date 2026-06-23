using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.Application.ReferenciaApp
{
    public class ReferenciaCreate
    {
        public class Command : IRequest<Result<int>>
        {
            public int Id { get; set; }
            public int IdSoliciud { get; set; }
            public Parentesco Parentesco { get; set; }
            public string TelefonoLocal { get; set; }

            public required string Nombre { get; set; }

            public string Apellido_Paterno { get; set; }

            public string Apellido_Materno { get; set; }
            public string Calle { get; set; }
            public string numero { get; set; }
            public string numeroInterior { get; set; }
            public string EntreCalles { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
            public string Pais { get; set; }
            public int CodigoPostal { get; set; }
            public string Estado { get; set; }
            public string Municipio { get; set; }
            public string Colonia { get; set; }
            public string TipoAsentamiento { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper _mapper, ILogger<Handler> _logger,
            IHttpContextAccessor http, IMediator mediator) : IRequestHandler<Command, Result<int>>
        {
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<int>.Failure("Los datos de la REFERENCIA no pueden ser nulos.", 400);

                await using var dbContext = await _factory.CreateAsync();

                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    dbContext.Set<Referencia>();
                    //foreach (Command c in request)
                    //{
                        var entidad = _mapper.Map<Referencia>(request);
                        dbContext.Add(entidad);
                    //}

                    var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<int>.Failure("Error al crear la REFERENCIA", 400);
                    }

                    await transaction.CommitAsync(cancellationToken);

                    return Result<int>.Success(entidad.Id);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error de base de datos al crear la Referencia");
                    return Result<int>.Failure($"Error de base de datos al crear la Referencia:", 500);
                }
            }
        }
    }


    public class ReferenciaUpdate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int IdRequest { get; set; }
            public int Id { get; set; }
            public int IdSoliciud { get; set; }
            public Parentesco Parentesco { get; set; }
            public string TelefonoLocal { get; set; }

            public required string Nombre { get; set; }

            public string Apellido_Paterno { get; set; }

            public string Apellido_Materno { get; set; }
            public string Calle { get; set; }
            public string numero { get; set; }
            public string numeroInterior { get; set; }
            public string EntreCalles { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
            public string Pais { get; set; }
            public int CodigoPostal { get; set; }
            public string Estado { get; set; }
            public string Municipio { get; set; }
            public string Colonia { get; set; }
            public string TipoAsentamiento { get; set; }

        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper _mapper, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<Unit>.Failure("Los datos de la Referencia no pueden ser nulos.", 400);

                if (request.Id != request.IdRequest)
                    return Result<Unit>.Failure("El identificador no coincide con el contenido.", 400);

                await using var context = await _factory.CreateAsync();

                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var ente = await context.Set<Referencia>().FindAsync([request.Id], cancellationToken);

                    if (ente == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("No se encontró la Referencia", 404);
                    }

                    //ente.FechaUltimaActualizacion = DateTime.UtcNow;
                    _mapper.Map(request, ente);

                    var result = await context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("Error al actualizar la Referencia", 400);
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return Result<Unit>.Success(Unit.Value);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error de base de datos al actualizar la Referencia: {Id:}", request.Id);
                    return Result<Unit>.Failure($"Error de base de datos al actualizar la Referencia: {request.Id}", 500);
                }
            }
        }
    }

    public class ReferenciaDelete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                await using var context = await _factory.CreateAsync();
                try
                {
                    var entidad = await context.Set<Referencia>().FindAsync([request.Id], cancellationToken); //.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    if (entidad == null)
                        return Result<Unit>.Failure("No se encontró la SOLICITUD", 404);
                    context.Remove(entidad);
                    var result = await context.SaveChangesAsync(cancellationToken) > 0;
                    return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("No fue posible eliminar la SOLICITUD", 400);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error de base de datos al eliminar la SOLICITUD: {request.Id}", request.Id);
                    return Result<Unit>.Failure($"Error de base de datos al eliminar la SOLICITUD: {request.Id}", 500);
                }
            }
        }
    }
}