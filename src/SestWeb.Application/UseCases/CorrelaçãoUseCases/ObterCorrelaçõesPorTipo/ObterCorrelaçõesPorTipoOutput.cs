using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorTipo
{
    public class ObterCorrelaçõesPorTipoOutput : UseCaseOutput<ObterCorrelaçõesPorTipoStatus>
    {
        private ObterCorrelaçõesPorTipoOutput(){}

        public IReadOnlyCollection<Correlação> Correlações { get; private set; }

        public static ObterCorrelaçõesPorTipoOutput CorrelaçõesObtidas(IReadOnlyCollection<Correlação> correlações)
        {
            return new ObterCorrelaçõesPorTipoOutput
            {
                Correlações = correlações,
                Status = ObterCorrelaçõesPorTipoStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterCorrelaçõesPorTipoOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterCorrelaçõesPorTipoOutput
            {
                Status = ObterCorrelaçõesPorTipoStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterCorrelaçõesPorTipoOutput CorrelaçõesNãoEncontradas(string mnemônico)
        {
            return new ObterCorrelaçõesPorTipoOutput
            {
                Status = ObterCorrelaçõesPorTipoStatus.CorrelaçõesNãoEncontradas,
                Mensagem = $"Não foi possível encontrar correlações para o mnemônico{mnemônico}."
            };
        }

        public static ObterCorrelaçõesPorTipoOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterCorrelaçõesPorTipoOutput
            {
                Status = ObterCorrelaçõesPorTipoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
