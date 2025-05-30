using SestWeb.Domain.Importadores.SestTR1.Utils;
using System.Collections.Generic;

namespace SestWeb.Domain.Importadores.SestTR1.Shallow
{
    public class LeitorShallowLitologiaTR1
    {
        public List<string> Litologias { get; private set; } = new List<string>();

        public void AdicionarLitologia(string linha)
        {
            var nomeLitologia = LeitorHelperTR1.ObterAtributo(linha, "Name");
            Litologias.Add(nomeLitologia);
        }
    }
}
