using FluentValidation;
using API.Seguridad.Application.Seguridad.Roles.Commands;

namespace API.Seguridad.Application.Seguridad.Roles.Validators
{
    public class CreateRolValidator : AbstractValidator<CreateRol.Command>
    {
        public CreateRolValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(30).WithMessage("El nombre no puede tener más de 30 caracteres.")
                .Matches("^[a-zA-Z ]*$").WithMessage("El nombre solo puede contener letras y espacios.");

            RuleFor(x => x.Descr)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede tener más de 100 caracteres.")
                .Matches("^[a-zA-Z ]*$").WithMessage("La descripción solo puede contener letras y espacios.");
        }
    }
}
