using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Output;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.Validator
{
    public class CálculoExpoenteDValidator : CálculoValidator<CálculoExpoenteD>, ICálculoExpoenteDValidator
    {
        public CálculoExpoenteDValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do cálculo de perfis que não são validadas na classe base.

            RuleFor(calcExpD => calcExpD).Custom((calcExpD, context) => ValidarExpoenteD(calcExpD, context));
        }

        private void ValidarExpoenteD(CálculoExpoenteD calcExpD, CustomContext context)
        {
            foreach (var perfilSaída in calcExpD.PerfisSaída.Perfis)
            {
                if (!ExpoenteDOutput.Perfis().Select(p => p.Mnemônico).Contains(perfilSaída.Mnemonico))
                    context.AddFailure($"Cálculo {calcExpD.Nome} inválido. Perfil: {perfilSaída.Nome}, com Mnemônico: {perfilSaída.Mnemonico}, não é válido como saída para o cálculo de ExpoenteD.");
            }
        }
    }
}
