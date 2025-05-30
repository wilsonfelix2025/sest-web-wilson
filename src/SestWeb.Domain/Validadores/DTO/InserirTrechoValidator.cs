using FluentValidation;
using SestWeb.Domain.DTOs.InserirTrecho;

namespace SestWeb.Domain.Validadores.DTO
{
    public class InserirTrechoValidator : AbstractValidator<InserirTrechoDTO>
    {
        public InserirTrechoValidator()
        {
            RuleFor(p => p).Custom((p, context) =>
            {
                if (p != null && p.PmLimite > p.PerfilSelecionado.PmMáximo.Valor)
                {
                    context.AddFailure($"Profundidade limite maior que último ponto do perfil");
                }
            });

        }
    }
}
