using FluentValidation;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using System;

namespace SestWeb.Application.Validadores
{
    public class CriarPerfilRelaçãoTensõesValidator : AbstractValidator<CriarPerfilRelaçãoInput>
    {
        private readonly IPerfilReadOnlyRepository _perfilRepository;

        public CriarPerfilRelaçãoTensõesValidator(IPerfilReadOnlyRepository perfilRepository,  Poço poço)
        {
            _perfilRepository = perfilRepository;

            RuleFor(p => p.Valores).NotEmpty();
           
            RuleFor(p => p).Custom(async (input, context) =>
            {
                var existePerfilComMesmoNome = await _perfilRepository.ExistePerfilComMesmoNome(input.NomePerfil, poço.Id);

                if (existePerfilComMesmoNome)
                {
                    context.AddFailure($"{input.NomePerfil} - já existe perfil com esse nome");
                }
            });
        }
    }
}
