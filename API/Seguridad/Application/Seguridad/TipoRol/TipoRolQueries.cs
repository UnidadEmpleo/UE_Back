using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.Seguridad.TipoRol
{
    public class GetTipoRolList
    {
        public class Query : IRequest<Result<List<TipoRolDto>>>
        {
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<List<TipoRolDto>>>
        {
            public async Task<Result<List<TipoRolDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var items = await context.TipoRoles.ToListAsync(cancellationToken);

                List<TipoRolDto> tipos = items.Select(p => new TipoRolDto{Id = p.Id,Name = p.Name, Activo = p.Activo}).ToList();

                return Result<List<TipoRolDto>>.Success(tipos);
            }
        }
    }
}
