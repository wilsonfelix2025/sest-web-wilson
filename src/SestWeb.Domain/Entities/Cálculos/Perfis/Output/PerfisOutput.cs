using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;

namespace SestWeb.Domain.Entities.Cálculos.Perfis.Output
{
    public static class PerfisOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {
            TiposPerfil.GeTipoPerfil(nameof(DTC)),
            TiposPerfil.GeTipoPerfil(nameof(DTMS)),
            TiposPerfil.GeTipoPerfil(nameof(VCL)),
            TiposPerfil.GeTipoPerfil(nameof(DTS)),
            TiposPerfil.GeTipoPerfil(nameof(PORO)),
            TiposPerfil.GeTipoPerfil(nameof(PERM)),
            TiposPerfil.GeTipoPerfil(nameof(RHOB)),
            TiposPerfil.GeTipoPerfil(nameof(RHOG)),
            TiposPerfil.GeTipoPerfil(nameof(DTMC))
        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }
}
