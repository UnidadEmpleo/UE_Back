namespace API.Seguridad.DTOs.AuditLogs
{
    public class LogEntryDto
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? Level { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? Exception { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }   
    }
}
