using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço
{
    public class ObterPerfisDeUmPoçoOutput : UseCaseOutput<ObterPerfisDeUmPoçoStatus>
    {
        private ObterPerfisDeUmPoçoOutput()
        {
        }

        public IReadOnlyCollection<PerfilBase> Perfis { get; set; }

        public static ObterPerfisDeUmPoçoOutput PerfisObtidos(IReadOnlyCollection<PerfilBase> perfis)
        {
            return new ObterPerfisDeUmPoçoOutput
            {
                Perfis = perfis,
                Status = ObterPerfisDeUmPoçoStatus.PerfisObtidos,
                Mensagem = "Perfis obtidos com sucesso."
            };
        }

        public static ObterPerfisDeUmPoçoOutput PerfisNãoObtidos(string mensagem = "")
        {
            return new ObterPerfisDeUmPoçoOutput
            {
                Status = ObterPerfisDeUmPoçoStatus.PerfisNãoObtidos,
                Mensagem = $"[ObterPerfisDeUmPoço] - {mensagem}"
            };
        }
    }
}
