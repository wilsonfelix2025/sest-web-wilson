using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Helpers;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Pontos;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Tests.BuscaBinariaTests
{
    [TestFixture]
    public class BuscaBinariaPorPmMenorIgualTests
    {
        [TestCase(10, 0)]
        [TestCase(20.10, 1)]
        [TestCase(20.14, 1)]
        [TestCase(20.13, 1)]
        [TestCase(20.12, 1)]
        [TestCase(20.11, 1)]
        [TestCase(22.4, 2)]
        [TestCase(25.8, 3)]
        public void DeveObterValoresIguais(double pm, int indiceEsperado)
        {
            var pontos = new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20.1,10,OrigemPonto.Calculado),
                new PontoOld(22.4,10,OrigemPonto.Calculado),
                new PontoOld(25.8,10,OrigemPonto.Calculado),
            };

            var result = pontos.BuscaBinariaPorPmMenorIgual(pm, out var index, out var equal);

            Check.That(result).IsTrue();
            Check.That(equal).IsTrue();
            Check.That(index).IsEqualTo(indiceEsperado);
        }

        [TestCase(20.15, 1)]
        [TestCase(20.16, 1)]
        [TestCase(20.17, 1)]
        [TestCase(20.18, 1)]
        [TestCase(20.19, 1)]
        public void DeveNãoObterValoresDiferentesPorArredondamento(double pm, int indiceEsperado)
        {
            var pontos = new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20.1,10,OrigemPonto.Calculado),
                new PontoOld(22.4,10,OrigemPonto.Calculado),
                new PontoOld(25.8,10,OrigemPonto.Calculado),
            };

            var result = pontos.BuscaBinariaPorPmMenorIgual(pm, out var index, out var equal);

            Check.That(result).IsTrue();
            Check.That(equal).IsFalse();
            Check.That(index).IsEqualTo(indiceEsperado);
        }

        [TestCase(15, 0)]
        [TestCase(21.1, 1)]
        [TestCase(22.5, 2)]
        [TestCase(23, 2)]
        public void DeveObterValoresPrimeiroValorMenor(double pm, int indiceEsperado)
        {
            var pontos = new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20.1,10,OrigemPonto.Calculado),
                new PontoOld(22.4,10,OrigemPonto.Calculado),
                new PontoOld(25.8,10,OrigemPonto.Calculado),
            };

            var result = pontos.BuscaBinariaPorPmMenorIgual(pm, out var index, out var equal);

            Check.That(result).IsTrue();
            Check.That(equal).IsFalse();
            Check.That(index).IsEqualTo(indiceEsperado);
        }

        [TestCase(9)]
        [TestCase(26)]
        public void DeveLidarComValoresForaDoIntervalo(double pm)
        {
            var pontos = new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado),
                new PontoOld(20.1,10,OrigemPonto.Calculado),
                new PontoOld(22.4,10,OrigemPonto.Calculado),
                new PontoOld(25.8,10,OrigemPonto.Calculado),
            };

            var result = pontos.BuscaBinariaPorPmMenorIgual(pm, out var index, out var equal);

            Check.That(result).IsFalse();
            Check.That(equal).IsFalse();
            Check.That(index).IsEqualTo(-1);
        }

        [Test]
        public void DeveLidarListaVazia()
        {
            var pontos = new List<PontoOld>();

            var result = pontos.BuscaBinariaPorPmMenorIgual(10, out var index, out var equal);

            Check.That(result).IsFalse();
            Check.That(equal).IsFalse();
            Check.That(index).IsEqualTo(-1);
        }

        [Test]
        public void DeveLidarListaComUmElementoEncontrando()
        {
            var pontos = new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado)
            };

            var result = pontos.BuscaBinariaPorPmMenorIgual(10, out var index, out var equal);

            Check.That(result).IsTrue();
            Check.That(equal).IsTrue();
            Check.That(index).IsEqualTo(0);
        }

        [TestCase(9)]
        [TestCase(26)]
        public void DeveLidarListaComUmElementoNãoEncontrando(double pm)
        {
            var pontos = new List<PontoOld>()
            {
                new PontoOld(10,10,OrigemPonto.Calculado)
            };

            var result = pontos.BuscaBinariaPorPmMenorIgual(pm, out var index, out var equal);

            Check.That(result).IsFalse();
            Check.That(equal).IsFalse();
            Check.That(index).IsEqualTo(-1);
        }
    }
}