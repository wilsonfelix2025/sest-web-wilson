using System;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    public class ConstantesDeRaio
    {
        public static ConstantesDeRaio CriarConstantesDeRaios(MatrizTensorDeTensoes matrizTensorDeTensoes, RaioPoco raioPoco, double raioCorrente, double poisson)
        {
            var matriz = matrizTensorDeTensoes.Value;

            raioCorrente = UnitConverter.PolToMeter(raioCorrente);

            var sigmaX = matriz[0, 0];
            var sigmaY = matriz[1, 1];
            var sigmaZ0 = matriz[2, 2];

            var tauXY = matriz[1, 0];
            var tauYZ = matriz[1, 2];
            var tauXZ = matriz[2, 0];

            double raioDoPoco2 = Math.Pow(raioPoco.Value, 2);
            double raioDoPoco4 = Math.Pow(raioPoco.Value, 4);
            double raioExterno2 = Math.Pow(raioCorrente, 2);
            double raioExterno4 = Math.Pow(raioCorrente, 4);

            //Tensões radiais
            var tensoesRadiaisParciais = new TensoesRadiaisParciais(sigmaX, sigmaY, tauXY, raioDoPoco2, raioDoPoco4, raioExterno2, raioExterno4);

            //Tensões tangenciais
            var tensoesTangenciaisParciais = new TensoesTangenciaisParciais(sigmaX, sigmaY, tauXY, raioDoPoco2, raioDoPoco4, raioExterno2, raioExterno4);

            //Tensões axiais
            var tensoesAxiaisParciais = new TensoesAxiaisParciais(poisson, sigmaX, sigmaY, sigmaZ0, tauXY, raioDoPoco2, raioDoPoco4, raioExterno2, raioExterno4);

            //parciais de tensão de cisalhamento (plano radial X theta)
            var TensoesCisalhamentoParciais = new TensoesCisalhamentoParciais(sigmaX, sigmaY, tauXY, tauYZ, tauXZ, raioDoPoco2, raioDoPoco4, raioExterno2, raioExterno4);

            return new ConstantesDeRaio(raioPoco, tensoesRadiaisParciais, tensoesTangenciaisParciais, tensoesAxiaisParciais, TensoesCisalhamentoParciais);
        }

        private static void NewMethod(double sigmaX, double sigmaY, double tauXY, double tauYZ, double tauXZ, RaioPoco raioPoco, double raioExterno)
        {
            var raioDoPoco2 = Math.Pow(raioPoco.Value, 2);
            var raioDoPoco4 = Math.Pow(raioPoco.Value, 4);
            var raioExterno2 = Math.Pow(raioExterno, 2);
            var raioExterno4 = Math.Pow(raioExterno, 4);

            var tensãoCisalhamentoRadialThetaSeno2Theta = ((sigmaY - sigmaX) / 2.0) * (1.0 - 3.0 * (raioDoPoco4 / raioExterno4) + 2.0 * (raioDoPoco2 / raioExterno2));
            var tensãoCisalhamentoRadialThetaCosseno2Theta = tauXY * (1.0 - 3.0 * (raioDoPoco4 / raioExterno4) + 2.0 * (raioDoPoco2 / raioExterno2));

            //parciais de tensão de cisalhamento (plano radial X axial)
            var tensãoCisalhamentoRadialAxialSenoTheta = tauYZ * (1.0 - raioDoPoco2 / raioExterno2);
            var tensãoCisalhamentoRadialAxialCossenoTheta = tauXZ * (1.0 - raioDoPoco2 / raioExterno2);


            //parciais de tensão de cisalhamento (plano theta X axial)
            var tensãoCisalhamentoThetaAxialSenoTheta = tauXZ * (1.0 + raioDoPoco2 / raioExterno2);
            var tensãoCisalhamentoThetaAxialCossenoTheta = tauYZ * (1.0 + raioDoPoco2 / raioExterno2);
        }


        private ConstantesDeRaio(RaioPoco raio, TensoesRadiaisParciais tensoesRadiaisParciais, TensoesTangenciaisParciais tensoesTangenciaisParciais, TensoesAxiaisParciais tensoesAxiaisParciais, TensoesCisalhamentoParciais tensoesCisalhamentoParciais)
        {
            Raio = raio;
            TensoesRadiaisParciais = tensoesRadiaisParciais;
            TensoesTangenciaisParciais = tensoesTangenciaisParciais;
            TensoesAxiaisParciais = tensoesAxiaisParciais;
            TensoesCisalhamentoParciais = tensoesCisalhamentoParciais;
        }

        public RaioPoco Raio { get; }
        public TensoesRadiaisParciais TensoesRadiaisParciais { get; }
        public TensoesTangenciaisParciais TensoesTangenciaisParciais { get; }
        public TensoesAxiaisParciais TensoesAxiaisParciais { get; }
        public TensoesCisalhamentoParciais TensoesCisalhamentoParciais { get; }
    }

    public class TensoesRadiaisParciais
    {
        public TensoesRadiaisParciais(double sigmaX, double sigmaY, double tauXY, double raioDoPoco2, double raioDoPoco4, double raioExterno2, double raioExterno4)
        {
            TensaoRadialParcial = ((sigmaX + sigmaY) / 2.0) * (1.0 - raioDoPoco2 / raioExterno2);
            TensaoRadialCosseno2ThetaParcial = ((sigmaX - sigmaY) / 2.0) * (1.0 + (3.0 * (raioDoPoco4 / raioExterno4) - 4.0 * (raioDoPoco2 / raioExterno2)));
            TensaoRadialSeno2ThetaParcial = tauXY * (1.0 + (3.0 * (raioDoPoco4 / raioExterno4)) - (4.0 * (raioDoPoco2 / raioExterno2)));
            TensaoRadialPesoDeLamaParcial = raioDoPoco2 / raioExterno2;
        }

        public double TensaoRadialParcial { get; }
        public double TensaoRadialCosseno2ThetaParcial { get; }
        public double TensaoRadialSeno2ThetaParcial { get; }
        public double TensaoRadialPesoDeLamaParcial { get; }
    }

    public class TensoesTangenciaisParciais
    {
        public TensoesTangenciaisParciais(double sigmaX, double sigmaY, double tauXY, double raioDoPoco2, double raioDoPoco4, double raio2, double raio4)
        {
            TensaoTangencialParcial = ((sigmaX + sigmaY) / 2.0) * (1 + raioDoPoco2 / raio2);
            TensaoTangencialCosseno2ThetaParcial = ((sigmaX - sigmaY) / 2.0) * (1.0 + 3.0 * (raioDoPoco4 / raio4));
            TensaoTangencialSeno2ThetaParcial = tauXY * (1.0 + 3.0 * (raioDoPoco4 / raio4));
            TensaoTangencialPesoDeLamaParcial = raioDoPoco2 / raio2;
        }

        public double TensaoTangencialParcial { get; }
        public double TensaoTangencialCosseno2ThetaParcial { get; }
        public double TensaoTangencialSeno2ThetaParcial { get; }
        public double TensaoTangencialPesoDeLamaParcial { get; }
    }

    public class TensoesAxiaisParciais
    {
        public TensoesAxiaisParciais(double poisson, double sigmaX, double sigmaY, double sigmaZ0, double tauXY, double raioDoPoco2, double raioDoPoco4, double raioExterno2, double raioExterno4)
        {
            TensaoAxialParcial = sigmaZ0;
            TensaoAxialCosseno2ThetaParcial = poisson * 2.0 * (sigmaX - sigmaY) * (raioDoPoco2 / raioExterno2);
            TensaoAxialSeno2ThetaParcial = poisson * 4.0 * tauXY * (raioDoPoco2 / raioExterno2);
            TensaoAxialPesoDeLamaParcial = 0;
        }

        public double TensaoAxialParcial { get; }
        public double TensaoAxialCosseno2ThetaParcial { get; }
        public double TensaoAxialSeno2ThetaParcial { get; }
        public double TensaoAxialPesoDeLamaParcial { get; }

    }

    public class TensoesCisalhamentoParciais
    {
        public TensoesCisalhamentoParciais(double sigmaX, double sigmaY, double tauXY, double tauYZ, double tauXZ, double raioDoPoco2, double raioDoPoco4, double raioExterno2, double raioExterno4)
        {
            TensaoCisalhamentoRadialThetaSeno2Theta = ((sigmaY - sigmaX) / 2.0) * (1.0 - 3.0 * (raioDoPoco4 / raioExterno4) + 2.0 * (raioDoPoco2 / raioExterno2));
            TensaoCisalhamentoRadialThetaCosseno2Theta = tauXY * (1.0 - 3.0 * (raioDoPoco4 / raioExterno4) + 2.0 * (raioDoPoco2 / raioExterno2));

            //parciais de tensão de cisalhamento (plano radial X axial)
            TensaoCisalhamentoRadialAxialSenoTheta = tauYZ * (1.0 - raioDoPoco2 / raioExterno2);
            TensaoCisalhamentoRadialAxialCossenoTheta = tauXZ * (1.0 - raioDoPoco2 / raioExterno2);

            //parciais de tensão de cisalhamento (plano theta X axial)
            TensaoCisalhamentoThetaAxialSenoTheta = tauXZ * (1.0 + raioDoPoco2 / raioExterno2) * -1;
            TensaoCisalhamentoThetaAxialCossenoTheta = tauYZ * (1.0 + raioDoPoco2 / raioExterno2);
        }

        //parciais de tensao de cisalhamento (plano radial X theta)
        public double TensaoCisalhamentoRadialThetaSeno2Theta { get; }
        public double TensaoCisalhamentoRadialThetaCosseno2Theta { get; }

        //parciais de tensao de cisalhamento (plano radial X axial)
        public double TensaoCisalhamentoRadialAxialSenoTheta { get; }
        public double TensaoCisalhamentoRadialAxialCossenoTheta { get; }

        //parciais de tensao de cisalhamento (plano theta X axial)
        public double TensaoCisalhamentoThetaAxialSenoTheta { get; }
        public double TensaoCisalhamentoThetaAxialCossenoTheta { get; }
    }
}
