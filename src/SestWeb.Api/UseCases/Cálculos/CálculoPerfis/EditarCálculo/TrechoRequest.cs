using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.EditarCálculo
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
