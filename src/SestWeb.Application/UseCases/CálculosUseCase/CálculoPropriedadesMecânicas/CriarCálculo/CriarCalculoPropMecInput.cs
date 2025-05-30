using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class CriarCalculoPropMecInput
    {
        public List<string> Correlações { get; set; }
        public List<string> ListaIdPerfisEntrada { get; set; }
        public string Nome { get; set; }
        public string IdPoço { get; set; }
        public List<TrechoPropMecInput> Trechos { get; set; }
        public List<RegiõesInput> Regiões { get; set; }
    }
}