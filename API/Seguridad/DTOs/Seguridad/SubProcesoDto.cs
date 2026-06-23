namespace API.Seguridad.DTOs.Seguridad
{
    public class SubProcesoDto
    {
        public string Descr { get; set; }
        public string Tipo { get; set; }
        public string Icono { get; set; }
        public string? Ruta { get; set; }
        public int? ProcesoPadreId { get; set; }
        public bool Activo { get; set; } = true;
    }
}
