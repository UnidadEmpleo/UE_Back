namespace API.Seguridad.DTOs.Seguridad
{
    public class ProcesoDto
    {
        public int Id { get; set; }
        public string Descr { get; set; }
        public string Tipo { get; set; }
        public string? Icono { get; set; }
        public bool Activo { get; set; }
        public string? Ruta { get; set; }
        public int? ProcesoPadreId { get; set; }
        public short SistemaId { get; set; }
        public SistemaDto Sistema { get; set; }
        public List<ProcesoDto> Subprocesos { get; set; } = new();
        public string? Acciones { get; set; }
    }
}