using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec
{
    internal class LoadRelacionamentosCorrsPropMecUseCase : ILoadRelacionamentosCorrsPropMecUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly ILoaderRelacionamentosPropMec _loaderRelacionamentosPropMec;
        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecWriteOnlyRepository _relacionamentoCorrsPropMecWriteOnlyRepository;

        public LoadRelacionamentosCorrsPropMecUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, 
            ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, ILoaderRelacionamentosPropMec loaderRelacionamentosPropMec,
            IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository, IRelacionamentoCorrsPropMecWriteOnlyRepository relacionamentoCorrsPropMecWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _loaderRelacionamentosPropMec = loaderRelacionamentosPropMec;
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecWriteOnlyRepository = relacionamentoCorrsPropMecWriteOnlyRepository;
        }

        public async Task<LoadRelacionamentosCorrsPropMecOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return LoadRelacionamentosCorrsPropMecOutput.PoçoNãoEncontrado(idPoço);
                }

                var correlações = await GetAllCorrs(idPoço);

                if (correlações.Count == 0)
                {
                    return LoadRelacionamentosCorrsPropMecOutput.CorrelaçõesNãoEncontradas();
                }

                var relacionamentos = _loaderRelacionamentosPropMec.IsCorrsLoaded() ? _loaderRelacionamentosPropMec.GetRelacionamentos() : _loaderRelacionamentosPropMec.Load(correlações.ToList());

                if (relacionamentos.Count == 0)
                {
                    return LoadRelacionamentosCorrsPropMecOutput.RelacionamentosNãoEncontrados();
                }

                var relLoaded = await _relacionamentoCorrsPropMecReadyOnlyRepository.RelacionamentosSistemaCarregados();
                if (relLoaded)
                {
                    foreach (RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamento in relacionamentos)
                    {
                        var existeRelacionamento = await _relacionamentoCorrsPropMecReadyOnlyRepository.ExisteRelacionamento(relacionamento.Nome);

                        if (existeRelacionamento)
                            await _relacionamentoCorrsPropMecWriteOnlyRepository.UpdateRelacionamentoCorrsPropMec(
                                relacionamento);
                        else
                            await _relacionamentoCorrsPropMecWriteOnlyRepository.CriarRelacionamentoCorrsPropMec(
                                relacionamento);
                    }
                }
                else
                {
                    await _relacionamentoCorrsPropMecWriteOnlyRepository.InsertRelacionamentoCorrsPropMec(relacionamentos);
                }

                return LoadRelacionamentosCorrsPropMecOutput.RelacionamentosCarregados(relacionamentos);
            }
            catch (Exception e)
            {
                return LoadRelacionamentosCorrsPropMecOutput.RelacionamentosNãoCarregados(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<Correlação>> GetAllCorrs(string idPoço)
        {
            List<Correlação> corrs = new List<Correlação>();

            var correlações = await _correlaçãoReadOnlyRepository.ObterTodasCorrelações();

            if (correlações?.Count > 0)
                corrs.AddRange(correlações);

            var correlaçõesPoço = await _correlaçãoPoçoReadOnlyRepository.ObterTodasCorrelaçõesPoço(idPoço);

            if (correlaçõesPoço?.Count > 0)
                corrs.AddRange(correlaçõesPoço);

            return corrs;
        }
    }
}
