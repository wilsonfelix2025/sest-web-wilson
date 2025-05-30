using System;
using System.Threading.Tasks;
using SestWeb.Application.Helpers;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.PoçoUseCases.CriarPoço
{
    internal class CriarPoçoUseCase : ICriarPoçoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public CriarPoçoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<CriarPoçoOutput> Execute(string nome, TipoPoço tipoPoço)
        {
            try
            {
                var result = ValidarNome(nome).Result;

                if (result.Status == ValidationResultStatus.Falha)
                    return CriarPoçoOutput.JáExistePoçoComEsseNome();

                var novoPoço = PoçoFactory.CriarPoço(nome, nome, tipoPoço);

                await _poçoWriteOnlyRepository.CriarPoço(novoPoço);

                var poçoOutput = new PoçoOutput(novoPoço.Id.ToString(), novoPoço.Nome, novoPoço.TipoPoço);

                return CriarPoçoOutput.PoçoCriado(poçoOutput);
            }
            catch (Exception e)
            {
                return CriarPoçoOutput.PoçoNãoCriado(e.Message);
            }
        }

        private async Task<ValidationResult> ValidarNome(string nome)
        {
            if (await _poçoReadOnlyRepository.ExistePoçoComMesmoNome(nome))
                return new ValidationResult(ValidationResultStatus.Falha);

            return new ValidationResult(ValidationResultStatus.Sucesso);
        }
    }
}