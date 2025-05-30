using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.PublicarRelacionamentoCorrsPropMec
{
    public class PublicarRelacionamentoCorrsPropMecOutput : UseCaseOutput<PublicarRelacionamentoCorrsPropMecStatus>
    {
        private PublicarRelacionamentoCorrsPropMecOutput() { }

        public static PublicarRelacionamentoCorrsPropMecOutput RelacionamentoPublicado()
        {
            return new PublicarRelacionamentoCorrsPropMecOutput
            {
                Status = PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoPublicado,
                Mensagem = "Relacionamento publicado com sucesso.",
            };
        }

        public static PublicarRelacionamentoCorrsPropMecOutput RelacionamentoNãoEncontrado(string nome)
        {
            return new PublicarRelacionamentoCorrsPropMecOutput
            {
                Status = PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar um relacionamento com esse nome: {nome}"
            };
        }

        public static PublicarRelacionamentoCorrsPropMecOutput RelacionamentoNãoPublicado(string nome)
        {
            return new PublicarRelacionamentoCorrsPropMecOutput
            {
                Status = PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoNãoPublicado,
                Mensagem = $"Não foi possível publicar o relacionamento: {nome}"
            };
        }

        public static PublicarRelacionamentoCorrsPropMecOutput PoçoNãoEncontrado(string idPoço)
        {
            return new PublicarRelacionamentoCorrsPropMecOutput
            {
                Status = PublicarRelacionamentoCorrsPropMecStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }

        public static PublicarRelacionamentoCorrsPropMecOutput RelacionamentoExistente(string nome)
        {
            return new PublicarRelacionamentoCorrsPropMecOutput
            {
                Status = PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoExistente,
                Mensagem = $"Não foi possível publicar o relacionamento. Já existe um relacionamento com esse nome: {nome}."
            };
        }
    }
}
