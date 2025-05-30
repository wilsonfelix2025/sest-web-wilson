using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec
{
    internal class RemoverRelacionamentoCorrsPropMecUseCase : IRemoverRelacionamentoCorrsPropMecUseCase
    {
        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecWriteOnlyRepository _relacionamentoCorrsPropMecWriteOnlyRepository;

        public RemoverRelacionamentoCorrsPropMecUseCase(IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository,
            IRelacionamentoCorrsPropMecWriteOnlyRepository relacionamentoCorrsPropMecWriteOnlyRepository)
        {
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecWriteOnlyRepository = relacionamentoCorrsPropMecWriteOnlyRepository;
        }

        public async Task<RemoverRelacionamentoCorrsPropMecOutput> Execute(string nome)
        {
            try
            {
                var relacionamento = await _relacionamentoCorrsPropMecReadyOnlyRepository.ObterRelacionamentoPeloNome(nome);

                if (relacionamento == null)
                {
                    return RemoverRelacionamentoCorrsPropMecOutput.RelacionamentoNãoEncontrado(nome);
                }

                if (relacionamento.Origem == Origem.Fixa)
                {
                    return RemoverRelacionamentoCorrsPropMecOutput.RelacionamentoSemPermissãoParaRemoção(nome);
                }

                await _relacionamentoCorrsPropMecWriteOnlyRepository.RemoverRelacionamentoCorrsPropMec(nome);

                GerenciadorUcsCoesaAngat.RemoverRelacionamento(relacionamento);

                return RemoverRelacionamentoCorrsPropMecOutput.RelacionamentoRemovido();
            }
            catch (Exception e)
            {
                return RemoverRelacionamentoCorrsPropMecOutput.RelacionamentoNãoRemovido(e.Message);
            }
        }
    }
}
