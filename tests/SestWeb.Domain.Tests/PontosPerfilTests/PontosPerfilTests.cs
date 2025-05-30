using NFluent;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Pontos;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Tests.PontosPerfilTests
{
    [TestFixture]
    public class PontosPerfilTests
    {
        private PontosPerfilOld _pontos;
        private IConversorProfundidade _conversorProfundidade;

        [SetUp]
        public void Setup()
        {
            _pontos = new PontosPerfilOld();

            var trajetória = new Trajetória(MétodoDeCálculoDaTrajetória.RaioCurvatura);
            trajetória.Reset(new List<PontoDeTrajetória>
            {
                new PontoDeTrajetória(10,4,30),
                new PontoDeTrajetória(100,60,60),
            });

            var litologia = new Litologia(TipoLitologia.Adaptada, trajetória);
            //litologia.Reset(new List<PontoLitologia>()
            //{
            //    new PontoLitologia(10,TipoRocha.AGB),
            //    new PontoLitologia(100,TipoRocha.FLH),
            //});

            _pontos.ConversorProfundidade = trajetória;
            _pontos.Litologia = litologia;
            _conversorProfundidade = trajetória;
        }

        [Test]
        public void DeveRetornarPontosComPV()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(50,50,OrigemPonto.Calculado),
            });

            Check.That(_pontos.Pontos.All(x => x.Pv != 0)).IsTrue();
        }

        [Test]
        public void DevePreencherPVAtéATrajetória()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(110,50,OrigemPonto.Calculado),
            });

            for (int i = 0; i < _pontos.QuantidadeElementos - 2; i++)
            {
                Check.That(_pontos.Pontos[i].Pv != 0).IsTrue();
            }

            Check.That(_pontos.UltimoPonto.Pv).IsEqualTo(0);
        }

        [Test]
        public void DeveRemoverPontosRepetidosBaseadoNaTolerancia()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
            });

            Check.That(_pontos.QuantidadeElementos).IsEqualTo(1);

            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10.10,10,OrigemPonto.Calculado),
                new PontoOld(10.11,10,OrigemPonto.Calculado),
                new PontoOld(10.12,10,OrigemPonto.Calculado),
                new PontoOld(10.13,10,OrigemPonto.Calculado),
                new PontoOld(10.14,10,OrigemPonto.Calculado),
            });

            Check.That(_pontos.QuantidadeElementos).IsEqualTo(1);
        }

        [Test]
        public void DeveRemoverPontosRepetidos()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(10,10,OrigemPonto.Calculado),
            });

            Check.That(_pontos.QuantidadeElementos).IsEqualTo(1);
        }

        [Test]
        public void DeveRetornarPontoProfundidadeExata()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(50,50,OrigemPonto.Calculado),
            });

            var result = _pontos.TryGetPonto(30, out var ponto);

            Check.That(result).IsTrue();

            Check.That(ponto).IsNotNull();

            Check.That(ponto.Pm).IsCloseTo(30, .2);
        }

        [TestCase(9.6)]
        [TestCase(50.11)]
        public void DeveRetornarNullParaProfundidadeForaDoRange(double pm)
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(50,50,OrigemPonto.Calculado),
            });

            var result = _pontos.TryGetPonto(pm, out var ponto);

            Check.That(result).IsFalse();

            Check.That(ponto).IsNull();
        }

        [TestCase(10)]
        [TestCase(10.2)]
        [TestCase(10.3)]
        [TestCase(9.9)]
        public void DeveRetornarNullParaPontosCom1ElementoEBuscaPorElementoDiferente(double pm)
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10.11,10,OrigemPonto.Calculado),
            });

            var result = _pontos.TryGetPonto(pm, out var ponto);

            Check.That(result).IsFalse();
            Check.That(ponto).IsNull();
        }

        [Test]
        public void DeveRetornarOUnicoElementoDaListaCasoSejaIgual()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10.11,10,OrigemPonto.Calculado),
            });

            var result = _pontos.TryGetPonto(10.1, out var ponto);

            Check.That(result).IsTrue();
            Check.That(ponto).IsNotNull();
            Check.That(ponto.Pm).IsCloseTo(10.1, .2);
        }

        [Test]
        public void DeveRetornarOUnicoElementoDaListaCasoSejaIgualComUmaCasasDecimais()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10.11,10,OrigemPonto.Calculado),
            });

            var result = _pontos.TryGetPonto(10.1, out var ponto);

            Check.That(result).IsTrue();
            Check.That(ponto).IsNotNull();
            Check.That(ponto.Pm).IsCloseTo(10.1, .1);
        }

        [Test]
        public void DeveRetornarFalseCasoListaVazia()
        {
            var result = _pontos.TryGetPonto(10, out var ponto);

            Check.That(result).IsFalse();
            Check.That(ponto).IsNull();
        }

        [Test]
        public void DeveInterpolarValorIntermediaário()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(50,50,OrigemPonto.Calculado),
            });

            var result = _pontos.TryGetPonto(28, out var ponto);

            Check.That(result).IsTrue();
            Check.That(ponto).IsNotNull();
            Check.That(ponto.Pm).IsEqualTo(28);
            Check.That(ponto.Valor).IsEqualTo(28);
        }

        [Test]
        public void DeveObterPrimeiro_E_UltimoPonto()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(50,50,OrigemPonto.Calculado),
            });

            Check.That(_pontos.PrimeiroPonto.Pm).IsEqualTo(10);
            Check.That(_pontos.UltimoPonto.Pm).IsEqualTo(50);

            _pontos.Clear();

            Check.That(_pontos.PrimeiroPonto).IsNull();
            Check.That(_pontos.UltimoPonto).IsNull();
        }

        [Test]
        public void DeveInserirTipoRochaAosPontos()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(50,50,OrigemPonto.Calculado),
            });

            Check.That(_pontos.Pontos.All(x => x.TipoRocha != null)).IsTrue();
        }

        [Test]
        public void DeveInserirTipoRochaAosPontosAtéOLimiteDaLitologia()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(50,50,OrigemPonto.Calculado),
                new PontoOld(150,50,OrigemPonto.Calculado),
            });

            for (int i = 0; i < _pontos.QuantidadeElementos - 2; i++)
            {
                Check.That(_pontos.Pontos[i].TipoRocha != null).IsTrue();
            }

            Check.That(_pontos.UltimoPonto.TipoRocha).IsNull();
        }

        [Test]
        public void DeveObterMaximaProfundidadeComPV()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(150,50,OrigemPonto.Calculado),
                new PontoOld(200,50,OrigemPonto.Calculado),
            });

            Check.That(_pontos.GetMaximoPontoComPV()).IsEqualTo(new PontoOld(40, 40, OrigemPonto.Calculado));
        }

        [Test]
        public void DeveObterMaximaProfundidadeComPVComUmPonto()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
            });

            Check.That(_pontos.GetMaximoPontoComPV()).IsEqualTo(new PontoOld(10, 10, OrigemPonto.Calculado));
        }

        [Test]
        public void DeveObterMaximaProfundidadeComPVComTrajetóriaVazia()
        {
            _pontos.ConversorProfundidade = new Trajetória(MétodoDeCálculoDaTrajetória.RaioCurvatura);
            _pontos.Reset(new List<PontoOld>()
            {
               new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
            });

            Check.That(_pontos.GetMaximoPontoComPV()).IsNull();
        }

        [Test]
        public void DeveLancarExceçãoAoObterMaximaProfundidadeComPVSemPontos()
        {
            _pontos.Reset(new List<PontoOld>());

            Check.ThatCode(() => { _pontos.GetMaximoPontoComPV(); }).Throws<InvalidOperationException>();
        }

        [Test]
        public void DeveLancarExceçãoAoObterMaximaProfundidadeComLitologiaSemPontos()
        {
            _pontos.Reset(new List<PontoOld>());

            Check.ThatCode(() => { _pontos.GetMaximoPontoComLitologia(); }).Throws<InvalidOperationException>();
        }

        [Test]
        public void DeveObterMaximaProfundidadeComLitologiaConsiderandoLitologiaVazia()
        {
            //_pontos.Litologia = new Litologia(TipoLitologia.Adaptada);
            //_pontos.Reset(new List<PontoOld>()
            //{
            //   new PontoOld(10,10,OrigemPonto.Calculado),
            //    new PontoOld(20,20,OrigemPonto.Calculado),
            //    new PontoOld(30,30,OrigemPonto.Calculado),
            //    new PontoOld(40,40,OrigemPonto.Calculado),
            //});

            Check.That(_pontos.GetMaximoPontoComLitologia()).IsNull();
        }

        [Test]
        public void DeveObterMaximPontoComLitologiaConsiderantoUmPonto()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
            });

            Check.That(_pontos.GetMaximoPontoComLitologia().TipoRocha).IsEqualTo(TipoRocha.AGB);
        }

        [Test]
        public void DeveObterMaximoPontoComLitologia()
        {
            _pontos.Reset(new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20,20,OrigemPonto.Calculado),
                new PontoOld(30,30,OrigemPonto.Calculado),
                new PontoOld(40,40,OrigemPonto.Calculado),
                new PontoOld(150,50,OrigemPonto.Calculado),
                new PontoOld(200,50,OrigemPonto.Calculado),
            });

            Check.That(_pontos.GetMaximoPontoComLitologia().TipoRocha).IsEqualTo(TipoRocha.AGB);
            Check.That(_pontos.GetMaximoPontoComLitologia().Pm).IsEqualTo(40);
        }
    }
}