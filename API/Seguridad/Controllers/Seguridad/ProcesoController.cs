using System.Threading.Tasks;
using API.Seguridad.Domain;
using Microsoft.AspNetCore.Mvc;
using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Application.Seguridad.Procesos.Commands;
using API.Seguridad.Application.Seguridad.Procesos.Queries;

namespace API.Seguridad.Controllers.Seguridad
{
    public class ProcesoController : BaseApiController
    {
        
        [HttpGet]
        public async Task<ActionResult<List<ProcesoDto>>> GetProcesos()
        {
            return await Mediator.Send(new GetProcesoList.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcesoDto>> GetProceso(int id)
        {
            return HandleResult(await Mediator.Send(new GetProcesoDetails.Query { Id = id }));
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateProceso([FromBody] CreateProceso.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProceso(int id, [FromBody] UpdateProceso.Command command)
        {
            command.Id = id;
            return HandleResult(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProceso(int id)
        {
            return HandleResult(await Mediator.Send(new DeleteProceso.Command { Id = id }));
        }
        
    }
}