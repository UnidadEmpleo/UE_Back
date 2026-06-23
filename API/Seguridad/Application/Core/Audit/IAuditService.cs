using static API.Seguridad.Infrastructure.Audit.Enumerations.Audits;

namespace API.Seguridad.Application.Core.Audit
{
    public interface IAuditService
    {
        Task LogAsync(AuditEventType eventType, string userId, string userName, string? description = null, string? ipAddress = null, string? userAgent = null);
    }
}
