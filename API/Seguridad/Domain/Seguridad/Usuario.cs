using API.Seguridad.Domain.Audit;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa un usuario en el sistema.
    /// </summary>
    public class Usuario : IAuditable
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Primer apellido del usuario.
        /// </summary>
        public string PrimerApellido { get; set; }

        /// <summary>
        /// Segundo apellido del usuario.
        /// </summary>
        public string? SegundoApellido { get; set; }

        public string NombreCompleto => $"{Nombre} {PrimerApellido} {SegundoApellido}";

        /// <summary>
        /// Teléfono del usuario.
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Indica si el usuario está activo.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Tiempo de inactividad en minutos.
        /// </summary>
        public short TiempoInactividad { get; set; } = 30; // Tiempo en minutos

        /// <summary>
        /// Fecha de creación del usuario.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow; // Fecha y hora en UTC

        /// <summary>
        /// Fecha de la última actualización del usuario.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow; // Fecha y hora en UTC

        /// <summary>
        /// Identificador del Cuerpo laboral
        /// </summary>
        public string? CuerpoId { get; set; }
        /// <summary>
        /// Identificador de la Área o Región laboral
        /// </summary>
        public int RegionId {  get; set; }

        /// <summary>
        /// Identificador del grupo al que pertenece el usuario.
        /// </summary>
        public string? GrupoId { get; set; }

        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public string? RolId { get; set; }

        /// <summary>
        /// Navegación al rol asociado al usuario.
        /// </summary>
        [ForeignKey(nameof(RolId))]
        public AppIdentityRole? Rol { get; set; }

        /// <summary>
        /// Indica si el usuario es un administrador de dominio (AD).
        /// </summary>
        public bool EsUsuarioAD { get; set; } = false;

        // Relación uno a uno con AppUserIdentity
        /// <summary>
        /// Identificador de la identidad del usuario en la aplicación.
        /// </summary>
        public string? AppUserIdentityId { get; set; }
        public AppUserIdentity AppUserIdentity { get; set; }

        /// <summary>
        /// Relación uno a muchos con la entidad UsuarioGrupo.
        /// </summary>
        public ICollection<UsuarioCorporacion> UsuarioCorporaciones { get; set; } = new List<UsuarioCorporacion>();

        /// <summary>
        /// Contiene un JSON dinámico con atributos personalizados que varían según el rol del usuario.
        /// El contenido y la estructura del JSON dependen del tipo de rol (propiedad TipoDeRol) asignado al usuario.
        /// </summary>
        [JsonProperty] 
        public string Atributos { get; set; } = string.Empty;

        /// <summary>
        /// Contiene un JSON con informacion de configuracion del calendario
        /// El contenido y la estructura del JSON dependen del tipo de rol (propiedad TipoDeRol) asignado al usuario.
        /// </summary>
        [JsonProperty] 
        public string Calendario { get; set; } = string.Empty;

        /// <summary>
        /// Valida si el contenido de la propiedad Atributos es un JSON válido.
        /// </summary>
        /// <returns>True si es un JSON válido, false en caso contrario.</returns>
        public bool EsAtributosJsonValido()
        {
            if (string.IsNullOrWhiteSpace(Atributos))
                return true;

            try
            {
                Newtonsoft.Json.Linq.JToken.Parse(Atributos);
                return true;
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return false;
            }
        }

        // Plan (pseudocódigo):
        // - Evitar acceso a propiedad cuando Rol es null.
        // - Reemplazar la validación explícita por el operador condicional nulo (?.),
        //   que devolverá false si Rol es null sin lanzar excepción.
        // - Mantener la firma del método y el comportamiento esperado.

      
    }
}