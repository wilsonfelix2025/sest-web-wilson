using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.EstilosVisuais;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Application.UseCases.PerfilUseCases.EditarPerfil
{
    internal class EditarPerfilUseCase : IEditarPerfilUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarPerfilUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IPipelineUseCase pipelineUseCase)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarPerfilOutput> Execute(string idPerfil, string nome, string descrição, EstiloVisual estiloVisual, PontoDTO[] pontosDTO, bool emPv)
        {
            try
            {
                var existePerfil = await _perfilReadOnlyRepository.ExistePerfilComId(idPerfil);
                if (!existePerfil)
                {
                    return EditarPerfilOutput.PerfilNãoEncontrado(idPerfil);
                }

                var perfilPoço = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfilPoço.IdPoço);
                var trajetória = poço.Trajetória;
                var litologiaPadrão = poço.ObterLitologiaPadrão();
                var perfil = PerfisFactory.Create(perfilPoço.Mnemonico, perfilPoço.Nome, trajetória, litologiaPadrão);
                var pontosPerfilPoço = new List<Ponto>(perfilPoço.GetPontos());

                perfil.EditarId(perfilPoço.Id);
                perfil.EditarNome(nome);
                perfil.EditarDescrição(descrição);
                perfil.IdPoço = poço.Id;
                perfil.EditarEstiloVisual(estiloVisual);
                perfil.Trend = perfilPoço.Trend;
                perfil.IdCálculo = perfilPoço.IdCálculo;
                
                if (!emPv)
                {
                    loadPerfilPointsData(pontosDTO,out IList<Profundidade> pms, out IList<double> valores);
                    perfil.EditPontosEmPm(trajetória, pms, valores);
                } else
                {
                    CarregarPontosEmPv(pontosDTO, out var pvs, out var valores);
                    perfil.EditPontosEmPv(trajetória, pvs, valores);
                }

                await _perfilWriteOnlyRepository.AtualizarPerfil(perfil);

                var perfisAlterados = await _pipeUseCase.Execute(poço, perfil);

                return EditarPerfilOutput.PerfilEditado(perfil, perfisAlterados);
            }
            catch (Exception e)
            {
                return EditarPerfilOutput.PerfilNãoEditado(e.Message);
            }
        }

        private void loadPerfilPointsData(PontoDTO[] pontosDTO, out IList<Profundidade> pms, out IList<double> valores)
        {
            pms = new List<Profundidade>();
            valores = new List<double>();

            foreach (var pontoDto in pontosDTO)
            {
                pms.Add(new Profundidade(StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 4)));
                valores.Add(StringUtils.ToDoubleInvariantCulture(pontoDto.Valor, 2));
            }
        }

        private void CarregarPontosEmPv(PontoDTO[] pontosDTO, out IList<Profundidade> pvs, out IList<double> valores)
        {
            pvs = new List<Profundidade>();
            valores = new List<double>();
            foreach (var pontoDto in pontosDTO)
            {
                pvs.Add(new Profundidade(StringUtils.ToDoubleInvariantCulture(pontoDto.Pv, 4)));
                valores.Add(StringUtils.ToDoubleInvariantCulture(pontoDto.Valor, 2));
            }
        }
    }
}
