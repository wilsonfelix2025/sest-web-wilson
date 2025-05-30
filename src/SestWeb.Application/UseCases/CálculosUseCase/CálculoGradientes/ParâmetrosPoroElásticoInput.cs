using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes
{
    public class ParâmetrosPoroElásticoInput
    {
        public double Kf { get; set; }
        public double Viscosidade { get; set; }
        public double CoeficienteReflexão { get; set; }
        public double CoeficienteInchamento { get; set; }
        public double CoeficienteDifusãoSoluto { get; set; }
        public double DensidadeFluidoFormação { get; set; }
        public double TemperaturaFormação { get; set; }
        public double ConcentraçãoSolFluidoPerfuração { get; set; }
        public double ConcentraçãoSolutoRocha { get; set; }
        public string TipoSal { get; set; }
        public double CoeficienteDissociaçãoSoluto { get; set; }
        public double MassaMolarSoluto { get; set; }
        public double ExpansãoTérmicaVolumeFluidoPoros { get; set; }
        public double TemperaturaPoço { get; set; }
        public double PropriedadeTérmicaTemperaturaFormação { get; set; }
        public double DifusidadeTérmica { get; set; }
        public double ExpansãoTérmicaRocha { get; set; }
        public List<string> Litologias { get; set; } = new List<string>();
        public double TemperaturaFormaçãoFisicoQuimica { get; set; }
    }
}