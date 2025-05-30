using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class CriarCalculoPropMecRequest
    {
        public List<string> Correlações { get; set; }
        public List<string> ListaIdPerfisEntrada { get; set; }
        public string Nome { get; set; }
        public string IdPoço { get; set; }
        public List<TrechoPropMecRequest> Trechos { get; set; }
        public List<RegiãoRequest> Regiões { get; set; }
    }
}