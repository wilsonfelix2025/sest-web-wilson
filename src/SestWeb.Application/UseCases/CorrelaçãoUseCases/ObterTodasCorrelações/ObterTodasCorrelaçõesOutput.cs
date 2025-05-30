using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrelações
{
    public class ObterTodasCorrelaçõesOutput : UseCaseOutput<ObterTodasCorrelaçõesStatus>
    {
        private ObterTodasCorrelaçõesOutput(){ }

        public IReadOnlyCollection<Correlação> Correlações { get; private set; }

        public static ObterTodasCorrelaçõesOutput CorrelaçõesObtidas(IReadOnlyCollection<Correlação> correlações)
        {
            return new ObterTodasCorrelaçõesOutput
            {
                Correlações = correlações,
                Status = ObterTodasCorrelaçõesStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterTodasCorrelaçõesOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterTodasCorrelaçõesOutput
            {
                Status = ObterTodasCorrelaçõesStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterTodasCorrelaçõesOutput CorrelaçõesNãoEncontradas()
        {
            return new ObterTodasCorrelaçõesOutput
            {
                Status = ObterTodasCorrelaçõesStatus.CorrelaçõesNãoEncontradas,
                Mensagem = $"Não foi possível encontrar correlações."
            };
        }

        public static ObterTodasCorrelaçõesOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterTodasCorrelaçõesOutput
            {
                Status = ObterTodasCorrelaçõesStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
