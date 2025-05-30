using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos
{
    public class Fraturas
    {
        private readonly DadosMalha dadosMalha;
        private readonly EntradasColapsos entradasColapsos;
        private readonly Malha.Malha malha;
        private readonly RaioPoco raioPoco;

        public double FI { get; private set; }
        public double FS { get; private set; }

        public Fraturas(DadosMalha dadosMalha, EntradasColapsos entradasColapsos)
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

        public void Calcular(bool fluidoPenetrante, bool isPoroelastico)
        {
            var pv = this.entradasColapsos.Pv;
            double pwInicial = 0;

            double fi, fs;

            if (isPoroelastico)
                fi = GetFIPoro(this.malha, this.raioPoco, pwInicial, fluidoPenetrante);
            else
                fi = GetFI(this.malha, this.raioPoco, pwInicial, fluidoPenetrante);

            if (fi < 0)
            {
                fi = 0;
            }

            pwInicial = UnitConverter.PsiToPascal(PPGToPsi(100, pv));

            if (isPoroelastico)
                fs = GetFSPoro(malha, raioPoco, pwInicial, fluidoPenetrante);
            else
                fs = GetFS(malha, raioPoco, pwInicial, fluidoPenetrante);



            //caso a fratura inferior tenha um valor maior que a fratura superior, inverto os valores
            if (fi > fs)
            {
                FI = fs;
                FS = fi;
            }
            else
            {
                FI = fi;
                FS = fs;
            }
        }

        private double GetFIPoro(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante)
        {
            var biot = this.entradasColapsos.Biot;
            var pressaoPoros = this.entradasColapsos.PressaoPoros;
            var poisson = this.entradasColapsos.Poisson;

            if (ehFluidoPenetrante)
            {
                if (biot == 1)
                    return 0;
            }

            var pontos = this.malha.GetPontosParedePoco();
            var restr = UnitConverter.PsiToPascal(this.entradasColapsos.ResistenciaTracao);

            Func<double, double> GetMinRadialEfetiva = (pw) =>
            {
                var listTensoes = new List<double>();
                foreach (var ponto in pontos)
                {
                    var tensoesAoRedorPoco = new TensoesAoRedorPocoModeloPoroelastico(pw, this.entradasColapsos.DadosEntradaModeloPoroElastico, ponto);
                    var efetiva = tensoesAoRedorPoco.TensaoRadial - tensoesAoRedorPoco.Pressao * biot;
                    listTensoes.Add(efetiva);
                }

                return listTensoes.Min();
            };


            var pv = this.entradasColapsos.Pv;
            var min = GetMinRadialEfetiva(pwInicial);
            var delta = UnitConverter.PsiToPascal(PPGToPsi(2, pv));

            var ultimoPwIncremento = pwInicial;
            var pwIncremento = pwInicial + delta;
            while (min < restr)
            {
                ultimoPwIncremento = pwIncremento;
                pwIncremento = pwIncremento + delta;
                min = GetMinRadialEfetiva(pwIncremento);
            }

            Func<double, double> getDerivada = (pw) =>
            {
                var inferior = GetMinRadialEfetiva(pw - delta);
                var superior = GetMinRadialEfetiva(pw + delta);

                return (superior - inferior) / (2 * delta);
            };

            return UnitConverter.PascalToPsi(GetPressaoPorConvergencia(GetMinRadialEfetiva, getDerivada, pwInicial, restr));

        }

        private double GetFI(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante)
        {
            var biot = this.entradasColapsos.Biot;

            if (ehFluidoPenetrante)
            {
                if (biot == 1)
                    return 0;
            }

            var pontos = this.malha.GetPontosParedePoco();

            var pressaoPoros = this.entradasColapsos.PressaoPoros;
            var poisson = this.entradasColapsos.Poisson;
            var restr = UnitConverter.PsiToPascal(this.entradasColapsos.ResistenciaTracao);

            Func<double, double> GetMinRadialEfetiva = (pw) =>
            {
                var listTensoes = new List<double>();
                foreach (var ponto in pontos)
                {
                    var tensoesAoRedorPoco = new TensoesAoRedorPocoModeloElastico();
                    tensoesAoRedorPoco.CalcularElasticoNaoPenetrante(ponto, pw, pressaoPoros);

                    if (ehFluidoPenetrante)
                    {
                        tensoesAoRedorPoco.CalcularElasticoPenetrante(biot, poisson, pw, raioPoco, pressaoPoros, ponto.Raio);
                    }

                    var efetiva = tensoesAoRedorPoco.TensaoRadial - tensoesAoRedorPoco.Pressao * biot;
                    listTensoes.Add(efetiva);
                }

                return UnitConverter.PsiToPascal(listTensoes.Min());
            };


            var pv = this.entradasColapsos.Pv;
            var min = GetMinRadialEfetiva(pwInicial);
            var delta = UnitConverter.PsiToPascal(PPGToPsi(2, pv));

            var ultimoPwIncremento = pwInicial;
            var pwIncremento = pwInicial + delta;
            while (min < restr)
            {
                ultimoPwIncremento = pwIncremento;
                pwIncremento = pwIncremento + delta;
                min = GetMinRadialEfetiva(pwIncremento);
            }

            Func<double, double> getDerivada = (pw) =>
            {
                var inferior = GetMinRadialEfetiva(pw - delta);
                var superior = GetMinRadialEfetiva(pw + delta);

                return (superior - inferior) / (2 * delta);
            };

            return UnitConverter.PascalToPsi(GetPressaoPorConvergencia(GetMinRadialEfetiva, getDerivada, pwInicial, restr));

        }

        private double PPGToPsi(double ppg, double pv)
        {
            return 0.1704 * ppg * pv;
        }


        private double GetFS(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante)
        {
            var pontos = this.malha.GetPontosParedePoco();
            var pressaoPoros = this.entradasColapsos.PressaoPoros;
            var biot = this.entradasColapsos.Biot;
            var poisson = this.entradasColapsos.Poisson;
            var restr = UnitConverter.PsiToPascal(this.entradasColapsos.ResistenciaTracao);

            Func<double, double> GetMinTangencialEfetiva = (pw) =>
            {
                var listTensoes = new List<Tuple<double, double, double, double, double>>();
                foreach (var ponto in pontos)
                {
                    ITensoesAoRedorPoco tensoesAoRedorPoco;
                    double tensaoTangencialNaoPenetrante = 0;

                    var tensoesAoRedorPocoElastico = new TensoesAoRedorPocoModeloElastico();
                    tensoesAoRedorPocoElastico.CalcularElasticoNaoPenetrante(ponto, pw, pressaoPoros);
                    tensaoTangencialNaoPenetrante = tensoesAoRedorPocoElastico.TensaoTangencial;

                    if (ehFluidoPenetrante)
                    {
                        tensoesAoRedorPocoElastico.CalcularElasticoPenetrante(biot, poisson, pw, raioPoco, pressaoPoros, ponto.Raio);
                    }

                    tensoesAoRedorPoco = tensoesAoRedorPocoElastico;


                    var efetiva = tensoesAoRedorPoco.TensaoTangencial - tensoesAoRedorPoco.Pressao * biot;
                    listTensoes.Add(new Tuple<double, double, double, double, double>(efetiva, UnitConverter.RadiandoParaGraus(ponto.Angulo), tensoesAoRedorPoco.Pressao, tensoesAoRedorPoco.TensaoTangencial, tensaoTangencialNaoPenetrante));
                }

                return UnitConverter.PsiToPascal(listTensoes.OrderBy(x => x.Item1).First().Item1);
            };


            var pv = this.entradasColapsos.Pv;
            var min = GetMinTangencialEfetiva(pwInicial);
            var delta = UnitConverter.PsiToPascal(PPGToPsi(2, pv));

            var pwIncremento = pwInicial - delta;
            while (min < restr)
            {
                pwIncremento = pwIncremento - delta;
                min = GetMinTangencialEfetiva(pwIncremento);
            }

            Func<double, double> getDerivada = (pw) =>
            {
                var inferior = GetMinTangencialEfetiva(pw - delta);
                var superior = GetMinTangencialEfetiva(pw + delta);

                return (superior - inferior) / (2 * delta);
            };

            return UnitConverter.PascalToPsi(GetPressaoPorConvergencia(GetMinTangencialEfetiva, getDerivada, pwInicial, restr));

        }

        private double GetFSPoro(Malha.Malha malha, RaioPoco raioPoco, double pwInicial, bool ehFluidoPenetrante)
        {
            var pontos = this.malha.GetPontosParedePoco();
            var pressaoPoros = this.entradasColapsos.PressaoPoros;
            var biot = this.entradasColapsos.Biot;
            var poisson = this.entradasColapsos.Poisson;
            var restr = UnitConverter.PsiToPascal(this.entradasColapsos.ResistenciaTracao);

            Func<double, double> GetMinTangencialEfetiva = (pw) =>
            {
                var listTensoes = new List<Tuple<double, double, double, double, double>>();
                foreach (var ponto in pontos)
                {

                    ITensoesAoRedorPoco tensoesAoRedorPoco;
                    double tensaoTangencialNaoPenetrante = 0;
                    tensoesAoRedorPoco = new TensoesAoRedorPocoModeloPoroelastico(pw, this.entradasColapsos.DadosEntradaModeloPoroElastico, ponto);


                    var efetiva = tensoesAoRedorPoco.TensaoTangencial - tensoesAoRedorPoco.Pressao * biot;
                    listTensoes.Add(new Tuple<double, double, double, double, double>(efetiva, UnitConverter.RadiandoParaGraus(ponto.Angulo), tensoesAoRedorPoco.Pressao, tensoesAoRedorPoco.TensaoTangencial, tensaoTangencialNaoPenetrante));
                }

                return listTensoes.OrderBy(x => x.Item1).First().Item1;
            };


            var pv = this.entradasColapsos.Pv;
            var min = GetMinTangencialEfetiva(pwInicial);
            var delta = UnitConverter.PsiToPascal(PPGToPsi(0.1, pv));

            var pwIncremento = pwInicial;
            while (min < restr)
            {
                pwIncremento = pwIncremento - delta;
                min = GetMinTangencialEfetiva(pwIncremento);
            }

            Func<double, double> getDerivada = (pw) =>
            {
                var inferior = GetMinTangencialEfetiva(pw - delta);
                var superior = GetMinTangencialEfetiva(pw + delta);

                return (superior - inferior) / (2 * delta);
            };

            return UnitConverter.PascalToPsi(GetPressaoPorConvergencia(GetMinTangencialEfetiva, getDerivada, pwIncremento, restr));

        }

        private double GetPressaoPorConvergencia(Func<double, double> f, Func<double, double> d, double guess, double target = 1)
        {
            const double tolerance = 1e-10;
            return RootFinding.Newton(new FunctionOfOneVariable(f), new FunctionOfOneVariable(d), guess, tolerance, target);
        }
    }
}
