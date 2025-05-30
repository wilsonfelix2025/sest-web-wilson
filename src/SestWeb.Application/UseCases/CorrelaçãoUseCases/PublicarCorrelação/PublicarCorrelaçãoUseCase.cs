using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.PublicarCorrelação
{
    internal class PublicarCorrelaçãoUseCase : IPublicarCorrelaçãoUseCase
    {
        private readonly ICorrelaçãoPoçoWriteOnlyRepository _correlaçãoPoçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoWriteOnlyRepository _correlaçãoWriteOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;

        public PublicarCorrelaçãoUseCase(ICorrelaçãoPoçoWriteOnlyRepository correlaçãoPoçoWriteOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository,
            IPoçoReadOnlyRepository poçoReadOnlyRepository, ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoWriteOnlyRepository correlaçãoWriteOnlyRepository, ICorrelaçãoFactory correlaçãoFactory)
        {
            _correlaçãoPoçoWriteOnlyRepository = correlaçãoPoçoWriteOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoWriteOnlyRepository = correlaçãoWriteOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
        }

        public async Task<PublicarCorrelaçãoOutput> Execute(string idPoço, string nome)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return PublicarCorrelaçãoOutput.PoçoNãoEncontrado(idPoço);
                }

                var existeCorrelação = await _correlaçãoReadOnlyRepository.ExisteCorrelação(nome);
                if (existeCorrelação)
                {
                    return PublicarCorrelaçãoOutput.CorrelaçãoExistente(nome);
                }

                var corr = await _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçãoPoçoPeloNome(idPoço, nome);
                if (corr == null)
                    return PublicarCorrelaçãoOutput.CorrelaçãoNãoEncontrada(nome);

                await _correlaçãoPoçoWriteOnlyRepository.RemoverCorrelaçãoPoço(nome);

                var result = _correlaçãoFactory
                    .CreateCorrelação(nome, corr.Autor.Nome, corr.Autor.Chave, corr.Descrição, Origem.Usuário.ToString(), corr.Expressão.Bruta, out ICorrelação correlação);

                if (!result.IsValid)
                    return PublicarCorrelaçãoOutput.CorrelaçãoNãoPublicada(string.Join("\n", result.Errors));

                await _correlaçãoWriteOnlyRepository.CriarCorrelação((Correlação)correlação);

                return PublicarCorrelaçãoOutput.CorrelaçãoPublicada();
            }
            catch (Exception e)
            {
                return PublicarCorrelaçãoOutput.CorrelaçãoNãoPublicada(e.Message);
            }
        }
    }
}
