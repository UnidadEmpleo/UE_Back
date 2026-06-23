namespace API.Seguridad.Infrastructure
{
    using System.Security.Claims;

    public static class AuthContext
    {
        public static string? GetAuthSource(ClaimsPrincipal user)
        {
            return user.FindFirst("auth_source")?.Value;
        }

        public static bool IsAzureAd(ClaimsPrincipal user)
        {
            return GetAuthSource(user) == "azure_ad";
        }

        public static bool IsLocalJwt(ClaimsPrincipal user)
        {
            return GetAuthSource(user) == "local_jwt";
        }

        public static bool HasRole(ClaimsPrincipal user, string role)
        {
            if (user == null)
                return false;

            // Azure AD usa normalmente el claim "roles", local podría usar "role"
            var azureRoles = user.FindAll("roles").Select(c => c.Value);
            var localRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

            return azureRoles.Concat(localRoles).Contains(role);
        }

        public static string? GetUserId(ClaimsPrincipal user)
        {
            if (user == null)
                return null;    

            // Puedes adaptar esto a tu lógica. Algunos emisores usan sub, oid, nameidentifier, etc.
            return user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value ??
                   user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   user.FindFirst("sub")?.Value ??
                   user.FindFirst("oid")?.Value;
        }

        public static string? GetUserName(ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            // Puedes adaptar esto a tu lógica. Algunos emisores usan name, preferred_username, etc.
            return user.FindFirst(ClaimTypes.Name)?.Value ??
                   user.FindFirst("preferred_username")?.Value ??
                   user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetTipoRol(ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            return user.FindFirst("TipoRol")?.Value;
        }
    }

}
