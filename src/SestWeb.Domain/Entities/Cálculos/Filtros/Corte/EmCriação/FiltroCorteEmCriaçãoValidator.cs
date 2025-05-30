using System.Linq;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Corte.EmCriação
{
    public class FiltroCorteEmCriaçãoValidator : CálculoEmCriaçãoValidator<FiltroCorteEmCriação>, IFiltroCorteEmCriaçãoValidator
    {
        public FiltroCorteEmCriaçãoValidator()
        {
            RuleFor(filtro => filtro.LimiteInferior).GreaterThan(0).When(filtro => filtro.LimiteInferior.HasValue);
            RuleFor(filtro => filtro.LimiteSuperior).GreaterThan(0).When(filtro => filtro.LimiteSuperior.HasValue);
            RuleFor(filtro => filtro.PerfisEntrada.First().ContémPontos()).NotEmpty();
        }
    }
}
