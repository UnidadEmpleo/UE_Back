using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.DTOs.Seguridad;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.Seguridad.Usuarios.Queries
{
    public class GetUserList
    {
        public class Query : IRequest<Result<List<UserDto>>>
        {
        }

        public class Handler(AppDbContext context, 
            IMapper mapper) : IRequestHandler<Query, Result<List<UserDto>>>
        {
            public async Task<Result<List<UserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = await context
                    .Usuarios
                    .Include(context => context.AppUserIdentity)
                    .ToListAsync(cancellationToken);

                if (!users.Any())
                {
                    return Result<List<UserDto>>.Failure("No se encontraron usuarios", 404);
                }

                var usersDto = mapper.Map<List<UserDto>>(users);

                return Result<List<UserDto>>.Success(usersDto);
            }
        }
    }

    public class GetUserById
    {
        public class Query : IRequest<Result<UserDto>>
        {
            public string Id { get; set; }
        }

        public class Handler(IMapper mapper,
            AppDbContext context,
            IConfiguration configuration) : IRequestHandler<Query, Result<UserDto>>
        {
            public async Task<Result<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await context
                    .Usuarios
                    .Include(Usuarios => Usuarios.UsuarioCorporaciones)
                    .Include(Usuarios => Usuarios.AppUserIdentity)
                    .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (user == null)
                {
                    return Result<UserDto>.Failure("Usuario no encontrado", 404);
                }

                var userDto = mapper.Map<UserDto>(user);               

                var permisos = await context.Permisos
                    .Where(p => p.UsuarioId == user.Id)
                    .ToListAsync(cancellationToken);

                userDto.Permisos = permisos.Select(p => new PermisoDTO
                {
                    ProcesoId = p.ProcesoId.Value,
                    RolId = p.RolId,
                    UsuarioId = p.UsuarioId,
                    Acceso = p.Acceso,
                    Id = p.Id
                })
                .ToList();

                return Result<UserDto>.Success(userDto);
            }
         }
    }
}
