using API.UnidadEmpleo.Application.CartaCompromisoApp;
using FluentValidation;

namespace API.UnidadEmpleo.Application.ReferenciaApp
{
    public class ReferenciaValidator : AbstractValidator<ReferenciaCreate.Command>
    {
        public ReferenciaValidator()
        {
            RuleFor(x => x.IdSoliciud).NotEmpty().WithMessage("La solicitud no puede ser null.");
        }
    }
}
