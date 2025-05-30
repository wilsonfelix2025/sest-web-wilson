using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Importadores.Deep.Sest5;

namespace SestWeb.Domain.Tests.TrajetoriaTests
{
    [TestFixture]
    public class TrajetoriaTests
    {
        [Test]
        public void TesteTrajetória()
        {
            var leitorSest5 = new LeituraArquivoSest5("ArquivosEntradaParaTestes/Jo_trajetoria.xml");

            if (!leitorSest5.TryObterTrajetória(out var trajetória))
            {
                throw new InvalidOperationException("Não conseguiu ler os dados do sest 5");
            }

            PontoDeTrajetória ponto;

            var result = trajetória.TryGetPonto(new Profundidade(671), out ponto);
            Check.That(result).IsTrue();
            Check.That(ponto.Inclinação).IsEqualTo(0);
            Check.That(ponto.Azimute).IsEqualTo(0);
            Check.That(ponto.Pv.Valor).IsCloseTo(671, .2);

            result = trajetória.TryGetPonto(new Profundidade(5112), out ponto);
            Check.That(result).IsTrue();
            Check.That(ponto.Inclinação).IsCloseTo(0.059999999999999991, .2);
            Check.That(ponto.Azimute).IsCloseTo(158.59000000000000, .2);
            Check.That(ponto.Pv.Valor).IsCloseTo(4999.5, .2);

            result = trajetória.TryGetPonto(new Profundidade(0), out ponto);
            Check.That(result).IsTrue();
            Check.That(ponto.Inclinação).IsCloseTo(0, .2);
            Check.That(ponto.Azimute).IsCloseTo(0, .2);
            Check.That(ponto.Pv.Valor).IsCloseTo(0, .2);

            result = trajetória.TryGetPonto(new Profundidade(-1), out ponto);
            Check.That(result).IsFalse();

            result = trajetória.TryGetPonto(new Profundidade(5123), out ponto);
            Check.That(result).IsFalse();
        }

        [Test]
        public void TesteTrajetóriaObterPVConsiderandoInclinaçãoEAzimute()
        {
            var trajetória = new Trajetória(MétodoDeCálculoDaTrajetória.RaioCurvatura);

            trajetória.Reset(new List<PontoDeTrajetória>()
            {
                new PontoDeTrajetória(1000,10,15),
                new PontoDeTrajetória(1500,20,25),
                new PontoDeTrajetória(2000,30,40),
                new PontoDeTrajetória(2500,35,10),
                new PontoDeTrajetória(2700,35,35),
                new PontoDeTrajetória(2900,35,35),
                new PontoDeTrajetória(4000,35,45),
            });

            // valores exatos]

            PontoDeTrajetória ponto;

            var result = trajetória.TryGetPonto(new Profundidade(1000), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 10, 15, 994.9);

            result = trajetória.TryGetPonto(new Profundidade(1500), out ponto);
            Assert.True(result);

            result = trajetória.TryGetPonto(new Profundidade(1500), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 20, 25, 1477.3);

            result = trajetória.TryGetPonto(new Profundidade(2000), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 30, 40, 1929.9);

            result = trajetória.TryGetPonto(new Profundidade(2500), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 35, 10, 2351.5);

            result = trajetória.TryGetPonto(new Profundidade(2700), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 35, 35, 2515.3);

            result = trajetória.TryGetPonto(new Profundidade(2900), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 35, 35, 2679.1);

            result = trajetória.TryGetPonto(new Profundidade(4000), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 35, 45, 3580.2);

            //profundidade intermediária
            result = trajetória.TryGetPonto(new Profundidade(1481.72), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 19.634399999999999, 24.520502489606603, 1460.0835094240376);

            // profundidade anterior ao primeiro ponto de trajetória
            result = trajetória.TryGetPonto(new Profundidade(800), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 8, 15, 797.4);

            // profundidade anterior ao primeiro ponto de trajetória
            result = trajetória.TryGetPonto(new Profundidade(3990), out ponto);
            Check.That(result).IsTrue();
            CheckPontoTrajetória(ponto, 35, 44.909090909090907, 3571.9583462930927);

            //extremos
            result = trajetória.TryGetPonto(new Profundidade(4000.1), out ponto);
            Check.That(result).IsFalse();

            result = trajetória.TryGetPonto(new Profundidade(-1), out ponto);
            Check.That(result).IsFalse();
        }

        private void CheckPontoTrajetória(PontoDeTrajetória ponto, double inclinação, double azimute, double pv)
        {
            Check.That(ponto.Inclinação).IsCloseTo(inclinação, .2);
            Check.That(ponto.Azimute).IsCloseTo(azimute, .2);
            Check.That(ponto.Pv.Valor).IsCloseTo(pv, .1);
        }
    }
}