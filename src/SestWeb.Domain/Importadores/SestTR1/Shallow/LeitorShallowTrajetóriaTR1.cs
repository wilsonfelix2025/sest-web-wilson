using SestWeb.Domain.Importadores.SestTR1.Utils;
using System.Collections.Generic;

namespace SestWeb.Domain.Importadores.SestTR1.Shallow
{
    public class LeitorShallowTrajetóriaTR1
    {
        public List<string> Trajetórias { get; private set; } = new List<string>();

        public void AdicionarTrajetória(string linha)
        {
            var nomeTrajetória = LeitorHelperTR1.ObterAtributo(linha, "Name");
            Trajetórias.Add(nomeTrajetória);
        }
    }
}
