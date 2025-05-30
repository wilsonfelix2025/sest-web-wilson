using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.CriarCálculo
{
    public class TrechoRequest
    {
        public string TipoPerfil { get; set; }
        public double PmTopo { get; set; }
        public double PmBase { get; set; }
        public string Correlação { get; set; }
        public List<ParametrosRequest> ListaParametros { get; set; }
    }
}
