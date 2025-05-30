using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Validator
{
    public class CálculoPressãoPorosValidator : CálculoValidator<CálculoPressãoPoros>, ICálculoPressãoPorosValidator
    {
        public CálculoPressãoPorosValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do cálculo de pressão de poros que não são validadas na classe base.
        }
    }
}
