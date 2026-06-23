using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using API.Seguridad.Domain.Seguridad;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly RoleManager<AppIdentityRole> _roleManager;

    public TokenService(IConfiguration configuration, RoleManager<AppIdentityRole> roleManager)
    {
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<string> GenerateTokenAsync(AppUserIdentity user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Agregar roles como claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Agregar TipoRol claims para cada rol del usuario
        var tiposRol = new HashSet<TipoRoles>();
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null && role.Activo)
            {
                tiposRol.Add(role.TipoRol);
            }
        }

        // Agregar cada TipoRol único como claim
        foreach (var tipoRol in tiposRol)
        {
            claims.Add(new Claim("TipoRol", ((int)tipoRol).ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenMinutes"] ?? "30")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Mantener la versión sincrónica para compatibilidad hacia atrás
    public string GenerateToken(AppUserIdentity user, IList<string> roles)
    {
        return GenerateTokenAsync(user, roles).GetAwaiter().GetResult();
    }

    public ClaimsPrincipal? ValidateToken(string token)
        => ValidateToken(token, validateLifetime: true);

    public ClaimsPrincipal? ValidateTokenIgnoreLifetime(string token)
        => ValidateToken(token, validateLifetime: false);

    public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public (string rawToken, string hash, DateTime expires) GenerateRefreshToken()
    {
        var minutes = int.Parse(_configuration["Jwt:RefreshTokenMinutes"] ?? "1440"); // default 1 dia
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hash = Hash(raw);
        return (raw, hash, DateTime.UtcNow.AddMinutes(minutes));
    }

    public string Hash(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }
}
