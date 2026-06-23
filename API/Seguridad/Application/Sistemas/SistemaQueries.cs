using API.Common.Domain;
using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.Sistemas
{
    public class GetSistemasList
    {
        public class Query : IRequest<Result<List<SistemaDto>>>
        {
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<List<SistemaDto>>>
        {
            public async Task<Result<List<SistemaDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var sistemas = await context.Sistemas.ToListAsync(cancellationToken);

                return Result<List<SistemaDto>>.Success(mapper.Map<List<SistemaDto>>(sistemas));
            }
        }
    }

    public class GetSistemaById
    {
        public class Query : IRequest<Result<SistemaDto>>
        {
            public short Id { get; set; }
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<SistemaDto>>
        {
            public async Task<Result<SistemaDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var entidad = await context.Sistemas.FirstOrDefaultAsync(r => r.Id == request.Id);

                if (entidad == null)
                {
                    return Result<SistemaDto>.Failure("Sistema no encontrado", 404);
                }

                return Result<SistemaDto>.Success(mapper.Map<SistemaDto>(entidad));
            }
        }
    }
}
