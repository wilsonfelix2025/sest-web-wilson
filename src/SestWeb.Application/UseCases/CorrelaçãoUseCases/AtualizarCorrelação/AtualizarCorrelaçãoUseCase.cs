using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelação
{
    internal class AtualizarCorrelaçãoUseCase : IAtualizarCorrelaçãoUseCase
    {
        private readonly ICorrelaçãoWriteOnlyRepository _correlaçãoWriteOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;

        public AtualizarCorrelaçãoUseCase(ICorrelaçãoWriteOnlyRepository correlaçãoWriteOnlyRepository, ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory)
        {
            _correlaçãoWriteOnlyRepository = correlaçãoWriteOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
        }

        public async Task<AtualizarCorrelaçãoOutput> Execute(string nome, string descrição, string expressão)
        {
            try
            {
                var corr = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(nome);
                if (corr == null)
                    return AtualizarCorrelaçãoOutput.CorrelaçãoNãoEncontrada(nome);

                var result = _correlaçãoFactory
                    .UpdateCorrelação(nome, corr.Autor.Nome, corr.Autor.Chave, descrição, corr.Origem.ToString(), expressão, out ICorrelação correlação);

                if (!result.IsValid)
                    return AtualizarCorrelaçãoOutput.CorrelaçãoNãoAtualizada(string.Join("\n", result.Errors));

                await _correlaçãoWriteOnlyRepository.UpdateCorrelação((Correlação)correlação);

                return AtualizarCorrelaçãoOutput.CorrelaçãoAtualizada((Correlação)correlação);
            }
            catch (Exception e)
            {
                return AtualizarCorrelaçãoOutput.CorrelaçãoNãoAtualizada(e.Message);
            }
        }
    }
}
