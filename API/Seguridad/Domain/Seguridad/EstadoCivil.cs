using System.ComponentModel.DataAnnotations;

namespace API.Seguridad.Domain.Seguridad
{
    public class EstadoCivil
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        /// <summary>
        /// Fecha en la que se creó el registro.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de la última actualización del registro.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow;
    }
}