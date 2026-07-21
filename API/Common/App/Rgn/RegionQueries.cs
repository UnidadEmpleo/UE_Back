using API.Common.Domain;
using API.Seguridad.Application.Core;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Common.App.Rgn
{
    public class GetRegion
    {
        public class Query : IRequest<Result<Region>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper mapper) : IRequestHandler<Query, Result<Region>>
        {
            public async Task<Result<Region>> Handle(Query request, CancellationToken cancellationToken)
            {
                //await using var dbContext = await _factory.CreateAsync();
                var baseQuery = dbContext.Set<Region>().AsNoTracking()
                            .Where(x => x.Id == request.Id);
                Region? item = await baseQuery.FirstOrDefaultAsync(cancellationToken);

                if (item == null)
                    return Result<Region>.Failure("No se encontró el CUERPO", 404);

                return Result<Region>.Success(item);
            }
        }
    }

    public class GetRegionList
    {
        public class Query : IRequest<Result<List<Region>>> { }

        public class Handler(UnidadEmpleoDbContext dbContext) : IRequestHandler<Query, Result<List<Region>>>
        {
            public async Task<Result<List<Region>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<Region>().AsNoTracking().ToListAsync(cancellationToken);

                return Result<List<Region>>.Success(entidades);
            }
        }
    }



    public class GetPagedRegiones
    {
        public class Query : IRequest<Result<PagedResult<RegioDto>>>
        {
            public string? region { get; set; }
            public PaginationParams Pagination { get; set; } = new();
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper _mapper) : IRequestHandler<Query, Result<PagedResult<RegioDto>>>
        {
            public async Task<Result<PagedResult<RegioDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pagination = request.Pagination ?? new PaginationParams();

                if (pagination.PageNumber <= 0 || pagination.PageSize <= 0)
                {
                    return Result<PagedResult<RegioDto>>.Failure("PageNumber and PageSize must be greater than zero", 400);
                }

                var validSortFields = new[] { "Alias" };
                if (!string.IsNullOrEmpty(pagination.SortBy) && !validSortFields.Contains(pagination.SortBy))
                {
                    return Result<PagedResult<RegioDto>>.Failure($"Invalid SortBy field: {pagination.SortBy}", 400);
                }

                await using var context = await _factory.CreateAsync();

                var query = context.Set<Region>().AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(request.region))
                {
                    var nombreLower = request.region.ToLowerInvariant();
                    query = query.Where(p => (p.region).ToLower().Contains(nombreLower));
                }


                // Ejecutar COUNT antes de la paginación
                var totalRecords = await query.CountAsync(cancellationToken);

                if (!string.IsNullOrEmpty(pagination.SortBy))
                {
                    query = pagination.SortDirection == "desc"
                        ? query.OrderByDescending(e => EF.Property<object>(e, pagination.SortBy))
                        : query.OrderBy(e => EF.Property<object>(e, pagination.SortBy));
                }
                else
                {
                    //query = query.OrderByDescending(p => p.FechaCreacion);
                }

                var paged = await query
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                var dtos = _mapper.Map<List<RegioDto>>(paged);

                var result = new PagedResult<RegioDto>
                {
                    Data = dtos,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize),
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };

                return Result<PagedResult<RegioDto>>.Success(result);
            }
        }
    }
}
