using API.Seguridad.Controllers;
using API.UnidadEmpleo.Application.CartaCompromisoApp;
using API.UnidadEmpleo.Application.ReferenciaApp;
using API.UnidadEmpleo.Application.SolicitudSvs;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;

namespace API.UnidadEmpleo.Controller
{
    public class ReferenciaController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create(ReferenciaCreate.Command[] commands)
        {
            IActionResult response = Ok(3);
            foreach (var command in commands) {
                var res = HandleResult(await Mediator.Send(command));
                if (res.GetType() != typeof(OkObjectResult) || res.GetType() != typeof(OkResult))
                    response = res;
            }
            return response;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ReferenciaUpdate.Command command)
        {
            command.IdRequest = id;
            return HandleResult(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return HandleResult(await Mediator.Send(new ReferenciaDelete.Command { Id = id }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecordId(int id)
        {
            return HandleResult(await Mediator.Send(new GetReferencia.Query { Id = id }));
        }

        [HttpGet]
        public async Task<IActionResult> ListAspirantes()
        {
            return HandleResult(await Mediator.Send(new GetReferenciaList.Query()));
        }

        [HttpGet("solicitud/{idsolicitud}")]
        public async Task<IActionResult> GetRecordBySolicitudId(int idsolicitud)
        {
            return HandleResult(await Mediator.Send(new GetReferenciaListBySolicitud.Query { idsolicitud = idsolicitud }));
        }
    }
}
