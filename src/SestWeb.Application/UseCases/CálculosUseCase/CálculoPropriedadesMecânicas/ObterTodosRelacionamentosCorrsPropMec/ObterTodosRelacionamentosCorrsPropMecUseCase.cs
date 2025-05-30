using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodosRelacionamentosCorrsPropMec
{
    internal class ObterTodosRelacionamentosCorrsPropMecUseCase : IObterTodosRelacionamentosCorrsPropMecUseCase
    {
        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository _relacionamentoCorrsPropMecPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterTodosRelacionamentosCorrsPropMecUseCase(IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterTodosRelacionamentosCorrsPropMecOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterTodosRelacionamentosCorrsPropMecOutput.PoçoNãoEncontrado(idPoço);
                }

                var correlações = await GetAllRelationships(idPoço);

                if (correlações.Count == 0)
                {
                    return ObterTodosRelacionamentosCorrsPropMecOutput.RelacionamentosNãoEncontrados();
                }

                return ObterTodosRelacionamentosCorrsPropMecOutput.RelacionamentosObtidos(correlações);
            }
            catch (Exception e)
            {
                return ObterTodosRelacionamentosCorrsPropMecOutput.RelacionamentosNãoObtidos(e.Message);
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
    }
}
