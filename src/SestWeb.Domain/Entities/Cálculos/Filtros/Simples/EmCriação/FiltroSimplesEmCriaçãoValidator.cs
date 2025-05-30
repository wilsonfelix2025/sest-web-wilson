using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using FluentValidation;


namespace SestWeb.Domain.Entities.Cálculos.Filtros.Simples.EmCriação
{
    public class FiltroSimplesEmCriaçãoValidator : CálculoEmCriaçãoValidator<FiltroSimplesEmCriação>, IFiltroSimplesEmCriaçãoValidator
    {
        public FiltroSimplesEmCriaçãoValidator()
        {
            RuleFor(filtro => filtro.DesvioMáximo).GreaterThan(0);
            RuleFor(filtro => filtro.LimiteInferior).GreaterThan(0).When(filtro => filtro.LimiteInferior.HasValue);
            RuleFor(filtro => filtro.LimiteSuperior).GreaterThan(0).When(filtro => filtro.LimiteSuperior.HasValue);
            RuleFor(filtro => filtro.PerfisEntrada.First().ContémPontos()).NotEmpty();
        }
    }
}
