using API.Common.Domain;
using API.Seguridad.Application.Core;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.UnidadEmpleo.Application.Cuerpoa
{
  
    public class GetCuerpo
    {
        public class Query : IRequest<Result<Cuerpo>>
        {
            public string Id { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper mapper) : IRequestHandler<Query, Result<Cuerpo>>
        {
            public async Task<Result<Cuerpo>> Handle(Query request, CancellationToken cancellationToken)
            {
                //await using var dbContext = await _factory.CreateAsync();

                var baseQuery = dbContext.Set<Cuerpo>().AsNoTracking()
                            .Where(x => x.Id == request.Id)
                            .Include(b => b.Regiones);
                Cuerpo? item = await baseQuery.FirstOrDefaultAsync(cancellationToken);
                
                if (item == null)
                    return Result<Cuerpo>.Failure("No se encontró el CUERPO", 404);

                return Result<Cuerpo>.Success(item);

            }
        }
    }

    public class GetCuerpoList
    {
        public class Query : IRequest<Result<List<Cuerpo>>> { }

        public class Handler(UnidadEmpleoDbContext dbContext, IMemoryCache _cache) : IRequestHandler<Query, Result<List<Cuerpo>>>
        {
            public async Task<Result<List<Cuerpo>>> Handle(Query request, CancellationToken cancellationToken)
            {
                
                var entidades = await _cache.GetOrCreateAsync($"corporacion", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(25);

                    var retorno = await dbContext.Set<Cuerpo>().Include(b => b.Regiones).AsNoTracking().ToListAsync(cancellationToken);

                    return retorno;
                });

                return Result<List<Cuerpo>>.Success(entidades);
            }
        }
    }

    

    public class GetPagedCuerpos
    {
        public class Query : IRequest<Result<PagedResult<CuerpoDto>>>
        {
            public string? alias { get; set; }
            public PaginationParams Pagination { get; set; } = new();
        }

        public class Handler(UnidadEmpleoDbContext context, IMapper _mapper) : IRequestHandler<Query, Result<PagedResult<CuerpoDto>>>
        {
            public async Task<Result<PagedResult<CuerpoDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pagination = request.Pagination ?? new PaginationParams();

                if (pagination.PageNumber <= 0 || pagination.PageSize <= 0)
                {
                    return Result<PagedResult<CuerpoDto>>.Failure("PageNumber and PageSize must be greater than zero", 400);
                }

                var validSortFields = new[] { "Alias" };
                if (!string.IsNullOrEmpty(pagination.SortBy) && !validSortFields.Contains(pagination.SortBy))
                {
                    return Result<PagedResult<CuerpoDto>>.Failure($"Invalid SortBy field: {pagination.SortBy}", 400);
                }


                var query = context.Set<Cuerpo>().AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(request.alias))
                {
                    var nombreLower = request.alias.ToLowerInvariant();
                    query = query.Where(p =>(p.alias).ToLower().Contains(nombreLower));
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

                var dtos = _mapper.Map<List<CuerpoDto>>(paged);

                var result = new PagedResult<CuerpoDto>
                {
                    Data = dtos,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize),
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };

                return Result<PagedResult<CuerpoDto>>.Success(result);
            }
        }
    }
}
