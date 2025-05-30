using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.EmCriação
{
    public class FiltroLitologiaEmCriaçãoValidator : CálculoEmCriaçãoValidator<FiltroLitologiaEmCriação>, IFiltroLitologiaEmCriaçãoValidator
    {
        public FiltroLitologiaEmCriaçãoValidator()
        {
            RuleFor(filtro => filtro.LimiteInferior).GreaterThan(0).When(filtro => filtro.LimiteInferior.HasValue);
            RuleFor(filtro => filtro.LimiteSuperior).GreaterThan(0).When(filtro => filtro.LimiteSuperior.HasValue);
            RuleFor(filtro => filtro.PerfisEntrada.First().ContémPontos()).NotEmpty();
            RuleFor(filtro => filtro.Litologias.Count > 0);
            RuleFor(FILTRO => FILTRO.Litologias).Custom((x, context) =>
            {
                var validação = ValidateExisteLito(x);
                if (!string.IsNullOrWhiteSpace(validação))
                {
                    context.AddFailure(validação);
                }
            });
        }

        private string ValidateExisteLito(List<string> litos)
        {
            var retorno = string.Empty;
            
            //TODO Validar se string existe como tipo de rocha

            return retorno;
        }
    }
}
