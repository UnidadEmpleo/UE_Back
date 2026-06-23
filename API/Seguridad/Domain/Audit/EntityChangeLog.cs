using static API.Seguridad.Infrastructure.Audit.Enumerations.Audits;

namespace API.Seguridad.Domain.Audit
{
    public class EntityChangeLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
        public string UserName { get; set; }
        public EntityActionType ChangeType { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string? Changes { get; set; } // JSON con los valores nuevos y/o antiguos
    }

}
