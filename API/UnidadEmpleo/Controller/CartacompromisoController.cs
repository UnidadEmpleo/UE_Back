using API.Seguridad.Controllers;
using API.UnidadEmpleo.Application.CartaCompromisoApp;
using API.UnidadEmpleo.Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.UnidadEmpleo.Controller
{
    public class CartacompromisoController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create(CartaCompromisoCreate.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CartaCompromisoUpdate.Command command)
        {
            command.IdRequest = id;
            return HandleResult(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return HandleResult(await Mediator.Send(new CartaCompromisoDelete.Command { Id = id }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecordId(int id)
        {
            return HandleResult(await Mediator.Send(new GetCartaCompromiso.Query { Id = id }));
        }

        [HttpGet]
        public async Task<IActionResult> ListRecords()
        {
            return HandleResult(await Mediator.Send(new GetCartaCompromisoList.Query()));
        }
        
        [HttpGet("solicitud/{idsolicitud}")]
        public async Task<IActionResult> ListRecordsBySolicitud(int idsolicitud)
        {
            return HandleResult(await Mediator.Send(new GetCartaCompromisoListBySolicitud.Query { IdSolicitud = idsolicitud }));
        }
    }
}
