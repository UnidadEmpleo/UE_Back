using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using API.Seguridad.Domain.Seguridad;
namespace API.Seguridad.Domain.Seguridad
{
    public class AppIdentityRole : IdentityRole
    {
        public string? Descripcion { get; set; }

        /// <summary>
        /// Valor del rol en Azure AD, por ejemplo, "rol.ejemplo".
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Indica si el rol está activo o no.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Fecha en la que se creó el rol.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de la última actualización del rol.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Tipo de rol, que puede ser médico, elemento o administrativo.
        ///     
        public TipoRoles TipoRol { get; set; } 
    }

    public enum TipoRoles
    {
        NoDefinido          = 0,
        Subdirector         = 1,
        Gerente             = 2,
        AtencionRegistro    = 3,
        Psicologo           = 4,
        Medico              = 5,
        Antidoping          = 6,
        Capturista          = 7,
        Administrador       = 8
            
    }

}
