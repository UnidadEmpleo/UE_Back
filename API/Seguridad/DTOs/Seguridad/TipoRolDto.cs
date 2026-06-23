namespace API.Seguridad.DTOs.Seguridad
{
    public class TipoRolDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Activo { get; set; } = true;
    }
}
