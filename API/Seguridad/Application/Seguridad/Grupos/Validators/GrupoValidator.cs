using FluentValidation;
using API.Seguridad.Application.Seguridad.Grupos.Commands;

namespace API.Seguridad.Application.Seguridad.Grupos.Validators
{
    public class CreateGrupoValidator : AbstractValidator<CreateGrupo.Command>
    {
        public CreateGrupoValidator()
        {
            RuleFor(x => x.Descr)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede tener más de 100 caracteres.")
                .Matches("^[a-zA-Z0-9 ]*$").WithMessage("La descripción solo puede contener letras, números y espacios.");

            RuleFor(x => x.Nombre)
               .NotEmpty().WithMessage("El nombre es obligatoria.")
               .MaximumLength(30).WithMessage("El nombre no puede tener más de 30 caracteres.")
               .Matches("^[a-zA-Z0-9 ]*$").WithMessage("El nombre solo puede contener letras, números y espacios.");
        }
    }
}