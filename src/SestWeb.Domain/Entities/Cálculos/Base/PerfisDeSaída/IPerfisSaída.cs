using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída
{
    public interface IPerfisSaída
    {
        List<PerfilBase> Perfis { get; }
        List<string> IdPerfis { get; set; }
        void AtualizarPvs(IConversorProfundidade conversor);
        void ApagarPontos();
        void Renomear(string nomeCálculoNovo, string nomeCálculoAntigo);
        bool TemSaidaZerada();
    }
}
