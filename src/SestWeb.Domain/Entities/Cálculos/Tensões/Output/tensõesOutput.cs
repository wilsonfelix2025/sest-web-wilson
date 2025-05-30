using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;

namespace SestWeb.Domain.Entities.Cálculos.Tensões.Output
{
    public static class tensõesOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {
            TiposPerfil.GeTipoPerfil(nameof(THORmin)),
            TiposPerfil.GeTipoPerfil(nameof(THORmax)),
            TiposPerfil.GeTipoPerfil(nameof(GFRAT_σh)),
            TiposPerfil.GeTipoPerfil(nameof(GTHORmax)),
        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }
}
