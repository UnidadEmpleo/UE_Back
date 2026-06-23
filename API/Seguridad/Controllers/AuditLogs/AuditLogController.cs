using System.Collections.Generic;
using System.Threading.Tasks;
using API.Seguridad.Controllers;
using API.Seguridad.DTOs.AuditLogs;
using Microsoft.AspNetCore.Mvc;
using static API.Seguridad.Application.AuditLogs.Queries.AuditLogQueries;

namespace API.Seguridad.Controllers.AuditLogs
{
    /// <summary>
    /// Controlador para gestionar los registros de auditoría.
    /// </summary>
    public class AuditLogController : BaseApiController
    {
        /// <summary>
        /// Obtiene los registros de auditoría.
        /// </summary>
        /// <param name="query" >Consulta para filtrar los registros de auditoría.</param>
        /// <returns>Una lista de registros de auditoría.</returns>
        /// <response code="200">Devuelve la lista de registros de auditoría.</response>
        /// <response code="400">Si los parámetros de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error en el servidor.</response>

        [HttpPost("filtered")]
        public async Task<ActionResult<List<AuditLogDto>>> GetFilteredAuditLogs(           
            [FromBody] GetFilteredAuditLogs.Query query)
        {
            return HandleResult(await Mediator.Send(query));
        }
    }
}
