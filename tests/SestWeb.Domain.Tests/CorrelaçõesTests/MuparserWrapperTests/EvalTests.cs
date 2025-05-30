using System;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class EvalTests
    {
        private Parser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("Teste").Parser;
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

        [TestCase("1 + 2", 3.0)]
        [TestCase("2 * 3", 6.0)]
        [TestCase("6 / 2", 3.0)]
        public void EvalEmExpressõesApenasComOperaçõesNuméricasDeveRealizarOCálculo(string operação, double result)
        {
            _parser.Expr = operação;
            var res = _parser.Eval();

            Check.That(res).IsEqualTo(result);
        }

        [TestCase("1 + 2","2 + 3", 3.0, 5.0)]
        [TestCase("1 * 2", "2 * 3", 2.0, 6.0)]
        [TestCase("1 / 2", "3 / 2", 0.5, 1.5)]
        public void EvalMultiExecutaDuasEquaçõesSimultaneamente(string equação1, string equação2, double result1, double result2)
        {
            _parser.Expr = $"{equação1}, {equação2}";

            var res = _parser.EvalMulti();

            // verifica se a quantidade de resultados está correta
            Check.That(res.Length).IsEqualTo(2);

            // verifica se o resultado está correto
            Check.That(res[0]).IsEqualTo(result1);
            Check.That(res[1]).IsEqualTo(result2);
        }

        [TestCase("1 + 2", "2 + 3", "3 + 4", "4 + 5", "5 + 6", "6 + 7", "7 + 8", "8 + 9", "9 + 10", 3,5,7,9,11,13,15,17,19)]
        public void EvalMultiExecutaNEquaçõesSimultaneamente(string equação1, string equação2, string equação3,
            string equação4, string equação5, string equação6, string equação7, string equação8, string equação9,
            double result1, double result2, double result3, double result4 , double result5, double result6,
            double result7, double result8, double result9)
        {
            _parser.Expr = $"{equação1}, {equação2}, {equação3}, {equação4}, {equação5}, {equação6}, {equação7}, {equação8}, {equação9}";

            var res = _parser.EvalMulti();

            // verifica se a quantidade de resultados está correta
            Check.That(res.Length).IsEqualTo(9);

            // verifica se o resultado está correto
            Check.That(res[0]).IsEqualTo(result1);
            Check.That(res[1]).IsEqualTo(result2);
            Check.That(res[2]).IsEqualTo(result3);
            Check.That(res[3]).IsEqualTo(result4);
            Check.That(res[4]).IsEqualTo(result5);
            Check.That(res[5]).IsEqualTo(result6);
            Check.That(res[6]).IsEqualTo(result7);
            Check.That(res[7]).IsEqualTo(result8);
            Check.That(res[8]).IsEqualTo(result9);
        }

        [Test]
        public void EvalBulkDeveCalcularVetorialmente()
        {
            _parser.Expr = "c = a + b";

            // cria as variáveis
            double[] a = { 1, 2, 3, 4, 5 };
            double[] b = { 5, 4, 3, 2, 1 };
            double[] c = { 0, 0, 0, 0, 0 };

            // define as variáveis
            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("c", c);

            // faz o cálculo
            var res = _parser.EvalBulk(5);

            // verifica se calculou corretamente
            Check.That(res.Length).IsEqualTo(c.Length);

            foreach (var result in res)
            {
                Check.That(result).IsEqualTo(6);
            }
        }

        [Test]
        public void EvalBulkDeveCalcularCorretamenteGrandesVetores()
        {
            int tamanho = 100000;
            _parser.Expr = "c = a + b";

            // cria as variáveis
            double[] a = new double[tamanho];
            double[] b = new double[tamanho];
            double[] c = new double[tamanho];

            for (int index = 0; index < tamanho; index++)
            {
                a[index] = index;
                b[index] = tamanho - 1 - index;
            }

            // define as variáveis
            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("c", c);

            // faz o cálculo
            var res = _parser.EvalBulk(tamanho);

            // verifica se calculou corretamente
            Check.That(res.Length).IsEqualTo(c.Length);

            for (int index = 0; index < tamanho; index++)
            {
                Check.That(res[index]).IsEqualTo(tamanho-1);
            }
        }

        [Test]
        public void EvalBulkDeveSuportarCriaçãoDeVariáveisDeclaradasNaEquação()
        {
            var muParserNetWrapper = new MuParserNetWrapper("Teste");
            var parser = muParserNetWrapper.Parser;

            parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            parser.Expr = "c = 1, a + b + c";

            // cria as variáveis
            double[] a = { 2, 3, 3, 4, 5 };
            double[] b = { 5, 4, 3, 2, 1 };

            parser.DefineVar("a", a);
            parser.DefineVar("b", b);

            // faz o cálculo
            var res = parser.EvalBulk(2);

            // verifica se calculou corretamente
            Check.That(res.Length).IsEqualTo(2);


            // Resultados diferentes no LINUX 
            double expectedValue = 8;

            //if (IsLinux)
            //{
            //    expectedValue = 7;
            //}

            // C deve ser inteiro 8. 1 vem do c, e 7 de a + b
            foreach (var result in res)
            {
                Check.That(result).IsEqualTo(expectedValue);
            }
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                bool éLinux = (p == 4) || (p == 6) || (p == 128);

                if(éLinux)
                    Console.WriteLine("*** Linux detectado ***");
                else
                {
                    Console.WriteLine("*** Windows detectado ***");
                }
                return éLinux;
            }
        }
    }
}
