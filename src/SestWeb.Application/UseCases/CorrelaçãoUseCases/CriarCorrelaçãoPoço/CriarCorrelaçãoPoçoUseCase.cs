using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação.Factory;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelaçãoPoço
{
    internal class CriarCorrelaçãoPoçoUseCase : ICriarCorrelaçãoPoçoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoWriteOnlyRepository _correlaçãoPoçoWriteOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoFactory _correlaçãoPoçoFactory;

        public CriarCorrelaçãoPoçoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, ICorrelaçãoPoçoWriteOnlyRepository correlaçãoPoçoWriteOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoFactory correlaçãoPoçoFactory)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _correlaçãoPoçoWriteOnlyRepository = correlaçãoPoçoWriteOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoFactory = correlaçãoPoçoFactory;
        }

        public async Task<CriarCorrelaçãoPoçoOutput> Execute(string idPoço, string nome, string nomeAutor, string chaveAutor, string descrição, string expressão)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return CriarCorrelaçãoPoçoOutput.PoçoNãoEncontrado(idPoço);
                }

                var existeCorrelaçãoPoço = await _correlaçãoPoçoReadOnlyRepository.ExisteCorrelaçãoPoço(idPoço, nome);
                var existeCorrelação = await _correlaçãoReadOnlyRepository.ExisteCorrelação(nome);
                if (existeCorrelaçãoPoço || existeCorrelação)
                    return CriarCorrelaçãoPoçoOutput.CorrelaçãoExistente(nome);

                var result = _correlaçãoPoçoFactory
                    .CreateCorrelação(idPoço, nome, nomeAutor, chaveAutor, descrição, expressão, out ICorrelaçãoPoço correlação);

                if (!result.IsValid)
                    return CriarCorrelaçãoPoçoOutput.CorrelaçãoNãoCriada(string.Join("\n", result.Errors));

                await _correlaçãoPoçoWriteOnlyRepository.CriarCorrelaçãoPoço((CorrelaçãoPoço)correlação);

                return CriarCorrelaçãoPoçoOutput.CorrelaçãoCriada((CorrelaçãoPoço)correlação);
            }
            catch (Exception e)
            {
                return CriarCorrelaçãoPoçoOutput.CorrelaçãoNãoCriada(e.Message);
            }
        }
    }
}
