using System.Linq;
using FluentValidation;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Validadores
{
    public class TrajetoriaValidator : AbstractValidator<Trajetória>
    {
        public TrajetoriaValidator()
        {
            RuleFor(x => x.Pontos).Custom((pontos, context) =>
            {
                var temPmNegativo = pontos.Any(l => l.Pm.Valor < 0);

                if (temPmNegativo)
                {
                    context.AddFailure($"Há profundidade com valor negativo");
                }
            });


            RuleFor(x => x.Pontos).Custom((pontos, context) =>
            {
                var temAzimuteInvalido = pontos.Any(l => l.Azimute < 0 || l.Azimute > 360);

                //if (temAzimuteInvalido)
                //{
                //    context.AddFailure($"Há profundidade com valor de azimute menor que 0 ou maior que 360");
                //}
            });

        }
    }
}
