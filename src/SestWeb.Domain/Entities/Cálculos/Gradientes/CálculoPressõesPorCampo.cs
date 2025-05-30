using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos;
using SestWeb.Domain.Entities.Cálculos.Gradientes.FatorPlastificacao;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes
{
    public class CálculoPressõesPorCampo
    {
        private readonly DadosMalha dadosMalha;
        private readonly EntradasColapsos entradasColapsos;
        private readonly Malha.Malha malha;
        private readonly RaioPoco raioPoco;

        public CálculoPressõesPorCampo(DadosMalha dadosMalha, EntradasColapsos entradasColapsos)
        {
            this.entradasColapsos = entradasColapsos;
            this.dadosMalha = dadosMalha;

            var inclinacao = this.entradasColapsos.Inclinacao;
            var azimute = this.entradasColapsos.Azimute;
            var tensaoMaior = this.entradasColapsos.TensaoMaior;
            var tensaoMenor = this.entradasColapsos.TensaoMenor;
            var tensaoVertical = this.entradasColapsos.TensaoVertical;
            var azimuteMenor = this.entradasColapsos.AzimuteMenor;
            var poisson = this.entradasColapsos.Poisson;
            var diametroPoco = this.entradasColapsos.DiametroPoco;

            this.raioPoco = new RaioPoco(this.entradasColapsos.DiametroPoco);

            if (this.entradasColapsos.DadosEntradaModeloPoroElastico == null)
            {
                var malhaBuilder = new MalhaBuilder(dadosMalha, inclinacao, azimute, tensaoMenor, tensaoMaior, tensaoVertical, azimuteMenor);
                this.malha = malhaBuilder.Build(diametroPoco, poisson, false);
            }
            else
            {
                var dadosPoro = this.entradasColapsos.DadosEntradaModeloPoroElastico;

                var malhaBuilder = new MalhaBuilder(dadosMalha, inclinacao, azimute, dadosPoro.TensaoMenor, dadosPoro.TensaoMaior, dadosPoro.TensaoVertical, azimuteMenor);
                this.malha = malhaBuilder.Build(diametroPoco, poisson, true);
                this.entradasColapsos.DadosEntradaModeloPoroElastico.MatrizTensorDeTensoes = malhaBuilder.MatrizTensorDeTensoes;
            }

        }

        public List<CampoResult> CalcularCampo(double pw)
        {
            var possuiAreaPlastificada = entradasColapsos.AreaPlastificada != 0.0;

            Func<double, double> getFp = GetFpCalculator(entradasColapsos.EhFluidoPenetrante, entradasColapsos.EhPoroelastico, possuiAreaPlastificada);

            getFp(UnitConverter.PsiToPascal(pw));

            var pontos90 = this.malha.Pontos.Where(x => x.Angulo >= UnitConverter.PolToMeter(90));

            return this.malha.Pontos.Select(x => new CampoResult() { Value = x.FatorPlastificação, Coord = new Coordenadas() { X = x.X, Y = x.Y } }).ToList();

        }

        private Func<double, double> GetFpCalculator(bool ehFluidoPenetrante, bool isPoroelastico, bool areaPlastificada)
        {
            var pv = this.entradasColapsos.Pv;
            var pressaoPoros = this.entradasColapsos.PressaoPoros;
            var biot = this.entradasColapsos.Biot;
            var ucs = this.entradasColapsos.Ucs;
            var angat = this.entradasColapsos.AnguloAtrito;
            var restr = this.entradasColapsos.ResistenciaTracao;

            if (isPoroelastico)
            {
                var calculator = new FpCalculatorPoroelastico(malha, this.entradasColapsos.DadosEntradaModeloPoroElastico, raioPoco, pressaoPoros, ucs, angat, restr, areaPlastificada);
                return calculator.CalcularFp;
            }

            if (ehFluidoPenetrante)
            {
                var poisson = this.entradasColapsos.Poisson;

                var calculator = FpCalculator.ElasticoPenetrante(pv, malha, raioPoco, pressaoPoros, biot, ucs, angat, restr, poisson, areaPlastificada, CritérioRupturaGradientesEnum.MohrCoulomb, 0);
                return calculator.CalcularFp;
            }
            else
            {
                var calculator = FpCalculator.ElasticoNaoPenetrante(pv, malha, raioPoco, pressaoPoros, biot, ucs, angat, restr, areaPlastificada, CritérioRupturaGradientesEnum.MohrCoulomb, 0);
                return calculator.CalcularFp;
            }
        }
    }

    public class CampoResult
    {
        public double Value { get; set; }
        public Coordenadas Coord { get; set; }
    }

    public class Coordenadas
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public enum TipoCampo
    {
        [Description("Pressão de poros")]
        fd_P, // Formation pressure (pore pressure)
        [Description("Tensão total radial")]
        fd_Sr, // Total Sr
        [Description("Tensão total tangencial")]
        fd_Sth, // Total Stheta
        [Description("Tensão total axial")]
        fd_Sz, // Total Sz
        [Description("Tensão efetiva radial")]
        fd_Sr_e, // Effective Sr
        [Description("Tensão efetiva tangencial")]
        fd_Sth_e, // Effective Stheta
        [Description("Tensão efetiva axial")]
        fd_Sz_e, // Effective Sz
        [Description("Tensão cisalh (r.theta)")]
        fd_Srth, // Shear
        [Description("Tensão cisalh (theta.z)")]
        fd_Sthz, // Shear
        [Description("Tensão cisalh (r.z)")]
        fd_Srz, // Shear
        [Description("S1 total")]
        fd_S1, // Total S1
        [Description("S2 total")]
        fd_S2, // Total S2
        [Description("S3 total")]
        fd_S3, // Total S3
        [Description("S1 efetiva")]
        fd_S1_e, // Effective S1
        [Description("S2 efetiva")]
        fd_S2_e, // Effective S2
        [Description("S3 efetiva")]
        fd_S3_e, // Effective S3
        [Description("Fator de Plastificação")]
        fd_yield_factor, // Yield factor
        [Description("Temperatura")]
        fd_temperature, // Temperature
        [Description("Concentração")]
        fd_concentration,
        fd_n
    }
}
