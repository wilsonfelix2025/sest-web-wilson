using System;
using System.Collections.Generic;
using System.Text;
using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelaçãoPoço
{
    public class RemoverCorrelaçãoPoçoOutput : UseCaseOutput<RemoverCorrelaçãoPoçoStatus>
    {
        private RemoverCorrelaçãoPoçoOutput() { }

        public static RemoverCorrelaçãoPoçoOutput CorrelaçãoRemovida()
        {
            return new RemoverCorrelaçãoPoçoOutput
            {
                Status = RemoverCorrelaçãoPoçoStatus.CorrelaçãoRemovida,
                Mensagem = "Correlação removida com sucesso."
            };
        }

        public static RemoverCorrelaçãoPoçoOutput CorrelaçãoNãoRemovida(string nome)
        {
            return new RemoverCorrelaçãoPoçoOutput
            {
                Status = RemoverCorrelaçãoPoçoStatus.CorrelaçãoNãoRemovida,
                Mensagem = $"Não foi possível remover a correlação: {nome}"
            };
        }

        public static RemoverCorrelaçãoPoçoOutput CorrelaçãoNãoEncontrada(string nome)
        {
            return new RemoverCorrelaçãoPoçoOutput
            {
                Status = RemoverCorrelaçãoPoçoStatus.CorrelaçãoNãoEncontrada,
                Mensagem = $"Não foi possível encontrar correlação com nome: {nome}."
            };
        }

        public static RemoverCorrelaçãoPoçoOutput CorrelaçãoSemPermissãoParaRemoção(string nome)
        {
            return new RemoverCorrelaçãoPoçoOutput
            {
                Status = RemoverCorrelaçãoPoçoStatus.CorrelaçãoSemPermissãoParaRemoção,
                Mensagem = $"Não é permitida a remoção de uma correlação do sistema: {nome}."
            };
        }

        public static RemoverCorrelaçãoPoçoOutput PoçoNãoEncontrado(string idPoço)
        {
            return new RemoverCorrelaçãoPoçoOutput
            {
                Status = RemoverCorrelaçãoPoçoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
