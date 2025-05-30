using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.Output
{
    public static class ExpoenteDOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {
            TiposPerfil.GeTipoPerfil(nameof(Entities.Perfis.TiposPerfil.ExpoenteD))
        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }
}
