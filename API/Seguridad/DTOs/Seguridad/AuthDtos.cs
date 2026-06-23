using System.ComponentModel.DataAnnotations;

namespace API.Seguridad.DTOs.Seguridad
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } = "";
        [Required]
        public string Nombre { get; set; } = "";
        [Required]
        public string PrimerApellido { get; set; } = "";
        public string SegundoApellido { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string GrupoId { get; set; }
        public string RolId { get; set; }
        public bool Requiere2FA { get; set; } = false;  
    }

    public class LoginDto
    {
        public string? Userid { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
    }

    public class ChangePasswordDto
    {
        public string Username { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordDto
    {
        public string Username { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class AddRoleDto
    {
        public string Username { get; set; }
        public string RoleName { get; set; }
        public string Descripcion { get; set; } = "";
    }

    public class Enable2FADto
    {
        public string Username { get; set; }
    }

    public class Reset2FADto
    {
        public string Username { get; set; }
    }

    public class Verify2FADto
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }
}