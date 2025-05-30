using SestWeb.Application.Helpers;
using System;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço
{
    public class RemoverRelacionamentoCorrsPropMecOutputPoço : UseCaseOutput<RemoverRelacionamentoCorrsPropMecStatusPoço>
    {
        private RemoverRelacionamentoCorrsPropMecOutputPoço() { }

        public static RemoverRelacionamentoCorrsPropMecOutputPoço RelacionamentoRemovido()
        {
            return new RemoverRelacionamentoCorrsPropMecOutputPoço
            {
                Status = RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoRemovido,
                Mensagem = "Relacionamento removido com sucesso."
            };
        }

        public static RemoverRelacionamentoCorrsPropMecOutputPoço RelacionamentoNãoRemovido(string nome)
        {
            return new RemoverRelacionamentoCorrsPropMecOutputPoço
            {
                Status = RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoNãoRemovido,
                Mensagem = $"Não foi possível remover o relacionamento: {nome}"
            };
        }

        public static RemoverRelacionamentoCorrsPropMecOutputPoço RelacionamentoNãoEncontrado(string nome)
        {
            return new RemoverRelacionamentoCorrsPropMecOutputPoço
            {
                Status = RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar relacionamento com nome: {nome}."
            };
        }

        public static RemoverRelacionamentoCorrsPropMecOutputPoço RelacionamentoSemPermissãoParaRemoção(string nome)
        {
            return new RemoverRelacionamentoCorrsPropMecOutputPoço
            {
                Status = RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoSemPermissãoParaRemoção,
                Mensagem = $"Não é permitida a remoção de um relacionamento do sistema: {nome}."
            };
        }

        public static RemoverRelacionamentoCorrsPropMecOutputPoço PoçoNãoEncontrado(string idPoço)
        {
            return new RemoverRelacionamentoCorrsPropMecOutputPoço
            {
                Status = RemoverRelacionamentoCorrsPropMecStatusPoço.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
