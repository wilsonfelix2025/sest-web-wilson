
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SestWeb.Application.UseCases.MontagemPerfis
{
    public class MontarPerfisInput
    {
        public string NomeRhob { get; set; }
        public string NomeDTC { get; set; }
        public string NomeDTS { get; set; }
        public string NomeGRay { get; set; }
        public string NomeResist { get; set; }
        public string NomeNPhi { get; set; }
        public bool RemoverTendência { get; set; }
        public List<MontagemPerfisListaInput> Lista { get; set; } = new List<MontagemPerfisListaInput>();
    }
}
