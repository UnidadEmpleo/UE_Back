using API.Seguridad.Domain.Audit;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa un permiso asignado a un usuario o rol.
    /// </summary>
    public class Permiso : IAuditable
    {
        /// <summary>
        /// Identificador único del permiso.
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Identificador del usuario asociado al permiso.
        /// </summary>
        public string? UsuarioId { get; set; } // Mantener como referencia lógica, pero sin relación directa

        /// <summary>
        /// Identificador del rol asociado al permiso.
        /// </summary>
        public string? RolId { get; set; }

        /// <summary>
        /// Identificador del proceso asociado al permiso.
        /// </summary>
        public int? ProcesoId { get; set; }

        public Proceso Proceso { get; set; } = null!;

        /// <summary>
        /// Nivel de acceso otorgado por el permiso. "lectura", "escritura"
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string? Acceso { get; set; } // Valores posibles: "lectura", "escritura"
    }
}
