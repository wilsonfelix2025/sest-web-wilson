using System.Runtime.InteropServices;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class OperaçõesSequenciaisTests
    {
        private Parser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("TestesOperaçõesSequenciais").Parser;
        }

        [Test]
        public void AÚltimaOperaçãoDeveUsarOsValoresCalculadosNasOperaçõesAnteriores()
        {
            double[] a = { 0, 1, 2 };
            double[] b = { 1, 5, 10 };
            double[] c = { 1, 2, 3 };
            double[] d = { 2, 6, 11 };
            double[] y = { 0, 0, 0 };
            double[] z = { 0, 0, 0 };
            double[] x = { 0, 0, 0 };

            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("c", c);
            _parser.DefineVar("d", d);
            _parser.DefineVar("x", x);
            _parser.DefineVar("y", y);
            _parser.DefineVar("z", z);

            // y e z são cálculados e depois x é cálculado usando os dados dos cálculos anteriores

            _parser.Expr = @"   y = a + b,                              
                               z = c + d, 
                               x = z-y";
            _parser.EvalBulk(3);

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            double expectedValue = 2;

            //if (!isWindows)
            //    expectedValue = 0;


            Check.That(x[0]).IsEqualTo(expectedValue);
            Check.That(x[1]).IsEqualTo(expectedValue);
            Check.That(x[2]).IsEqualTo(expectedValue);
        }

        [Test]
        public void OperaçõesSequenciaisUsandoSetVarFactoryComVáriasVariáveisSendoCadaUmaDependenteDasAnteriores()
        {
            _parser.Expr = "var1 = 2, var2 = 5, dtc = 3, dts = var1 * (dtc ^ 2), rhob = (dtc * dts) + var2";

            // adiciona a função de factory
            _parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            // executa
            _parser.Eval();

            // pega a variável 'const1'
            var var1 = _parser.Vars["var1"];
            // verifica o valor 
            Check.That(var1.Value).IsEqualTo(2.0);

            // pega a variável 'const2'
            var var2 = _parser.Vars["var2"];
            // verifica o valor 
            Check.That(var2.Value).IsEqualTo(5.0);

            // pega a variável 'dtc'
            var dtc = _parser.Vars["dtc"];
            // verifica o valor
            Check.That(dtc.Value).IsEqualTo(3.0);

            // pega a variável 'dts'
            var dts = _parser.Vars["dts"];
            // verifica o valor 
            Check.That(dts.Value).IsEqualTo(18.0);

            // pega a variável 'rhob'
            var rhob = _parser.Vars["rhob"];
            // verifica o valor 
            Check.That(rhob.Value).IsEqualTo(59.0);
        }

        [Test]
        public void TesteSetVarFactoryComÁrvoreDeDependências()
        {
            var raiz = "DTC = 2";
            var separador = ",";
            var primeiroNível = "DTS = DTC ^ 2, RHOB = DTC ^ 3";
            var segundoNível = "GRAY = DTS * 2, CALIP = DTS * 3, REST = RHOB + 2, BIOT = RHOB + 3";
            var terceiroNível =
                "GPORO = GRAY + 1, GFRAT = GRAY + 2, GCOLS = CALIP + 1, GCOLI = CALIP + 2, GSOBR = REST + 1, GECD = REST + 2, GTHORM = BIOT + 1, GTHORm = BIOT + 2";

            _parser.Expr = raiz + separador + primeiroNível + separador + segundoNível + separador + terceiroNível;

            // adiciona a função de factory
            _parser.SetVarFactory((string name, object userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);

            // executa
            _parser.Eval();

            // raiz
            // pega a variável 'DTC'
            var dtc = _parser.Vars["DTC"];
            Check.That(dtc.Value).IsEqualTo(2.0);

            // Primeiro nível

            // pega a variável 'DTS'
            var dts = _parser.Vars["DTS"];
            Check.That(dts.Value).IsEqualTo(4.0);

            // pega a variável 'RHOB'
            var rhob = _parser.Vars["RHOB"];
            Check.That(rhob.Value).IsEqualTo(8.0);

            // Segundo nível

            // pega a variável 'GRAY'
            var gray = _parser.Vars["GRAY"];
            Check.That(gray.Value).IsEqualTo(8.0);

            // pega a variável 'CALIP'
            var calip = _parser.Vars["CALIP"];
            Check.That(calip.Value).IsEqualTo(12.0);

            // pega a variável 'REST'
            var rest = _parser.Vars["REST"];
            Check.That(rest.Value).IsEqualTo(10.0); 

            // pega a variável 'BIOT'
            var biot = _parser.Vars["BIOT"];
            Check.That(biot.Value).IsEqualTo(11.0);

            // Terceiro nível

            // pega a variável 'GPORO'
            var gporo = _parser.Vars["GPORO"];
            Check.That(gporo.Value).IsEqualTo(9.0);

            // pega a variável 'GFRAT'
            var gfrat = _parser.Vars["GFRAT"];
            Check.That(gfrat.Value).IsEqualTo(10.0);

            // pega a variável 'GCOLS'
            var gcols = _parser.Vars["GCOLS"];
            Check.That(gcols.Value).IsEqualTo(13.0);

            // pega a variável 'GCOLI'
            var gcoli = _parser.Vars["GCOLI"];
            Check.That(gcoli.Value).IsEqualTo(14.0);

            // pega a variável 'GSOBR'
            var gsobr = _parser.Vars["GSOBR"];
            Check.That(gsobr.Value).IsEqualTo(11.0);

            // pega a variável 'GECD'
            var gecd = _parser.Vars["GECD"];
            Check.That(gecd.Value).IsEqualTo(12.0);

            // pega a variável 'GTHORM'
            var gthorm = _parser.Vars["GTHORM"];
            Check.That(gthorm.Value).IsEqualTo(12.0);

            // pega a variável 'GTHORm'
            var gthoRm = _parser.Vars["GTHORm"];
            Check.That(gthoRm.Value).IsEqualTo(13.0);
        }
    }
}
