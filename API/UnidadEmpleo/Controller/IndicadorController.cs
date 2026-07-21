using API.Seguridad.Controllers;
using API.UnidadEmpleo.Application.AspiranteApp;
using API.UnidadEmpleo.Application.Informes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.UnidadEmpleo.Controller
{
    public class IndicadorController : BaseApiController
    {
        
        [HttpPost]
        public async Task<IActionResult> Indicadores(GetIndicadores.Query filters)
        {
            return HandleResult(await Mediator.Send(filters));
        }
    }
}
