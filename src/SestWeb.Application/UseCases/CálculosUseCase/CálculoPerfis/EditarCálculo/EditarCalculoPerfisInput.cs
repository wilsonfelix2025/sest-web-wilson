using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.EditarCálculo
{
    public class EditarCalculoPerfisInput
    {
        public List<string> Correlações { get; set; }
        public List<string> ListaIdPerfisEntrada { get; set; }
        public List<ParametroInput> Parâmetros { get; set; }
        public List<TrechoInput> Trechos { get; set; }
        public string Nome { get; set; }
        public string IdPoço { get; set; }
        public string IdCálculo { get; set; }
    }
}
