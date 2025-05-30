using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    public class MatrizTensorDeTensoes
    {
        private readonly double inclinacao;
        private readonly double azimute;
        private readonly double tensaoMenor;
        private readonly double tensaoMaior;
        private readonly double tensaoVertical;
        private readonly double azimuteMenor;

        public double Psh { get; private set; }
        public double Ssh { get; private set; }
        public double AngRot { get; private set; }

        public Matrix<double> Value { get; private set; }
        public MatrizTensorDeTensoes(double inclinacao, double azimute, double tensaoMenor, double tensaoMaior, double tensaoVertical, double azimuteMenor)
        {
            this.azimuteMenor = azimuteMenor;
            this.tensaoVertical = tensaoVertical;
            this.tensaoMaior = tensaoMaior;
            this.tensaoMenor = tensaoMenor;
            this.azimute = azimute;
            this.inclinacao = inclinacao;

            GerarMatriz();
            CalcularSsh();
            CalcularPsh();
            CalcularÂnguloDeRotação();

        }

        private void CalcularÂnguloDeRotação()
        {
            const double MINIMO = 0.00001;

            if (Ssh < MINIMO)
            {
                AngRot = 0;
                return;
            }
            var sx = this.Value[0, 0];
            var tauXY = this.Value[0, 1];
            var sy = this.Value[1, 1];
            var x = (sx - sy) / 2;

            if (Math.Abs(x) <= MINIMO)
            {
                if (-tauXY > 0)
                    AngRot = Math.PI / 4;
                else
                    AngRot = -Math.PI / 4;
            }
            else
            {
                if (Math.Abs(tauXY) <= MINIMO)
                {
                    if (-x < 0)
                        AngRot = 0;
                    else
                        AngRot = Math.PI / 2;
                }
                else
                {
                    AngRot = 0.5 * Math.Atan(2 * (tauXY / (sx - sy)));
                }
            }


        }
        private void CalcularSsh()
        {
            var a = this.Value[0, 0];
            var b = this.Value[0, 1];
            var c = this.Value[1, 1];

            Ssh = Math.Sqrt((Math.Pow(((a - c) / 2), 2) + Math.Pow(b, 2)));
        }

        private void CalcularPsh()
        {
            var a = this.Value[0, 0];
            var b = this.Value[1, 1];

            Psh = (a + b) / 2;
        }
        private void GerarMatriz()
        {
            var builder = Matrix<double>.Build;

            var matrizTensoesPrincipais = builder.DenseOfDiagonalArray(new[] { this.tensaoMaior, this.tensaoMenor, this.tensaoVertical });

            var inclicacaoTrajetoria = this.inclinacao;
            inclicacaoTrajetoria *= Math.PI / 180;

            //teta : azimute da tensão horizontal menor = AZTHM
            var azimuteTensãoHorizontalMenor = this.tensaoMenor.AlmostEqual(this.tensaoMaior, 0.0001) ? 0 : this.azimuteMenor;

            var a = 90 - this.azimute + azimuteTensãoHorizontalMenor;
            a *= Math.PI / 180;

            double[][] cossenosDiretores =
            {
                new double[] { Math.Cos(a) * Math.Cos(inclicacaoTrajetoria), -Math.Sin(a), Math.Cos(a) * Math.Sin(inclicacaoTrajetoria) },
                new double[] { Math.Sin(a) * Math.Cos(inclicacaoTrajetoria), Math.Cos(a), Math.Sin(a) * Math.Sin(inclicacaoTrajetoria) },
                new double[] { -Math.Sin(inclicacaoTrajetoria), 0, Math.Cos(inclicacaoTrajetoria) }
            };

            var matrizDeCossenosDiretores = builder.DenseOfColumnArrays(cossenosDiretores);

            var transpostaDeCossenosDiretores = matrizDeCossenosDiretores.Transpose();

            var transpostaXTensoesPrincipais = matrizTensoesPrincipais.Multiply(transpostaDeCossenosDiretores);
            var matrizTensorDeTensões = matrizDeCossenosDiretores.Multiply(transpostaXTensoesPrincipais);

            Value = matrizTensorDeTensões;
        }
    }
}
