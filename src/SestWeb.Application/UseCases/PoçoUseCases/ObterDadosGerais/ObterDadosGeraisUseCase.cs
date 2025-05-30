using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterDadosGerais
{
    internal class ObterDadosGeraisUseCase : IObterDadosGeraisUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterDadosGeraisUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterDadosGeraisOutput> Execute(string id)
        {
            try
            {
                var dadosGerais = await _poçoReadOnlyRepository.ObterDadosGerais(id);

                if (dadosGerais == null)
                    return ObterDadosGeraisOutput.PoçoNãoEncontrado(id);

                return ObterDadosGeraisOutput.DadosGeraisObtidos(dadosGerais);
            }
            catch (Exception e)
            {
                return ObterDadosGeraisOutput.DadosGeraisNãoObtidos(e.Message);
            }
        }
    }
}