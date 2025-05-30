using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.Output
{
    public static class SobrecargaOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {
            TiposPerfil.GeTipoPerfil(nameof(TVERT)),
            TiposPerfil.GeTipoPerfil(nameof(GSOBR))
        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }
}
