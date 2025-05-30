using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação.Factory;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelaçãoPoço
{
    internal class AtualizarCorrelaçãoPoçoUseCase : IAtualizarCorrelaçãoPoçoUseCase
    {
        private readonly ICorrelaçãoPoçoWriteOnlyRepository _correlaçãoPoçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoFactory _correlaçãoPoçoFactory;

        public AtualizarCorrelaçãoPoçoUseCase(ICorrelaçãoPoçoWriteOnlyRepository correlaçãoPoçoWriteOnlyRepository,
            ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository, ICorrelaçãoPoçoFactory correlaçãoPoçoFactory)
        {
            _correlaçãoPoçoWriteOnlyRepository = correlaçãoPoçoWriteOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _correlaçãoPoçoFactory = correlaçãoPoçoFactory;
        }

        public async Task<AtualizarCorrelaçãoPoçoOutput> Execute(string idPoço, string nome, string descrição, string expressão)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return AtualizarCorrelaçãoPoçoOutput.PoçoNãoEncontrado(idPoço);
                }

                var corr = await _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçãoPoçoPeloNome(idPoço, nome);
                if (corr == null)
                    return AtualizarCorrelaçãoPoçoOutput.CorrelaçãoNãoEncontrada(nome);

                var result = _correlaçãoPoçoFactory
                    .UpdateCorrelação(idPoço, nome, corr.Autor.Nome, corr.Autor.Chave, descrição, corr.Origem.ToString(), expressão, out ICorrelaçãoPoço correlação);

                if (!result.IsValid)
                    return AtualizarCorrelaçãoPoçoOutput.CorrelaçãoNãoAtualizada(string.Join("\n", result.Errors));

                await _correlaçãoPoçoWriteOnlyRepository.UpdateCorrelaçãoPoço((CorrelaçãoPoço)correlação);

                return AtualizarCorrelaçãoPoçoOutput.CorrelaçãoAtualizada((CorrelaçãoPoço)correlação);
            }
            catch (Exception e)
            {
                return AtualizarCorrelaçãoPoçoOutput.CorrelaçãoNãoAtualizada(e.Message);
            }
        }
    }
}
