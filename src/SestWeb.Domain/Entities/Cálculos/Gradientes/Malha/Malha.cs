using System;
using System.Collections.Immutable;
using System.Linq;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    public class Malha
    {
        internal Malha(DadosMalha dadosMalha, double diametro, MatrizTensorDeTensoes matrizTensorDeTensoes, double poisson, bool isPoroElástico)
        {
            DadosMalha = dadosMalha;
            this.Diametro = diametro;
            this.RaioPoço = new RaioPoco(diametro);
            this.IsPoroElástico = isPoroElástico;
            GerarPontos(matrizTensorDeTensoes, poisson);
            GerarTriangulos();
        }

        public DadosMalha DadosMalha { get; }
        public double Diametro { get; }
        public bool IsPoroElástico { get; }
        public RaioPoco RaioPoço { get; }

        public ImmutableList<PontoDaMalha> Pontos { get; private set; }
        public ImmutableList<Triangulo> Triangulos { get; private set; }

        public ImmutableList<PontoDaMalha> GetPontosParedePoco()
        {
            return Pontos.FindAll(x => x.Raio == this.RaioPoço.Value).ToImmutableList();
        }

        public double ObterAreaPlastificada()
        {
            var area = Triangulos.Sum(triângulo => triângulo.ObterAreaPlastificada());
            area *= 2;
            double raio = UnitConverter.MeterToPol(this.RaioPoço.Value);
            area /= Math.PI * raio * raio;
            area *= 100;
            return area;
        }

        private void GerarPontos(MatrizTensorDeTensoes matrizTensorDeTensoes, double poisson)
        {
            double raioInterno = this.Diametro / 2;
            double raioExterno = this.DadosMalha.AnguloInternoPorExterno * raioInterno;

            int numeroDeAngulos = this.DadosMalha.AnguloDivisao + 1;
            int numeroDeRaios = this.DadosMalha.RaioDivisao + 1;

            var incrementoMinimoDeRaio = 2 / (1 + this.DadosMalha.AnguloMaxMinIncremento) * (raioExterno - raioInterno) /
                                         this.DadosMalha.RaioDivisao;

            var incrementoMaximoDeRaio = incrementoMinimoDeRaio * this.DadosMalha.AnguloMaxMinIncremento;
            var d = (incrementoMaximoDeRaio - incrementoMinimoDeRaio) / (this.DadosMalha.RaioDivisao - 1);
            double iTheta = this.DadosMalha.AnguloTotal / this.DadosMalha.AnguloDivisao;

            var builder = ImmutableList.CreateBuilder<PontoDaMalha>();

            for (int i = 0; i < numeroDeAngulos; i++)
            {
                //angulo em radianos
                var angulo = i * iTheta * Math.PI / 180.0;
                if (IsPoroElástico)
                    angulo = angulo + matrizTensorDeTensoes.AngRot;

                var constantesDeAngulo = new ConstantesDeAngulo(angulo);

                var raio = raioInterno;
                var incrementoRaio = incrementoMinimoDeRaio;

                for (int j = 0; j < numeroDeRaios; j++)
                {
                    var x = raio * Math.Cos(angulo);
                    var y = raio * Math.Sin(angulo);

                    var constanteRaio = ConstantesDeRaio.CriarConstantesDeRaios(matrizTensorDeTensoes, this.RaioPoço, raio, poisson);
                    builder.Add(new PontoDaMalha(x, y, angulo, raio, constanteRaio, constantesDeAngulo));

                    raio += incrementoRaio;
                    incrementoRaio += d;
                }
            }

            Pontos = builder.ToImmutableList();
        }
        private void GerarTriangulos()
        {
            var divisaoDaMalha = this.DadosMalha.AnguloDivisao;
            var numeroAngulos = this.DadosMalha.AnguloDivisao;
            var numeroRaios = this.DadosMalha.RaioDivisao + 1;

            var builder = ImmutableList.CreateBuilder<Triangulo>();

            //Geração de triângulos
            for (int i = 0; i < numeroAngulos; i++)
            {
                if (i < divisaoDaMalha)
                {
                    //
                    //          +---------------+----- i + 1
                    //          |             . |
                    //          |   T3      .   |
                    //          |   Upp   .     |
                    //          |       .       |      angle
                    //          |     .    T3   |
                    //          |   .      Low  |
                    //          | .             |
                    //          +---------------+----- i
                    //          |               |
                    //          j             j + 1
                    //               Radius
                    for (int j = 0; j < this.DadosMalha.RaioDivisao; j++)
                    {
                        var p1 = Pontos[i * numeroRaios + j];
                        var p2 = Pontos[i * numeroRaios + j + 1];
                        var p3 = Pontos[(i + 1) * numeroRaios + j + 1];
                        var p4 = Pontos[(i + 1) * numeroRaios + j];

                        builder.Add(new Triangulo(p1, p2, p3));
                        builder.Add(new Triangulo(p1, p3, p4));
                    }
                }
                else
                {
                    //
                    //          +---------------+----- i + 1
                    //          | .             |
                    //          |   .     T3    |
                    //          |     .   Upp   |
                    //          |       .       |      angle
                    //          |  T3     .     |
                    //          |  Low      .   |
                    //          |             . |
                    //          +---------------+----- i
                    //          |               |
                    //          j             j + 1
                    //               Radius
                    for (int j = 0; j < this.DadosMalha.RaioDivisao; j++)
                    {
                        var p1 = Pontos[i * numeroRaios + j];
                        var p2 = Pontos[i * numeroRaios + j + 1];
                        var p3 = Pontos[(i + 1) * numeroRaios + j];
                        var p4 = Pontos[(i + 1) * numeroRaios + j + 1];

                        builder.Add(new Triangulo(p1, p2, p3));
                        builder.Add(new Triangulo(p2, p4, p3));

                    }
                }
            }

            Triangulos = builder.ToImmutableList();
        }
    }

    public struct RaioPoco
    {
        public double Value { get; }
        public RaioPoco(double diametro)
        {
            var raio = diametro / 2;

            Value = UnitConverter.PolToMeter(raio);
        }
    }
}
