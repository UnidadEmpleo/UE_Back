using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.Seguridad.Roles.Queries
{
    public class GetRolList
    {
        public class Query : IRequest<Result<List<ApplicationRoleDto>>>
        {
        }

        public class Handler(IMapper mapper, 
            RoleManager<AppIdentityRole> roleManager,
            IConfiguration configuration) : IRequestHandler<Query, Result<List<ApplicationRoleDto>>>
        {
            public async Task<Result<List<ApplicationRoleDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = new List<ApplicationRoleDto>();
                
                var roles = await roleManager
                    .Roles
                    .Where(r => r.Name != "SuperAdmin") // Exclude SuperAdmin role
                    .ToListAsync(cancellationToken);

                return Result<List<ApplicationRoleDto>>.Success(mapper.Map<List<ApplicationRoleDto>>(roles));
            }
        }
    }

    public class GetRoleById
    {
        public class Query : IRequest<Result<ApplicationRoleDto>>
        {
            public string Id { get; set; }
        }

        public class Handler(RoleManager<AppIdentityRole> roleManager,
            UserManager<AppUserIdentity> userManager,
            IMapper mapper, 
            AppDbContext context, 
            IConfiguration configuration) : IRequestHandler<Query, Result<ApplicationRoleDto>>
        {
            public async Task<Result<ApplicationRoleDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var role = await roleManager
                    .Roles
                    .FirstOrDefaultAsync(r => r.Id == request.Id);

                if (role == null)
                {
                    return Result<ApplicationRoleDto>.Failure("Rol no encontrado", 404);
                }

                var userDtos = new List<UserDto>();

                // Obtiene los usuarios que pertenecen al rol especificado
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);

                if (usersInRole != null && usersInRole.Any())
                {
                    var appUserIds = usersInRole.Select(u => u.Id).ToList();

                    var usuarios = await context.Usuarios
                        .Where(u => appUserIds.Contains(u.AppUserIdentityId))
                        .ToListAsync(cancellationToken);

                    userDtos = mapper.Map<List<UserDto>>(usuarios);
                }

                // Obtener los procesos asociados al rol desde la base de datos
                var procesos = await context.RolesProcesos
                    .Where(rp => rp.RolId == request.Id.ToString())
                    .Select(rp => rp.Proceso)
                    .ToListAsync(cancellationToken);

                var procesoDtos = mapper.Map<List<Proceso>>(procesos);

                // Mapear el rol a ApplicationRoleDto
                var roleDto = mapper.Map<ApplicationRoleDto>(role);

                // Agregar los usuarios y procesos al DTO del rol
                roleDto.AssignedUsers = userDtos;
                roleDto.Procesos = procesoDtos;

                return Result<ApplicationRoleDto>.Success(roleDto);
            }
        }
    }
}