
using FluentValidation;

namespace API.UnidadEmpleo.Application.SolicitudSvs
{
    public class SolicitudValidator : AbstractValidator<SolicitudCreate.Command>
    {
        public SolicitudValidator()
        {
            RuleFor(x => x.CorporacionId).NotEmpty().WithMessage("Corporación Id es obligatorio");
            RuleFor(x => x.Curp).NotEmpty().WithMessage("CURP es obligatorio");
        }
    }
}
