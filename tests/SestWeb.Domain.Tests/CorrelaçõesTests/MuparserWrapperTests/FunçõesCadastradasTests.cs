using System;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class FunçõesCadastradasTests
    {
        private Parser _parser;
        private const double precisão = 0.0001;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("TesteFunçõesCadastradas").Parser;
        }

        [Test]
        public void TesteSenoCosenoTangente()
        {
            _parser.Expr = "sin(0)";
            var calculado = _parser.Eval();
            var esperado = 0.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "cos(0)";
            calculado = _parser.Eval();
            esperado = 1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "tan(0)";
            calculado = _parser.Eval();
            esperado = 0.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sin(_pi/2)";
            calculado = _parser.Eval();
            Check.That(calculado).IsEqualTo(1.0);

            _parser.Expr = "cos(_pi/2)";
            calculado = _parser.Eval();
            esperado = 0.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            // Teste de tangente de PI/2 falha pq deveria dar infinito. cada biblioteca assume o seu infinito.
            //_parser.Expr = "tan(_pi/2)";
            //calculado = _parser.Eval();
            //esperado = Math.Tan(PI / 2);
            //Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sin(_pi)";
            calculado = _parser.Eval();
            esperado = 0.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "cos(_pi)";
            calculado = _parser.Eval();
            esperado = -1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "tan(_pi)";
            calculado = _parser.Eval();
            esperado = 0.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sin(_pi + _pi / 2)";
            calculado = _parser.Eval();
            esperado = -1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "cos(_pi + _pi / 2)";
            calculado = _parser.Eval();
            esperado = 0.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            // Teste de tangente de 3*PI/2 falha pq deveria dar infinito. cada biblioteca assume o seu infinito.
            //_parser.Expr = "tan(_pi + _pi/2)";
            //calculado = _parser.Eval();
            //esperado = Math.Tan(Math.PI + Math.PI/2);
            //Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sin(4214.3213)";
            calculado = _parser.Eval();
            esperado = Math.Sin(4214.3213);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "cos(4214.3213)";
            calculado = _parser.Eval();
            esperado = Math.Cos(4214.3213);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "tan(4214.3213)";
            calculado = _parser.Eval();
            esperado = Math.Tan(4214.3213);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteArcoSenoCosenoTangente()
        {
            _parser.Expr = "asin(0)";
            var calculado = _parser.Eval();
            var esperado = Math.Asin(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "acos(0)";
            calculado = _parser.Eval();
            esperado = Math.Acos(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "atan(0)";
            calculado = _parser.Eval();
            esperado = Math.Atan(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "asin(1)";
            calculado = _parser.Eval();
            esperado = Math.Asin(1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "acos(1)";
            calculado = _parser.Eval();
            esperado = Math.Acos(1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "atan(1)";
            calculado = _parser.Eval();
            esperado = Math.Atan(1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "asin(-1)";
            calculado = _parser.Eval();
            esperado = Math.Asin(-1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "acos(-1)";
            calculado = _parser.Eval();
            esperado = Math.Acos(-1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "atan(-1)";
            calculado = _parser.Eval();
            esperado = Math.Atan(-1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "asin(0.43212313)";
            calculado = _parser.Eval();
            esperado = Math.Asin(0.43212313);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "acos(0.43212313)";
            calculado = _parser.Eval();
            esperado = Math.Acos(0.43212313);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "atan(4321.2313)";
            calculado = _parser.Eval();
            esperado = Math.Atan(4321.2313);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteSenoCosenoTangenteHyperbolico()
        {
            _parser.Expr = "sinh(0)";
            var calculado = _parser.Eval();
            var esperado = Math.Sinh(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "cosh(0)";
            calculado = _parser.Eval();
            esperado = Math.Cosh(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "tanh(0)";
            calculado = _parser.Eval();
            esperado = Math.Tanh(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sinh(-20)";
            calculado = _parser.Eval();
            esperado = Math.Sinh(-20);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "cosh(-20)";
            calculado = _parser.Eval();
            esperado = Math.Cosh(-20);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "tanh(-20)";
            calculado = _parser.Eval();
            esperado = Math.Tanh(-20);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sinh(20)";
            calculado = _parser.Eval();
            esperado = Math.Sinh(20);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "cosh(20)";
            calculado = _parser.Eval();
            esperado = Math.Cosh(20);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "tanh(20)";
            calculado = _parser.Eval();
            esperado = Math.Tanh(20);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteExponencialELogaritmo()
        {
            _parser.Expr = "exp(0)";
            var calculado = _parser.Eval();
            var esperado = Math.Exp(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "ln(1)";
            calculado = _parser.Eval();
            esperado = Math.Log(1, Math.E);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log2(1)";
            calculado = _parser.Eval();
            esperado = Math.Log(1, 2);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log10(1)";
            calculado = _parser.Eval();
            esperado = Math.Log10(1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "exp(1)";
            calculado = _parser.Eval();
            esperado = Math.Exp(1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "ln(_e)";
            calculado = _parser.Eval();
            esperado = Math.Log(Math.E, Math.E);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log2(2)";
            calculado = _parser.Eval();
            esperado = Math.Log(2, 2);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log10(10)";
            calculado = _parser.Eval();
            esperado = Math.Log10(10);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "exp(-1)";
            calculado = _parser.Eval();
            esperado = Math.Exp(-1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "ln(1/_e)";
            calculado = _parser.Eval();
            esperado = Math.Log(1 / Math.E, Math.E);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log2(0.5)";
            calculado = _parser.Eval();
            esperado = Math.Log(0.5, 2);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log10(0.1)";
            calculado = _parser.Eval();
            esperado = Math.Log10(0.1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log2(1024)";
            calculado = _parser.Eval();
            esperado = Math.Log(1024, 2);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "log10(10000000000)";
            calculado = _parser.Eval();
            esperado = Math.Log10(10000000000);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteSqrt()
        {
            _parser.Expr = "sqrt(0)";
            var calculado = _parser.Eval();
            var esperado = Math.Sqrt(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sqrt(2)";
            calculado = _parser.Eval();
            esperado = Math.Sqrt(2);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sqrt(0.5)";
            calculado = _parser.Eval();
            esperado = Math.Sqrt(0.5);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sqrt(4)";
            calculado = _parser.Eval();
            esperado = Math.Sqrt(4);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteSignAbs()
        {
            _parser.Expr = "sign(0)";
            double calculado = _parser.Eval();
            double esperado = Math.Sign(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sign(1)";
            calculado = _parser.Eval();
            esperado = Math.Sign(1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sign(-1)";
            calculado = _parser.Eval();
            esperado = Math.Sign(-1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sign(100.2)";
            calculado = _parser.Eval();
            esperado = Math.Sign(100.2);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sign(-100.2)";
            calculado = _parser.Eval();
            esperado = Math.Sign(-100.2);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "abs(0)";
            calculado = _parser.Eval();
            esperado = Math.Abs(0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "abs(1)";
            calculado = _parser.Eval();
            esperado = Math.Abs(1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "abs(-1)";
            calculado = _parser.Eval();
            esperado = Math.Abs(-1);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "abs(10.123)";
            calculado = _parser.Eval();
            esperado = Math.Abs(10.123);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "abs(-10.123)";
            calculado = _parser.Eval();
            esperado = Math.Abs(-10.123);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteRint()
        {
            // Biblioteca sempre arredonda para o inteiro mais próximo. Caso estejam equidistantes, arredonda para o maior número (Com sinal)

            _parser.Expr = "rint(0)";
            double calculado = _parser.Eval();
            double esperado = Math.Round(0.0, 0, MidpointRounding.AwayFromZero);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(0.5)";
            calculado = _parser.Eval();
            esperado = Math.Round(0.5, 0, MidpointRounding.AwayFromZero);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(1)";
            calculado = _parser.Eval();
            esperado = Math.Round(1.0, 0, MidpointRounding.AwayFromZero);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(0.96)";
            calculado = _parser.Eval();
            esperado = Math.Round(0.96, 0, MidpointRounding.AwayFromZero);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(0.123)";
            calculado = _parser.Eval();
            esperado = Math.Round(0.123, 0, MidpointRounding.AwayFromZero);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(-0.5)";
            calculado = _parser.Eval();
            esperado = Math.Round(-0.5);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(-1)";
            calculado = _parser.Eval();
            esperado = Math.Round(-1.0);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(-0.96)";
            calculado = _parser.Eval();
            esperado = Math.Round(-0.96);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "rint(-0.123)";
            calculado = _parser.Eval();
            esperado = Math.Round(-0.123);
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteIf()
        {
            _parser.Expr = "1 < 2 ? 10: 5";
            double calculado = _parser.Eval();
            double esperado = 10.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "1 > 2 ? 10: 5";
            calculado = _parser.Eval();
            esperado = 5.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteMinMax()
        {
            _parser.Expr = "min(-1, 2, 10)";
            double calculado = _parser.Eval();
            double esperado = -1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "min(-1)";
            calculado = _parser.Eval();
            esperado = -1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "min(6, 3, 5, 1, 7, 3)";
            calculado = _parser.Eval();
            esperado = 1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "max(-1, 2, 10)";
            calculado = _parser.Eval();
            esperado = 10.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "max(-1)";
            calculado = _parser.Eval();
            esperado = -1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "max(6, 3, 5, 1, 7, 3)";
            calculado = _parser.Eval();
            esperado = 7.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteSum()
        {
            _parser.Expr = "sum(-1, 2, 10)";
            var calculado = _parser.Eval();
            var esperado = 11.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sum(10)";
            calculado = _parser.Eval();
            esperado = 10.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "sum(10, 31, 3, 5, 2)";
            calculado = _parser.Eval();
            esperado = 51.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteAvg()
        {
            _parser.Expr = "avg(1, 2, 3)";
            var calculado = _parser.Eval();
            var esperado = 2.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "avg(1, 2)";
            calculado = _parser.Eval();
            esperado = 1.5;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "avg(1)";
            calculado = _parser.Eval();
            esperado = 1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }
    }
}
