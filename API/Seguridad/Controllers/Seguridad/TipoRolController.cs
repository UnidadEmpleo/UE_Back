using API.Seguridad.Application.Seguridad.Corporaciones.Queries;
using API.Seguridad.Application.Seguridad.TipoRol;
using API.Seguridad.DTOs.Seguridad;
using Microsoft.AspNetCore.Mvc;

namespace API.Seguridad.Controllers.Seguridad
{
    public class TipoRolController : BaseApiController
    {

        /// <summary>         
        /// Obtiene la lista de Tipos de Rol.
        /// </summary>
        /// <param </param>
        /// <returns>Lista de Tipos de Rol.</returns>
        /// <response code="200">Lista de Tipos obtenida correctamente.</response>
        /// <response code="404">No se encontraron TiposRol.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        public async Task<ActionResult<List<TipoRolDto>>> GetTiposRol()
        {
            return HandleResult(await Mediator.Send(new GetTipoRolList.Query()));
        } 
    }
}
