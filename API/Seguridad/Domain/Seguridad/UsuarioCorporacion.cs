using API.Seguridad.Domain.Audit;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa la relación entre un usuario y una corporación.
    /// </summary>
    public class UsuarioCorporacion : IAuditable
    {
        /// <summary>
        /// Identificador del usuario asociado.
        /// </summary>
        public string UsuarioId { get; set; }
        
        /// <summary>
        /// Usuario asociado a la corporación.
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Identificador de la corporación asociada.
        /// </summary>
        public string CorporacionId { get; set; }

        /// <summary>
        /// Corporación asociada al usuario.
        /// </summary>
        public Corporacion Corporacion { get; set; }
    }
}
