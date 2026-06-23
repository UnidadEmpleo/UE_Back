namespace API.Seguridad.Domain.Seguridad
{
    public class TipoRolesNoUtil 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
    }
}
