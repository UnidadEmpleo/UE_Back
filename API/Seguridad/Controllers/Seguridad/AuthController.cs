using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using API.Persistence;
using QRCoder;
using System.Security.Claims;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Infrastructure.Audit;
using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Controllers;

namespace API.Seguridad.Controllers.Seguridad
{
    public class AuthController : BaseApiController
    {
        private readonly UserManager<AppUserIdentity> _userManager;
        private readonly RoleManager<AppIdentityRole> _roleManager;
        private readonly SignInManager<AppUserIdentity> _signInManager;
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<AppUserIdentity> userManager,
            RoleManager<AppIdentityRole> roleManager,
            SignInManager<AppUserIdentity> signInManager,
            TokenService tokenService,
            AppDbContext context,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }
        /*
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var user = new AppUserIdentity 
            { 
                UserName = registerDto.Username, 
                Email = registerDto.Email, 
                TwoFactorEnabled = registerDto.Requiere2FA
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { Message = "Usuario registrado con exito" });
        }*/

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null) return Unauthorized("Usuario o cont�a no validos");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized("Usuario o contrase�a no validos "+ user.UserName+"  "+ user.PasswordHash);

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                var validProviders = await _userManager.GetValidTwoFactorProvidersAsync(user);
                if (!validProviders.Contains("Authenticator"))
                {
                    return BadRequest("2FA no esta habilitado para este usuario.");
                }

                return Ok(new { Requires2FA = true });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = await _tokenService.GenerateTokenAsync(user, roles);

            var (rawRt, hashRt, expires) = _tokenService.GenerateRefreshToken();
            var entity = new UserRefreshToken
            {
                TokenHash = hashRt,
                UserId = user.Id,
                Expires = expires,
                Created = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _context.UserRefreshTokens.Add(entity);
            await _context.SaveChangesAsync();

            var tiposRol = new HashSet<TipoRoles>();
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null && role.Activo)
                {
                    tiposRol.Add(role.TipoRol);
                }
            }

            return Ok(new { User = new { user.Id, user.UserName, tiposRol }, Token = accessToken, RefreshToken = rawRt, RefreshTokenExpires = expires });
        }

        [AllowAnonymous]
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> EnableTwoFactor([FromBody] Enable2FADto enable2FADto)
        {
            var user = await _userManager.FindByNameAsync(enable2FADto.Username);
            if (user == null)
            {
                return Unauthorized("Usuario no existe");
            }

            // Generate shared key for the authenticator
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // Generate the QR code URI for scanning in the authenticator app
            var qrCodeUri = GenerateQrCodeUri(user.Email!, key, "SIPRE");

            // Generate the QR code as a base64 image
            string qrCodeBase64 = string.Empty;
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q))
            {
                var qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
            }

            return Ok(new
            {
                QrCodeUri = qrCodeUri,
                QrCodeImage = $"data:image/png;base64,{qrCodeBase64}",
                Message = "Scan the QR code with your authenticator app to enable 2FA."
            });
        }

        [HttpPost("disable-2fa")]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("Usuario no existe");

            // Deshabilitar 2FA
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded) return BadRequest("Failed to disable 2FA.");

            return Ok(new { Message = "2FA has been disabled successfully." });
        }

        [HttpPost("reset-2fa")]
        public async Task<IActionResult> ResetTwoFactor([FromBody] Reset2FADto reset2FaDTO)
        {
            var user = await _userManager.FindByNameAsync(reset2FaDTO.Username);
            if (user == null) return Unauthorized("Usuario no existe");

            // Reset the authenticator key
            var resetKeyResult = await _userManager.ResetAuthenticatorKeyAsync(user);
            if (!resetKeyResult.Succeeded) return BadRequest("Failed to reset authenticator key.");

            // Disable 2FA
            var disable2FAResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2FAResult.Succeeded) return BadRequest("Failed to disable 2FA.");

            return Ok(new { Message = "2FA settings have been reset successfully." });
        }

        [AllowAnonymous]
        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor(Verify2FADto verify2FADto)
        {
            var user = await _userManager.FindByNameAsync(verify2FADto.Username);
            if (user == null) return Unauthorized("Invalid username");

            // Verificar el token generado por la app de autenticaci�n
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", verify2FADto.Token);
            if (!isValid) return Unauthorized("Invalid 2FA token");

            // Habilitar 2FA para el usuario
            await _userManager.SetTwoFactorEnabledAsync(user, true);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = await _tokenService.GenerateTokenAsync(user, roles);

            var (rawRt, hashRt, expires) = _tokenService.GenerateRefreshToken();
            var entity = new UserRefreshToken
            {
                TokenHash = hashRt,
                UserId = user.Id,
                Expires = expires,
                Created = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _context.UserRefreshTokens.Add(entity);
            await _context.SaveChangesAsync();

            try
            {
                var usuario = await _context
                    .Usuarios
                    .Where(u => u.AppUserIdentityId == user.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Convert.ToString(HttpContext.Request.Headers["User-Agent"]);

                await AuditService.LogAsync(Enumerations.Audits.AuditEventType.Login, usuario?.Id ?? string.Empty,
                    verify2FADto.Username ?? "System", Enumerations.Audits.AuditEventType.Login.ToString(), userIp, userAgent);

                await AuditService.LogAsync(Enumerations.Audits.AuditEventType.TwoFAVerified, usuario?.Id ?? string.Empty,
                    verify2FADto.Username ?? "System", Enumerations.Audits.AuditEventType.TwoFAVerified.ToString(), userIp, userAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AuthController)}.{nameof(Verify2FADto)}, Error logging 2FA verification");
            }

            return Ok(new AuthResponseDto { Token = accessToken, RefreshToken = rawRt, RefreshTokenExpires = expires });
        }

        private string GenerateQrCodeUri(string email, string key, string app)
        {
            return $"otpauth://totp/{email}?secret={key}&issuer={app}&digits=6";
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.AccessToken) || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest("Invalid request");

            // allow expired access token (ignore lifetime)
            var principal = _tokenService.ValidateTokenIgnoreLifetime(dto.AccessToken);
            if (principal == null)
                return Unauthorized("Invalid access token");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized("Invalid access token claims");

            var hashRt = _tokenService.Hash(dto.RefreshToken);
            var stored = await _context.UserRefreshTokens
                .Where(r => r.UserId == userId && r.TokenHash == hashRt)
                .FirstOrDefaultAsync();

            if (stored == null || !stored.IsActive)
                return Unauthorized("Invalid refresh token");

            // rotate
            stored.Revoked = DateTime.UtcNow;
            stored.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            stored.ReasonRevoked = "Rotated";

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var newAccess = await _tokenService.GenerateTokenAsync(user, roles);
            var (rawRt, newHash, expires) = _tokenService.GenerateRefreshToken();
            stored.ReplacedByTokenHash = newHash;

            _context.UserRefreshTokens.Add(new UserRefreshToken
            {
                TokenHash = newHash,
                UserId = user.Id,
                Expires = expires,
                Created = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto { Token = newAccess, RefreshToken = rawRt, RefreshTokenExpires = expires });
        }

        [HttpPost("add-role")]
        public async Task<ActionResult> AddRole(AddRoleDto addRoleDto)
        {
            var roleExists = await _roleManager.RoleExistsAsync(addRoleDto.RoleName);
            if (!roleExists)
            {
                var newRole = new AppIdentityRole()
                {
                    Name = addRoleDto.RoleName,
                    Descripcion = addRoleDto.Descripcion
                };
                var roleResult = await _roleManager.CreateAsync(newRole);
                if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            }

            var user = await _userManager.FindByNameAsync(addRoleDto.Username);
            if (user == null) return NotFound("User not found");

            var result = await _userManager.AddToRoleAsync(user, addRoleDto.RoleName);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { Message = "Role added to user successfully" });
        }

        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false)
                return NoContent();

            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var usuario = await _context
                .Usuarios
                .Where(u => u.AppUserIdentityId == user.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return Ok(new
            {
                user.UserName,
                user.Email,
                Id = usuario?.Id ?? string.Empty,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == false)
                return NoContent();

            var userName = User.Identity?.Name;

            await _signInManager.SignOutAsync();

            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Convert.ToString(HttpContext.Request.Headers["User-Agent"]);

            var userId = await _userManager.FindByNameAsync(userName!);

            await AuditService.LogAsync(Enumerations.Audits.AuditEventType.Logout, userId.Id ?? "System",
                userName ?? "System", Enumerations.Audits.AuditEventType.Logout.ToString(), userIp, userAgent);

            return NoContent();
        }
    
    }
}
