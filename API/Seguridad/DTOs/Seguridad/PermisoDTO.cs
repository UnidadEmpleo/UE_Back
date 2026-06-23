namespace API.Seguridad.DTOs.Seguridad
{
    public class PermisoDTO
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string RolId { get; set; }
        public int ProcesoId { get; set; }
        public string Acceso { get; set; }
    }
}
