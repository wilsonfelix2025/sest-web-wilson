using SestWeb.Domain.Enums;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarCálculo
{
    public class CriarCálculoTensõesInSituInput
    {
        public string IdPoço { get; set; }
        public string NomeCálculo { get; set; }
        public string PerfilTensãoVerticalId { get; set; }
        public string PerfilGPOROId { get; set; }
        public string PerfilPoissonId { get; set; }
        public string PerfilTHORminId { get; set; }
        public double? Coeficiente { get; set; }
        public List<LotInput> ListaLot { get; set; }
        public DepleçãoInput Depleção { get; set; }
        public MetodologiaCálculoTensãoHorizontalMaiorEnum TensãoHorizontalMaiorMetodologiaCálculo { get; set; }
        public MetodologiaCálculoTensãoHorizontalMenorEnum TensãoHorizontalMenorMetodoligiaCálculo { get; set; }
        public RelaçãoTensãoInput RelaçãoTensão { get; set; }
        public BreakoutInput Breakout { get; set; }
        public FraturasTrechosVerticaisInput FraturasTrechosVerticais { get; set; }
    }
}
