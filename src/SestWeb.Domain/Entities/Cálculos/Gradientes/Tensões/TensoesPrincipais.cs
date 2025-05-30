using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões
{
    public class TensoesPrincipais
    {
        public double TensaoPrincipal1 { get; private set; }
        public double TensaoPrincipal2 { get; private set; }
        public double TensaoPrincipal3 { get; private set; }

        public double TensaoPrincipalEfetiva1 { get; private set; }
        public double TensaoPrincipalEfetiva2 { get; private set; }
        public double TensaoPrincipalEfetiva3 { get; private set; }

        //TODO remover pressaoPoros,
        public void CalcularTensoesPrincipais(RaioPoco raioPoco, double raioCorrente, ITensoesAoRedorPoco tensoesAoRedor, double pressaoPoros, double biot)
        {
            if (EhParedePoco(raioPoco, raioCorrente))
            {
                CalcularTensoesPrincipaisParedePoco(tensoesAoRedor, biot);
            }
            else
            {
                CalcularTensoesPrincipaisForaDaParedePoco(tensoesAoRedor, biot);
            }
        }

        public void CalcularTensoesPrincipaisForaDaParedePoco(ITensoesAoRedorPoco tensoesAoRedor, double biot)
        {
            var invariantes = ObterInvariantes(tensoesAoRedor);

            var a1 = -invariantes.Item1;
            var a2 = invariantes.Item2;
            var a3 = -invariantes.Item3;

            var Q = (3.0 * a2 - a1 * a1) / 9.0;
            var J = (9.0 * a1 * a2 - 27.0 * a3 - 2.0 * a1 * a1 * a1) / 54.0;
            var D = Q * Q * Q + J * J;

            double r1, r2, r3;

            if (D == 0.0)
            {
                r1 = 2.0 * Math.Pow(J, 1.0 / 3.0) - a1 / 3.0;
                r2 = r3 = -Math.Pow(J, 1.0 / 3.0) - a1 / 3.0;
            }
            else if (D > 0.0)
            {
                r1 = Math.Pow(J + Math.Sqrt(D), 1.0 / 3.0) + Math.Pow(J - Math.Sqrt(D), 1.0 / 3.0) - a1 / 3.0;
                r2 = r1;
                r3 = r1;
            }
            else
            { // D < 0.0

                var teta = Math.Acos(J / Math.Sqrt(-Q * Q * Q));
                var tetaEmGrau = (180 / Math.PI) * teta;

                var a = 2.0 * Math.Sqrt(-Q);

                r1 = a * Math.Cos((tetaEmGrau / 3.0) * Math.PI / 180) - a1 / 3.0;
                r2 = a * Math.Cos(((tetaEmGrau + 180) / 3.0) * Math.PI / 180) - a1 / 3.0;
                r3 = a * Math.Cos(((tetaEmGrau + 360) / 3.0) * Math.PI / 180) - a1 / 3.0;
            }

            var tensoes = new[] { r1, r2, r3 };

            Array.Sort(tensoes);

            this.TensaoPrincipal1 = tensoes[2];
            this.TensaoPrincipal2 = tensoes[1];
            this.TensaoPrincipal3 = tensoes[0];

            var pressao = tensoesAoRedor.Pressao;

            this.TensaoPrincipalEfetiva1 = this.TensaoPrincipal1 - pressao * biot;
            this.TensaoPrincipalEfetiva2 = this.TensaoPrincipal2 - pressao * biot;
            this.TensaoPrincipalEfetiva3 = this.TensaoPrincipal3 - pressao * biot;
        }

        private Tuple<double, double, double> ObterInvariantes(ITensoesAoRedorPoco tensoesAoRedor)
        {
            var tensaoRadial = tensoesAoRedor.TensaoRadial;
            var tensaoAxial = tensoesAoRedor.TensaoAxial;
            var tensaoTangencial = tensoesAoRedor.TensaoTangencial;
            var tensaoCisalhanteRadialAxial = tensoesAoRedor.TensaoCisalhanteNoPlanoRadialAxial;
            var tensaoCisalhanteRadialTangencial = tensoesAoRedor.TensaoCisalhanteNoPlanoRadialTangencial;
            var tensaoCisalhanteTangencialAxial = tensoesAoRedor.TensaoCisalhanteNoPlanoTangencialAxial;


            var invariante1 = tensaoRadial + tensaoTangencial + tensaoAxial;

            var invariante2 = (tensaoRadial * tensaoTangencial) + (tensaoTangencial * tensaoAxial) + (tensaoRadial * tensaoAxial) -
                              Math.Pow(tensaoCisalhanteRadialAxial, 2) - Math.Pow(tensaoCisalhanteRadialTangencial, 2) - Math.Pow(tensaoCisalhanteTangencialAxial, 2);

            var invariante3Parcial1 = tensaoRadial * tensaoAxial * tensaoTangencial;
            var invariante3Parcial2 = 2 * tensaoCisalhanteRadialAxial * tensaoCisalhanteRadialTangencial * tensaoCisalhanteTangencialAxial;
            var invariante3Parcial3 = tensaoTangencial * Math.Pow(tensaoCisalhanteRadialAxial, 2);
            var invariante3Parcial4 = tensaoRadial * Math.Pow(tensaoCisalhanteTangencialAxial, 2);
            var invariante3Parcial5 = tensaoAxial * Math.Pow(tensaoCisalhanteRadialTangencial, 2);

            var invariante3 = invariante3Parcial1 + invariante3Parcial2 - invariante3Parcial3 - invariante3Parcial4 - invariante3Parcial5;

            return new Tuple<double, double, double>(invariante1, invariante2, invariante3);
        }

        private void CalcularTensoesPrincipaisParedePoco(ITensoesAoRedorPoco tensoesAoRedor, double biot)
        {
            var tensaoAxial = tensoesAoRedor.TensaoAxial;
            var tensaoTangencial = tensoesAoRedor.TensaoTangencial;
            var tensaoCisalhanteNoPlanoTangencialAxial = tensoesAoRedor.TensaoCisalhanteNoPlanoTangencialAxial;


            var tensaoPrincipalA = tensoesAoRedor.TensaoRadial;

            var tensaoPrincipalB = ((tensaoAxial + tensaoTangencial) / 2) +
                                   (Math.Sqrt(Math.Pow((tensaoAxial - tensaoTangencial) / 2, 2) +
                                   Math.Pow(tensaoCisalhanteNoPlanoTangencialAxial, 2)));

            var tensaoPrincipalC = ((tensaoAxial + tensaoTangencial) / 2) -
                                   (Math.Sqrt(Math.Pow((tensaoAxial - tensaoTangencial) / 2, 2) +
                                   Math.Pow(tensaoCisalhanteNoPlanoTangencialAxial, 2)));

            var tensoes = new[] { tensaoPrincipalA, tensaoPrincipalB, tensaoPrincipalC };

            Array.Sort(tensoes);

            this.TensaoPrincipal1 = tensoes[2];
            this.TensaoPrincipal2 = tensoes[1];
            this.TensaoPrincipal3 = tensoes[0];

            var pressao = tensoesAoRedor.Pressao;

            this.TensaoPrincipalEfetiva1 = this.TensaoPrincipal1 - pressao * biot;
            this.TensaoPrincipalEfetiva2 = this.TensaoPrincipal2 - pressao * biot;
            this.TensaoPrincipalEfetiva3 = this.TensaoPrincipal3 - pressao * biot;
        }

        private bool EhParedePoco(RaioPoco raioPoco, double raioCorrente)
        {
            return raioPoco.Value == raioCorrente;
        }
    }
}
