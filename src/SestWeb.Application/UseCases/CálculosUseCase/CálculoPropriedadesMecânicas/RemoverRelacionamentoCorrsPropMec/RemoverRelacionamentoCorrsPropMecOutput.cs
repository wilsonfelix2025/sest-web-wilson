using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec
{
    public class RemoverRelacionamentoCorrsPropMecOutput : UseCaseOutput<RemoverRelacionamentoCorrsPropMecStatus>
    {
        private RemoverRelacionamentoCorrsPropMecOutput() { }

        public static RemoverRelacionamentoCorrsPropMecOutput RelacionamentoRemovido()
        {
            return new RemoverRelacionamentoCorrsPropMecOutput
            {
                Status = RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoRemovido,
                Mensagem = "Relacionamento removido com sucesso."
            };
        }

        public static RemoverRelacionamentoCorrsPropMecOutput RelacionamentoNãoRemovido(string nome)
        {
            return new RemoverRelacionamentoCorrsPropMecOutput
            {
                Status = RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoNãoRemovido,
                Mensagem = $"Não foi possível remover o relacionamento: {nome}"
            };
        }

        public static RemoverRelacionamentoCorrsPropMecOutput RelacionamentoNãoEncontrado(string nome)
        {
            return new RemoverRelacionamentoCorrsPropMecOutput
            {
                Status = RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar relacionamento com nome: {nome}."
            };
        }

        public static RemoverRelacionamentoCorrsPropMecOutput RelacionamentoSemPermissãoParaRemoção(string nome)
        {
            return new RemoverRelacionamentoCorrsPropMecOutput
            {
                Status = RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoSemPermissãoParaRemoção,
                Mensagem = $"Não é permitida a remoção de um relacionamento do sistema: {nome}."
            };
        }
    }
}
