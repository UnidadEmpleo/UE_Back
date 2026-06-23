using API.Common.App.Rgn;
using API.Seguridad.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace API.Common.Controller
{
    public class RegionController : BaseApiController
    {


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetRegion(int Id)
        {
            return HandleResult(await Mediator.Send(new GetRegion.Query { Id = Id }));
        }

        [HttpGet]
        public async Task<IActionResult> ListRegion()
        {
            return HandleResult(await Mediator.Send(new GetRegionList.Query()));
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedRegiones(
            [FromQuery] string? region,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = "Alias",
            [FromQuery] string sortDirection = "desc")
        {
            var query = new GetPagedRegiones.Query
            {
                region = region,

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
