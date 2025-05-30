using System.Linq;
using FluentValidation;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Validadores.DTO
{
    public class LitologiaDTOValidator : AbstractValidator<LitologiaDTO>
    {
        public LitologiaDTOValidator()
        {
            
            RuleFor(lito => lito.Classificação).NotEmpty();

            RuleFor(lito => lito.Pontos).Custom((x, context) =>
            {
                var temPmNegativo = x.Any(l => l.Pm.ToDouble() < 0);

                if (temPmNegativo)
                {
                    context.AddFailure($"Há profundidade da litologia com valor negativo");
                }

                
            });
        }
       
    }
}
