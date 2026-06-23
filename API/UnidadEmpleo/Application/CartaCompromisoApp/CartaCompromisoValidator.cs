using API.UnidadEmpleo.Application.AspiranteApp;
using API.UnidadEmpleo.Domain;
using FluentValidation;

namespace API.UnidadEmpleo.Application.CartaCompromisoApp
{
    public class CartaCompromisoValidator : AbstractValidator<CartaCompromisoCreate.Command>
    {
        public CartaCompromisoValidator()
        {
            RuleFor(x => x.IdSoliciud).NotEmpty().WithMessage("La de la solicitud no puede ser null.");
        }
    }
}
