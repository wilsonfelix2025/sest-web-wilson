using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.PoçoUseCases.AdicionarPoçoApoio
{
    internal class AdicionarPoçoApoioUseCase : IAdicionarPoçoApoioUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public AdicionarPoçoApoioUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<AdicionarPoçoApoioOutput> Execute(string idPoço, string idPoçoApoio)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                if (poço == null)
                    return AdicionarPoçoApoioOutput.SemSucesso("Poço não encontrado");

                var poçoApoio = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(idPoçoApoio);

                if(poçoApoio == null)
                    return AdicionarPoçoApoioOutput.SemSucesso("Poço apoio não encontrado");
                
                if (poço.IdsPoçosApoio.Contains(idPoçoApoio))
                    return AdicionarPoçoApoioOutput.ComSucesso();

                poço.AdicionarPoçoApoio(idPoçoApoio);

                await _poçoWriteOnlyRepository.AtualizarListaPoçoApoio(poço.Id, poço.IdsPoçosApoio);
                
                return AdicionarPoçoApoioOutput.ComSucesso();
            }
            catch (Exception e)
            {
                return AdicionarPoçoApoioOutput.SemSucesso(e.Message);
            }
        }

    }
}