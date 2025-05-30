using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelaçãoPoço
{
    internal class RemoverCorrelaçãoPoçoUseCase : IRemoverCorrelaçãoPoçoUseCase
    {
        private readonly ICorrelaçãoPoçoWriteOnlyRepository _correlaçãoPoçoWriteOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public RemoverCorrelaçãoPoçoUseCase(ICorrelaçãoPoçoWriteOnlyRepository correlaçãoPoçoWriteOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoPoçoWriteOnlyRepository = correlaçãoPoçoWriteOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<RemoverCorrelaçãoPoçoOutput> Execute(string idPoço, string nome)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return RemoverCorrelaçãoPoçoOutput.PoçoNãoEncontrado(idPoço);
                }

                var correlação = await _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçãoPoçoPeloNome(idPoço, nome);

                if (correlação == null)
                {
                    return RemoverCorrelaçãoPoçoOutput.CorrelaçãoNãoEncontrada(nome);
                }

                if (correlação.Origem == Origem.Fixa)
                {
                    return RemoverCorrelaçãoPoçoOutput.CorrelaçãoSemPermissãoParaRemoção(nome);
                }

                await _correlaçãoPoçoWriteOnlyRepository.RemoverCorrelaçãoPoço(nome);

                return RemoverCorrelaçãoPoçoOutput.CorrelaçãoRemovida();
            }
            catch (Exception e)
            {
                return RemoverCorrelaçãoPoçoOutput.CorrelaçãoNãoRemovida(e.Message);
            }
        }
    }
}
