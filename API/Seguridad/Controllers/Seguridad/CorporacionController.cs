using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Controllers;
using Microsoft.AspNetCore.Mvc;
using API.Seguridad.Application.Seguridad.Corporaciones.Queries;

namespace API.Seguridad.Controllers.Seguridad
{
    /// <summary>
    /// Controlador para gestionar las corporaciones.
    /// </summary>
    public class CorporacionController : BaseApiController
    {
        
        /// <summary>         
        /// Obtiene la lista de corporaciones.
        /// </summary>
        /// <param </param>
        /// <returns>Lista de corporaciones.</returns>
        /// <response code="200">Lista de corporaciones obtenida correctamente.</response>
        /// <response code="404">No se encontraron corporaciones.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        public async Task<ActionResult<List<CorporacionDTO>>> GetCorporaciones()
        {
            return HandleResult(await Mediator.Send(new GetCorporacionList.Query()));
        }
        
    }
}
