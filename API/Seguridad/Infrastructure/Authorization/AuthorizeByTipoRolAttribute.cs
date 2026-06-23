using Microsoft.AspNetCore.Authorization;
using API.Seguridad.Domain.Seguridad;

namespace API.Seguridad.Infrastructure.Authorization
{
    /// <summary>
    /// Atributo personalizado para autorizar acceso basado en el tipo de rol del usuario.
    /// Este atributo verifica los claims TipoRol del token JWT para determinar el acceso.
    /// </summary>
    /// <example>
    /// <code>
    /// // A nivel de controller - todos los endpoints requieren estos tipos de rol
    /// [AuthorizeByTipoRol(TipoRoles.AdministradorSistema, TipoRoles.Administrativo)]
    /// public class UsersController : BaseApiController { }
    /// 
    /// // A nivel de acción - solo este endpoint requiere este tipo de rol
    /// [HttpDelete("{id}")]
    /// [AuthorizeByTipoRol(TipoRoles.AdministradorSistema)]
    /// public async Task&lt;ActionResult&gt; DeleteUser(string id) { }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeByTipoRolAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Constructor que inicializa el atributo con los tipos de rol permitidos.
        /// </summary>
        /// <param name="tiposRol">Uno o más tipos de rol que tienen acceso al recurso.</param>
        /// <exception cref="ArgumentException">Se lanza si no se especifica ningún tipo de rol.</exception>
        //public AuthorizeByTipoRolAttribute(params TipoRoles[] tiposRol)
        public AuthorizeByTipoRolAttribute(params TipoRoles[] tiposRol)
        {
            if (tiposRol == null || tiposRol.Length == 0)
            {
                throw new ArgumentException("Debe especificar al menos un tipo de rol", nameof(tiposRol));
            }

            // Generar el nombre de la política basado en los tipos de rol
            // Ejemplo: TipoRol_4_3 para AdministradorSistema (4) y Administrativo (3)
            Policy = $"TipoRol_{string.Join("_", tiposRol.Select(t => (int)t))}";
        }
    }
}