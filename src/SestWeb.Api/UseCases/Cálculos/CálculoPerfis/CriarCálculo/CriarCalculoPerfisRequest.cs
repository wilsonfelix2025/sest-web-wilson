
using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.CriarCálculo
{
    public class CriarCalculoPerfisRequest
    {
        public List<string> Correlações { get; set; } 
        public List<string> ListaIdPerfisEntrada { get; set; }
        public List<ParametrosRequest> Parâmetros { get; set; } 
        public List<TrechoRequest> Trechos { get; set; }
        public string Nome { get; set; }
        public string IdPoço { get; set; }
    }
}
