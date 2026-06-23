namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa una corporación en el sistema.
    /// </summary>
    public class Corporacion
    {
        /// <summary>
        /// Identificador único de la corporación.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre de la corporación.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Descripción de la corporación.
        /// </summary>
        public string? Descripcion { get; set; }
        /// <summary>
        /// Logo de la corporación.
        /// </summary>
        public string? Logo { get; set; } = null;
        public string? LogoDerechoReporte { get; set; }
        public ICollection<UsuarioCorporacion> UsuarioCorporaciones { get; set; } = new List<UsuarioCorporacion>();
    }
}
