using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;
using System;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelação
{
    public class CriarCorrelaçãoOutput : UseCaseOutput<CriarCorrelaçãoStatus>
    {
        private CriarCorrelaçãoOutput()
        {
        }

        public Correlação Correlação { get; set; }

        public static CriarCorrelaçãoOutput CorrelaçãoCriada(Correlação correlação)
        {
            return new CriarCorrelaçãoOutput
            {
                Status = CriarCorrelaçãoStatus.CorrelaçãoCriada,
                Mensagem = "Correlação criada com sucesso.",
                Correlação = correlação
            };
        }

        public static CriarCorrelaçãoOutput CorrelaçãoExistente(string nome)
        {
            return new CriarCorrelaçãoOutput
            {
                Status = CriarCorrelaçãoStatus.CorrelaçãoNãoCriada,
                Mensagem = $"Já existe uma correlação com esse nome: {nome}"
            };
        }

        public static CriarCorrelaçãoOutput CorrelaçãoNãoCriada(string nome)
        {
            return new CriarCorrelaçãoOutput
            {
                Status = CriarCorrelaçãoStatus.CorrelaçãoNãoCriada,
                Mensagem = $"Não foi possível criar correlação: {nome}"
            };
        }

        public static CriarCorrelaçãoOutput PoçoNãoEncontrado(string idPoço)
        {
            return new CriarCorrelaçãoOutput
            {
                Status = CriarCorrelaçãoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
