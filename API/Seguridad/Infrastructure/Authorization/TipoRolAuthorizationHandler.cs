using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using API.Seguridad.Domain.Seguridad;

namespace API.Seguridad.Infrastructure.Authorization
{
    /// <summary>
    /// Handler que verifica si el usuario actual tiene un claim TipoRol con el tipo de rol requerido.
    /// </summary>
    public class TipoRolAuthorizationHandler : AuthorizationHandler<TipoRolRequirement>
    {
        private readonly ILogger<TipoRolAuthorizationHandler> _logger;

        public TipoRolAuthorizationHandler(ILogger<TipoRolAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            TipoRolRequirement requirement)
        {
            try
            {
                // Obtener todos los claims TipoRol del usuario
                var tipoRolClaims = context.User.FindAll("TipoRol").ToList();

                if (!tipoRolClaims.Any())
                {
                    _logger.LogWarning("Usuario no tiene claims TipoRol");
                    context.Fail();
                    return Task.CompletedTask;
                }

                // Convertir los claims a TipoRoles
                var userTiposRol = new HashSet<TipoRoles>();
                foreach (var claim in tipoRolClaims)
                {
                    if (int.TryParse(claim.Value, out var tipoRolInt) && 
                        Enum.IsDefined(typeof(TipoRoles), tipoRolInt))
                    {
                        userTiposRol.Add((TipoRoles)tipoRolInt);
                    }
                }

                // Verificar si alguno de los TipoRol del usuario coincide con los requeridos
                var hasRequiredTipoRol = requirement.TiposRolPermitidos.Any(required => userTiposRol.Contains(required));

                if (hasRequiredTipoRol)
                {
                    var userName = context.User.Identity?.Name ?? "Unknown";
                    _logger.LogInformation(
                        "Usuario {UserName} autorizado con TipoRol: {TiposRol}", 
                        userName,
                        string.Join(", ", userTiposRol));
                    
                    context.Succeed(requirement);
                }
                else
                {
                    var userName = context.User.Identity?.Name ?? "Unknown";
                    _logger.LogWarning(
                        "Usuario {UserName} no tiene ningún TipoRol permitido. Tiene: {UserTipos}, Requiere: {RequiredTipos}", 
                        userName,
                        string.Join(", ", userTiposRol),
                        string.Join(", ", requirement.TiposRolPermitidos));
                    
                    context.Fail();
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar autorización por tipo de rol mediante claims");
                context.Fail();
                return Task.CompletedTask;
            }
        }
    }
}