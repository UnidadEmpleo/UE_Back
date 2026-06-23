using API.Seguridad.Infrastructure;

namespace API.Seguridad.Middleware
{
    public class CorporacionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorporacionMiddleware> _logger;

        public CorporacionMiddleware(RequestDelegate next, ILogger<CorporacionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ICorporacionContextAccessor accessor)
        {
            if (context.Request.Headers.TryGetValue("CorporacionId", out var corpId))
            {
                accessor.CorporacionId = corpId;
                _logger.LogInformation("CorporacionId recibido: {0}", corpId);
            }
            else
            {
                _logger.LogWarning("CorporacionId no encontrado en los headers.");
            }

            if (context.Request.Headers.TryGetValue("SistemaId", out var sistemaId))
            {
                accessor.SistemaId = sistemaId;
                _logger.LogInformation("SistemaId recibido: {0}", sistemaId);
            }
            else
            {
                _logger.LogWarning("SistemaId no encontrado en los headers.");
            }

            await _next(context);
        }
    }
}
