using System.Security.Claims;
using System.Text.Json;
using API.Seguridad.Application.Core.Audit;
using API.Seguridad.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;

namespace API.Seguridad.Middleware
{
    public class AuditMiddleware(ILogger<ExceptionMiddleware> logger,
        IAuditService auditService, IHostEnvironment env) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Omitir auditoría para endpoints específicos (ej: swagger, health checks)
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/health"))
            {
                await next(context);
                return;
            }

            var userId = context
                .User
                .Claims
                .FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value ?? "System";
            
            var userName = context
                .User
                .Identity?
                .Name ?? "System";

            var userIp = context.Connection.RemoteIpAddress?.ToString();

            if ((Convert.ToString(context.Request.Path).Contains("/api/authazure/login", StringComparison.OrdinalIgnoreCase) ||
                 Convert.ToString(context.Request.Path).Contains("/api/auth/login", StringComparison.OrdinalIgnoreCase)) &&
                context.Request.Method == "POST")
            {
                // Obtener el nombre de usuario del cuerpo de la solicitud
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();               
                var requestBodyJson = JsonSerializer.Deserialize<JsonElement>(requestBody);
                var usernameFromBody = requestBodyJson.GetProperty("userName").GetString();

                userName = usernameFromBody ?? userName;

                await auditService.LogAsync(Enumerations.Audits.AuditEventType.Login, userId, userName, string.Empty, userIp);
            }
            else if ((context.Request.Path.Equals("/api/authazure/logout", StringComparison.OrdinalIgnoreCase) ||
                      context.Request.Path.Equals("/api/auth/logout", StringComparison.OrdinalIgnoreCase)) &&
                     context.Request.Method == "POST")
            {
                await auditService.LogAsync(Enumerations.Audits.AuditEventType.Logout, userId, userName, string.Empty, userIp);
            }

            await next(context);
        }
    }
}
