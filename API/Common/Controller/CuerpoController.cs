using API.Seguridad.Controllers;
using API.UnidadEmpleo.Application.Cuerpoa;
using Microsoft.AspNetCore.Mvc;


namespace API.Common.Controller
{
    public class CuerpoController : BaseApiController
    {


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCuerp(string Id)
        {
            return HandleResult(await Mediator.Send(new GetCuerpo.Query { Id = Id }));
        }

        [HttpGet]
        public async Task<IActionResult> ListCuerpo()
        {
            return HandleResult(await Mediator.Send(new GetCuerpoList.Query()));
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedCuerpos(
            [FromQuery] string? Alias,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = "Alias",
            [FromQuery] string sortDirection = "desc")
        {
            var query = new GetPagedCuerpos.Query
            {
                alias = Alias,
                
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
