namespace API.Seguridad.Infrastructure
{
    public interface ICorporacionContextAccessor
    {
        string? CorporacionId { get; set; }
        string? SistemaId { get; set; } 
        string? CuerpoId { get; set; }
        string? RegionId{ get; set; }
        
    }
}
