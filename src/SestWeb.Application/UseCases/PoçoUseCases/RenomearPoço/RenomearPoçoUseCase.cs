using System;
using System.Threading.Tasks;
using SestWeb.Application.Helpers;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PoçoUseCases.RenomearPoço
{
    internal class RenomearPoçoUseCase : IRenomearPoçoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RenomearPoçoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RenomearPoçoOutput> Execute(string id, string nomePoço)
        {
            try
            {
                var validationResult = ValidarNome(nomePoço).Result;

                if (validationResult.Status == ValidationResultStatus.Falha)
                    return RenomearPoçoOutput.JáExistePoçoComEsseNome();

                var poço = await _poçoReadOnlyRepository.ObterPoço(id);

                if (poço == null)
                    return RenomearPoçoOutput.PoçoNãoEncontrado(id);

                poço.DadosGerais.Identificação.Nome = nomePoço;

                var result = await _poçoWriteOnlyRepository.AtualizarPoço(poço);

                if (result)
                {
                    return RenomearPoçoOutput.PoçoRenomeado();
                }

                return RenomearPoçoOutput.PoçoNãoRenomeado();
            }
            catch (Exception e)
            {
                return RenomearPoçoOutput.PoçoNãoRenomeado(e.Message);
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
