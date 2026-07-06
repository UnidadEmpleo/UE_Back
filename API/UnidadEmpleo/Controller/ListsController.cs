
using API.UnidadEmpleo.Application.Lists;
using Microsoft.AspNetCore.Mvc;
using API.Seguridad.Controllers;


namespace API.UnidadEmpleo.Controller
{
    public class ListsController : BaseApiController
    {
        [HttpGet("corporacion")]
        public async Task<IActionResult> GetCorporaciones()
        {
            return HandleResult(await Mediator.Send(new GetCorporacionesQuery()));
        }

        [HttpGet("region")]
        public async Task<IActionResult> GetRegiones() {
            return HandleResult(await Mediator.Send(new GetRegionesQuery()));
        }
    }
}
