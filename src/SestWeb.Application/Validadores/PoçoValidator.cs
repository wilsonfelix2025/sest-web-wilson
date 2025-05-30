using FluentValidation;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.Validadores
{
    public class PoçoValidator : AbstractValidator<Poço>
    {
        private readonly IPoçoReadOnlyRepository _poçoRepository;

        public PoçoValidator(IPoçoReadOnlyRepository poçoRepository)
        {
            _poçoRepository = poçoRepository;

            RuleFor(p => p.DadosGerais.Identificação.Nome).Custom(async (nome, context) =>
            {
                // TODO: Verificar lógica de existir essa regra
                //var existePoçoComMesmoNome = await _poçoRepository.ExistePoçoComMesmoNome(nome);

                //if (existePoçoComMesmoNome)
                //{
                //    context.AddFailure($"{nome} - já existe poço com esse nome");
                //}
            });
        }
    }
}
