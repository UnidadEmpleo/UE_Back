using API.Common.App;
using API.Seguridad.Controllers;
using API.UnidadEmpleo.Application.Cuerpoa;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Common.Controller
{
    public class CodigoPostalController : BaseApiController
    {
        [HttpGet("{codigopostal}")]
        public async Task<IActionResult> GetDatosAsentaByCP(int codigopostal)
        {
            return HandleResult(await Mediator.Send(new GetDatosAsentamientoByCP.Query { c_codigo = codigopostal }));
        }

    }


}
