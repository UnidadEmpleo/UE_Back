using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace API.UnidadEmpleo.Application.CartaCompromisoApp
{
    public class GetCartaCompromiso
    {
        public class Query : IRequest<Result<CartaCompromiso>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper mapper) : IRequestHandler<Query, Result<CartaCompromiso>>
        {
            public async Task<Result<CartaCompromiso>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();
                var baseQuery = dbContext.Set<CartaCompromiso>().AsNoTracking()
                            .Where(x => x.Id == request.Id);
                CartaCompromiso? queryData = await baseQuery
                    .FirstOrDefaultAsync(cancellationToken);
                if (queryData == null)
                    return Result<CartaCompromiso>.Failure("No se encontró la Carta Compromiso", 404);

                return Result<CartaCompromiso>.Success(queryData);
            }
        }
    }

    public class GetCartaCompromisoList
    {
        public class Query : IRequest<Result<List<CartaCompromiso>>> { }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<CartaCompromiso>>>
        {
            public async Task<Result<List<CartaCompromiso>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<CartaCompromiso>()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<CartaCompromiso>>.Success(entidades);
            }
        }
    }


    public class GetCartaCompromisoListBySolicitud
    {
        public class Query : IRequest<Result<List<CartaCompromiso>>> {
            public int IdSolicitud { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<CartaCompromiso>>>
        {
            public async Task<Result<List<CartaCompromiso>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<CartaCompromiso>()
                    .Where(x => x.IdSoliciud == request.IdSolicitud)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<CartaCompromiso>>.Success(entidades);
            }
        }
    }
}
