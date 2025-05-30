using System.Collections.Generic;

namespace SestWeb.Domain.Importadores.Shallow
{
    public class DadosLidos : RetornoDTO
    {
        public bool TemTrajetória { get; set; }
        public bool TemSapatas { get; set; }
        public bool TemObjetivos { get; set; }
        public bool TemLitologia { get; set; }
        public IEnumerable<string> Litologias { get; set; }
        public bool TemEstratigrafia { get; set; }
        public List<RetornoPerfis> Perfis { get; set; }
        public bool TemDadosGerais { get; set; }
        public List<string> Trajetórias { get; set; } = new List<string>();
        public Dictionary<string, PoçoImportado> Poços { get; set; } = new Dictionary<string, PoçoImportado>();
        public bool TemRegistros { get; set; }
        public bool TemEventos { get; set; }
    }
}
