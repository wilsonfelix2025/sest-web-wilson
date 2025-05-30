using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.Validator
{
    public class FiltroLinhaBaseFolhelhoValidator : CálculoValidator<FiltroLinhaBaseFolhelho>, IFiltroLinhaBaseFolhelhoValidator
    {
        public FiltroLinhaBaseFolhelhoValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do filtro
            RuleFor(filtro => filtro.PerfilLBF).NotEmpty();
            RuleFor(filtro => filtro.PerfilLBF.Trend).NotNull();
            RuleFor(filtro => filtro.LimiteInferior).GreaterThan(0).When(filtro => filtro.LimiteInferior.HasValue);
            RuleFor(filtro => filtro.LimiteSuperior).GreaterThan(0).When(filtro => filtro.LimiteSuperior.HasValue);
        }
    }
}
