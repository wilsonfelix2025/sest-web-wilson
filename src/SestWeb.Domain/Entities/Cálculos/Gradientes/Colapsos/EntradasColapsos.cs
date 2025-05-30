using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos
{
    public class EntradasColapsos
    {
        private double _tensaoMenor;
        private double _tensaoMaior;
        private double _tensaoVertical;
        private double _pressaoPoros;
        private double _pw;
        private double _areaPlastificada = 0;

        public double Inclinacao { get; set; }
        public double Azimute { get; set; }
        public double TensaoMenor { get => _tensaoMenor; set => _tensaoMenor = UnitConverter.PsiToPascal(value); }
        public double TensaoMaior { get => _tensaoMaior; set => _tensaoMaior = UnitConverter.PsiToPascal(value); }
        public double TensaoVertical { get => _tensaoVertical; set => _tensaoVertical = UnitConverter.PsiToPascal(value); }
        public double AzimuteMenor { get; set; }
        public double Pv { get; set; }
        public double Poisson { get; set; }
        public double DiametroPoco { get; set; }
        public double PressaoPoros { get => _pressaoPoros; set => _pressaoPoros = UnitConverter.PsiToPascal(value); }
        public double Biot { get; set; }
        public double Ucs { get; set; }
        public double AnguloAtrito { get; set; }
        public double ResistenciaTracao { get; set; }
        public double Coesao { get; set; }
        public double Pw { get => _pw; set => _pw = UnitConverter.PsiToPascal(value); }

        public double AreaPlastificada { get => _areaPlastificada; set => _areaPlastificada = value; }

        public bool EhFluidoPenetrante { get; set; }
        public bool EhPoroelastico => DadosEntradaModeloPoroElastico != null;
        public DadosEntradaModeloPoroElastico DadosEntradaModeloPoroElastico { get; set; }
        public CritérioRupturaGradientesEnum TipoCritérioRuptura { get; set; }
    }
}
