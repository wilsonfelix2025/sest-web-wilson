using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.FatorPlastificacao;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos
{
    public class Colapsos
    {
        private readonly DadosMalha dadosMalha;
        private readonly EntradasColapsos entradasColapsos;
        private readonly Malha.Malha malha;
        private readonly RaioPoco raioPoco;
        private double _cI;
        private double _cS;
        private readonly CritérioRupturaGradientesEnum tipoCritérioRuptura;

        public double CI { get => _cI; private set => _cI = UnitConverter.PascalToPsi(value); }
        public double CS { get => _cS; private set => _cS = UnitConverter.PascalToPsi(value); }
        public double FPCI { get; set; }
        public double FPCS { get; set; }
        public double APCI { get; set; }
        public double APCS { get; set; }
        public bool NãoConvergiuCI { get; set; }
        public bool NãoConvergiuCS { get; set; }
        const int QUANTIDADELIMITEITERAÇÕES = 1000;
        const double tolerância = 0.1;


        public Colapsos(DadosMalha dadosMalha, EntradasColapsos entradasColapsos, CritérioRupturaGradientesEnum tipoCritério)
        {
            this.entradasColapsos = entradasColapsos;
            this.dadosMalha = dadosMalha;
            this.tipoCritérioRuptura = tipoCritério;

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
        public void Calcular(bool fluidoPenetrante, bool isPoroelastico, double fraturaInferior, double fraturaSuperior)
        {

            var pv = this.entradasColapsos.Pv;
            double pwInicial = 0;
            NãoConvergiuCI = false;
            NãoConvergiuCS = false;
            double ci, cs;

            if (this.entradasColapsos.AreaPlastificada != 0)
            {
                ci = GetCI_AP(this.malha, this.raioPoco, pwInicial, fluidoPenetrante, isPoroelastico, fraturaInferior);
            }
            else
            {
                ci = GetCI(this.malha, this.raioPoco, pwInicial, fluidoPenetrante, isPoroelastico, fraturaInferior);
            }

            if (ci < 0)
            {
                ci = 0;
            }

            if (this.entradasColapsos.AreaPlastificada != 0)
            {
                cs = GetCS_AP(malha, raioPoco, pwInicial, fluidoPenetrante, isPoroelastico, fraturaSuperior, ci);
            }
            else
            {
                pwInicial = ci;
                cs = GetCS(malha, raioPoco, pwInicial, fluidoPenetrante, isPoroelastico, fraturaSuperior);
            }

            CI = ci;
            CS = cs;
        }
        private double GetCI(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante, bool isPoroelastico, double fraturaInferior)
        {
            var pv = this.entradasColapsos.Pv;
            var limiteDeIterações = 1;

            pwInicial = UnitConverter.PsiToPascal(fraturaInferior);

            Func<double, double> getFp = GetFpCalculator(ehFluidoPenetrante, isPoroelastico, false);

            var delta = UnitConverter.PsiToPascal(PPGToPsi(0.1, pv));

            var fp = getFp(pwInicial);

            var ultimoPwIncremento = pwInicial;
            var pwIncremento = pwInicial + delta;
            var valorSuperior = 0.0;
            var valorInferior = 0.0;

            //se o fp for 1, já retorno o valor do colapso inferior como o valor da fratura inferior
            if (fp == 1)
                return UnitConverter.PsiToPascal(fraturaInferior);

            //se o fp com a fratura inferior for maior que um, já tenho o valor superior e vou achar o valor inferior
            //para passar para o método de Brent
            if (fp > 1)
            {
                valorSuperior = ultimoPwIncremento;
                while (fp > 1 && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES)
                {
                    ultimoPwIncremento = pwIncremento;
                    fp = getFp(pwIncremento);
                    pwIncremento += delta;
                    limiteDeIterações++;
                }

                valorInferior = ultimoPwIncremento;
            }
            else if (fp > 0 && fp < 1)
            {
                //se for entre 0 e 1, faço um testo com pw=0 e se novamente der entre 0 e 1, retorno 0
                //caso nesse novo teste não der entre 0 e 1, pego o valor da fratura inferior e diminuo o delta e vou tentanto encontrar
                //os valores inferiores e superiores
                fp = getFp(0);
                if (fp > 0 && fp < 1)
                    return 0;

                valorInferior = ultimoPwIncremento;
                pwIncremento -= delta;
                fp = getFp(pwIncremento);
                while (fp < 1 && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES)
                {
                    ultimoPwIncremento = pwIncremento;
                    fp = getFp(pwIncremento);
                    pwIncremento -= delta;
                    limiteDeIterações++;
                }
                valorSuperior = ultimoPwIncremento;
            }
            else if (fp < 0)
            {
                valorInferior = ultimoPwIncremento;
                while (fp < 1 && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES)
                {
                    ultimoPwIncremento = pwIncremento;
                    fp = getFp(pwIncremento);
                    pwIncremento += delta;
                    limiteDeIterações++;
                }
                valorSuperior = ultimoPwIncremento;
            }

            if (limiteDeIterações == QUANTIDADELIMITEITERAÇÕES)
            {
                APCI = CalcularAP();
                FPCI = fp;
                NãoConvergiuCI = true;
                //return valorInferior;
                return UnitConverter.PsiToPascal(fraturaInferior);
            }

            //testo o fp com o valor que o Brent encontrou para verificar se realmente houve a convergência
            var valorConvergenciaBrent = GetPressaoPorConvergencia(getFp, valorInferior, valorSuperior);
            fp = getFp(valorConvergenciaBrent);
            if (Math.Abs(fp - 1) > tolerância)
            {
                APCI = CalcularAP();
                FPCI = fp;
                NãoConvergiuCI = true;
                return UnitConverter.PsiToPascal(fraturaInferior);
            }
            else
                return valorConvergenciaBrent;
        }

        private double CalcularAP()
        {
            var ap = this.malha.ObterAreaPlastificada();
            return ap;
        }

        private double GetCS(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante, bool isPoroelastico, double fraturaSuperior)
        {
            var pv = this.entradasColapsos.Pv;
            var limiteDeIterações = 1;

            Func<double, double> getFp = GetFpCalculator(ehFluidoPenetrante, isPoroelastico, false);

            var delta = UnitConverter.PsiToPascal(PPGToPsi(0.1, pv));

            var pwInicialCS = UnitConverter.PsiToPascal(fraturaSuperior);
            var fp = getFp(pwInicialCS);
            var valorSuperior = 0.0;
            var valorInferior = 0.0;
            var ultimoPwIncremento = pwInicialCS;
            var pwIncremento = pwInicialCS;

            if (fp == 1)
                return UnitConverter.PsiToPascal(fraturaSuperior);

            //se o fp com a fratura superior for maior que um, já tenho o valor superior e vou achar o valor inferior
            //para passar para o método de Brent
            if (fp > 1)
            {
                valorSuperior = pwInicialCS;
                while (fp > 1 && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES)
                {
                    ultimoPwIncremento = pwIncremento;
                    fp = getFp(pwIncremento);
                    pwIncremento -= delta;
                    limiteDeIterações++;
                }

                valorInferior = ultimoPwIncremento;
            }
            else if (fp < 1)
            {
                //caso seja menor q 1, pego o valor da fratura inferior e somo o delta e vou tentanto encontrar
                //os valores inferiores e superiores
                valorInferior = ultimoPwIncremento;
                while (fp < 1 && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES)
                {
                    ultimoPwIncremento = pwIncremento;
                    fp = getFp(pwIncremento);
                    pwIncremento += delta;
                    limiteDeIterações++;
                }
                valorSuperior = ultimoPwIncremento;
            }

            if (limiteDeIterações == QUANTIDADELIMITEITERAÇÕES)
            {
                APCS = CalcularAP();
                FPCS = fp;
                NãoConvergiuCS = true;
                return UnitConverter.PsiToPascal(fraturaSuperior);
                //return valorSuperior;
            }


            //testo o fp com o valor que o Brent encontrou para verificar se realmente houve a convergência
            var valorConvergenciaBrent = GetPressaoPorConvergencia(getFp, valorInferior, valorSuperior);
            fp = getFp(valorConvergenciaBrent);
            if (Math.Abs(fp - 1) > tolerância)
            {
                APCS = CalcularAP();
                FPCS = fp;
                NãoConvergiuCS = true;
                return UnitConverter.PsiToPascal(fraturaSuperior);
            }
            else
                return valorConvergenciaBrent;

        }

        private double GetCI_AP(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante, bool isPoroelastico, double fraturaInferior)
        {
            var limiteDeIterações = 1;
            var pv = this.entradasColapsos.Pv;
            var ap_usuario = this.entradasColapsos.AreaPlastificada;
            var delta = UnitConverter.PsiToPascal(PPGToPsi(0.1, pv));
            Func<double, double> getFp = GetFpCalculator(ehFluidoPenetrante, isPoroelastico, true);
            var ci = GetCI(malha, raioPoco, pwInicial, ehFluidoPenetrante, isPoroelastico, fraturaInferior);

            if (NãoConvergiuCI)
                return UnitConverter.PsiToPascal(fraturaInferior);

            if (ci == 0)
            {
                APCI = 0;
                NãoConvergiuCI = true;
                return UnitConverter.PsiToPascal(fraturaInferior);
            }

            var ultimopw = ci;

            getFp(ultimopw);
            var ap = this.malha.ObterAreaPlastificada();

            var pwDecremento = ci;

            while (ap < ap_usuario && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES)
            {
                ultimopw = pwDecremento;
                pwDecremento -= delta;
                getFp(pwDecremento);
                ap = this.malha.ObterAreaPlastificada();
                limiteDeIterações++;
            }

            Func<double, double> getAp = (pw) =>
            {
                getFp(pw);
                return this.malha.ObterAreaPlastificada();
            };

            //testo o ap com o valor que o Brent encontrou para verificar se realmente houve a convergência
            var valorConvergenciaBrent = GetPressaoPorConvergencia(getAp, pwDecremento, ultimopw, ap_usuario);
            var fp = getFp(valorConvergenciaBrent);
            ap = this.malha.ObterAreaPlastificada();
            if (Math.Abs(ap - ap_usuario) > tolerância)
            {
                APCI = ap;
                FPCI = fp;
                NãoConvergiuCI = true;
                return valorConvergenciaBrent;
            }
            else
                return valorConvergenciaBrent;

        }

        private double GetCS_AP(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante, bool isPoroelastico, double fraturaSuperior, double colapsoInferior)
        {
            var limiteDeIterações = 1;
            var pv = this.entradasColapsos.Pv;
            var ap_usuario = this.entradasColapsos.AreaPlastificada;
            var delta = UnitConverter.PsiToPascal(PPGToPsi(0.1, pv));
            var pwInicialCS = pwInicial + delta;
            Func<double, double> getFp = GetFpCalculator(ehFluidoPenetrante, isPoroelastico, true);
            var cs = GetCS(malha, raioPoco, pwInicialCS, ehFluidoPenetrante, isPoroelastico, fraturaSuperior);

            if (NãoConvergiuCS)
                return UnitConverter.PsiToPascal(fraturaSuperior);


            Func<double, double> getAp = (pw) =>
            {
                getFp(pw);
                return this.malha.ObterAreaPlastificada();
            };

            var ultimopw = cs;
            var pwIncremento = cs;

            var ap = getAp(pwIncremento);

            if (ap > ap_usuario)
            {
                while (ap > ap_usuario && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES && pwIncremento >= colapsoInferior)
                {
                    ultimopw = pwIncremento;
                    pwIncremento -= delta;
                    getFp(pwIncremento);
                    ap = this.malha.ObterAreaPlastificada();
                    limiteDeIterações++;
                }
            }
            else
            {
                while (ap < ap_usuario && limiteDeIterações < QUANTIDADELIMITEITERAÇÕES)
                {
                    ultimopw = pwIncremento;
                    pwIncremento += delta;
                    getFp(pwIncremento);
                    ap = this.malha.ObterAreaPlastificada();
                    limiteDeIterações++;
                }
            }

            if (limiteDeIterações == 1000)
            {
                var fpLimite = getFp(pwIncremento);
                APCS = ap;
                FPCS = fpLimite;
                NãoConvergiuCS = true;
                return pwIncremento;
            }
            else if (pwIncremento <= colapsoInferior)
            {
                var fpLimite = getFp(colapsoInferior);
                ap = this.malha.ObterAreaPlastificada();
                APCS = ap;
                FPCS = fpLimite;
                NãoConvergiuCS = true;
                return colapsoInferior;
            }

            //testo o ap com o valor que o Brent encontrou para verificar se realmente houve a convergência
            var valorConvergenciaBrent = GetPressaoPorConvergencia(getAp, pwIncremento, ultimopw, ap_usuario);
            var fp = getFp(valorConvergenciaBrent);
            ap = this.malha.ObterAreaPlastificada();
            if (Math.Abs(ap - ap_usuario) > tolerância)
            {
                APCS = ap;
                FPCS = fp;
                NãoConvergiuCS = true;
                return valorConvergenciaBrent;
            }
            else
                return valorConvergenciaBrent;

        }

        private double GetPressaoPorConvergencia(Func<double, double> f, double left, double right, double target = 1)
        {
            const double tolerance = 1e-10;
            return RootFinding.Brent(new FunctionOfOneVariable(f), left, right, tolerance, target);
        }

        private Func<double, double> GetFpCalculator(bool ehFluidoPenetrante, bool isPoroelastico, bool areaPlastificada)
        {
            var pv = this.entradasColapsos.Pv;
            var pressaoPoros = this.entradasColapsos.PressaoPoros;
            var biot = this.entradasColapsos.Biot;
            var ucs = this.entradasColapsos.Ucs;
            var angat = this.entradasColapsos.AnguloAtrito;
            var restr = this.entradasColapsos.ResistenciaTracao;
            var coesao = this.entradasColapsos.Coesao;

            if (isPoroelastico)
            {
                var calculator = new FpCalculatorPoroelastico(malha, this.entradasColapsos.DadosEntradaModeloPoroElastico, raioPoco, pressaoPoros, ucs, angat, restr, areaPlastificada);
                return calculator.CalcularFp;
            }

            if (ehFluidoPenetrante)
            {
                var poisson = this.entradasColapsos.Poisson;

                var calculator = FpCalculator.ElasticoPenetrante(pv, malha, raioPoco, pressaoPoros, biot, ucs, angat, restr, poisson, areaPlastificada, tipoCritérioRuptura, coesao);
                return calculator.CalcularFp;
            }
            else
            {
                var calculator = FpCalculator.ElasticoNaoPenetrante(pv, malha, raioPoco, pressaoPoros, biot, ucs, angat, restr, areaPlastificada, tipoCritérioRuptura, coesao);
                return calculator.CalcularFp;
            }
        }

        private double PPGToPsi(double ppg, double pv)
        {
            return 0.1704 * ppg * pv;
        }
    }
}
