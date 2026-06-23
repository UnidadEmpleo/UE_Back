using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.UnidadEmpleo.Application.ReferenciaApp
{
    public class GetReferencia
    {
        public class Query : IRequest<Result<Referencia>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper mapper) : IRequestHandler<Query, Result<Referencia>>
        {
            public async Task<Result<Referencia>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();
                var baseQuery = dbContext.Set<Referencia>().AsNoTracking()
                            .Where(x => x.Id == request.Id);
                Referencia? queryData = await baseQuery
                    .FirstOrDefaultAsync(cancellationToken);
                if (queryData == null)
                    return Result<Referencia>.Failure("No se encontró la Carta Compromiso", 404);

                return Result<Referencia>.Success(queryData);
            }
        }
    }

    public class GetReferenciaList
    {
        public class Query : IRequest<Result<List<Referencia>>> { }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<Referencia>>>
        {
            public async Task<Result<List<Referencia>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<Referencia>()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<Referencia>>.Success(entidades);
            }
        }
    }

    public class GetReferenciaListBySolicitud
    {
        public class Query : IRequest<Result<List<Referencia>>> {
            public int idsolicitud { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<Referencia>>>
        {
            public async Task<Result<List<Referencia>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<Referencia>()
                    .Where(x => x.IdSoliciud == request.idsolicitud)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<Referencia>>.Success(entidades);
            }
        }
    }
}
