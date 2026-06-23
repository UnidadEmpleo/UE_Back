namespace API.Seguridad.DTOs.Seguridad
{
    public class UserDto
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string? UserName { get; set; }
        public int TiempoInactividad { get; set; } = 30;
        public List<string?> Corporaciones { get; set; } = [];
        public string? RolId { get; set; }
        public string? GrupoId { get; set; }
        public bool Activo { get; set; }
        public bool EsUsuarioAD { get; set; } = false;
        public DateTime FechaCreacion { get; set; }
        public List<PermisoDTO> Permisos { get; set; } = [];
        public DateTime FechaUltimaActualizacion { get; set; }
        public string? CuerpoId { get; set; }
        public int RegionId { get; set; }
    }
}
