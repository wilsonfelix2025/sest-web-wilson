using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodasCorrelaçõesPossíveisPropMec
{
    internal class ObterTodasCorrelaçõesPossíveisPropMecUseCase : IObterTodasCorrelaçõesPossíveisPropMecUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository _relacionamentoCorrsPropMecPoçoReadOnlyRepository;

        public ObterTodasCorrelaçõesPossíveisPropMecUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository,
            IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
        }

        public async Task<ObterTodasCorrelaçõesPossíveisPropMecOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterTodasCorrelaçõesPossíveisPropMecOutput.PoçoNãoEncontrado(idPoço);
                }

                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                await ResetGerenciadorUcsCoesaAngat(idPoço);
                var correlaçõesPossíveis = GerenciadorUcsCoesaAngat.GetAllPropMecPossibleCorrs(poço);

                return ObterTodasCorrelaçõesPossíveisPropMecOutput.CorrelaçõesObtidas(correlaçõesPossíveis);
            }
            catch (Exception e)
            {
                return ObterTodasCorrelaçõesPossíveisPropMecOutput.CorrelaçõesNãoObtidas(e.Message);
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
