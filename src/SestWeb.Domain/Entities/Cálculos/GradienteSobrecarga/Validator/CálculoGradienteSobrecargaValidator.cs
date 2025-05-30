using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.Validator
{
    public class CálculoGradienteSobrecargaValidator: CálculoValidator<CálculoGradienteSobrecarga>, ICálculoGradienteSobrecargaValidator
    {
        public CálculoGradienteSobrecargaValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            RuleFor(calcGradSobr => calcGradSobr).Custom((calcGradSobr, context) => ValidarPerfis(calcGradSobr, context));
        }

        public ValidationResult Validate(ICálculoGradienteSobrecarga cálculo)
        {
            return new ValidationResult();
        }

        private void ValidarPerfis(CálculoGradienteSobrecarga calcGradSobr, CustomContext context)
        {
            if (calcGradSobr.PerfisEntrada.Perfis.Count > 1)
            {
                context.AddFailure($"Cálculo {calcGradSobr.Nome} inválido. Apenas um perfil de entrada deve ser fornecido.");
            }
            else if (calcGradSobr.PerfisEntrada.Perfis.Count == 0)
            {
                context.AddFailure($"Cálculo {calcGradSobr.Nome} inválido. Nenhum perfil de entrada foi fornecido.");
            }

            if (calcGradSobr.PerfisEntrada.Perfis[0].Mnemonico != "TVERT")
            {
                context.AddFailure($"Cálculo {calcGradSobr.Nome} inválido. O perfil de entrada precisa ser do tipo TVERT.");
            }
        }
    }
}
