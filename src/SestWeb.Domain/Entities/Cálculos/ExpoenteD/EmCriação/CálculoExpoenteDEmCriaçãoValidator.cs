using System;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Correlação;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.EmCriação
{
    public class CálculoExpoenteDEmCriaçãoValidator : CálculoEmCriaçãoValidator<CálculoExpoenteDEmCriação>, ICálculoExpoenteDEmCriaçãoValidator
    {
        public CálculoExpoenteDEmCriaçãoValidator()
        {
            RuleFor(calcExpD => calcExpD.Correlação)
                .Must(CorrelaçãoDeveSerVálida)
                .WithMessage(
                    $"Correlação do cálculo de ExpoenteD deve ser uma das seguintes opções: \n{string.Join("\n", Enum.GetNames(typeof(CorrelaçãoExpoenteD)))}!");
        }

        private bool CorrelaçãoDeveSerVálida(string correlação)
        {
            return Enum.TryParse(correlação, out CorrelaçãoExpoenteD value);
        }
    }
}
