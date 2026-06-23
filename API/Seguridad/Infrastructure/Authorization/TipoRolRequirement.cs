using Microsoft.AspNetCore.Authorization;
using API.Seguridad.Domain.Seguridad;

namespace API.Seguridad.Infrastructure.Authorization
{
    /// <summary>
    /// Requirement que especifica qué tipos de roles tienen acceso a un recurso.
    /// </summary>
    public class TipoRolRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Array de tipos de rol permitidos para acceder al recurso.
        /// </summary>
        public TipoRoles[] TiposRolPermitidos { get; }

        /// <summary>
        /// Constructor que inicializa el requirement con los tipos de rol permitidos.
        /// </summary>
        /// <param name="tiposRolPermitidos">Uno o más tipos de rol que tienen acceso.</param>
        public TipoRolRequirement(params TipoRoles[] tiposRolPermitidos)
        {
            if (tiposRolPermitidos == null || tiposRolPermitidos.Length == 0)
            {
                throw new ArgumentException("Debe especificar al menos un tipo de rol", nameof(tiposRolPermitidos));
            }

            TiposRolPermitidos = tiposRolPermitidos;
        }
    }
}