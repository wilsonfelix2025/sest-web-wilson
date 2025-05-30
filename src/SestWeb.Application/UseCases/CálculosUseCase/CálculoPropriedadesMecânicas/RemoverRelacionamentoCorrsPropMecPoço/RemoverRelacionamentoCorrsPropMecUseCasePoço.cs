using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço
{
    internal class RemoverRelacionamentoCorrsPropMecUseCasePoço : IRemoverRelacionamentoCorrsPropMecUseCasePoço
    {
        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository _relacionamentoCorrsPropMecPoçoReadOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository _relacionamentoCorrsPropMecPoçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public RemoverRelacionamentoCorrsPropMecUseCasePoço(IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository relacionamentoCorrsPropMecPoçoWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
            _relacionamentoCorrsPropMecPoçoWriteOnlyRepository = relacionamentoCorrsPropMecPoçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<RemoverRelacionamentoCorrsPropMecOutputPoço> Execute(string idPoço, string nome)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return RemoverRelacionamentoCorrsPropMecOutputPoço.PoçoNãoEncontrado(idPoço);
                }

                var relacionamento = await _relacionamentoCorrsPropMecPoçoReadOnlyRepository.ObterRelacionamentoCorrsPropMecPoçoPeloNome(idPoço, nome);

                if (relacionamento == null)
                {
                    return RemoverRelacionamentoCorrsPropMecOutputPoço.RelacionamentoNãoEncontrado(nome);
                }

                if (relacionamento.Origem == Origem.Fixa)
                {
                    return RemoverRelacionamentoCorrsPropMecOutputPoço.RelacionamentoSemPermissãoParaRemoção(nome);
                }

                await _relacionamentoCorrsPropMecPoçoWriteOnlyRepository.RemoverRelacionamentoCorrsPropMecPoço(nome);

                GerenciadorUcsCoesaAngat.RemoverRelacionamento(relacionamento);

                return RemoverRelacionamentoCorrsPropMecOutputPoço.RelacionamentoRemovido();
            }
            catch (Exception e)
            {
                return RemoverRelacionamentoCorrsPropMecOutputPoço.RelacionamentoNãoRemovido(e.Message);
            }
        }
    }
}
