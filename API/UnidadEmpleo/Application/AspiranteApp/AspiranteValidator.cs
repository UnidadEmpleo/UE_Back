using FluentValidation;

namespace API.UnidadEmpleo.Application.AspiranteApp
{
    public class AspiranteValidator : AbstractValidator<AspiranteCreate.Command>
    {
        public AspiranteValidator()
        {

            RuleFor(x => x.Curp)
                    .NotEmpty().WithMessage("La Curp es obligatoria.")
                    .MaximumLength(18).WithMessage("El CURP no puede exceder 18 caracteres.");
               
            RuleFor(x => x.Nombre)
                    .NotEmpty().WithMessage("El nombre es obligatorio.")
                    .MaximumLength(100);

            RuleFor(x => x.Apellido_Paterno)
                    .NotEmpty().WithMessage("El primer apellido paterno es obligatorio.")
                    .MaximumLength(100);

            RuleFor(x => x.Apellido_Materno)
                    .MaximumLength(100).WithMessage("El segundo apellido materno no puede exceder de 100 caracteres.");

            RuleFor(x => x.Fecha_Nacimiento)
                    .NotEmpty().WithMessage("La fecha de nacimiento es obligatorio.");

            RuleFor(x => x.TelefonoCelular)
                    .MaximumLength(10);

            RuleFor(x => x.Curp)
                    .MaximumLength(18).WithMessage("El CURP no puede exceder 18 caracteres.");

            RuleFor(x => x.Rfc)
                    .NotEmpty().WithMessage("El RFC es obligatorio.")
                    .MaximumLength(13).WithMessage("El RFC no puede exceder 13 caracteres.");
                    

        }
    }
}
