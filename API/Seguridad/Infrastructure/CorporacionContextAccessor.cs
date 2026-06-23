namespace API.Seguridad.Infrastructure
{
    public class CorporacionContextAccessor : ICorporacionContextAccessor
    {
        public string? CorporacionId { get; set; }
        public string? SistemaId { get; set; }
        public string? CuerpoId { get; set; }
        public string? RegionId { get; set; }
    }
}
