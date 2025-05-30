using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Validadores.DTO
{
    public class PerfilDTOValidator : AbstractValidator<PerfilDTO>
    {
        public PerfilDTOValidator(List<string> nomesPerfisExistentes)
        {
            RuleFor(p => p.Nome).NotEmpty().Custom((nome, context) =>
            {
                if (nomesPerfisExistentes.Contains(nome.Trim()))
                {
                    context.AddFailure($"Poço já possui um perfil com o nome: {nome}");
                }
            });

            RuleFor(p => p.PontosDTO).Custom((x, context) =>
            {
                Parallel.ForEach(x, ponto =>
                {
                    if (!double.TryParse(ponto.Pm, out var value) || value < 0)
                    {
                        context.AddFailure($"A profundidade: {ponto.Pm} não é um valor numérico válido.");
                    }
                    else if (value < 0)
                    {
                        context.AddFailure($"A profundidade: {ponto.Pm} não é válida. PM deve ser maior do que zero.");
                    }

                    if (!double.TryParse(ponto.Valor, out value))
                    {
                        context.AddFailure($"O valor: {ponto.Valor} não é um um valor numérico válido.");
                    }
                });
            });
        }
    }
}
