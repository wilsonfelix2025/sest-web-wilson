using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.Tensões.Validator
{
    public class CálculoTensõesValidator : CálculoValidator<CálculoTensões>, ICálculoTensõesValidator
    {
        public CálculoTensõesValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do cálculo de tensões que não são validadas na classe base.
        }
    }
}
