using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.TensãoVertical.Output
{
    class TensãoVerticalOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {
            TiposPerfil.GeTipoPerfil(nameof(TVERT)),
        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }
}
