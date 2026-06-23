using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Refresh token persistence model (hashed token stored, raw token only sent once to client).
    /// </summary>
    public class UserRefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string TokenHash { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? ReplacedByTokenHash { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReasonRevoked { get; set; }

        [Required]
        public DateTime Expires { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;
        [NotMapped]
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
