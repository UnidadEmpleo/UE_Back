using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.Application.CartaCompromisoApp
{
    public class CartaCompromisoCreate
    {
        public class Command : IRequest<Result<int>>
        {
            public int Id { get; set; }
            public int IdSoliciud { get; set; }
            public TipoCarta tipo { get; set; }
            public StatusDocumento Status { get; set; }
            public DateOnly FechaEmision { get; set; }
            public DateOnly FechaCompromiso { get; set; }
            //public Solicitud Solicitud { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper _mapper, ILogger<Handler> _logger,
            IHttpContextAccessor http, IMediator mediator) : IRequestHandler<Command, Result<int>>
        {
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<int>.Failure("Los datos de la CARTA COMPROMISO no pueden ser nulos.", 400);

                try
                {
                    var entidad = _mapper.Map<CartaCompromiso>(request);
                    dbContext.Set<CartaCompromiso>().Add(entidad);
                    var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)                                           
                        return Result<int>.Failure("Error al crear la Carta Compromiso", 400);                    
                    return Result<int>.Success(entidad.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error de base de datos al crear la Carta Compromiso");
                    return Result<int>.Failure($"Error de base de datos al crear la Carta Compromiso:", 500);
                }
            }
        }
    }


    public class CartaCompromisoUpdate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int IdRequest { get; set; }
            public int Id { get; set; }
            public int IdSoliciud { get; set; }
            public TipoCarta tipo { get; set; }
            public StatusDocumento Status { get; set; }
            public DateOnly FechaEmision { get; set; }
            public DateOnly FechaCompromiso { get; set; }
            //public Solicitud Solicitud { get; set; }

        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper _mapper, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<Unit>.Failure("Los datos de la Carta Compromiso no pueden ser nulos.", 400);
                if (request.Id != request.IdRequest)
                    return Result<Unit>.Failure("El identificador no coincide con el contenido.", 400);

                    try
                    {
                        var ente = await dbContext.Set<CartaCompromiso>().FindAsync([request.Id], cancellationToken);

                        if (ente == null)                        
                            return Result<Unit>.Failure("No se encontró la Carta Compromiso id " + request.Id, 404);
                        
                        _mapper.Map(request, ente);

                        var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

                        if (!result)
                            return Result<Unit>.Failure("Error al actualizar la Carta Compromiso", 400);
                        
                        return Result<Unit>.Success(Unit.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error de base de datos al actualizar la Carta Compromiso: {Id:}", request.Id);
                        return Result<Unit>.Failure($"Error de base de datos al actualizar la Carta Compromiso: {request.Id}", 500);
                    }
                
            }
        }
    }

    public class CartaCompromisoDelete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext context, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
               
                try
                {
                    var entidad = await context.Set<CartaCompromiso>().FindAsync([request.Id], cancellationToken); //.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    if (entidad == null)
                        return Result<Unit>.Failure("No se encontró la Carta Compromiso", 404);
                    context.Remove(entidad);
                    var result = await context.SaveChangesAsync(cancellationToken) > 0;
                    return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("No fue posible eliminar la Carta Compromiso", 400);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error de base de datos al eliminar la Carta Compromiso: {request.Id}", request.Id);
                    return Result<Unit>.Failure($"Error de base de datos al eliminar la Carta Compromiso: {request.Id}", 500);
                }
            }
        }
    }
}
