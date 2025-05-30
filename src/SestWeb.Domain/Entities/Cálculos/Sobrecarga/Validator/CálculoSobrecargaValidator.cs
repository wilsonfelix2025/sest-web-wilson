using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.Validator
{
    public class CálculoSobrecargaValidator : CálculoValidator<CálculoSobrecarga>, ICálculoSobrecargaValidator
    {
        public CálculoSobrecargaValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do cálculo de sobrecarga que não são validadas na classe base.
        }
    }
}
