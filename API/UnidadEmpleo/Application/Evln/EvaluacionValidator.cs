using API.UnidadEmpleo.Application.ReferenciaApp;
using FluentValidation;

namespace API.UnidadEmpleo.Application.Evln
{
    public class EvaluacionValidator : AbstractValidator<ReferenciaCreate.Command>
    {
        public EvaluacionValidator()
        {
            RuleFor(x => x.IdSoliciud).NotEmpty().WithMessage("La solicitud no puede ser null.");
        }
    }
}
