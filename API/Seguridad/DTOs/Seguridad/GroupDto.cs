namespace API.Seguridad.DTOs.Seguridad
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descr { get; set; }
        public bool Activo { get; set; } = true;
        public List<UserDto> Usuarios { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTimeOffset? CreatedDateTime { get; set; }   
    }
}
