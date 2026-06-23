using API.Seguridad.Domain.Audit;
using API.Persistence;
using static API.Seguridad.Infrastructure.Audit.Enumerations.Audits;

namespace API.Seguridad.Application.Core.Audit
{
    public class AuditService(AppDbContext context) : IAuditService
    {
        public async Task LogAsync(AuditEventType eventType, string userId, string userName, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            var audit = new AuditLog
            {
                EventType = eventType,
                UserId = userId,
                UserName = userName,
                Description = description,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            context.AuditLogs.Add(audit);
            await context.SaveChangesAsync();
        }
    }

}
