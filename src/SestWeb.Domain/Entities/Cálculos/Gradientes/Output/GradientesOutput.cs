using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Output
{
    public static class GradientesOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {
            TiposPerfil.GeTipoPerfil(nameof(GQUEBRA)),
            TiposPerfil.GeTipoPerfil(nameof(GCOLS)),
            TiposPerfil.GeTipoPerfil(nameof(GCOLI))
        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }

}
