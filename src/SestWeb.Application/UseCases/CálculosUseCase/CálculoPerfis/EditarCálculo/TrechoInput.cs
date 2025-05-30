using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.EditarCálculo
{
    public class TrechoInput
    {
        public string TipoPerfil { get; set; }
        public double PmTopo { get; set; }
        public double PmBase { get; set; }
        public string Correlação { get; set; }
        public List<ParametroInput> ListaParametros { get; set; }
    }
}
