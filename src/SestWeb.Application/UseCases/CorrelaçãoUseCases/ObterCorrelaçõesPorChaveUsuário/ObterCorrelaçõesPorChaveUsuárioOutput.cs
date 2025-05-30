using System;
using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorChaveUsuário
{
    public class ObterCorrelaçõesPorChaveUsuárioOutput : UseCaseOutput<ObterCorrelaçõesPorChaveUsuárioStatus>
    {
        private ObterCorrelaçõesPorChaveUsuárioOutput() { }

        public IReadOnlyCollection<Correlação> Correlações { get; private set; }

        public static ObterCorrelaçõesPorChaveUsuárioOutput CorrelaçõesObtidas(IReadOnlyCollection<Correlação> correlações)
        {
            return new ObterCorrelaçõesPorChaveUsuárioOutput
            {
                Correlações = correlações,
                Status = ObterCorrelaçõesPorChaveUsuárioStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterCorrelaçõesPorChaveUsuárioOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterCorrelaçõesPorChaveUsuárioOutput
            {
                Status = ObterCorrelaçõesPorChaveUsuárioStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterCorrelaçõesPorChaveUsuárioOutput CorrelaçõesNãoEncontradas(string userKey)
        {
            return new ObterCorrelaçõesPorChaveUsuárioOutput
            {
                Status = ObterCorrelaçõesPorChaveUsuárioStatus.CorrelaçõesNãoEncontradas,
                Mensagem = $"Não foi possível encontrar correlações para o usuário com chave: {userKey}."
            };
        }

        public static ObterCorrelaçõesPorChaveUsuárioOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterCorrelaçõesPorChaveUsuárioOutput
            {
                Status = ObterCorrelaçõesPorChaveUsuárioStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
