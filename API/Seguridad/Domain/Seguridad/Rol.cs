using API.Seguridad.Domain.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa un rol en el sistema.
    /// </summary>
    public class Rol : IAuditable
    {
        public string Id { get; set; }
        public required string Nombre { get; set; }
        public string Descr { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
        public ICollection<RolProceso> Procesos { get; set; } = new List<RolProceso>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}