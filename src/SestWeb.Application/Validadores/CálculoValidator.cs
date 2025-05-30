using FluentValidation;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.Validadores
{
    public class CálculoValidator : AbstractValidator<Cálculo>
    {
        private readonly IPoçoReadOnlyRepository _poçoRepository;

        public CálculoValidator(IPoçoReadOnlyRepository poçoRepository, Poço poço)
        {
            _poçoRepository = poçoRepository;

            RuleFor(p => p).Custom(async (calc, context) =>
            {
                var existeCálculoComMesmoNome = await _poçoRepository.ExisteCálculoComMesmoNome(calc.Nome, poço.Id);

                if (existeCálculoComMesmoNome)
                {
                    context.AddFailure($"{calc.Nome} - já existe cálculo com esse nome");
                }
            });
        }
    }
}
