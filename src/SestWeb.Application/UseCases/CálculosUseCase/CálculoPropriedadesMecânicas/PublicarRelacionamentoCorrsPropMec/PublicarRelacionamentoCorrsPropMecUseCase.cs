using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.PublicarRelacionamentoCorrsPropMec
{

    internal class PublicarRelacionamentoCorrsPropMecUseCase : IPublicarRelacionamentoCorrsPropMecUseCase
    {
        private readonly IRelacionamentoCorrsPropMecReadyOnlyRepository _relacionamentoCorrsPropMecReadyOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecWriteOnlyRepository _relacionamentoCorrsPropMecWriteOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoReadOnlyRepository _relacionamentoCorrsPropMecPoçoReadOnlyRepository;
        private readonly IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository _relacionamentoCorrsPropMecPoçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IRelacionamentoPropMecFactory _relacionamentoPropMecFactory;

        public PublicarRelacionamentoCorrsPropMecUseCase(IRelacionamentoCorrsPropMecReadyOnlyRepository relacionamentoCorrsPropMecReadyOnlyRepository,
            IRelacionamentoCorrsPropMecWriteOnlyRepository relacionamentoCorrsPropMecWriteOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoReadOnlyRepository relacionamentoCorrsPropMecPoçoReadOnlyRepository,
            IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository relacionamentoCorrsPropMecPoçoWriteOnlyRepository,
            IPoçoReadOnlyRepository poçoReadOnlyRepository, IRelacionamentoPropMecFactory relacionamentoPropMecFactory)
        {
            _relacionamentoCorrsPropMecReadyOnlyRepository = relacionamentoCorrsPropMecReadyOnlyRepository;
            _relacionamentoCorrsPropMecWriteOnlyRepository = relacionamentoCorrsPropMecWriteOnlyRepository;
            _relacionamentoCorrsPropMecPoçoReadOnlyRepository = relacionamentoCorrsPropMecPoçoReadOnlyRepository;
            _relacionamentoCorrsPropMecPoçoWriteOnlyRepository = relacionamentoCorrsPropMecPoçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _relacionamentoPropMecFactory = relacionamentoPropMecFactory;
        }

        public async Task<PublicarRelacionamentoCorrsPropMecOutput> Execute(string idPoço, string nome)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return PublicarRelacionamentoCorrsPropMecOutput.PoçoNãoEncontrado(idPoço);
                }

                var existeRelacionamento = await _relacionamentoCorrsPropMecReadyOnlyRepository.ExisteRelacionamento(nome);
                if (existeRelacionamento)
                {
                    return PublicarRelacionamentoCorrsPropMecOutput.RelacionamentoExistente(nome);
                }

                var relacionamento = await _relacionamentoCorrsPropMecPoçoReadOnlyRepository.ObterRelacionamentoCorrsPropMecPoçoPeloNome(idPoço, nome);
                if (relacionamento == null)
                    return PublicarRelacionamentoCorrsPropMecOutput.RelacionamentoNãoEncontrado(nome);

                await _relacionamentoCorrsPropMecPoçoWriteOnlyRepository.RemoverRelacionamentoCorrsPropMecPoço(nome);

                var result = _relacionamentoPropMecFactory.CreateRelacionamentoPropMec(relacionamento.GrupoLitológico.Nome, Origem.Usuário.ToString(),
                    relacionamento.Autor.Nome, relacionamento.Autor.Chave, relacionamento.Ucs, relacionamento.Coesa, relacionamento.Angat, relacionamento.Restr,
                    out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoPropMec);

                if (!result.IsValid)
                    return PublicarRelacionamentoCorrsPropMecOutput.RelacionamentoNãoPublicado(string.Join("\n", result.Errors));

                await _relacionamentoCorrsPropMecWriteOnlyRepository.CriarRelacionamentoCorrsPropMec(relacionamentoPropMec);

                GerenciadorUcsCoesaAngat.RemoverRelacionamento(relacionamento);
                GerenciadorUcsCoesaAngat.AdicionarRelacionamento(relacionamentoPropMec);

                return PublicarRelacionamentoCorrsPropMecOutput.RelacionamentoPublicado();
            }
            catch (Exception e)
            {
                return PublicarRelacionamentoCorrsPropMecOutput.RelacionamentoNãoPublicado(e.Message);
            }
        }
    }
}
