namespace API.Seguridad.DTOs.AuditLogs
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string EventType { get; set; }
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
