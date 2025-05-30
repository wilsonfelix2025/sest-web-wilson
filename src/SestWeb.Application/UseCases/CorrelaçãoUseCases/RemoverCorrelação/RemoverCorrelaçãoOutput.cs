using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelação
{
    public class RemoverCorrelaçãoOutput : UseCaseOutput<RemoverCorrelaçãoStatus>
    {
        private RemoverCorrelaçãoOutput(){ }

        public static RemoverCorrelaçãoOutput CorrelaçãoRemovida()
        {
            return new RemoverCorrelaçãoOutput
            {
                Status = RemoverCorrelaçãoStatus.CorrelaçãoRemovida,
                Mensagem = "Correlação removida com sucesso."
            };
        }

        public static RemoverCorrelaçãoOutput CorrelaçãoNãoRemovida(string nome)
        {
            return new RemoverCorrelaçãoOutput
            {
                Status = RemoverCorrelaçãoStatus.CorrelaçãoNãoRemovida,
                Mensagem = $"Não foi possível remover a correlação: {nome}"
            };
        }

        public static RemoverCorrelaçãoOutput CorrelaçãoNãoEncontrada(string nome)
        {
            return new RemoverCorrelaçãoOutput
            {
                Status = RemoverCorrelaçãoStatus.CorrelaçãoNãoEncontrada,
                Mensagem = $"Não foi possível encontrar correlação com nome: {nome}."
            };
        }

        public static RemoverCorrelaçãoOutput CorrelaçãoSemPermissãoParaRemoção(string nome)
        {
            return new RemoverCorrelaçãoOutput
            {
                Status = RemoverCorrelaçãoStatus.CorrelaçãoSemPermissãoParaRemoção,
                Mensagem = $"Não é permitida a remoção de uma correlação do sistema: {nome}."
            };
        }
    }
}
