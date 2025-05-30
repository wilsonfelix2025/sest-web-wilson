using System;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class ExpressionTests
    {
        private Parser _parser;
        private const double precisão = 0.0001;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("Teste").Parser;
        }

        [TestCase("a + b")]
        [TestCase("a + b ")]
        [TestCase("a + b  ")]
        public void ExpressãoDeveSerReconhecidaComEspaçoNoFinal(string arg)
        {
            _parser.Expr = arg;

            // verifica se a expressão foi setada acrescentando um espaço no final
            Check.That(_parser.Expr).IsEqualTo(arg + " ");
        }

        [TestCase("a + b")]
        [TestCase(" a + b")]
        [TestCase("  a + b")]
        public void EspaçosNoInícioDaExpressãoDevemSerReconhecidos(string arg)
        {
            _parser.Expr = arg;

            // verifica se a expressão foi setada mantendo os espaços iniciais
            Check.That(_parser.Expr).IsEqualTo(arg + " ");
        }

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void EvalEmExpressõesComVariáveisNãoDefinidasDeveLançarParserError()
        {
            _parser.Expr = "a + b";

            //Check.ThatCode(() => { _parser.Eval(); }).Throws<ParserError>();
            //Action act = () => _parser.Eval();
            //act.Should().Throw<Exception>()
            //    .WithMessage("Unexpected token \"a\" found at position 0.");

            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [TestCase("1 + 2",3.0)]
        [TestCase("2 * 3", 6.0)]
        [TestCase("6 / 2", 3.0)]
        public void EvalEmExpressõesApenasComOperaçõesNuméricasDeveRealizarOCálculo(string operação, double result)
        {
            _parser.Expr = operação;
            var res = _parser.Eval();

            Check.That(res).IsEqualTo(result);
        }

        [Test]
        public void PrecedênciaMatemáticaDasOperaçõesDeveSerRespeitada()
        {
            double x = 2.0;
            _parser.Expr = "x = x + 3 * 4 - (x + 3) * 4";

            _parser.DefineVar("x", x);
            var calculado = _parser.Eval();
            var esperado = -6.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void ParêntesesDevemEstabelecerPrecedênciaSobreAsOperações()
        {
            _parser.Expr = "sqrt((-((((2 - 1) * 3) + (2 / 3.) * 4) - 2 * 3 * (3 + 4) + 1 - 2 / 3)))";

            var calculado = _parser.Eval();
            var esperado = 6.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }
    }
}
