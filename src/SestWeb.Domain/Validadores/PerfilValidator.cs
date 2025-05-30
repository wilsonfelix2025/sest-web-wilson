using System.Linq;
using FluentValidation;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Validadores
{
    public class PerfilValidator : AbstractValidator<PerfilBase>
    {
        public PerfilValidator()
        {
            RuleFor(p => p.Nome).NotEmpty();

            RuleFor(x => x.GetPontos()).Custom((pontos, context) =>
            {
                var temPmNegativo = pontos.Any(l => l.Pm.Valor < 0);

                if (temPmNegativo)
                {
                    context.AddFailure($"Há profundidade do perfil com valor negativo");
                }
            });
        }
    }
}
