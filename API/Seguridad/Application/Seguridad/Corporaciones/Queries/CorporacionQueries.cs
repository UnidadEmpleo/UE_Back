using API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Application.Core;

namespace API.Seguridad.Application.Seguridad.Corporaciones.Queries
{
    public class GetCorporacionList
    {
        public class Query : IRequest<Result<List<CorporacionDTO>>>
        {
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<List<CorporacionDTO>>>
        {
            public async Task<Result<List<CorporacionDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var corporaciones = await context.Corporaciones.ToListAsync(cancellationToken);
                return Result<List<CorporacionDTO>>.Success(mapper.Map<List<CorporacionDTO>>(corporaciones));
            }
        }
    }

    public class GetCorporacionNombre
    {
        public class Query : IRequest<Result<CorporacionDTO>>
        {
            public string Nombre { get; set; }
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<CorporacionDTO>>
        {
            public async Task<Result<CorporacionDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var corporaciones = await context.Corporaciones.FirstOrDefaultAsync(r => r.Nombre == request.Nombre);
                return Result<CorporacionDTO>.Success(mapper.Map<CorporacionDTO>(corporaciones));
            }
        }
    }
}