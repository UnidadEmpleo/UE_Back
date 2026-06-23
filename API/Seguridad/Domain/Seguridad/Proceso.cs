using API.Seguridad.Domain.Audit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa un proceso en el sistema.
    /// </summary>
    public class Proceso : IAuditable
    {
        /// <summary>
        /// Identificador único del proceso.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descripción del proceso.
        /// </summary>
        public required string Descr { get; set; }

        /// <summary>
        /// Tipo del proceso. P: Proceso, S: Subproceso
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Icono asociado al proceso.
        /// </summary>
        public string? Icono { get; set; }

        /// <summary>
        /// Indica si el proceso está activo o no.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Ruta del proceso.
        /// </summary>
        public string? Ruta {  get; set; }

        /// <summary>
        /// Identificador del proceso padre, si aplica.
        /// </summary>
        public int? ProcesoPadreId { get; set; }

        /// <summary>
        /// Fecha de creación del proceso.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de la última actualización del proceso.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Acciones asociadas al proceso, como "Crear", "Editar", "Eliminar".
        /// </summary>
        [MaxLength(150)]
        public string? Acciones { get; set; } // Acciones asociadas al proceso, como "Crear", "Editar", "Eliminar"  

        // Relación auto-referenciada
        [JsonIgnore]
        public Proceso ProcesoPadre { get; set; }

        /// <summary>
        /// Colección de procesos hijos (subprocesos).
        /// </summary>
        public ICollection<Proceso> Subprocesos { get; set; } = []!;

        public ICollection<RolProceso> Roles { get; set; } = []!;

        public short SistemaId { get; set; }
        public Sistema Sistema { get; set; }

    }
}