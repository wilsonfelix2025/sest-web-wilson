using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoLDA
{
    public class CalcularNormalizaçãoLDAInput
    {
        public string IdPoço { get; set; }
        public double? Coeficiente { get; set; }
        public List<LotInput> ListaLot { get; set; } = new List<LotInput>();
        public string PerfilTensãoVerticalId { get; set; }
    }
}