using System;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class OperadoresCustomizadosTests
    {
        private Parser _parser;
        private const double precisão = 0.0001;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("TestesOperadoresCustomizados").Parser;
        }
        

        [Test]
        public void TesteInfixOprtUnário()
        {
            _parser.DefineInfixOprt("^", _unaryInfixFunc);

            _parser.Expr = "^3";
            var calculado = _parser.Eval();
            var esperado = 8.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteDiffX2()
        {
            double x = 0.0;
            var varX = _parser.DefineVar("x", x);

            _parser.Expr = "x^2";
            varX.Value = 2;
            var calculado = _parser.Eval();
            var esperado = 4.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            varX.Value = 3;
            calculado = _parser.Eval();
            esperado = 9.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteDiffExp()
        {
            double x = 0.0;
            var varX = _parser.DefineVar("x", x);

            _parser.Expr = "exp(x)";
            varX.Value = 0.0;
            var calculado = _parser.Eval();
            var esperado = 1.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            varX.Value = 1;
            calculado = _parser.Eval();
            esperado = Math.E;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TestePostfixOprtUnário()
        {
            _parser.DefinePostfixOprt("#", _unaryPostfixFunc);

            _parser.Expr = "2#";
            var calculado = _parser.Eval();
            var esperado = 2.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "6#";
            calculado = _parser.Eval();
            esperado = 720.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteOperadorBinário()
        {
            _parser.DefineOprt("add", _binaryFunc);
            _parser.DefineOprt("mul", _binary2Func);

            _parser.Expr = "2 add 3";
            var calculado = _parser.Eval();
            var esperado = 5.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 mul 3";
            calculado = _parser.Eval();
            esperado = 6.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 mul 3 add 4";
            calculado = _parser.Eval();
            esperado = 10.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 mul (3 add 4)";
            calculado = _parser.Eval();
            esperado = 14.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 add 3 mul 4";
            calculado = _parser.Eval();
            esperado = 20.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 add (3 mul 4)";
            calculado = _parser.Eval();
            esperado = 14.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteOperadorBinárioComOperadorCadastradoPrefixado()
        {
            _parser.DefineOprt("add", _binaryFunc);
            _parser.DefineOprt("mul", _binary2Func);

            _parser.Expr = "5 + 2 add 3";
            var calculado = _parser.Eval();
            var esperado = 10.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 mul 3";
            calculado = _parser.Eval();
            esperado = 21.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 mul 3 add 4";
            calculado = _parser.Eval();
            esperado = 25.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 mul (3 add 4)";
            calculado = _parser.Eval();
            esperado = 49.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 add 3 mul 4";
            calculado = _parser.Eval();
            esperado = 40.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 add (3 mul 4)";
            calculado = _parser.Eval();
            esperado = 19.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteOperadorBinárioComOperadorCadastradoPosfixado()
        {
            _parser.DefineOprt("add", _binaryFunc);
            _parser.DefineOprt("mul", _binary2Func);

            _parser.Expr = "2 add 3 + 5";
            var calculado = _parser.Eval();
            var esperado = 10.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 mul 3 + 6";
            calculado = _parser.Eval();
            esperado = 18.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "(2 mul 3) + 6";
            calculado = _parser.Eval();
            esperado = 12.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 mul 3 add 4 + 10";
            calculado = _parser.Eval();
            esperado = 20.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 mul (3 add 4) + 5";
            calculado = _parser.Eval();
            esperado = 24.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 add 3 mul 4 + 5";
            calculado = _parser.Eval();
            esperado = 45.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "2 add (3 mul 4) + 5";
            calculado = _parser.Eval();
            esperado = 19.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteOperadorBinárioComOperadorCadastradoPosfixadoEOperadorCadastradoPrefixado()
        {
            _parser.DefineOprt("add", _binaryFunc);
            _parser.DefineOprt("mul", _binary2Func);

            _parser.Expr = "5 + 2 add 3 + 5";
            var calculado = _parser.Eval();
            var esperado = 15.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 mul 3 + 6";
            calculado = _parser.Eval();
            esperado = 63.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + (2 mul 3) + 6";
            calculado = _parser.Eval();
            esperado = 17.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 mul 3 add 4 + 10";
            calculado = _parser.Eval();
            esperado = 35.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 mul (3 add 4) + 5";
            calculado = _parser.Eval();
            esperado = 84.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 add 3 mul 4 + 5";
            calculado = _parser.Eval();
            esperado = 90.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();

            _parser.Expr = "5 + 2 add (3 mul 4) + 5";
            calculado = _parser.Eval();
            esperado = 24.0;
            Check.That(Math.Abs(calculado - esperado) < precisão).IsTrue();
        }

        [Test]
        public void TesteDefineInfixOprt()
        {
            _parser.Expr = "|2";

            // Verifica que antes não havia InfixOprts
            Check.That(_parser.InfixOprts.Count).IsEqualTo(0);

            // cria o operador '|'
            _parser.DefineInfixOprt("|", val =>
            {
                // eleva o número ao cubo
                return val * val * val;
            }, true);

            // Verifica que o InfixOprt definido está cadastrado
            Check.That(_parser.InfixOprts.Count).IsEqualTo(1);

            // executa e verifica o resultado
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(8.0);
        }

        [Test]
        public void TesteDefinePostfixOprt()
        {
            _parser.Expr = "2|";

            // Verifica que antes não havia PostfixOprts cadastrados
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(0);

            // cria o operador '|'
            _parser.DefinePostfixOprt("|", val =>
            {
                // eleva o número a quarta potência
                return val * val * val * val;
            }, true);

            // Verifica que o PostfixOprt definido está cadastrado
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(1);

            // executa e verifica o resultado
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(16.0);
        }

        [Test]
        public void TesteDefineDoisPostfixOprtsAninhadosComParênteses()
        {
            // Verifica que antes não havia PostfixOprts cadastrados
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(0);

            // cria o operador 'add'
            _parser.DefinePostfixOprt("add", val =>
            {
                return val + val;
            }, true);

            // cria o operador 'mul'
            _parser.DefinePostfixOprt("mul", val =>
            {
                return val * val;
            }, true);

            // Verifica que o PostfixOprt definido está cadastrado
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(2);

            // executa e verifica o resultado
            _parser.Expr = "(3add)mul";
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(36.0);

            _parser.Expr = "(3mul)add";
            res = _parser.Eval();
            Check.That(res).IsEqualTo(18.0);
        }

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void TesteDefineDoisPostfixOprtsAninhadosSemParênteses()
        {
            // Verifica que antes não havia PostfixOprts cadastrados
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(0);

            // cria o operador 'add'
            _parser.DefinePostfixOprt("add", val =>
            {
                return val + val;
            }, true);

            // cria o operador 'mul'
            _parser.DefinePostfixOprt("mul", val =>
            {
                return val * val;
            }, true);

            // Verifica que os PostfixOprts definidos estão cadastrados
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(2);

            // executa e verifica o resultado
            _parser.Expr = "3addmul";

            // verifica o disparo do _parserError
            //Check.ThatCode(() => { _parser.Eval(); }).Throws<ParserError>();
            //Action act = () => _parser.Eval();
            //act.Should().Throw<Exception>()
            //    .WithMessage("Unexpected token \"mul\" found at position 4.");

            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [Test]
        public void TesteDefineDoisInfixOprtsAninhadosComParênteses()
        {
            // Verifica que antes não havia InfixOprts
            Check.That(_parser.InfixOprts.Count).IsEqualTo(0);

            // cria o operador '+'
            _parser.DefineInfixOprt("+", val =>
            {
                return val + val;
            }, true);

            // cria o operador '*'
            _parser.DefineInfixOprt("*", val =>
            {
                return val * val;
            }, true);

            // Verifica que os InfixOprts definido estão cadastrados
            Check.That(_parser.InfixOprts.Count).IsEqualTo(2);

            // executa e verifica o resultado
            _parser.Expr = "+(*3)";
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(18.0);

            // executa e verifica o resultado
            _parser.Expr = "*(+3)";
            res = _parser.Eval();
            Check.That(res).IsEqualTo(36.0);
        }

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void TesteDefineDoisInfixOprtsAninhadosSemParênteses()
        {
            // Verifica que antes não havia InfixOprts
            Check.That(_parser.InfixOprts.Count).IsEqualTo(0);

            // cria o operador '+'
            _parser.DefineInfixOprt("+", val =>
            {
                return val + val;
            }, true);

            // cria o operador '*'
            _parser.DefineInfixOprt("*", val =>
            {
                return val * val;
            }, true);

            // Verifica que os InfixOprts definido estão cadastrados
            Check.That(_parser.InfixOprts.Count).IsEqualTo(2);

            // executa e verifica o resultado
            _parser.Expr = "+*3";

            // verifica o disparo do _parserError
            //Check.ThatCode(() => { _parser.Eval(); }).Throws<ParserError>();
            //Action act = () => _parser.Eval();
            //act.Should().Throw<Exception>()
            //    .WithMessage("Unexpected operator \"*\" found at position 2");

            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [Test]
        public void TesteDefineInfixAndPostfixOprtJuntosSemEstabelecerPrecedência()
        {
            // A precedência é do postfix

            // Infixs possíveis :  /+-*^?<>=#!$%&|~'_
            // Postfixs

            // cria o operador '*'
            _parser.DefineInfixOprt("*", val =>
            {
                return val * 2;
            }, true);

            // cria o operador '+'
            _parser.DefineInfixOprt("+", val =>
            {
                return val + 2;
            }, true);

            // cria o operador 'add'
            _parser.DefinePostfixOprt("add", val =>
            {
                return val + 3;
            }, true);

            // cria o operador 'mul'
            _parser.DefinePostfixOprt("mul", val =>
            {
                return val * 3;
            }, true);

            // executa e verifica o resultado
            _parser.Expr = "*2add";
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(10.0);

            // executa e verifica o resultado
            _parser.Expr = "+2mul";
            res = _parser.Eval();
            Check.That(res).IsEqualTo(8.0);
        }

        [Test]
        public void TesteDefineInfixAndPostfixOprtJuntosComPrecedênciaAoInfix()
        {
            // Infixs possíveis :  /+-*^?<>=#!$%&|~'_
            // Postfixs

            // cria o operador '*'
            _parser.DefineInfixOprt("*", val =>
            {
                return val * 2;
            }, true);

            // cria o operador '|'
            _parser.DefinePostfixOprt("add", val =>
            {
                return val + 3;
            }, true);

            _parser.Expr = "(*2)add";

            // executa e verifica o resultado
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(7.0);
        }

        [Test]
        public void TesteDefineInfixAndPostfixOprtJuntosComPrecedênciaAoPostfix()
        {
            // Infixs possíveis :  /+-*^?<>=#!$%&|~'_
            // Postfixs

            // cria o operador '*'
            _parser.DefineInfixOprt("*", val =>
            {
                // eleva o número ao cubo
                return val * 2;
            }, true);

            // cria o operador '|'
            _parser.DefinePostfixOprt("add", val =>
            {
                // eleva o número a quarta
                return val + 3;
            }, true);

            _parser.Expr = "*(2add)";

            // executa e verifica o resultado
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(10.0);
        }

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void TesteDefineInfixAndPostfixComMesmoOprt()
        {
            _parser.Expr = "|2|";

            // cria o operador 't'
            _parser.DefineInfixOprt("|", val =>
            {
                // eleva o número ao cubo
                return val * 2;
            }, true);

            // verifica o disparo do _parserError
            //Check.ThatCode(() =>
            //{  // cria o operador 't'
            //    _parser.DefinePostfixOprt("|", val =>
            //    {
            //        // eleva o número a quarta
            //        return val * 3;
            //    }, true);
            //}).Throws<ParserError>();

            //Action act = () => _parser.DefinePostfixOprt("|", val => { return val * 3;}, true);
            //act.Should().Throw<Exception>()
            //    .WithMessage("Name conflict");

            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [Test]
        public void TesteDefineOprt()
        {
            _parser.Expr = "2 | 5";

            // cria o operador '|'
            _parser.DefineOprt("|", (double val1, double val2) =>
            {
                // multiplica os números
                return val1 * val2;
            });

            var res = _parser.Eval();
            Check.That(res).IsEqualTo(10.0);
        }

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void DefiniçãoDeOperadorPosFixComMesmoNomeDeUmOperadorDisparaParserError()
        {
            _parser.Expr = "2|1|";

            // cria o operador '|'
            _parser.DefineOprt("|", (double val1, double val2) => val1 * val2);

            // verifica o disparo do ParserError
            //Check.ThatCode(() => { _parser.DefinePostfixOprt("|", val => val + val, true); }).Throws<ParserError>();

            //Action act = () => _parser.DefinePostfixOprt("|", val => val + val, true);
            //act.Should().Throw<Exception>()
            //    .WithMessage("Name conflict");
            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [Test]
        public void TesteOprtComPrecedênciaInfixOprtComMesmoIdentificador()
        {
            _parser.Expr = "|3|2";


            // Oprt tem precedência sobre InfixOprt, logo: InfixOprt só atua no primeiro |3 = 3+3. 

            // cria o operador '|'
            _parser.DefineOprt("|", (double val1, double val2) =>
            {
                // multiplica os números
                return val1 * val2;
            });

            // cria o operador Infix 't'
            _parser.DefineInfixOprt("|", val =>
            {
                // eleva o número ao cubo
                return val + val;
            }, true);

            var res = _parser.Eval();
            Check.That(res).IsEqualTo(12.0);
        }

        [Test]
        public void TesteClearInfixOprt()
        {
            // Verifica que antes não havia InfixOprts
            Check.That(_parser.InfixOprts.Count).IsEqualTo(0);

            // cria o operador '|'
            _parser.DefineInfixOprt("|", val =>
            {
                // eleva o número ao cubo
                return val * val * val;
            }, true);

            // Verifica que o InfixOprt definido está cadastrado
            Check.That(_parser.InfixOprts.Count).IsEqualTo(1);

            // Limpa o operador
            _parser.CleanInfixOprt();

            // Verifica que o operador foi limpo
            Check.That(_parser.InfixOprts.Count).IsEqualTo(0);
        }

        [Test]
        public void TesteClearPostOprt()
        {
            // Verifica que antes não havia InfixOprts
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(0);

            // cria o operador '|'
            _parser.DefinePostfixOprt("|", val =>
            {
                // eleva o número ao cubo
                return val * val * val;
            }, true);

            // Verifica que o InfixOprt definido está cadastrado
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(1);

            // Limpa o operador
            _parser.CleanPostfixOprt();

            // Verifica que o operador foi limpo
            Check.That(_parser.PostfixOprts.Count).IsEqualTo(0);
        }

        private static double _unaryInfixFunc(double arg)
        {
            return Math.Pow(2, arg);
        }

        private static double _unaryPostfixFunc(double arg)
        {
            double ret = 1.0;

            for (int i = 1; i <= arg; i++)
            {
                ret *= i;
            }

            return ret;
        }

        private static double _binaryFunc(double arg1, double arg2)
        {
            return arg1 + arg2;
        }

        private static double _binary2Func(double arg1, double arg2)
        {
            return arg1 * arg2;
        }
    }
}
