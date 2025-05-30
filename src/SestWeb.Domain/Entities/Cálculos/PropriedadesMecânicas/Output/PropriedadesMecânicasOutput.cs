using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Output
{
    public static class PropriedadesMecânicasOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {
            TiposPerfil.GeTipoPerfil(nameof(POISS)),
            TiposPerfil.GeTipoPerfil(nameof(YOUNG)),
            TiposPerfil.GeTipoPerfil(nameof(KS)),
            TiposPerfil.GeTipoPerfil(nameof(BIOT)),
            TiposPerfil.GeTipoPerfil(nameof(UCS)),
            TiposPerfil.GeTipoPerfil(nameof(COESA)),
            TiposPerfil.GeTipoPerfil(nameof(ANGAT)),
            TiposPerfil.GeTipoPerfil(nameof(RESTR))
        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }
}
