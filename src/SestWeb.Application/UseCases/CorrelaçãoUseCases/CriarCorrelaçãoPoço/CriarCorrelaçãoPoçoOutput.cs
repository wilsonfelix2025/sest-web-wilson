using System;
using System.Collections.Generic;
using System.Text;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelaçãoPoço
{
    public class CriarCorrelaçãoPoçoOutput : UseCaseOutput<CriarCorrelaçãoPoçoStatus>
    {
        private CriarCorrelaçãoPoçoOutput() { }

        public CorrelaçãoPoço CorrelaçãoPoço { get; set; }

        public static CriarCorrelaçãoPoçoOutput CorrelaçãoCriada(CorrelaçãoPoço correlaçãoPoço)
        {
            return new CriarCorrelaçãoPoçoOutput
            {
                Status = CriarCorrelaçãoPoçoStatus.CorrelaçãoCriada,
                Mensagem = "Correlação criada com sucesso.",
                CorrelaçãoPoço = correlaçãoPoço
            };
        }

        public static CriarCorrelaçãoPoçoOutput CorrelaçãoExistente(string nome)
        {
            return new CriarCorrelaçãoPoçoOutput
            {
                Status = CriarCorrelaçãoPoçoStatus.CorrelaçãoNãoCriada,
                Mensagem = $"Já existe uma correlação com esse nome: {nome}"
            };
        }

        public static CriarCorrelaçãoPoçoOutput CorrelaçãoNãoCriada(string nome)
        {
            return new CriarCorrelaçãoPoçoOutput
            {
                Status = CriarCorrelaçãoPoçoStatus.CorrelaçãoNãoCriada,
                Mensagem = $"Não foi possível criar correlação: {nome}"
            };
        }

        public static CriarCorrelaçãoPoçoOutput PoçoNãoEncontrado(string idPoço)
        {
            return new CriarCorrelaçãoPoçoOutput
            {
                Status = CriarCorrelaçãoPoçoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
