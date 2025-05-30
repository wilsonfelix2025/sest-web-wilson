using FluentValidation;
using SestWeb.Domain.MontagemPerfis;

namespace SestWeb.Domain.Validadores
{
    public class MontarLitologiaValidator : AbstractValidator<MontadorDeLitologia>
    {
        public MontarLitologiaValidator()
        {
            //Quando não tiver litologia, o montador vai preencher com -1 (out)
            //RuleFor(montador => montador.PoçoCorrelação.ObterLitologiaPadrão().Pontos).NotEmpty();

            
        }

    }
}
