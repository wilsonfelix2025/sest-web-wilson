using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;
using SestWeb.Domain.Entities.Cálculos.Perfis.Output;

namespace SestWeb.Domain.Entities.Cálculos.Perfis.Validator
{
    public class CálculoPerfisValidator : CálculoValidator<CálculoPerfis>, ICálculoPerfisValidator
    {
        public CálculoPerfisValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do cálculo de perfis que não são validadas na classe base.

            RuleFor(calcPefis => calcPefis).Custom((calcPefis, context) => ValidarPerfis(calcPefis, context));
        }

        private void ValidarPerfis(CálculoPerfis calcPefis, CustomContext context)
        {
            //foreach (var perfilEntrada in calcPefis.PerfisEntrada.Perfis)
            //{
            //    if(!calcPefis.lCorrelação.PerfisEntrada.Tipos.Contains(perfilEntrada.Mnemônico))
            //        context.AddFailure($"Cálculo {calcPefis.Nome} inválido. Perfil: {perfilEntrada.Nome}, com Mnemônico: {perfilEntrada.Mnemônico}, não é perfil de entrada na Correlação: {calcPefis.Correlação.Nome}, estabelecida para o cálculo.");
            //}

            foreach (var perfilSaída in calcPefis.PerfisSaída.Perfis)
            {
                if(!PerfisOutput.Perfis().Select(p=>p.Mnemônico).Contains(perfilSaída.Mnemonico))
                    context.AddFailure($"Cálculo {calcPefis.Nome} inválido. Perfil: {perfilSaída.Nome}, com Mnemônico: {perfilSaída.Mnemonico}, não é válido como saída para o cálculo de perfis.");
            }

            foreach (var perfilEntrada in calcPefis.PerfisEntrada.Perfis)
            {
                if (perfilEntrada.PodeSerEntradaCálculoPerfis == false)
                    context.AddFailure($"Cálculo {calcPefis.Nome} inválido. Perfil: {perfilEntrada.Nome}, com Mnemônico: {perfilEntrada.Mnemonico}, não é válido como entrada para o cálculo de perfis.");
            }
        }
    }
}
