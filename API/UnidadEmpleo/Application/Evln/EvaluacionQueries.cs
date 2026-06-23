using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.UnidadEmpleo.Application.Evln
{
    public class GetEvaluacion
    {
        public class Query : IRequest<Result<Evaluacion>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper mapper) : IRequestHandler<Query, Result<Evaluacion>>
        {
            public async Task<Result<Evaluacion>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();
                var baseQuery = dbContext.Set<Evaluacion>().AsNoTracking().Where(x => x.Id == request.Id);
                Evaluacion? queryData = await baseQuery.FirstOrDefaultAsync(cancellationToken);
                if (queryData == null)
                    return Result<Evaluacion>.Failure("No se encontró la Carta Compromiso", 404);

                return Result<Evaluacion>.Success(queryData);
            }
        }
    }

    public class GetEvaluacionList
    {
        public class Query : IRequest<Result<List<Evaluacion>>> { }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<Evaluacion>>>
        {
            public async Task<Result<List<Evaluacion>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<Evaluacion>()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<Evaluacion>>.Success(entidades);
            }
        }
    }

    public class GetEvaluacionListBySolicitud
    {
        public class Query : IRequest<Result<List<Evaluacion>>>
        {
            public int idsolicitud { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<Evaluacion>>>
        {
            public async Task<Result<List<Evaluacion>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<Evaluacion>()
                    .Where(x => x.IdSoliciud == request.idsolicitud)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<Evaluacion>>.Success(entidades);
            }
        }
    }
}
