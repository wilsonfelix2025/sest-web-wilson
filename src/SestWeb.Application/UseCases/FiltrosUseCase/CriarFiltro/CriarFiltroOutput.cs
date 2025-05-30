
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro
{
    public class CriarFiltroOutput : UseCaseOutput<CriarFiltroStatus>
    {
        public CriarFiltroOutput()
        {
            
        }

        public PerfilBase Perfil { get; set; }
        public Cálculo Cálculo { get; set; }

        public static CriarFiltroOutput FiltroCriado(PerfilBase perfil, Cálculo cálculo)
        {
            return new CriarFiltroOutput
            {
                Status = CriarFiltroStatus.FiltroCriado,
                Mensagem = "Filtro criado com sucesso",
                Perfil = perfil,
                Cálculo = cálculo
            };
        }

        public static CriarFiltroOutput FiltroNãoCriado(string msg)
        {
            return new CriarFiltroOutput
            {
                Status = CriarFiltroStatus.FiltroNãoCriado,
                Mensagem = $"Não foi possível criar filtro. {msg}"
            };
        }
    }
}
