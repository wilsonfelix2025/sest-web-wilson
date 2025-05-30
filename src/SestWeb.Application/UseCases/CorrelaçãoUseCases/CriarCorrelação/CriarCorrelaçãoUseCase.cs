using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelação
{
    internal class CriarCorrelaçãoUseCase : ICriarCorrelaçãoUseCase
    {
        private readonly ICorrelaçãoWriteOnlyRepository _correlaçãoWriteOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public CriarCorrelaçãoUseCase(ICorrelaçãoWriteOnlyRepository correlaçãoWriteOnlyRepository, ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository,
            ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoWriteOnlyRepository = correlaçãoWriteOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<CriarCorrelaçãoOutput> Execute(string idPoço, string nome, string nomeAutor, string chaveAutor, string descrição, string expressão)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return CriarCorrelaçãoOutput.PoçoNãoEncontrado(idPoço);
                }

                var existeCorrelaçãoPoço = await _correlaçãoPoçoReadOnlyRepository.ExisteCorrelaçãoPoço(idPoço, nome);
                var existeCorrelação = await _correlaçãoReadOnlyRepository.ExisteCorrelação(nome);
                if (existeCorrelaçãoPoço || existeCorrelação)
                    return CriarCorrelaçãoOutput.CorrelaçãoExistente(nome);

                var result = _correlaçãoFactory
                    .CreateCorrelação(nome, nomeAutor, chaveAutor, descrição, Origem.Usuário.ToString(), expressão, out ICorrelação correlação);

                if(!result.IsValid)
                    return CriarCorrelaçãoOutput.CorrelaçãoNãoCriada(string.Join("\n", result.Errors));

                await _correlaçãoWriteOnlyRepository.CriarCorrelação((Correlação)correlação);

                return CriarCorrelaçãoOutput.CorrelaçãoCriada((Correlação)correlação);
            }
            catch (Exception e)
            {
                return CriarCorrelaçãoOutput.CorrelaçãoNãoCriada(e.Message);
            }
        }
    }
}
