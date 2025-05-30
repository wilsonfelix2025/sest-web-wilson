using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Validator
{
    public class CálculoGradientesValidator : CálculoValidator<CálculoGradientes>, ICálculoGradientesValidator
    {
        public CálculoGradientesValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do cálculo de gradientes que não são validadas na classe base.
        }
    }
}
