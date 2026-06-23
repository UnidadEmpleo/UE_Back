using API.Seguridad.Domain.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa un grupo de usuarios en el sistema.
    /// </summary>
    public class Grupo : IAuditable
    {
        /// <summary>
        /// Identificador único del grupo.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre del grupo.
        /// </summary>
        public required string Nombre { get; set; }

        /// <summary>
        /// Descripción del grupo.
        /// </summary>
        public string Descr { get; set; }

        /// <summary>
        /// Indica si el grupo está activo.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Fecha en la que se creó el grupo.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow; // Fecha y hora en UTC

        /// <summary>
        /// Fecha de la última actualización del grupo.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow; // Fecha y hora en UTC

        /// <summary>
        /// Colección de usuarios asociados al grupo.
        /// </summary>
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}