using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.MontagemPerfis
{
    public class Request
    {
        public List<MontarPerfisRequest> ListaTrechos { get; set; }
        public string IdPoço { get; set; }
        public string NomeRhob { get; set; }
        public string NomeDTC { get; set; }
        public string NomeDTS { get; set; }
        public string NomeGRay { get; set; }
        public string NomeResist { get; set; }
        public string NomeNPhi { get; set; }
        public bool RemoverTendência { get; set; }

    }
}
