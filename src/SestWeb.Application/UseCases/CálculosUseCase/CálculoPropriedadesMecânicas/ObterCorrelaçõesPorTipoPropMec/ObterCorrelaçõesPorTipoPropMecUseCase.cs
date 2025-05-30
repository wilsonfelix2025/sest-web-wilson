using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPorTipoPropMec
{

    internal class ObterCorrelaçõesPorTipoPropMecUseCase : IObterCorrelaçõesPorTipoPropMecUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository _relacionamentoCorrsPropMecPoçoReadOnlyRepository;

        public ObterCorrelaçõesPorTipoPropMecUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
        }

        public async Task<ObterCorrelaçõesPorTipoPropMecOutput> Execute(string idPoço, string grupoLitológico, string mnemônico)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterCorrelaçõesPorTipoPropMecOutput.PoçoNãoEncontrado(idPoço);
                }

                var grupoLito = GrupoLitologico.GetFromName(grupoLitológico);
                if (grupoLito == null)
                    return ObterCorrelaçõesPorTipoPropMecOutput.GrupoLitológicoNãoEncontrado(grupoLitológico);

                await ResetGerenciadorUcsCoesaAngat(idPoço);
                var correlações = GerenciadorUcsCoesaAngat.ObterCorrelaçõesPossíveisPorGrupoLitologicoETipo(grupoLito, mnemônico);

                return ObterCorrelaçõesPorTipoPropMecOutput.CorrelaçõesObtidas(correlações);
            }
            catch (Exception e)
            {
                return ObterCorrelaçõesPorTipoPropMecOutput.CorrelaçõesNãoObtidas(e.Message);
            }
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
