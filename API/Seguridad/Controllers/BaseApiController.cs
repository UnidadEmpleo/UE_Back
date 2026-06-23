
using Microsoft.AspNetCore.Mvc;
using MediatR;
using API.Seguridad.Application.Core;
using API.Seguridad.Application.Core.Audit;

namespace API.Seguridad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;
        public IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetService<IMediator>()
                ?? throw new Exception("IMediator services is unavailable");

        private IAuditService? _auditService;
        public IAuditService AuditService =>
            _auditService ??= HttpContext.RequestServices.GetService<IAuditService>()
                ?? throw new Exception("IAuditService services is unavailable");

        public ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess && result.Value != null)
            {
                return Ok(result.Value);
            }

            if (!result.IsSuccess && result.Code == 404)
            {
                return NotFound(result.Error);
            }

            return BadRequest(result.Error);
        }
        public ActionResult HandleResult<T>(Result<T[]> result)
        {
            if (result.IsSuccess && result.Value != null)
            {
                return Ok(result.Value);
            }

            if (!result.IsSuccess && result.Code == 404)
            {
                return NotFound(result.Error);
            }

            return BadRequest(result.Error);
        }
    }
}