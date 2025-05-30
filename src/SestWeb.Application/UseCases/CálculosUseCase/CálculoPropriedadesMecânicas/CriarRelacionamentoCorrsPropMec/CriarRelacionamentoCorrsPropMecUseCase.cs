using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMec
{
    internal class CriarRelacionamentoCorrsPropMecUseCase : ICriarRelacionamentoCorrsPropMecUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecWriteOnlyRepository _relacionamentoCorrsPropMecWriteOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository _relacionamentoCorrsPropMecPoçoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly IRelacionamentoPropMecFactory _relacionamentoPropMecFactory;

        public CriarRelacionamentoCorrsPropMecUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository,
            IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository,
            IRelacionamentoCorrsPropMecWriteOnlyRepository relacionamentoCorrsPropMecWriteOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository,
            ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository,
            ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, IRelacionamentoPropMecFactory relacionamentoPropMecFactory)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecWriteOnlyRepository = relacionamentoCorrsPropMecWriteOnlyRepository;
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _relacionamentoPropMecFactory = relacionamentoPropMecFactory;
        }

        public async Task<CriarRelacionamentoCorrsPropMecOutput> Execute(string idPoço,
            string grupoLitológico, string nomeAutor, string chaveAutor, string corrUcs,
            string corrCoesa, string corrAngat, string corrRestr)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return CriarRelacionamentoCorrsPropMecOutput.PoçoNãoEncontrado(idPoço);
                }

                var existeCorr = await ExisteCorrelação(idPoço, corrUcs.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecOutput.CorrelaçãoInexistente(corrUcs);

                existeCorr = await ExisteCorrelação(idPoço, corrCoesa.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecOutput.CorrelaçãoInexistente(corrCoesa);

                existeCorr = await ExisteCorrelação(idPoço, corrAngat.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecOutput.CorrelaçãoInexistente(corrAngat);

                existeCorr = await ExisteCorrelação(idPoço, corrRestr.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecOutput.CorrelaçãoInexistente(corrRestr);

                var grupoLito = GrupoLitologico.GetFromName(grupoLitológico);
                if (grupoLito == null)
                    return CriarRelacionamentoCorrsPropMecOutput.GrupoLitológicoNãoEncontrado(grupoLitológico);

                var existeRel =
                    await ExisteRelacionamento(idPoço, grupoLitológico, corrUcs, corrCoesa, corrAngat, corrRestr);

                if (existeRel)
                    return CriarRelacionamentoCorrsPropMecOutput.RelacionamentoExistente(grupoLitológico, corrUcs, corrCoesa, corrAngat, corrRestr);

                var result = _relacionamentoPropMecFactory.CreateRelacionamentoPropMec(grupoLitológico, Origem.Usuário.ToString(),
                    nomeAutor, chaveAutor, GetCorr(idPoço, corrUcs).Result, GetCorr(idPoço, corrCoesa).Result, GetCorr(idPoço, corrAngat).Result, GetCorr(idPoço, corrRestr).Result,
                    out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoPropMec);

                if (!result.IsValid)
                    return CriarRelacionamentoCorrsPropMecOutput.RelacionamentoNãoCriado(string.Join("\n", result.Errors));

                await _relacionamentoCorrsPropMecWriteOnlyRepository.CriarRelacionamentoCorrsPropMec(relacionamentoPropMec);

                return CriarRelacionamentoCorrsPropMecOutput.RelacionamentoCriado(relacionamentoPropMec);
            }
            catch (Exception e)
            {
                return CriarRelacionamentoCorrsPropMecOutput.RelacionamentoNãoCriado(e.Message);
            }
        }

        private async Task<bool> ExisteCorrelação(string idPoço, string nome)
        {
            var existeCorrelação = await _correlaçãoReadOnlyRepository.ExisteCorrelação(nome);

            if (existeCorrelação)
                return true;

            return await _correlaçãoPoçoReadOnlyRepository.ExisteCorrelaçãoPoço(idPoço, nome);
        }

        private async Task<bool> ExisteRelacionamento(string idPoço, string grupoLitológico, string corrUcs, string corrCoesa, string corrAngat, string corrRestr)
        {
            var nome = GetRelName(grupoLitológico, corrUcs, corrCoesa, corrAngat, corrRestr);

            var existeRelPoço =
                await _relacionamentoCorrsPropMecPoçoReadOnlyRepository.ExisteRelacionamentoCorrsPropMecPoço(idPoço, nome);

            if (existeRelPoço)
                return existeRelPoço;

            return await _relacionamentoCorrsPropMecReadyOnlyRepository.ExisteRelacionamento(nome);
        }

        private string GetRelName(string grupoLitológico, string corrUcs, string corrCoesa, string corrAngat, string corrRestr)
        {
            return grupoLitológico + "_" + corrUcs + "_" + corrCoesa + "_" + corrAngat + "_" +
                corrRestr;
        }

        private async Task<Correlação> GetCorr(string idPoço, string nome)
        {
            var existeCorrelação = await _correlaçãoReadOnlyRepository.ExisteCorrelação(nome.Trim());

            if (existeCorrelação)
                return await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(nome);

            return await _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçãoPoçoPeloNome(idPoço, nome.Trim());
        }
    }
}
