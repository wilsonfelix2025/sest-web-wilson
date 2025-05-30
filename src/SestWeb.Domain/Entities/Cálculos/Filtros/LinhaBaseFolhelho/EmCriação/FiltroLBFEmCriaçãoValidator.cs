using System.Linq;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.EmCriação
{
    public class FiltroLBFEmCriaçãoValidator : CálculoEmCriaçãoValidator<FiltroLBFEmCriação>, IFiltroLBFEmCriaçãoValidator
    {
        public FiltroLBFEmCriaçãoValidator()
        {
            RuleFor(filtro => filtro.LimiteInferior).GreaterThan(0).When(filtro => filtro.LimiteInferior.HasValue);
            RuleFor(filtro => filtro.LimiteSuperior).GreaterThan(0).When(filtro => filtro.LimiteSuperior.HasValue);
            RuleFor(filtro => filtro.PerfisEntrada.First().ContémPontos()).NotEmpty();
            RuleFor(filtro => filtro.PerfilLBF).NotEmpty();
        }
    }
}
