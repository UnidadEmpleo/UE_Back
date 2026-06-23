namespace API.Common.Domain
{
    public class Area
    {

        /// <summary>
        /// Identificador único del área.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descripción del área.
        /// </summary>
        public required string Descr { get; set; }

        /// <summary>
        /// Tipo del proceso. 0: raiz, id: id_padre
        /// </summary>
        public int PadreId{ get; set; }

        /// <summary>
        /// Fecha de creación del usuario.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow; // Fecha y hora en UTC

        /// <summary>
        /// Fecha de la última actualización del usuario.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow; // Fecha y hora en UTC
    }
}
