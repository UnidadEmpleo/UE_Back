using API.Seguridad.Application.Seguridad.Roles.Commands;
using API.Seguridad.Application.Seguridad.Roles.Queries;
using API.Seguridad.Application.Sistemas;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Seguridad.Controllers.Seguridad
{
    public class SistemasController : BaseApiController
    {

        [HttpGet]
        [AuthorizeByTipoRol(TipoRoles.Administrador)]
        public async Task<ActionResult<List<Sistema>>> GetSistemas()
        {
            return HandleResult(await Mediator.Send(new GetSistemasList.Query()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationRoleDto>> GetSistema(short id)
        {
            return HandleResult(await Mediator.Send(new GetSistemaById.Query { Id = id }));
        }

    }
}
