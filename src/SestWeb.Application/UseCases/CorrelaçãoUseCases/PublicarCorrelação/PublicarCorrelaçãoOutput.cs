using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.PublicarCorrelação
{
    public class PublicarCorrelaçãoOutput : UseCaseOutput<PublicarCorrelaçãoStatus>
    {
        private PublicarCorrelaçãoOutput() { }

        public static PublicarCorrelaçãoOutput CorrelaçãoPublicada()
        {
            return new PublicarCorrelaçãoOutput
            {
                Status = PublicarCorrelaçãoStatus.CorrelaçãoPublicada,
                Mensagem = "Correlação atualizada com sucesso.",
            };
        }

        public static PublicarCorrelaçãoOutput CorrelaçãoNãoEncontrada(string nome)
        {
            return new PublicarCorrelaçãoOutput
            {
                Status = PublicarCorrelaçãoStatus.CorrelaçãoNãoEncontrada,
                Mensagem = $"Não foi possível encontrar uma correlação com esse nome: {nome}"
            };
        }

        public static PublicarCorrelaçãoOutput CorrelaçãoNãoPublicada(string nome)
        {
            return new PublicarCorrelaçãoOutput
            {
                Status = PublicarCorrelaçãoStatus.CorrelaçãoNãoPublicada,
                Mensagem = $"Não foi possível atualizar correlação: {nome}"
            };
        }

        public static PublicarCorrelaçãoOutput PoçoNãoEncontrado(string idPoço)
        {
            return new PublicarCorrelaçãoOutput
            {
                Status = PublicarCorrelaçãoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }

        public static PublicarCorrelaçãoOutput CorrelaçãoExistente(string nome)
        {
            return new PublicarCorrelaçãoOutput
            {
                Status = PublicarCorrelaçãoStatus.CorrelaçãoExistente,
                Mensagem = $"Não foi possível publicar a correlação. Já existe uma correlação com esse nome: {nome}."
            };
        }
    }
}
