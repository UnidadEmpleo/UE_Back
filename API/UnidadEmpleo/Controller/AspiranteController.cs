
using API.Common.Domain;
using API.Persistence;
using API.Seguridad.Application.Seguridad.Corporaciones.Queries;
using API.Seguridad.Controllers;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Infrastructure;
using API.Seguridad.Infrastructure.Authorization;
using API.UnidadEmpleo.Application.AspiranteApp;
using API.UnidadEmpleo.Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.UnidadEmpleo.Controller
{
    
    public class AspiranteController : BaseApiController
    {
        private readonly AppDbContext _context;
        
        public AspiranteController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        
        public async Task<IActionResult> Create(AspiranteCreate.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }
        
        [HttpPut("{curp}")]
        
        public async Task<IActionResult> Update(string curp, AspiranteUpdate.Command command)
        {
            command.CurpRequest = curp;
            return HandleResult(await Mediator.Send(command));
        }

        [HttpDelete("{curp}")]
        [AuthorizeByTipoRol(TipoRoles.Gerente)]
        public async Task<IActionResult> Delete(string curp)
        {
            return HandleResult(await Mediator.Send(new AspiranteDelete.Command { Curp = curp }));
        }

        [HttpGet("{curp}/{region}/{perfil}/{cuerpo}")]
        public async Task<IActionResult> GetAspirante(string curp, string cuerpo, int region, int perfil)
        {
            return HandleResult(await Mediator.Send(new GetAspirante.Query { Curp = curp, cuerpoId = cuerpo, perfil = perfil, region = region }));
        }

        [HttpGet]
        public async Task<IActionResult> ListAspirantes()
        {
            return HandleResult(await Mediator.Send(new GetAspiranteList.Query()));
        }

        [HttpPost("perfil")]
        public async Task<IActionResult> ListAspirantesByX(GetAspiranteCapturaGerencia.Query options)  //string cuerpo, int region, int perfil,int sit)
        {
                return HandleResult(await Mediator.Send(options));
                //new GetAspiranteCapturaGerencia.Query { cuerpoId = cuerpo,regionId = region, perfilId = perfil, situacion = sit}));
        }


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
    }
}
