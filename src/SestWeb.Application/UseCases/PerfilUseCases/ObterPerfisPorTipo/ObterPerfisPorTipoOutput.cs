using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisPorTipo
{
    public class ObterPerfisPorTipoOutput : UseCaseOutput<ObterPerfisPorTipoStatus>
    {
        private ObterPerfisPorTipoOutput()
        {
        }

        public IReadOnlyCollection<PerfilBase> Perfis { get; set; }

        public static ObterPerfisPorTipoOutput PerfisObtidos(IReadOnlyCollection<PerfilBase> perfis)
        {
            return new ObterPerfisPorTipoOutput
            {
                Perfis = perfis,
                Status = ObterPerfisPorTipoStatus.PerfisObtidos,
                Mensagem = "Perfis obtidos com sucesso."
            };
        }

        public static ObterPerfisPorTipoOutput PerfisNãoObtidos(string mensagem = "")
        {
            return new ObterPerfisPorTipoOutput
            {
                Status = ObterPerfisPorTipoStatus.PerfisNãoObtidos,
                Mensagem = $"Não foi possível obter perfis. {mensagem}"
            };
        }
    }
}
