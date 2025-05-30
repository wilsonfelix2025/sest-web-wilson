using System.Collections.Generic;

namespace SestWeb.Domain.Importadores.Shallow
{
    public class PoçoImportado
    {
        public string Tipo { get; set; } = "";
        public IList<string> Litologias { get; set; } = new List<string>();
        public List<RetornoPerfis> Perfis { get; set; } = new List<RetornoPerfis>();
    }
}
