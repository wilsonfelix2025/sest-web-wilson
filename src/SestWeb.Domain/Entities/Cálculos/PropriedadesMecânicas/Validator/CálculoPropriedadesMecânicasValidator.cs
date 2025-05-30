using FluentValidation;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Output;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Validator
{
    public class CálculoPropriedadesMecânicasValidator : CálculoValidator<CálculoPropriedadesMecânicas>, ICálculoPropriedadesMecânicasValidator
    {
        public CálculoPropriedadesMecânicasValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do cálculo de propriedades mecânicas que não são validadas na classe base.
            RuleFor(calc => calc).Custom((calcPefis, context) => ValidarPerfis(calcPefis, context));
        }

        private void ValidarPerfis(CálculoPropriedadesMecânicas calc, CustomContext context)
        {
            foreach (var perfilSaída in calc.PerfisSaída.Perfis)
            {
                if (!PropriedadesMecânicasOutput.Perfis().Select(p => p.Mnemônico).Contains(perfilSaída.Mnemonico))
                    context.AddFailure($"Cálculo {calc.Nome} inválido. Perfil: {perfilSaída.Nome}, com Mnemônico: {perfilSaída.Mnemonico}, não é válido como saída para o cálculo de perfis.");
            }

        }   
    }
}
