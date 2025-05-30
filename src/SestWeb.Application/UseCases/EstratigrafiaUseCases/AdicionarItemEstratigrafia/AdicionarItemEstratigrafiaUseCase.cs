using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia
{
    internal class AdicionarItemEstratigrafiaUseCase : IAdicionarItemEstratigrafiaUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public AdicionarItemEstratigrafiaUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<AdicionarItemEstratigrafiaOutput> Execute(AdicionarItemEstratigrafiaInput input)
        {
            try
            {
                var estratigrafia = await _poçoReadOnlyRepository.ObterEstratigrafia(input.IdPoço);
                var trajetória = await _poçoReadOnlyRepository.ObterTrajetória(input.IdPoço);

                if(estratigrafia is null || trajetória is null)
                    return AdicionarItemEstratigrafiaOutput.ItemEstratigrafiaNãoAdicionado($"Não foi possível obter estratigrafia do poço {input.IdPoço}.");

                var itemCriado = estratigrafia.CriarItemEstratigrafia(trajetória, input.Tipo, input.PM, input.Sigla, input.Descrição, input.Idade);

                if(!itemCriado)
                    return AdicionarItemEstratigrafiaOutput.ItemEstratigrafiaNãoAdicionado($"Não foi possível criar item de estratigrafia com PM = {input.PM}");

                var result = await _poçoWriteOnlyRepository.AtualizarEstratigrafia(input.IdPoço, estratigrafia);
            
                if(result)
                    return AdicionarItemEstratigrafiaOutput.ItemEstratigrafiaAdicionado();

                return AdicionarItemEstratigrafiaOutput.ItemEstratigrafiaNãoAdicionado($"Não foi possível adicionar item de estratigrafia com PM = {input.PM}");
            }
            catch (Exception e)
            {
                return AdicionarItemEstratigrafiaOutput.ItemEstratigrafiaNãoAdicionado(e.Message);
            }
        }
    }
}
