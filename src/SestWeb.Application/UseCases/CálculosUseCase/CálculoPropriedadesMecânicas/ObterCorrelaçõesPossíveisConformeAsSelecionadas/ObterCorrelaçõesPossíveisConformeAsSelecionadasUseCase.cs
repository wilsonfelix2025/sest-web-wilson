using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas
{
    internal class ObterCorrelaçõesPossíveisConformeAsSelecionadasUseCase : IObterCorrelaçõesPossíveisConformeAsSelecionadasUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository _relacionamentoCorrsPropMecPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterCorrelaçõesPossíveisConformeAsSelecionadasUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, 
            ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository,
            IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository, IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput> Execute(string idPoço, string grupoLitológicoSelecionado, string ucsSelecionada, string coesaSelecionada,
            string angatSelecionada, string restrSelecionada)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput.PoçoNãoEncontrado(idPoço);
                }

                var grupoLitológico = GrupoLitologico.GetFromName(grupoLitológicoSelecionado);
                if(grupoLitológico == null)
                    return ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput.GrupoLitológicoNãoEncontrado(grupoLitológicoSelecionado);

                var correlaçãoUcsSelecionada = await GetCorrByName(idPoço, ucsSelecionada);
                var correlaçãoCoesaSelecionada = await GetCorrByName(idPoço, coesaSelecionada);
                var correlaçãoAngatSelecionada = await GetCorrByName(idPoço, angatSelecionada);
                var correlaçãoRestrSelecionada = await GetCorrByName(idPoço, restrSelecionada);

                await ResetGerenciadorUcsCoesaAngat(idPoço);
                var correlaçõesPossíveis = GerenciadorUcsCoesaAngat.ObterCorrelaçõesPossíveisPorGrupoLitologico(grupoLitológico,
                    correlaçãoUcsSelecionada, correlaçãoCoesaSelecionada, correlaçãoAngatSelecionada,
                    correlaçãoRestrSelecionada);

                await SetBiotPossibleCorrelations(idPoço, ucsSelecionada, correlaçõesPossíveis);

                return ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput.CorrelaçõesObtidas(correlaçõesPossíveis);
            }
            catch (Exception e)
            {
                return ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput.CorrelaçõesNãoObtidas(e.Message);
            }
        }

        private async Task SetBiotPossibleCorrelations(string idPoço, string ucsSelecionada, PropMecPossibleCorrsPerLitoGroup correlaçõesPossíveis)
        {
            List<Correlação> biotPossibleCorrelations = new List<Correlação>();
            
            var biotRegimeElástico = await GetCorrByName(idPoço, "BIOT_REGIME_ELÁSTICO");
            var biotRegimePoroElástico = await GetCorrByName(idPoço, "BIOT_REGIME_POROELÁSTICO");

            biotPossibleCorrelations.Add(biotRegimeElástico);
            biotPossibleCorrelations.Add(biotRegimePoroElástico);

            if (ucsSelecionada != null)
            {
                if (ucsSelecionada.Equals("UCS_PRASAD_ET_AL_1"))
                {
                    var biotPrasadEtAl1 = await GetCorrByName(idPoço, "BIOT_PRASAD_ET_AL_1");
                    biotPossibleCorrelations.Add(biotPrasadEtAl1);
                }
                else if(ucsSelecionada.Equals("UCS_PRASAD_ET_AL_2"))
                {
                    var biotPrasadEtAl2 = await GetCorrByName(idPoço, "BIOT_PRASAD_ET_AL_2");
                    biotPossibleCorrelations.Add(biotPrasadEtAl2);
                }
            }

            correlaçõesPossíveis.SetBiotPossibleCorrelations(biotPossibleCorrelations);
        }

        private async Task<Correlação> GetCorrByName(string idPoço, string name)
        {
            var existeCorr = await _correlaçãoReadOnlyRepository.ExisteCorrelação(name);

            if (existeCorr)
                return await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(name);

            return await _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçãoPoçoPeloNome(idPoço, name);
        }

        private async Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> GetAllRelationships(string idPoço)
        {
            List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> allRels = new List<RelacionamentoUcsCoesaAngatPorGrupoLitológico>();

            var relacionamentos = await _relacionamentoCorrsPropMecReadyOnlyRepository.ObterTodosRelacionamentos();

            if (relacionamentos?.Count > 0)
                allRels.AddRange(relacionamentos);

            var relacionamentospoço = await _relacionamentoCorrsPropMecPoçoReadOnlyRepository.ObterTodosRelacionamentosCorrsPropMecPoço(idPoço);

            if (relacionamentospoço?.Count > 0)
                allRels.AddRange(relacionamentospoço);

            return allRels;
        }

        private async Task ResetGerenciadorUcsCoesaAngat(string idPoço)
        {
            var relationships = GetAllRelationships(idPoço);
            GerenciadorUcsCoesaAngat.Reset(relationships.Result.ToList());
        }
    }
}
