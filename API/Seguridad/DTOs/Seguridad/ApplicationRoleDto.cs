using API.Seguridad.Domain.Seguridad;

namespace API.Seguridad.DTOs.Seguridad;
public class ApplicationRoleDto
{
    public string Id { get; set; } // Unique identifier for the role
    public string Name { get; set; } // Name of the role
    public string Descripcion { get; set; } // Description of the role
    public string Value { get; set; } // Value used in tokens
    public bool Activo { get; set; } // Indicates if the role is active

    // Optional: Users and groups assigned to the role
    public List<UserDto> AssignedUsers { get; set; }
    public List<GroupDto> AssignedGroups { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaUltimaActualizacion { get; set; }
    public List<Proceso> Procesos { get;  set; }
    public TipoRoles TipoRol { get; set; } = 0;
}
