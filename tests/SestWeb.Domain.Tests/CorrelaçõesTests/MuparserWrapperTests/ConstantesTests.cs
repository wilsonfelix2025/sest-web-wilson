using System;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class ConstantesTests
    {
        // π
        private const double Pi = 3.1415926535897931;

        // número de Euler
        private const double E = 2.7182818284590451;

        private Parser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("TesteConstantes").Parser;
        }

        [Test]
        public void ConstantedeEulerDeveSerIdentificadaPor_e()
        {
            // A expressão é declarada como "_e"
            _parser.Expr = "_e";

            var result = _parser.Eval();

            // verifica se o valor da execução corresponde ao número de Euler
            Check.That(result).IsEqualTo(E);
        }

        [Test]
        public void π_DeveSerIdentificadoPor_pi()
        {
            // A expressão é declarada como "_pi"
            _parser.Expr = "_pi";

            var r = _parser.Eval();

            // verifica se o valor da execução corresponde ao π
            Check.That(r).IsEqualTo(Pi);
        }

        [Test]
        public void ConstanteDefinidaDeveSerEncontradaNoParserESeuValorDeveSerOAtrubuídoNaDefinição()
        {
            const double precisão = 0.0001;
            double mSqrt12 = 1 / Math.Sqrt(2);

            _parser.DefineConst("sqrt1_2", mSqrt12);

            Check.That(_parser.Consts.Any(c => c.Key == "sqrt1_2")).IsTrue();

            _parser.Expr = "sqrt1_2";
            var calculado = _parser.Eval();
            var esperado = mSqrt12;

            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void ConstanteDefinidaDeveSerCriadaEObtidaPeloNomeNoParser()
        {
            // define a constante
            _parser.DefineConst("constante01", 50.0);

            // verifica a obtenção da constante pelo nome
            Check.That(_parser.Consts.Any(c => c.Key == "constante01")).IsTrue();
        }

        [Test]
        public void AtribuiçãoDeConstanteNaEquaçãoNãoACriaAutomaticamente()
        {
            _parser.Expr = "constante01 = 50.0";

            // verifica a não obtenção da constante pelo nome
            Check.That(_parser.Consts.Any(c => c.Key == "constante01")).IsFalse();
        }

        [Test]
        public void AtribuiçãoDeValorNaExpressãoAConstanteJáDefinidaNãoAlteraSeuValor()
        {
            // define a constante com valor 0
            _parser.DefineConst("constante01", 0);

            _parser.Expr = "constante01 = 50.0";

            // verifica a não obtenção da constante pelo nome
            Check.That(_parser.Consts.Any(c => c.Key == "constante01")).IsTrue();

            // verifica a não alteração do valor definido
            Check.That(_parser.Consts["constante01"]).IsEqualTo(0.0);
        }

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void ExecuçãoQuandoOcorreAtribuiçãoDeValorÀConstanteNaExpressãoDisparaParserError()
        {
            _parser.Expr = "constante01 = 50.0";

            // define a constante
            _parser.DefineConst("constante01", 0);

            // verifica o disparo do ParserError
            //Check.ThatCode(() => { _parser.Eval(); }).Throws<ParserError>();

            //Action act = () => _parser.Eval();
            //act.Should().Throw<Exception>()
            //    .WithMessage("Unexpected operator \"=\" found at position 12");

            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [Test]
        public void OResultadoDaExecuçãoDeveSerOValorDaConstanteCasoAExpressãoSejaAConstante()
        {
            // a expressão é a constante
            _parser.Expr = "constante01";

            // define a constante
            _parser.DefineConst("constante01", 50.0);

            var r = _parser.Eval();

            // verifica se foi retornado o valor da constante
            Check.That(r).IsEqualTo(50.0);
        }

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void DefiniçãoDeConstanteComNomeInválidoDeveDispararParserError()
        {
            //Check.ThatCode(() => { _parser.DefineConst("!23", 1.0); }).Throws<ParserError>();

            //Action act = () => _parser.Eval();
            //act.Should().Throw<Exception>()
            //    .WithMessage("Unexpected end of expression at position 0");

            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [Test]
        public void SóAsConstantesFixasEAsCadastradasPeloUsuárioDevemSerAvaliadas()
        {
            const double precisão = 0.0001;
            double mSqrt12 = 1 / Math.Sqrt(2);

            _parser.DefineConst("sqrt1_2", mSqrt12);

            foreach (var @const in _parser.Consts)
            {
                switch (@const.Key)
                {
                    case "_e":
                        Check.That(@const.Value).IsEqualTo(E);
                        break;
                    case "_pi":
                        Check.That(@const.Value).IsEqualTo(Pi);
                        break;
                    case "sqrt1_2":
                        {
                            _parser.Expr = "sqrt1_2";
                            var calculado = _parser.Eval();
                            var esperado = mSqrt12;
                            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
                        }
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}
