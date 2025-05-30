using System;
using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterAutoresCorrelações
{
    public class ObterAutoresCorrelaçõesOutput : UseCaseOutput<ObterAutoresCorrelaçõesStatus>
    {
        private ObterAutoresCorrelaçõesOutput() { }

        public IReadOnlyCollection<Autor> Autores { get; private set; }

        public static ObterAutoresCorrelaçõesOutput AutoresObtidos(IReadOnlyCollection<Autor> autores)
        {
            return new ObterAutoresCorrelaçõesOutput
            {
                Autores = autores,
                Status = ObterAutoresCorrelaçõesStatus.AutoresObtidos,
                Mensagem = "Autores obtidos com sucesso."
            };
        }

        public static ObterAutoresCorrelaçõesOutput AutoresNãoObtidos(string mensagem)
        {
            return new ObterAutoresCorrelaçõesOutput
            {
                Status = ObterAutoresCorrelaçõesStatus.AutoresNãoObtidos,
                Mensagem = $"Não foi possível obter os autores das correlações. {mensagem}"
            };
        }

        public static ObterAutoresCorrelaçõesOutput AutoresNãoEncontrados()
        {
            return new ObterAutoresCorrelaçõesOutput
            {
                Status = ObterAutoresCorrelaçõesStatus.AutoresNãoEncontrados,
                Mensagem = $"Não foi possível encontrar autores para as correlações."
            };
        }

        public static ObterAutoresCorrelaçõesOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterAutoresCorrelaçõesOutput
            {
                Status = ObterAutoresCorrelaçõesStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
