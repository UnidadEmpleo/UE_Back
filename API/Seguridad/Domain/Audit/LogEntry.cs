using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Seguridad.Domain.Audit
{
    [Table("Logs")]
    public class LogEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Message { get; set; }

        public string? MessageTemplate { get; set; }

        public string? Level { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string? Exception { get; set; }

        public string? Properties { get; set; }

        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public string? RequestPath { get; set; }

        public string? RequestMethod { get; set; }

        public int? StatusCode { get; set; }

        public float? Elapsed { get; set; }
    }
}
