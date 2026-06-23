using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Seguridad.Application.Seguridad.Usuarios.Commands;
using API.Seguridad.Application.Seguridad.Usuarios.Queries;

namespace API.Seguridad.Controllers.Seguridad
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseApiController
    {
        
        private readonly UsuarioCacheService _usuarioCacheService;
        
        public UsersController(UsuarioCacheService usuarioCacheService) : base()
        {
            _usuarioCacheService = usuarioCacheService;
        }

        [HttpGet]
        [AuthorizeByTipoRol(TipoRoles.Administrador)]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            return HandleResult(await Mediator.Send(new GetUserList.Query()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            return HandleResult(await Mediator.Send(new GetUserById.Query { Id = id }));
        }

        [HttpPost]
        [AuthorizeByTipoRol(TipoRoles.Administrador)]
        public async Task<ActionResult<string>> CreateUser([FromBody] CreateUser.Command command)
        {
            _usuarioCacheService.ClearCache();
            return HandleResult(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        [AuthorizeByTipoRol(TipoRoles.Administrador)]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUser.Command command)
        {
            command.Id = id;
            return HandleResult(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [AuthorizeByTipoRol(TipoRoles.Administrador)]
        public async Task<ActionResult> DeleteUser(string id)
        {
            return HandleResult(await Mediator.Send(new ActiveInactiveUser.Command { Id = id, Activo = false }));
        }

        [HttpPost("clear-cache")]
        public IActionResult ClearCache()
        {
            _usuarioCacheService.ClearCache();
            return Ok("Cache de usuarios limpiado.");
        }
        
    }
}
