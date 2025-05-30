using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec.Factory;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço
{

    internal class CriarRelacionamentoCorrsPropMecPoçoUseCase : ICriarRelacionamentoCorrsPropMecPoçoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository;

        private readonly IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository
            _relacionamentoCorrsPropMecPoçoWriteOnlyRepository;

        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly IRelacionamentoPropMecPoçoFactory _relacionamentoPropMecPoçoFactory;

        public CriarRelacionamentoCorrsPropMecPoçoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository relacionamentoCorrsPropMecPoçoWriteOnlyRepository,
            IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository,
            ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository,
            ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, IRelacionamentoPropMecPoçoFactory relacionamentoPropMecPoçoFactory)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
            _relacionamentoCorrsPropMecPoçoWriteOnlyRepository = relacionamentoCorrsPropMecPoçoWriteOnlyRepository;
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _relacionamentoPropMecPoçoFactory = relacionamentoPropMecPoçoFactory;
        }

        public async Task<CriarRelacionamentoCorrsPropMecPoçoOutput> Execute(string idPoço,
            string grupoLitológico, string nomeAutor, string chaveAutor, string corrUcs,
            string corrCoesa, string corrAngat, string corrRestr)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.PoçoNãoEncontrado(idPoço);
                }

                var existeCorr = await ExisteCorrelação(idPoço, corrUcs.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.CorrelaçãoInexistente(corrUcs);

                existeCorr = await ExisteCorrelação(idPoço, corrCoesa.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.CorrelaçãoInexistente(corrCoesa);

                existeCorr = await ExisteCorrelação(idPoço, corrAngat.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.CorrelaçãoInexistente(corrAngat);

                existeCorr = await ExisteCorrelação(idPoço, corrRestr.Trim());
                if (!existeCorr)
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.CorrelaçãoInexistente(corrRestr);

                var grupoLito = GrupoLitologico.GetFromName(grupoLitológico);
                if (grupoLito == null)
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.GrupoLitológicoNãoEncontrado(grupoLitológico);

                var existeRel =
                    await ExisteRelacionamento(idPoço, grupoLitológico, corrUcs, corrCoesa, corrAngat, corrRestr);

                if (existeRel)
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.RelacionamentoExistente(grupoLitológico, corrUcs, corrCoesa, corrAngat, corrRestr);

                var result = _relacionamentoPropMecPoçoFactory.CreateRelacionamentoPropMecPoço(idPoço, grupoLitológico,
                    nomeAutor, chaveAutor, GetCorr(idPoço, corrUcs).Result, GetCorr(idPoço, corrCoesa).Result, GetCorr(idPoço, corrAngat).Result, GetCorr(idPoço, corrRestr).Result,
                    out RelacionamentoPropMecPoço relacionamentoPropMecPoço);

                if (!result.IsValid)
                    return CriarRelacionamentoCorrsPropMecPoçoOutput.RelacionamentoNãoCriado(string.Join("\n", result.Errors));

                await _relacionamentoCorrsPropMecPoçoWriteOnlyRepository.CriarRelacionamentoCorrsPropMecPoço(relacionamentoPropMecPoço);

                return CriarRelacionamentoCorrsPropMecPoçoOutput.RelacionamentoCriado(relacionamentoPropMecPoço);
            }
            catch (Exception e)
            {
                return CriarRelacionamentoCorrsPropMecPoçoOutput.RelacionamentoNãoCriado(e.Message);
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

            if(existeCorrelação)
                return await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(nome);

            return await _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçãoPoçoPeloNome(idPoço, nome.Trim());
        }
    }
}
