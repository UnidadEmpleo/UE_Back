using Microsoft.AspNetCore.Identity;

namespace API.Seguridad.Domain.Seguridad
{
    /// <summary>
    /// Representa la identidad de un usuario en la aplicación, extendiendo la funcionalidad de IdentityUser.
    /// </summary>
    public class AppUserIdentity : IdentityUser
    {
        /// <summary>
        /// Relación uno a uno con la entidad Usuario.
        /// </summary>
        public Usuario Usuario { get; set; }
        /// <summary>
        /// Fecha en la que se creó la identidad del usuario.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Fecha de la última actualización de la identidad del usuario.
        /// </summary>
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow;
        public async Task<bool> IsInRoleAsync(UserManager<AppUserIdentity> userManager, string role)
        {
            return await userManager.IsInRoleAsync(this, role);
        }
    }
}
