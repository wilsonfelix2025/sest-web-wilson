using System.Threading.Tasks;
using FluentValidation;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Validadores.DTO
{
    public class TrajetoriaDTOValidator : AbstractValidator<TrajetóriaDTO>
    {
        public TrajetoriaDTOValidator()
        {
            RuleFor(t => t.Pontos).Custom((x, context) =>
                {
                    Parallel.ForEach(x, ponto =>
                    {
                        if (!double.TryParse(ponto.Pm, out var value) || value < 0)
                        {
                            context.AddFailure($"{x} não é um número válido ou é menor que 0 para PM da trajetória");
                        }

                        if (!double.TryParse(ponto.Inclinação, out value) || value < 0)
                        {
                            context.AddFailure($"{x} não é um número válido ou é menor que 0 para inclinação da trajetória");
                        }

                        //if (!double.TryParse(ponto.Azimute, out value) || value < 0)
                        //{
                        //    context.AddFailure($"{x} não é um número válido ou é menor que 0 para azimute da trajetória");
                        //}
                    });
                });
        }
    }
}
