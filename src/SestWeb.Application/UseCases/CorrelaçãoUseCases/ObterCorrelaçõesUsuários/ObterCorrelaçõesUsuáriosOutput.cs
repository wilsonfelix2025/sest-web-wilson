using System;
using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesUsuários
{
    public class ObterCorrelaçõesUsuáriosOutput : UseCaseOutput<ObterCorrelaçõesUsuáriosStatus>
    {
        private ObterCorrelaçõesUsuáriosOutput(){}

        public IReadOnlyCollection<Correlação> Correlações { get; private set; }

        public static ObterCorrelaçõesUsuáriosOutput CorrelaçõesObtidas(IReadOnlyCollection<Correlação> correlações)
        {
            return new ObterCorrelaçõesUsuáriosOutput
            {
                Correlações = correlações,
                Status = ObterCorrelaçõesUsuáriosStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterCorrelaçõesUsuáriosOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterCorrelaçõesUsuáriosOutput
            {
                Status = ObterCorrelaçõesUsuáriosStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterCorrelaçõesUsuáriosOutput CorrelaçõesNãoEncontradas()
        {
            return new ObterCorrelaçõesUsuáriosOutput
            {
                Status = ObterCorrelaçõesUsuáriosStatus.CorrelaçõesNãoEncontradas,
                Mensagem = $"Não foi possível encontrar correlações."
            };
        }

        public static ObterCorrelaçõesUsuáriosOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterCorrelaçõesUsuáriosOutput
            {
                Status = ObterCorrelaçõesUsuáriosStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
