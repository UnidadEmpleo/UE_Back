using API.Seguridad.Controllers;
using API.UnidadEmpleo.Application.SolicitudSvs;
using Microsoft.AspNetCore.Mvc;

namespace API.UnidadEmpleo.Controller
{
    public class SolicitudController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create(SolicitudCreate.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }
        

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SolicitudUpdate.Command command)
        {
            command.IdRequest = id;
            return HandleResult(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return HandleResult(await Mediator.Send(new SolicitudDelete.Command { Id = id}));
        }

        [HttpPost("list")]
        public async Task<IActionResult> selectByPerfil(GetSolicitudByPerfil2.Query query)
        {
            return HandleResult(await Mediator.Send(query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecordId(int id)
        {
            return HandleResult(await Mediator.Send(new GetSolicitud.Query { Id = id }));
        }

        [HttpGet]
        public async Task<IActionResult> ListSolicitudes()
        {
            return HandleResult(await Mediator.Send(new GetSolicitudList.Query()));
        }

        [HttpGet("Aspirante/{curp}")]
        public async Task<IActionResult> ListSolicitudesPorAspirante(string curp)
        {
            return HandleResult(await Mediator.Send(new GetSolicitudListByAspirante.Query { Curp = curp }));
        }

        /*
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedAspirantes(
            [FromQuery] string? Nombre,
            [FromQuery] string? Curp,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = "Nombre",
            [FromQuery] string sortDirection = "desc")
        {
            var query = new GetPagedAspirantes.Query
            {
                Nombre = Nombre,
                Curp = Curp,
                Pagination = new PaginationParams
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                }
            };

            var result = await Mediator.Send(query);
            return HandleResult(result);
        }
        */
    }
}
