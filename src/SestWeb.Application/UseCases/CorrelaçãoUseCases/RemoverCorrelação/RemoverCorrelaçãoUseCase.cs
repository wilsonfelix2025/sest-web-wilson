using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelação
{
    internal class RemoverCorrelaçãoUseCase : IRemoverCorrelaçãoUseCase
    {
        private readonly ICorrelaçãoWriteOnlyRepository _correlaçãoWriteOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;

        public RemoverCorrelaçãoUseCase(ICorrelaçãoWriteOnlyRepository correlaçãoWriteOnlyRepository, ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository)
        {
            _correlaçãoWriteOnlyRepository = correlaçãoWriteOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
        }

        public async Task<RemoverCorrelaçãoOutput> Execute(string nome)
        {
            try
            {
                var correlação = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(nome);

                if (correlação == null)
                {
                    return RemoverCorrelaçãoOutput.CorrelaçãoNãoEncontrada(nome);
                }

                if (correlação.Origem == Origem.Fixa)
                {
                    return RemoverCorrelaçãoOutput.CorrelaçãoSemPermissãoParaRemoção(nome);
                }

                await _correlaçãoWriteOnlyRepository.RemoverCorrelação(nome);

                return RemoverCorrelaçãoOutput.CorrelaçãoRemovida();
            }
            catch (Exception e)
            {
                return RemoverCorrelaçãoOutput.CorrelaçãoNãoRemovida(e.Message);
            }
        }
    }
}
