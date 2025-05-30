using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrsLeve
{
    public class ObterTodasCorrsLeveOutput : UseCaseOutput<ObterTodasCorrsLeveStatus>
    {
        private ObterTodasCorrsLeveOutput() { }

        public IReadOnlyCollection<Correlação> Correlações { get; private set; }

        public static ObterTodasCorrsLeveOutput CorrelaçõesObtidas(IReadOnlyCollection<Correlação> correlações)
        {
            return new ObterTodasCorrsLeveOutput
            {
                Correlações = correlações,
                Status = ObterTodasCorrsLeveStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterTodasCorrsLeveOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterTodasCorrsLeveOutput
            {
                Status = ObterTodasCorrsLeveStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterTodasCorrsLeveOutput CorrelaçõesNãoEncontradas()
        {
            return new ObterTodasCorrsLeveOutput
            {
                Status = ObterTodasCorrsLeveStatus.CorrelaçõesNãoEncontradas,
                Mensagem = $"Não foi possível encontrar correlações."
            };
        }

        public static ObterTodasCorrsLeveOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterTodasCorrsLeveOutput
            {
                Status = ObterTodasCorrsLeveStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
