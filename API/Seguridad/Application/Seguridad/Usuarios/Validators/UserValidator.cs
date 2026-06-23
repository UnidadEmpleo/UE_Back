using API.Seguridad.Application.Core;
using API.Seguridad.Application.Seguridad.Usuarios.Commands;
using FluentValidation;
using System.Text.RegularExpressions;

namespace API.Seguridad.Application.Seguridad.Usuarios.Validators
{
    /// <summary>
    /// Provides validation rules for the <see cref="CreateUser.Command"/> class.
    /// </summary>
    public class UserValidator : AbstractValidator<CreateUser.Command>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserValidator"/> class.
        /// </summary>
        public UserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de acceso es obligatorio.");

            // Validar que el nombre no sea nulo, vacío y no contenga números o caracteres especiales
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .Must(BeAValidName).WithMessage("El nombre no debe contener números ni caracteres especiales.")
                .MinimumLength(2).WithMessage("El nombre debe contener al menos 2 caracteres.");

            // Validar que el primer apellido no sea nulo, vacío y no contenga números o caracteres especiales
            RuleFor(x => x.PrimerApellido)
                .NotEmpty().WithMessage("El primer apellido es obligatorio.")
                .Must(BeAValidName).WithMessage("El primer apellido no debe contener números ni caracteres especiales.")
                .MinimumLength(2).WithMessage("El primer apellido debe contener al menos 2 caracteres.");

            // Validar que el LastName no sea nulo, vacío y no contenga números o caracteres especiales
            RuleFor(x => x.SegundoApellido)
                .Must(BeAValidName).WithMessage("El segundo apellido no debe contener números ni caracteres especiales.");

            RuleFor(x => x.EMail)
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\W]").WithMessage("La contraseña debe contener al menos un carácter especial.");

            RuleFor(x => x.Telefono)
                .Matches(@"^\d{10}$").WithMessage("El número de teléfono debe ser de 10 dígitos. Ejemplo: 6862345678");
        }

        /// <summary>
        /// Validates that the provided name does not contain numbers or special characters.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <returns><c>true</c> if the name is valid; otherwise, <c>false</c>.</returns>
        private bool BeAValidName(string? name)
        {
            if (name.IsNullOrWhiteSpace()) return false;

            // Expresión regular para permitir solo letras y espacios
            var regex = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");
            return regex.IsMatch(name);
        }
    }
}
