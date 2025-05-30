using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Output
{
    public static class PressãoPorosOutput
    {
        private static readonly List<TipoPerfil> _perfis = new List<TipoPerfil>
        {

        };

        public static List<TipoPerfil> Perfis()
        {
            return _perfis;
        }
    }
}
