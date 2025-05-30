using FluentValidation;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using System.Collections.Generic;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Validadores
{
    public class LitologiaValidator : AbstractValidator<Litologia>
    {
        public LitologiaValidator()
        {
            RuleFor(lito => lito.Classificação).NotEmpty();

            RuleForEach(lito => lito.GetPontos())
                .ChildRules(p => p.RuleFor(x => x.Pm.Valor).GreaterThan(0).WithMessage("PM da Litologia deve ser maior que 0"));

            RuleFor(lito => lito.GetPontos()).Custom((x, context) =>
            {
                var validação = ValidateOverlapping(x);
                if (!string.IsNullOrWhiteSpace(validação))
                {
                    context.AddFailure(validação);
                }
            });

        }
        
        private string ValidateOverlapping(IReadOnlyList<PontoLitologia> pontos)
        {
            var retorno = string.Empty;
            var primeiroPonto = true;
            var pontoAnterior = new PontoLitologia(new Profundidade(0), new Profundidade(0), "AGB", TipoProfundidade.PM, OrigemPonto.Importado, null);

            foreach (var ponto in pontos)
            {
                if (primeiroPonto == false && ponto.Pm <= pontoAnterior.Pm && !ponto.TipoRocha.Equals(pontoAnterior.TipoRocha))
                {
                    retorno = "Há sobreposição de litologia: " + ponto.Pm.ToString() + "-" + ponto.TipoRocha.Mnemonico +
                              " com " + pontoAnterior.Pm.ToString() + "-" + pontoAnterior.TipoRocha.Mnemonico;
                    return retorno;
                }

                primeiroPonto = false;
                pontoAnterior = ponto;
            }

            return retorno;
        }

    }
}
