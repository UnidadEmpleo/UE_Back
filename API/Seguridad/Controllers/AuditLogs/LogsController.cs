using API.Seguridad.Application.AuditLogs.Queries;
using API.Seguridad.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Seguridad.Controllers.AuditLogs
{
    public class LogsController : BaseApiController
    {
        [HttpPost("filtered")]
        public async Task<ActionResult> GetFilteredLogs([FromBody] LogQueries.GetFilteredLogs.Query query)
        {
            return HandleResult(await Mediator.Send(query));
        }
    }
}
