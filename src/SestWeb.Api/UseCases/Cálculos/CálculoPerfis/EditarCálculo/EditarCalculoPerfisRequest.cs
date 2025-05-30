
using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.EditarCálculo
{
    public class EditarCalculoPerfisRequest
    {
        public List<string> Correlações { get; set; } 
        public List<string> ListaIdPerfisEntrada { get; set; }
        public List<ParametrosRequest> Parâmetros { get; set; } 
        public List<TrechoRequest> Trechos { get; set; }
        public string IdCálculo { get; set; }
        public string Nome { get; set; }
        public string IdPoço { get; set; }

    }
}
