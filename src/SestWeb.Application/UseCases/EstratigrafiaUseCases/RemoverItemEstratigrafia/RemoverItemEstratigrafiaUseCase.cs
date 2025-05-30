using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.RemoverItemEstratigrafia
{
    internal class RemoverItemEstratigrafiaUseCase : IRemoverItemEstratigrafiaUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RemoverItemEstratigrafiaUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository,
            IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RemoverItemEstratigrafiaOutput> Execute(string idPoço, string tipo, double pm)
        {
            try
            {
                var estratigrafia = await _poçoReadOnlyRepository.ObterEstratigrafia(idPoço);

                if (estratigrafia is null)
                    return RemoverItemEstratigrafiaOutput.ItemEstratigrafiaNãoRemovido(
                        $"Não foi possível obter estratigrafia do poço {idPoço}.");

                var tipoEstratigrafia = TipoEstratigrafia.ObterPeloTipo(tipo);
                var remove = estratigrafia.RemoverItemEstratigrafia(tipoEstratigrafia.Tipo, new Profundidade(pm));

                if (!remove)
                    return RemoverItemEstratigrafiaOutput.ItemEstratigrafiaNãoRemovido(
                        $"Não foi possível remover item de estratigrafia com tipo = {tipoEstratigrafia.Nome} e PM = {pm}.");

                var result = await _poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, estratigrafia);

                if (result)
                    return RemoverItemEstratigrafiaOutput.ItemEstratigrafiaRemovido();

                return RemoverItemEstratigrafiaOutput.ItemEstratigrafiaNãoRemovido(
                    $"Não foi possível remover item de estratigrafia com tipo = {tipoEstratigrafia.Nome} e PM = {pm} do banco.");
            }
            catch (Exception e)
            {
                return RemoverItemEstratigrafiaOutput.ItemEstratigrafiaNãoRemovido(e.Message);
            }
        }
    }
}