using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.DTOs.Seguridad;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.Seguridad.Grupos.Queries
{
    public class GetGrupoList
    {
        public class Query : IRequest<Result<List<GroupDto>>>
        {
        }

        public class Handler(AppDbContext context, 
            IMapper mapper,
            IConfiguration configuration) : IRequestHandler<Query, Result<List<GroupDto>>>
        {
            public async Task<Result<List<GroupDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var grupos = await context
                    .Grupos
                    .ToListAsync(cancellationToken);

                var groupsDto = mapper.Map<List<GroupDto>>(grupos);                

                return Result<List<GroupDto>>.Success(groupsDto);
            }
        }
    }

    public class GetGrupoById
    {
        public class Query : IRequest<Result<GroupDto>>
        {
            public string Id { get; set; }
        }

        public class Handler(AppDbContext context, 
            IMapper mapper,
            IConfiguration configuration) : IRequestHandler<Query, Result<GroupDto>>
        {
            public async Task<Result<GroupDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var grupo = await context
                    .Grupos
                    .Include(g => g.Usuarios)
                    .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);   

                if (grupo == null)
                {
                    return Result<GroupDto>.Failure($"Grupo {request.Id} no encontrado", 404);
                }

                var groupDto = mapper.Map<GroupDto>(grupo);
                var userDtos = mapper.Map<List<UserDto>>(grupo.Usuarios);
                
                groupDto.Usuarios = userDtos;

                return Result<GroupDto>.Success(groupDto);
            }
        }
    }
}
