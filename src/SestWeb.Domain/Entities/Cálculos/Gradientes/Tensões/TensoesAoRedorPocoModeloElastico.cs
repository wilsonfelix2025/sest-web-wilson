using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões
{
    public class TensoesAoRedorPocoModeloElastico : ITensoesAoRedorPoco
    {
        private double _tensaoRadial;
        private double _tensaoTangencial;
        private double _tensaoAxial;
        private double _tensaoCisalhanteNoPlanoRadialTangencial;
        private double _tensaoCisalhanteNoPlanoTangencialAxial;
        private double _tensaoCisalhanteNoPlanoRadialAxial;
        private double _pressao;

        public double TensaoRadial { get => _tensaoRadial; private set => _tensaoRadial = UnitConverter.PascalToPsi(value); }
        public double TensaoTangencial { get => _tensaoTangencial; private set => _tensaoTangencial = UnitConverter.PascalToPsi(value); }
        public double TensaoAxial { get => _tensaoAxial; private set => _tensaoAxial = UnitConverter.PascalToPsi(value); }
        public double TensaoCisalhanteNoPlanoRadialTangencial { get => _tensaoCisalhanteNoPlanoRadialTangencial; private set => _tensaoCisalhanteNoPlanoRadialTangencial = UnitConverter.PascalToPsi(value); }
        public double TensaoCisalhanteNoPlanoTangencialAxial { get => _tensaoCisalhanteNoPlanoTangencialAxial; private set => _tensaoCisalhanteNoPlanoTangencialAxial = UnitConverter.PascalToPsi(value); }
        public double TensaoCisalhanteNoPlanoRadialAxial { get => _tensaoCisalhanteNoPlanoRadialAxial; private set => _tensaoCisalhanteNoPlanoRadialAxial = UnitConverter.PascalToPsi(value); }

        public double Pressao { get => _pressao; private set => _pressao = UnitConverter.PascalToPsi(value); }

        public void CalcularElasticoNaoPenetrante(PontoDaMalha pontoDaMalha, double pw, double pressaoPoros)
        {
            var constantesAngulo = pontoDaMalha.ConstantesDeAngulos;
            var constanteRaio = pontoDaMalha.ConstantesDeRaio;

            var tensoesRadiais = constanteRaio.TensoesRadiaisParciais;
            var tensoesTangenciais = constanteRaio.TensoesTangenciaisParciais;
            var tensoesAxiais = constanteRaio.TensoesAxiaisParciais;
            var tensoesCisalhamento = constanteRaio.TensoesCisalhamentoParciais;

            this.TensaoRadial = tensoesRadiais.TensaoRadialParcial +
                           tensoesRadiais.TensaoRadialCosseno2ThetaParcial * constantesAngulo.CossenoAnguloRadiano2 +
                           tensoesRadiais.TensaoRadialSeno2ThetaParcial * constantesAngulo.SenoAnguloRadiano2 +
                           tensoesRadiais.TensaoRadialPesoDeLamaParcial * pw;


            this.TensaoTangencial = tensoesTangenciais.TensaoTangencialParcial -
                               tensoesTangenciais.TensaoTangencialCosseno2ThetaParcial * constantesAngulo.CossenoAnguloRadiano2 -
                               tensoesTangenciais.TensaoTangencialSeno2ThetaParcial * constantesAngulo.SenoAnguloRadiano2 -
                               tensoesTangenciais.TensaoTangencialPesoDeLamaParcial * pw;

            this.TensaoAxial = tensoesAxiais.TensaoAxialParcial -
                          tensoesAxiais.TensaoAxialCosseno2ThetaParcial * constantesAngulo.CossenoAnguloRadiano2 -
                          tensoesAxiais.TensaoAxialSeno2ThetaParcial * constantesAngulo.SenoAnguloRadiano2 +
                          tensoesAxiais.TensaoAxialPesoDeLamaParcial * pw;


            this.TensaoCisalhanteNoPlanoRadialTangencial =
                tensoesCisalhamento.TensaoCisalhamentoRadialThetaSeno2Theta * constantesAngulo.SenoAnguloRadiano2 +
                tensoesCisalhamento.TensaoCisalhamentoRadialThetaCosseno2Theta * constantesAngulo.CossenoAnguloRadiano2;


            this.TensaoCisalhanteNoPlanoTangencialAxial =
                tensoesCisalhamento.TensaoCisalhamentoThetaAxialSenoTheta * constantesAngulo.SenoAnguloRadiano +
                tensoesCisalhamento.TensaoCisalhamentoThetaAxialCossenoTheta * constantesAngulo.CossenoAnguloRadiano;


            this.TensaoCisalhanteNoPlanoRadialAxial =
                tensoesCisalhamento.TensaoCisalhamentoRadialAxialCossenoTheta * constantesAngulo.CossenoAnguloRadiano +
                tensoesCisalhamento.TensaoCisalhamentoRadialAxialSenoTheta * constantesAngulo.SenoAnguloRadiano;

            this.Pressao = pressaoPoros;
        }

        public void CalcularElasticoPenetrante(double biot, double poisson, double pw, RaioPoco raioPoco, double pressaoPoros, double raio)
        {
            var re = 10 * raioPoco.Value;
            var re2 = Math.Pow(re, 2);
            var raio2 = Math.Pow(raio, 2);

            var raioPoco2 = Math.Pow(raioPoco.Value, 2);
            var nabla = biot * (1 - 2 * poisson) / (2 * (1 - poisson));

            var parcial = ((raioPoco2 / (re2 - raioPoco2)) * (1 - (re2 / raio2))) + Math.Log(re / raio) / Math.Log(re / raioPoco.Value);

            TensaoRadial = UnitConverter.PsiToPascal(TensaoRadial) - parcial * nabla * (pressaoPoros - pw);

            var parcialAxial = ((2 * poisson * raioPoco2) / (re2 - raioPoco2)) + ((2 / (Math.Log(re / raioPoco.Value))) * ((Math.Log(re / raio)) - (poisson / 2)));
            TensaoAxial = UnitConverter.PsiToPascal(TensaoAxial) - parcialAxial * nabla * (pressaoPoros - pw);

            var parcialTangencial = ((raioPoco2 / (re2 - raioPoco2)) * (1 + (re2 / raio2))) + ((Math.Log(re / raio) - 1) / (Math.Log(re / raioPoco.Value)));
            TensaoTangencial = UnitConverter.PsiToPascal(TensaoTangencial) - parcialTangencial * nabla * (pressaoPoros - pw);

            this.Pressao = pressaoPoros + (pw - pressaoPoros) * (Math.Log(raio / re) / Math.Log(raioPoco.Value / re));
        }
    }
}
