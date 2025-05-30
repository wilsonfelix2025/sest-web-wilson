using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.LoadCorrelaçõesSistema
{
    public class LoadCorrelaçõesSistemaOutput : UseCaseOutput<LoadCorrelaçõesSistemaStatus>
    {
        private LoadCorrelaçõesSistemaOutput() { }

        public IReadOnlyCollection<Correlação> Correlações { get; private set; }

        public static LoadCorrelaçõesSistemaOutput CorrelaçõesCarregadas(IReadOnlyCollection<Correlação> correlações)
        {
            return new LoadCorrelaçõesSistemaOutput
            {
                Correlações = correlações,
                Status = LoadCorrelaçõesSistemaStatus.CorrelaçõesCarregadas,
                Mensagem = "Correlações carregadas com sucesso."
            };
        }

        // public static LoadCorrelaçõesSistemaOutput CorrelaçõesJáCarregadas()
        // {
        //     return new LoadCorrelaçõesSistemaOutput
        //     {
        //         Status = LoadCorrelaçõesSistemaStatus.CorrelaçõesJáCarregadas,
        //         Mensagem = $"Correlações já carregadas."
        //     };
        // }

        public static LoadCorrelaçõesSistemaOutput CorrelaçõesNãoCarregadas(string mensagem)
        {
            return new LoadCorrelaçõesSistemaOutput
            {
                Status = LoadCorrelaçõesSistemaStatus.CorrelaçõesNãoCarregadas,
                Mensagem = $"Não foi possível carregar as correlações. {mensagem}"
            };
        }
    }
}
