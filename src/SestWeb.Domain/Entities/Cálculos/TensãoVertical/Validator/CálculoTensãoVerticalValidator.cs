using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.TensãoVertical.Validator
{
    class CálculoTensãoVerticalValidator: CálculoValidator<CálculoTensãoVertical>, ICálculoTensãoVerticalValidator
    {
        public CálculoTensãoVerticalValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            RuleFor(calcTvert => calcTvert).Custom((calcTvert, context) => ValidarPerfis(calcTvert, context));
        }

        public ValidationResult Validate(ICálculoTensãoVertical tvert)
        {
            return new ValidationResult();
        } 

        private void ValidarPerfis(CálculoTensãoVertical calcTvert, CustomContext context)
        {
            if (calcTvert.PerfisEntrada.Perfis.Count > 1)
            {
                context.AddFailure($"Cálculo {calcTvert.Nome} inválido. Apenas um perfil de entrada deve ser fornecido.");
            } 
            else if (calcTvert.PerfisEntrada.Perfis.Count == 0)
            {
                context.AddFailure($"Cálculo {calcTvert.Nome} inválido. Nenhum perfil de entrada foi fornecido.");
            }

            if (calcTvert.PerfisEntrada.Perfis[0].Mnemonico != "RHOB")
            {
                context.AddFailure($"Cálculo {calcTvert.Nome} inválido. O perfil de entrada precisa ser do tipo RHOB.");
            }
        }
    }
}
