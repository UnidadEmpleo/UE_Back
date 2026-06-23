using API.Seguridad.Domain.Audit;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa la relación entre un rol y un proceso.
    /// </summary>
    public class RolProceso : IAuditable
    {
        /// <summary>
        /// Identificador del rol asociado.
        /// </summary>
        public string? RolId { get; set; }

        /// <summary>
        /// Identificador del proceso asociado.
        /// </summary>
        public int? ProcesoId { get; set; }

        /// <summary>
        /// Proceso asociado al rol.
        /// </summary>
        public Proceso Proceso { get; set; } = null!;

        /// <summary>
        /// Fecha de creación de la relación.
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de la última actualización de la relación.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; }        
    }
}