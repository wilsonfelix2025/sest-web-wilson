using System;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class VariáveisTests
    {
        private Parser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("TesteVariáveis").Parser;
        }

        private const double Precisão = 0.0001;

        [Test]
        [Ignore("Teste não passa no build-server")]
        public void VariávelNãoDefinidaDeveDispararParserError()
        {
            _parser.Expr = "a";

            //Check.ThatCode(() => { _parser.Eval(); }).Throws<ParserError>();

            Action act = () => _parser.Eval();
            //act.Should().Throw<Exception>()
            //    .WithMessage("Unexpected token \"a\" found at position 0.");
            Assert.Throws<ParserException>(() => _parser.Eval());
        }

        [Test]
        public void TesteDefineVar()
        {
            _parser.Expr = "a";

            // define a variável 'a'
            var a = _parser.DefineVar("a", 10.0);
            var aRecuperado = _parser.Vars["a"].Value;

            Check.That(aRecuperado).IsEqualTo(10);
            Check.That(_parser.Vars.ContainsKey("a")).IsTrue();

            // calcula
            var res = _parser.Eval();
            Check.That(res).IsEqualTo(10);

            // muda o valor e testa denovo
            a.Value = 50.0;

            res = _parser.Eval();
            Check.That(res).IsEqualTo(50);
        }

        [Test]
        public void TesteDefineVarUtilizandoUmaVariávelLocal()
        {
            double x = 2.5;

            // define a variável 'x'
            _parser.DefineVar("x", x);

            _parser.Expr = "sqrt(x*x)";
            var calculado = _parser.Eval();
            var esperado = x;
            Check.That(Math.Abs(calculado - esperado) < Precisão).IsTrue();

            x = 10.0;
            _parser.DefineVar("x", x);
            calculado = _parser.Eval();
            esperado = x;
            Check.That(Math.Abs(calculado - esperado) < Precisão).IsTrue();
        }

        [Test]
        public void TesteAtribuição()
        {
            double x = 0.0;

            _parser.DefineVar("x", x);
            _parser.Expr = "x = _pi";

            var calculado = _parser.Eval();
            var esperado = 3.1415926535897931;
            Check.That(Math.Abs(calculado - esperado) < Precisão).IsTrue();
        }

        [Test]
        public void TesteIdentificaçãoVariávelExponencialComPreviaAtribuiçãoDeValor()
        {
            _parser.DefineVar("e", 1.34e10);
            _parser.Expr = "e";
            var calculado = _parser.Eval();
            var esperado = 1.34e10;
            Check.That(Math.Abs(calculado - esperado) < Precisão).IsTrue();
        }

        [Test]
        public void TesteIdentificaçãoVariávelExponencialComAtribuiçãoDeValorNaExpressão()
        {
            _parser.DefineVar("e", 0);
            _parser.Expr = "e = 1.34e10";
            var calculado = _parser.Eval();
            var esperado = 1.34e10;
            Check.That(Math.Abs(calculado - esperado) < Precisão).IsTrue();
        }

        [Test]
        public void TesteCharSeparadorNaExpressão()
        {
            double x = 0.0;
            double y = 0.0;

            _parser.DefineVar("x", x);
            _parser.DefineVar("y", y);
            _parser.SetArgSep('#'); // Se não for setado, a ',' (vírgula), é o separador padrão.
            _parser.Expr = "x = _pi # y = _e";

            _parser.Eval();
            var esperado = _parser.Consts.First(c => c.Key == "_pi").Value;
            var calculado = _parser.Vars["x"].Value;
            Check.That(Math.Abs(calculado - esperado) < Precisão).IsTrue();

            esperado = _parser.Consts.First(c => c.Key == "_e").Value;
            calculado = _parser.Vars["y"].Value;
            Check.That(Math.Abs(calculado - esperado) < Precisão).IsTrue();
        }

        [Test]
        public void TesteAtribuiçãoArrayEval()
        {
            double[] x = { 0.0, 1.0, 2.0 };

            _parser.Expr = "x = _pi";

            for (int index = 0; index < x.Length; index++)
            {
                _parser.DefineVar("x", x[index]);
                x[index] = _parser.Eval();
            }

            double[] esperado = { 3.1415926535897931, 3.1415926535897931, 3.1415926535897931 };

            for (int i = 0; i < x.Length; i++)
            {
                Check.That(Math.Abs(x[i] - esperado[i]) < Precisão).IsTrue();
            }
        }


        [Test]
        public void TesteAtribuiçãoArrayEvalBulk()
        {
            double[] x = { 0.0, 1.0, 2.0 };

            _parser.DefineVar("x", x);
            _parser.Expr = "x = _pi";

            double[] calculado = _parser.EvalBulk(3);
            double[] esperado = { 3.1415926535897931, 3.1415926535897931, 3.1415926535897931 };
            double[] recuparado = _parser.Vars["x"].ValueArray;

            for (int i = 0; i < calculado.Length; i++)
            {
                Check.That(Math.Abs(calculado[i] - esperado[i]) < Precisão).IsTrue();
                Check.That(Math.Abs(x[i] - esperado[i]) < Precisão).IsTrue();
                Check.That(Math.Abs(recuparado[i] - esperado[i]) < Precisão).IsTrue();
            }
        }

        [Test]
        public void TesteAtribuiçãoEAdiçãoEvalBulk()
        {
            _parser.Expr = "c = 1, a + b + c";

            // cria as variáveis
            double[] a = { 1, 2, 3, 4, 5 };
            double[] b = { 5, 4, 3, 2, 1 };
            double[] c = { 0, 0, 0, 0, 0 };

            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("c", c);

            // faz o cálculo
            var calculado = _parser.EvalBulk(5);

            // Verifica a atribuição
            double[] cRecuparado = _parser.Vars["c"].ValueArray;
            double[] cEsperado = { 1, 1, 1, 1, 1 };
            CollectionAssert.AreEqual(cEsperado, cRecuparado);

            // verifica se calculou corretamente
            Assert.AreEqual(5, calculado.Length);
            double[] esperado = { 7, 7, 7, 7, 7 };
            CollectionAssert.AreEqual(esperado, calculado);
        }

        [Test]
        public void TesteAtribuiçãoEAdiçãoEvalBulkComVarFactory()
        {
            // Nesse modo, não é necessário DefineVar para variáveis com atribuição já feita na expressão.
            // Porém na recuperação dessa variável, ela é unidimensional
            // Mas se comporta como se fosse multidimensional. Mesma dimensão das outras variáveis.

            _parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            _parser.Expr = "c = 1, a = a + 1, b = b + 2, a + b + c";

            // cria as variáveis
            double[] a = { 1, 2, 3, 4, 5 };
            double[] b = { 5, 4, 3, 2, 1 };

            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);

            // faz o cálculo
            var calculado = _parser.EvalBulk(5);

            // Verifica a atribuição
            double[] cRecuparado = _parser.Vars["c"].ValueArray;
            double[] cEsperado = { 1 };
            CollectionAssert.AreEqual(cEsperado, cRecuparado);

            // Verifica a atribuição
            double[] aRecuparado = _parser.Vars["a"].ValueArray;
            double[] aEsperado = { 2, 3, 4, 5, 6 };
            CollectionAssert.AreEqual(aEsperado, aRecuparado);
            CollectionAssert.AreEqual(aEsperado, a);

            // Verifica a atribuição
            double[] bRecuparado = _parser.Vars["b"].ValueArray;
            double[] bEsperado = { 7, 6, 5, 4, 3 };
            CollectionAssert.AreEqual(bEsperado, bRecuparado);
            CollectionAssert.AreEqual(bEsperado, b);

            // verifica se calculou corretamente
            Assert.AreEqual(5, calculado.Length);
            double[] esperado = { 10, 10, 10, 10, 10 };
            CollectionAssert.AreEqual(esperado, calculado);
        }

        [Test]
        public void TesteAtribuiçãoEAdiçãoEvalBulkComVarFactoryEComVariáveisDependentes()
        {
            // Nesse modo, não é necessário DefineVar para variáveis com atribuição já feita na expressão.
            // Porém na recuperação dessa variável, ela é unidimensional
            // Mas se comporta como se fosse multidimensional. Mesma dimensão das outras variáveis.
            // d é variável dependente e precisa ser explicitamente definida.

            _parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            _parser.Expr = "c = 1, a = a + 1, b = b + 2, d = a + b, e = d + c";

            // cria as variáveis
            double[] a = { 1, 2, 3, 4, 5 };
            double[] b = { 5, 4, 3, 2, 1 };
            double[] d = { 0, 0, 0, 0, 0 };
            double[] e = { 0, 0, 0, 0, 0 };

            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("d", d);
            _parser.DefineVar("e", e);

            // faz o cálculo
            var calculado = _parser.EvalBulk(5);

            // Verifica a atribuição
            double[] cRecuparado = _parser.Vars["c"].ValueArray;
            double[] cEsperado = { 1 };
            CollectionAssert.AreEqual(cEsperado, cRecuparado);

            // Verifica a atribuição
            double[] aRecuparado = _parser.Vars["a"].ValueArray;
            double[] aEsperado = { 2, 3, 4, 5, 6 };
            CollectionAssert.AreEqual(aEsperado, aRecuparado);
            CollectionAssert.AreEqual(aEsperado, a);

            // Verifica a atribuição
            double[] bRecuparado = _parser.Vars["b"].ValueArray;
            double[] bEsperado = { 7, 6, 5, 4, 3 };
            CollectionAssert.AreEqual(bEsperado, bRecuparado);
            CollectionAssert.AreEqual(bEsperado, b);

            // Verifica a atribuição
            double[] dRecuparado = _parser.Vars["d"].ValueArray;
            double[] dEsperado = { 9, 9, 9, 9, 9 };
            CollectionAssert.AreEqual(dEsperado, dRecuparado);

            // Verifica a atribuição
            double[] eRecuparado = _parser.Vars["e"].ValueArray;
            double[] eEsperado = { 10, 10, 10, 10, 10 };
            CollectionAssert.AreEqual(eEsperado, eRecuparado);
        }

        [Test]
        public void TesteAtribuiçãoEAdiçãoEvalComVarFactoryEComVariáveisDependentes()
        {
            _parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            _parser.Expr = "c = 1, a = a + 1, b = b + 2, d = a + b, e = d + c";

            // cria as variáveis
            double[] a = { 1, 2, 3, 4, 5 };
            double[] b = { 5, 4, 3, 2, 1 };
            double[] d = { 0, 0, 0, 0, 0 };
            double[] e = { 0, 0, 0, 0, 0 };

            for (int i = 0; i < 5; i++)
            {
                _parser.DefineVar("a", a[i]);
                _parser.DefineVar("b", b[i]);
                _parser.DefineVar("d", d[i]);
                _parser.DefineVar("e", e[i]);

                _parser.Eval();

                a[i] = _parser.Vars["a"].Value;
                b[i] = _parser.Vars["b"].Value;
                d[i] = _parser.Vars["d"].Value;
                e[i] = _parser.Vars["e"].Value;
            }

            // Verifica a atribuição
            double[] cRecuparado = _parser.Vars["c"].ValueArray;
            double[] cEsperado = { 1 };
            CollectionAssert.AreEqual(cEsperado, cRecuparado);

            // Verifica a atribuição
            double[] aEsperado = { 2, 3, 4, 5, 6 };
            CollectionAssert.AreEqual(aEsperado, a);

            // Verifica a atribuição
            double[] bEsperado = { 7, 6, 5, 4, 3 };
            CollectionAssert.AreEqual(bEsperado, b);

            // Verifica a atribuição
            double[] dEsperado = { 9, 9, 9, 9, 9 };
            CollectionAssert.AreEqual(dEsperado, d);

            // Verifica a atribuição
            double[] eEsperado = { 10, 10, 10, 10, 10 };
            CollectionAssert.AreEqual(eEsperado, e);
        }

        [Test]
        public void TesteAdiçãoERemoçãoDeVariáveis()
        {
            double x = 2.0, y = 3.0;

            Check.That(_parser.Vars.Count == 0).IsTrue();
            _parser.DefineVar("x", x);
            Check.That(_parser.Vars.Count == 1).IsTrue();
            _parser.DefineVar("y", y);
            Check.That(_parser.Vars.Count == 2).IsTrue();

            foreach (var keyValuePair in _parser.Vars)
            {
                switch (keyValuePair.Key)
                {
                    case "x":
                        Check.That(keyValuePair.Value.Value).IsEqualTo(x);
                        break;
                    case "y":
                        Check.That(keyValuePair.Value.Value).IsEqualTo(y);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }

            _parser.RemoveVar("x");
            Check.That(_parser.Vars.Count == 1).IsTrue();
            Check.That(_parser.Vars.ContainsKey("x")).IsFalse();
            Check.That(_parser.Vars.ContainsKey("y")).IsTrue();
        }

        [Test]
        public void TesteSetVarFactoryComUmaVariável()
        {
            _parser.Expr = "a = 5, a = a * 2 + a^2"; // 10 + 25 = 35

            // adiciona a função de factory
            _parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            // executa
            _parser.Eval();

            // pega a variável 'a'
            var a = _parser.Vars["a"];

            // verifica o valor dela
            Check.That(a.Value).IsEqualTo(35.0);
        }

        [Test]
        public void TesteSetVarFactoryComVáriasVariáveis()
        {
            _parser.Expr = "a = 5, b= 3, a = a * 2, b = b * b, c = (a * b) + 3";

            // adiciona a função de factory
            _parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            // executa
            _parser.Eval();

            // pega a variável 'c'
            var c = _parser.Vars["c"];
            // verifica o valor 
            Check.That(c.Value).IsEqualTo(93.0);

            // pega a variável 'a'
            var a = _parser.Vars["a"];
            // verifica o valor 
            Check.That(a.Value).IsEqualTo(10.0);

            // pega a variável 'b'
            var b = _parser.Vars["b"];
            // verifica o valor 
            Check.That(b.Value).IsEqualTo(9.0);
        }
    }
}
