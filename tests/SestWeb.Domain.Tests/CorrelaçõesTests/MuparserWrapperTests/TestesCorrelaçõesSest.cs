using System;
using System.Runtime.InteropServices;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    class TestesCorrelaçõesSest
    {
        private Parser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("TestesValidaçõesCustomizadas").Parser;
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOBZerado()
        {
            var exp = @"
                          (RHOB == 0) ?
                               RHOB = 1
                           : RHOB = 2";

            _parser.Expr = exp;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            //double expectedValue = 0;

            //if (isWindows)
               double expectedValue = 1;

            // cria as variáveis
            double[] rhob = { 0, 0, 0, 0, 0 };
            double[] rhobEsperado = { expectedValue, expectedValue, expectedValue, expectedValue, expectedValue };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOBPreenchido()
        {
            var exp = @"
                          (RHOB == 0) ?
                               RHOB = 1
                           : RHOB = 2";

            _parser.Expr = exp;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            //double expectedValue = 10;

            //if (isWindows)
              double  expectedValue = 2;

            // cria as variáveis
            double[] rhob = { 10, 10, 10, 10, 10 };
            double[] rhobEsperado = { expectedValue, expectedValue, expectedValue, expectedValue, expectedValue };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOBParcialmentePreenchido()
        {
            var exp = @"
                          (RHOB == 0) ?
                               RHOB = 1
                           : RHOB = 2";

            _parser.Expr = exp;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            //double expectedValue = 0;

            //if (isWindows)
                var expectedValue = 1;

            // cria as variáveis
            double[] rhob = { 10, 0, 10, 0, 10 };
            double[] rhobEsperado = { 2, expectedValue, 2, expectedValue, 2 };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOBZeradoComTruncamentoInferior()
        {
            var exp = @"
                          (RHOB == 0) ?
                                RHOB = 1
                            : RHOB = 2,
                          (RHOB < 1.8) ?
                             RHOB = 1.8
                            :0";
            _parser.Expr = exp;

            // cria as variáveis
            double[] rhob = { 0, 0, 0, 0, 0 };
            double[] rhobEsperado = { 1.8, 1.8, 1.8, 1.8, 1.8 };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOBPreenchidoComTruncamentoSuperior()
        {
            var exp = @"
                          (RHOB == 0) ?
                                RHOB = 1
                            : RHOB = 2,
                          (RHOB > 1.8) ?
                             RHOB = 1.8
                            :0";
            _parser.Expr = exp;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            //double expectedValue = 5;

            //if (isWindows)
               var expectedValue = 1.8;

            // cria as variáveis
            double[] rhob = { 5, 5, 5, 5, 5 };
            double[] rhobEsperado = { expectedValue, expectedValue, expectedValue, expectedValue, expectedValue };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOParcialmentePreenchidoBComTruncamentoInferiorESuperior()
        {
            var exp = @"
                          (RHOB == 0) ?
                                RHOB = 1
                            : RHOB = 2,
                          (RHOB < 1.8) ?
                             RHOB = 1.8
                            :0,
                          (RHOB > 1.9) ?
                             RHOB = 1.9
                            :0";

            _parser.Expr = exp;

            // cria as variáveis
            double[] rhob = { 0, 5, 0, 5, 0 };
            double[] rhobEsperado = { 1.8, 1.9, 1.8, 1.9, 1.8 };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOParcialmentePreenchidoBComTruncamentoInferiorESuperiorAninhados()
        {
            var exp = @"
                          (RHOB == 0) ?
                                RHOB = 1
                            : RHOB = 2,
                          (RHOB < 1.8) ?
                             RHOB = 1.8
                            : ((RHOB > 1.9) ?
                                 RHOB = 1.9
                                 :0)";

            _parser.Expr = exp;

            // cria as variáveis
            double[] rhob = { 0, 5, 0, 5, 0 };
            double[] rhobEsperado = { 1.8, 1.9, 1.8, 1.9, 1.8 };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOParcialmentePreenchidoBComTruncamentoInferiorESuperiorResultadoDentroDosLimitesVálidos()
        {
            var exp = @"
                          (RHOB == 0) ?
                                RHOB = 1.81
                            : RHOB = 1.88,
                          (RHOB < 1.8) ?
                             RHOB = 1.8
                            :0,
                          (RHOB > 1.9) ?
                             RHOB = 1.9
                            :0";

            _parser.Expr = exp;

            // cria as variáveis
            double[] rhob = { 0, 5, 0, 5, 0 };
            double[] rhobEsperado = { 1.81, 1.88, 1.81, 1.88, 1.81 };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteDeSeleçãoDeEntradaDoRHOBcomRHOParcialmentePreenchidoBComTruncamentoInferiorESuperiorAninhadosResultadoDentroDosLimitesVálidos()
        {
            var exp = @"
                          (RHOB == 0) ?
                                RHOB = 1.81
                            : RHOB = 1.88,
                          (RHOB < 1.8) ?
                             RHOB = 1.8
                            : ((RHOB > 1.9) ?
                                 RHOB = 1.9
                                 :0)";

            _parser.Expr = exp;

            // cria as variáveis
            double[] rhob = { 0, 5, 0, 5, 0 };
            double[] rhobEsperado = { 1.81, 1.88, 1.81, 1.88, 1.81 };

            _parser.DefineVar("RHOB", rhob);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void Teste_RHOB_GARDNER_ComRHOBSendoCalculadoEmTodosOsPontos()
        {
            var correlaçãoRhob = @"
                                    (RHOB == 0) ?
                                        RHOB = a * (((10 ^ 6) / DTC) ^ b)
                                      : 0";
            _parser.Expr = correlaçãoRhob;

            // cria as variáveis
            double[] a = { 0.23, 0.23, 0.23, 0.23, 0.23 };
            double[] b = { 0.25, 0.25, 0.25, 0.25, 0.25 };
            double[] rhob = { 0, 0, 0, 0, 0 };
            double[] dtc = { 134.528068, 134.522518, 134.516967, 134.511415, 134.505861 };
            double[] rhobEsperado = { 2.1356231, 2.135645127, 2.135667159, 2.135689196, 2.135711242 };

            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("DTC", dtc);
            _parser.DefineVar("RHOB", rhob);

            // faz o cálculo
            var rhob_calculado = _parser.EvalBulk(5);

            for (int index = 0; index < rhob_calculado.Length; index++)
            {
                var esperado = rhobEsperado[index];
                var calculado = rhob_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }
        }

        [Test]
        public void Teste_RHOB_GARDNER_CalculandoTodosPontosComTruncamentoSuperiorEInferior()
        {
            var correlaçãoRhob = @"
                                    (RHOB == 0) ?
                                        RHOB = a * (((10 ^ 6) / DTC) ^ b)
                                      : 0,
                                    (RHOB < 1.8) ?
                                        RHOB = 1.8
                                      : ((RHOB > 2.9) ?
                                            RHOB = 2.9
                                          : 0)";
            _parser.Expr = correlaçãoRhob;

            // cria as variáveis
            double[] a = { 0.23, 0.23, 0.23, 0.23, 0.23 };
            double[] b = { 0.25, 0.25, 0.25, 0.25, 0.25 };
            double[] rhob = { 0, 0, 0, 0, 0 };
            double[] dtc = { 134.528068, 134.522518, 134.516967, 134.511415, 134.505861 };
            double[] rhobEsperado = { 2.1356231, 2.135645127, 2.135667159, 2.135689196, 2.135711242 };

            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("DTC", dtc);
            _parser.DefineVar("RHOB", rhob);

            // faz o cálculo
            var rhob_calculado = _parser.EvalBulk(5);

            for (int index = 0; index < rhob_calculado.Length; index++)
            {
                var esperado = rhobEsperado[index];
                var calculado = rhob[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }
        }

        [Test]
        public void TesteDTSCujasProfundidadesSãoZeradasDeveSerCalculadoNessasProfundidades()
        {
            var expressão = @" 
 (DTS == 0) ?
     DTS = 100 : 0,
 (DTS > 500.0) ?
     DTS = 500.0 : 0,
 (DTS < 40.0) ?
     DTS = 40.0 : 0";
            _parser.Expr = expressão;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            //double expectedValue = 40;

            //if (isWindows)
                var expectedValue = 100;

            // cria as variáveis
            double[] dts = { 0, 0, 0, 0, 0 };
            double[] dtsEsperado = { expectedValue, expectedValue, expectedValue, expectedValue, expectedValue };

            _parser.DefineVar("DTS", dts);
            _parser.EvalBulk(5);

            for (int index = 0; index < dtsEsperado.Length; index++)
            {
                var dts_esperado = dtsEsperado[index];
                Check.That(dts[index]).IsEqualTo(dts_esperado);
            }
        }

        [Test]
        public void TesteDTSCujasProfundidadesSãoLidasDevemSerMantidas()
        {
            var expressão = @" 
 (DTS == 0) ?
     DTS = 100 : 0,
 (DTS > 500.0) ?
     DTS = 500.0 : 0,
 (DTS < 40.0) ?
     DTS = 40.0 : 0";
            _parser.Expr = expressão;

            // cria as variáveis
            double[] dts = { 50, 50, 50, 50, 50 };
            double[] dtsEsperado = { 50, 50, 50, 50, 50 };

            _parser.DefineVar("DTS", dts);
            _parser.EvalBulk(5);

            for (int index = 0; index < dtsEsperado.Length; index++)
            {
                var dts_esperado = dtsEsperado[index];
                Check.That(dts[index]).IsEqualTo(dts_esperado);
            }
        }

        [Test]
        public void TesteDTSApenasProfundidadesCalculadasDevemSerFiltradasSuperiormente()
        {
            var expressão = @" 
 (DTS == 0) ?
          DTS = 150,
         (DTS > 100.0) ? DTS = 100.0 : 0,
         (DTS < 40.0) ?  DTS = 40.0 : 0 : 0";
            _parser.Expr = expressão;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            double expectedValue = 100;

            //if (!isWindows)
            //    expectedValue = 40;

            // cria as variáveis
            double[] dts = { 0, 200, 0, 200, 0 };
            double[] dtsEsperado = { expectedValue, 200, expectedValue, 200, expectedValue };

            _parser.DefineVar("DTS", dts);
            _parser.EvalBulk(5);

            for (int index = 0; index < dtsEsperado.Length; index++)
            {
                var dts_esperado = dtsEsperado[index];
                Check.That(dts[index]).IsEqualTo(dts_esperado);
            }
        }

        [Test]
        public void TesteDTSApenasProfundidadesCalculadasDevemSerFiltradasInferiormente()
        {
            var expressão = @" 
 (DTS == 0) ?
          DTS = 20,
         (DTS > 100.0) ? DTS = 100.0 : 0,
         (DTS < 40.0) ?  DTS = 40.0 : 0 : 0";
            _parser.Expr = expressão;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            double expectedValue = 40;

            //if (!isWindows)
            //    expectedValue = 0;

            // cria as variáveis
            double[] dts = { 0, 200, 0, 200, 0 };
            double[] dtsEsperado = { expectedValue, 200, expectedValue, 200, expectedValue };

            _parser.DefineVar("DTS", dts);
            _parser.EvalBulk(5);

            for (int index = 0; index < dtsEsperado.Length; index++)
            {
                var dts_esperado = dtsEsperado[index];
                Check.That(dts[index]).IsEqualTo(dts_esperado);
            }
        }

        [Test]
        public void TesteDTS_MECPRO_SóDeveFiltrarNasProfundidadesCalculadas()
        {
            var expressão = @" x = 0,
 (RHOG < 2.7) ?
     x = ((DTC - 104.93) / (1.667 * RHOG - 3.968)) + 194.1 : x = ((DTC - 104.93) / (0.87 - 0.125 * RHOG)) + 194.1 ,
 (DTS == 0) ?
     DTS = DTC * (x * (1 - VCL) / DTC + RVCL * VCL),
    (DTS > 500.0) ?
         DTS = 500.0 : 0,
    (DTS < 40.0) ?
         DTS = 40.0 : 0 : 0";

            _parser.Expr = expressão;

            // Resultados diferentes no LINUX 
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);

            //double expectedValue = 40;

            //if (isWindows)
               double expectedValue = 122.07365930599367;

            // cria as variáveis
            double[] x = { 0, 0, 0, 0, 0 };
            double[] rvcl = { 1, 1, 1000, 1, 0 };
            double[] rhog = { 2, 2, 2, 2, 2 };
            double[] dtc = { 200, 200, 2000, 200, 227.9894 };
            double[] vcl = { 0.5, 0.5, 0.5, 0.5, 0.5 };
            double[] dts = { 0, 1000, 0, 1000, 0 };
            double[] dtsEsperado = { expectedValue, 1000, 500, 1000, 40 };

            _parser.DefineVar("DTS", dts);
            _parser.DefineVar("x", x);
            _parser.DefineVar("RVCL", rvcl);
            _parser.DefineVar("DTC", dtc);
            _parser.DefineVar("RHOG", rhog);
            _parser.DefineVar("VCL", vcl);
            _parser.EvalBulk(5);

            for (int index = 0; index < dtsEsperado.Length; index++)
            {
                var dts_esperado = dtsEsperado[index];
                Check.That(dts[index]).IsEqualTo(dts_esperado);
            }
        }

        [Test]
        public void TesteRHOB_GARDNER_SóDeveFiltrarNasProfundidadesCalculadas()
        {
            var expressão = @" 
 (RHOB == 0) ?
     RHOB = a * (((10 ^ 6) / DTC) ^ b),
 (RHOB < 1.8) ?
     RHOB = 1.8 : ((RHOB > 2.9) ? RHOB = 2.9 : 0),
 (LITHO == 0) ?
     RHOB = RHOG : 0 : 0";

            _parser.Expr = expressão;

            // cria as variáveis
            double[] a = { 2, 1, 1, 10, 1 };
            double[] b = { 0, 0, 0, 0, 0 };
            double[] dtc = { 200, 200, 200, 200, 200 };
            double[] litho = { 1, 1, 1, 1, 0 };
            double[] rhog = { 99, 99, 99, 99, 99 };
            double[] rhob = { 0, 1000, 0, 0, 0 };
            double[] rhobEsperado = { 2, 1000, 1.8, 2.9, 99 };

            _parser.DefineVar("RHOB", rhob);
            _parser.DefineVar("a", a);
            _parser.DefineVar("b", b);
            _parser.DefineVar("DTC", dtc);
            _parser.DefineVar("RHOG", rhog);
            _parser.DefineVar("LITHO", litho);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteRHOB_AGIP_SóDeveFiltrarNasProfundidadesCalculadas()
        {
            var expressão = @" 
 (RHOB == 0) ?
     (DTC <= 100) ?
         RHOB = a + 1 : RHOB = a + 2,
     (RHOB < 1.8) ?
        RHOB = 1.8 : ((RHOB > 2.9) ? RHOB = 2.9 : 0),
     (LITHO == 0) ?
         RHOB = RHOG : 0 : 0";

            _parser.Expr = expressão;

            // cria as variáveis
            double[] a = { 1, 1, 0.5, 1, 1 };
            double[] dtc = { 100, 200, 100, 100, 100 };
            double[] litho = { 1, 1, 1, 1, 0 };
            double[] rhog = { 99, 99, 99, 99, 99 };
            double[] rhob = { 0, 0, 0, 1000, 0 };
            double[] rhobEsperado = { 2, 2.9, 1.8, 1000, 99 };

            _parser.DefineVar("RHOB", rhob);
            _parser.DefineVar("a", a);
            _parser.DefineVar("DTC", dtc);
            _parser.DefineVar("RHOG", rhog);
            _parser.DefineVar("LITHO", litho);
            _parser.EvalBulk(5);

            for (int index = 0; index < rhobEsperado.Length; index++)
            {
                var rhob_esperado = rhobEsperado[index];
                Check.That(rhob[index]).IsEqualTo(rhob_esperado);
            }
        }

        [Test]
        public void TesteTHORminModeloElásticoSemDepleção()
        {
            var THORmin_ModeloElástico =
            @"THORmin = (TVERT - PPORO_Original) * (POISS / (1 - POISS)) + PPORO_Original,

            (deplecao == 0) ?
                0 :
                THORmin = THORmin - (1 - 2 * POISS) / (1 - POISS) * (PPORO_Original - PPORO_depletada),

            GFRAT_oh = THORmin/(0.1704*PROFUNDIDADE)";
            _parser.Expr = THORmin_ModeloElástico;

            // cria as variáveis
            double[] TVERT = { 5000.329549, 5002.136624, 5003.816794, 5005.492836, 5007.150715 };
            double[] POISS = { 0.340359, 0.376731, 0.396059, 0.39215, 0.399589 };
            double[] PPORO_Original = { 3674.49378984474, 3675.258246, 3762.061267, 3762.727692, 3762.72769156637 };
            double[] deplecao = { 0, 0, 0, 0, 0 };
            double[] PPORO_depletada = { 3674.49379, 3675.258246, 3669.472988, 3670.139412, 3670.805836 };
            double[] THORmin_esperado = { 4358.593373, 4477.281479, 4576.393217, 4564.488572, 4590.923297 };
            double[] THORmin = { 0, 0, 0, 0, 0 };
            double[] PROFUNDIDADE = { 2564.035176, 2564.562814, 2565.090452, 2565.61809, 2566.145728 };
            double[] GFRAT_oh = { 0, 0, 0, 0, 0 };
            double[] GFRAT_oh_esperado = { 9.975916027, 10.24546012, 10.47010552, 10.44072182, 10.49902889 };

            _parser.DefineVar("TVERT", TVERT);
            _parser.DefineVar("POISS", POISS);
            _parser.DefineVar("PPORO_Original", PPORO_Original);
            _parser.DefineVar("deplecao", deplecao);
            _parser.DefineVar("PPORO_depletada", PPORO_depletada);
            _parser.DefineVar("THORmin", THORmin);
            _parser.DefineVar("PROFUNDIDADE", PROFUNDIDADE);
            _parser.DefineVar("GFRAT_oh", GFRAT_oh);

            // faz o cálculo
            _parser.EvalBulk(5);
            var THORmin_calculado = _parser.Vars["THORmin"].ValueArray;
            var GFRAT_oh_calculado = _parser.Vars["GFRAT_oh"].ValueArray;

            for (int index = 0; index < THORmin_calculado.Length; index++)
            {
                var esperado = THORmin_esperado[index];
                var calculado = THORmin_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }

            for (int index = 0; index < GFRAT_oh_calculado.Length; index++)
            {
                var esperado = GFRAT_oh_esperado[index];
                var calculado = GFRAT_oh_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }
        }

        [Test]
        public void TesteTHORminModeloElásticoComDepleção()
        {
            var THORmin_ModeloElástico =
            @"THORmin = (TVERT - PPORO_Original) * (POISS / (1 - POISS)) + PPORO_Original,

            (deplecao == 0) ?
                0 :
                THORmin = THORmin - (1 - 2 * POISS) / (1 - POISS) * (PPORO_Original - PPORO_depletada),

            GFRAToh = THORmin/(0.1704*PROFUNDIDADE)";
            _parser.Expr = THORmin_ModeloElástico;

            // cria as variáveis
            double[] TVERT = { 5000.329549, 5002.136624, 5003.816794, 5005.492836, 5007.150715 };
            double[] POISS = { 0.340359, 0.376731, 0.396059, 0.39215, 0.399589 };
            double[] PPORO_Original = { 3674.49378984474, 3675.258246, 3762.061267, 3762.727692, 3762.72769156637 };
            double[] deplecao = { 1, 1, 1, 1, 1 };
            double[] delta_pporo = { 0, 0, 0, 0, 0 };
            double[] PPORO_depletada = { 3674.49379, 3675.258246, 3669.472988, 3670.139412, 3670.805836 };
            double[] calc_deplecao = { 0, 0, 0, 0, 0 };
            double[] delta_Sh = { 0, 0, 0, 0, 0 };
            double[] THORmin_esperado = { 4358.593373, 4477.281479, 4544.523487, 4531.632947, 4560.177806 };
            double[] THORmin = { 0, 0, 0, 0, 0 };
            double[] PROFUNDIDADE = { 2564.035176, 2564.562814, 2565.090452, 2565.61809, 2566.145728 };
            double[] GFRAT_oh = { 0, 0, 0, 0, 0 };
            double[] GFRAT_oh_esperado = { 9.975916027, 10.24546012, 10.39719233, 10.3655685, 10.42871671 };

            _parser.DefineVar("TVERT", TVERT);
            _parser.DefineVar("POISS", POISS);
            _parser.DefineVar("PPORO_Original", PPORO_Original);
            _parser.DefineVar("deplecao", deplecao);
            _parser.DefineVar("delta_pporo", delta_pporo);
            _parser.DefineVar("PPORO_depletada", PPORO_depletada);
            _parser.DefineVar("calc_deplecao", calc_deplecao);
            _parser.DefineVar("delta_Sh", delta_Sh);
            _parser.DefineVar("THORmin", THORmin);
            _parser.DefineVar("PROFUNDIDADE", PROFUNDIDADE);
            _parser.DefineVar("GFRAToh", GFRAT_oh);

            // faz o cálculo
            _parser.EvalBulk(5);
            var THORmin_calculado = _parser.Vars["THORmin"].ValueArray;
            var GFRAT_oh_calculado = _parser.Vars["GFRAToh"].ValueArray;

            for (int index = 0; index < THORmin_calculado.Length; index++)
            {
                var esperado = THORmin_esperado[index];
                var calculado = THORmin_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }

            for (int index = 0; index < GFRAT_oh_calculado.Length; index++)
            {
                var esperado = GFRAT_oh_esperado[index];
                var calculado = GFRAT_oh_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }
        }

        [Test]
        public void TesteTHORminNormalizaçãoLDASemDepleção()
        {
            var THORmin_NormalizaçãoLDA =
            @"THORmin = 1.4223 * DENSIDADE_AGUA_MAR * LAMINA_DAGUA + coef_ang * (PV-LAMINA_DAGUA-MR),
            (deplecao == 0) ?
                0 :
                THORmin = THORmin - (1 - 2 * POISS) / (1 - POISS) * (PPORO_Original - PPORO_depletada),

            GFRAT_oh = THORmin/(0.1704*PROFUNDIDADE)";
            _parser.Expr = THORmin_NormalizaçãoLDA;

            // cria as variáveis
            double[] deplecao = { 0, 0, 0, 0, 0 };
            double[] POISS = { 0.340359, 0.376731, 0.396059, 0.39215, 0.399589 };
            double[] PPORO_Original = { 3674.49378984474, 3675.258246, 3762.061267, 3762.727692, 3762.72769156637 };
            double[] PPORO_depletada = { 3674.49379, 3675.258246, 3669.472988, 3670.139412, 3670.805836 };
            double[] DENSIDADE_AGUA_MAR = { 1.04, 1.04, 1.04, 1.04, 1.04 };
            double[] LAMINA_DAGUA = { 1700, 1700, 1700, 1700, 1700 };
            double[] coef_ang = { 2.1701, 2.1701, 2.1701, 2.1701, 2.1701 };
            double[] PV = { 2564.035176, 2564.562814, 2565.090452, 2565.61809, 2566.145728 };
            double[] MR = { 27, 27, 27, 27, 27 };
            double[] THORmin_esperado = { 4331.076435, 4332.221463, 4333.36649, 4334.511517, 4335.656544 };
            double[] THORmin = { 0, 0, 0, 0, 0 };
            double[] PROFUNDIDADE = { 2564.035176, 2564.562814, 2565.090452, 2565.61809, 2566.145728 };
            double[] GFRAT_oh = { 0, 0, 0, 0, 0 };
            double[] GFRAT_oh_esperado = { 9.912935466, 9.91351615, 9.914096596, 9.914676803, 9.915256771 };

            _parser.DefineVar("deplecao", deplecao);
            _parser.DefineVar("POISS", POISS);
            _parser.DefineVar("PPORO_Original", PPORO_Original);
            _parser.DefineVar("PPORO_depletada", PPORO_depletada);
            _parser.DefineVar("DENSIDADE_AGUA_MAR", DENSIDADE_AGUA_MAR);
            _parser.DefineVar("LAMINA_DAGUA", LAMINA_DAGUA);
            _parser.DefineVar("coef_ang", coef_ang);
            _parser.DefineVar("PV", PV);
            _parser.DefineVar("MR", MR);
            _parser.DefineVar("THORmin", THORmin);
            _parser.DefineVar("PROFUNDIDADE", PROFUNDIDADE);
            _parser.DefineVar("GFRAT_oh", GFRAT_oh);

            // faz o cálculo
            _parser.EvalBulk(5);
            var THORmin_calculado = _parser.Vars["THORmin"].ValueArray;
            var GFRAT_oh_calculado = _parser.Vars["GFRAT_oh"].ValueArray;

            for (int index = 0; index < THORmin_calculado.Length; index++)
            {
                var esperado = THORmin_esperado[index];
                var calculado = THORmin_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }

            for (int index = 0; index < GFRAT_oh_calculado.Length; index++)
            {
                var esperado = GFRAT_oh_esperado[index];
                var calculado = GFRAT_oh_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }
        }

        [Test]
        public void TesteTHORminNormalizaçãoLDAComDepleção()
        {
            var THORmin_NormalizaçãoLDA =
            @"THORmin = 1.4223 * DENSIDADE_AGUA_MAR * LAMINA_DAGUA + coef_ang * (PV-LAMINA_DAGUA-MR),
            (deplecao == 0) ?
                0 :
                THORmin = THORmin - (1 - 2 * POISS) / (1 - POISS) * (PPORO_Original - PPORO_depletada),

            GFRAT_oh = THORmin/(0.1704*PROFUNDIDADE)";
            _parser.Expr = THORmin_NormalizaçãoLDA;

            // cria as variáveis
            double[] deplecao = { 1, 1, 1, 1, 1 };
            double[] POISS = { 0.340359, 0.376731, 0.396059, 0.39215, 0.399589 };
            double[] PPORO_Original = { 3674.49378984474, 3675.258246, 3762.061267, 3762.727692, 3762.72769156637 };
            double[] PPORO_depletada = { 3674.49379, 3675.258246, 3669.472988, 3670.139412, 3670.805836 };
            double[] DENSIDADE_AGUA_MAR = { 1.04, 1.04, 1.04, 1.04, 1.04 };
            double[] LAMINA_DAGUA = { 1700, 1700, 1700, 1700, 1700 };
            double[] coef_ang = { 2.1701, 2.1701, 2.1701, 2.1701, 2.1701 };
            double[] PV = { 2564.035176, 2564.562814, 2565.090452, 2565.61809, 2566.145728 };
            double[] MR = { 27, 27, 27, 27, 27 };
            double[] THORmin_esperado = { 4331.076435, 4332.221463, 4301.49676, 4301.655892, 4304.911054 };
            double[] THORmin = { 0, 0, 0, 0, 0 };
            double[] PROFUNDIDADE = { 2564.035176, 2564.562814, 2565.090452, 2565.61809, 2566.145728 };
            double[] GFRAT_oh = { 0, 0, 0, 0, 0 };
            double[] GFRAT_oh_esperado = { 9.912935466, 9.91351615, 9.841183404, 9.839523488, 9.844944597 };

            _parser.DefineVar("deplecao", deplecao);
            _parser.DefineVar("POISS", POISS);
            _parser.DefineVar("PPORO_Original", PPORO_Original);
            _parser.DefineVar("PPORO_depletada", PPORO_depletada);
            _parser.DefineVar("DENSIDADE_AGUA_MAR", DENSIDADE_AGUA_MAR);
            _parser.DefineVar("LAMINA_DAGUA", LAMINA_DAGUA);
            _parser.DefineVar("coef_ang", coef_ang);
            _parser.DefineVar("PV", PV);
            _parser.DefineVar("MR", MR);
            _parser.DefineVar("THORmin", THORmin);
            _parser.DefineVar("PROFUNDIDADE", PROFUNDIDADE);
            _parser.DefineVar("GFRAT_oh", GFRAT_oh);

            // faz o cálculo
            _parser.EvalBulk(5);
            var THORmin_calculado = _parser.Vars["THORmin"].ValueArray;
            var GFRAT_oh_calculado = _parser.Vars["GFRAT_oh"].ValueArray;

            for (int index = 0; index < THORmin_calculado.Length; index++)
            {
                var esperado = THORmin_esperado[index];
                var calculado = THORmin_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }

            for (int index = 0; index < GFRAT_oh_calculado.Length; index++)
            {
                var esperado = GFRAT_oh_esperado[index];
                var calculado = GFRAT_oh_calculado[index];
                esperado = Math.Round(esperado, 4, MidpointRounding.AwayFromZero);
                calculado = Math.Round(calculado, 4, MidpointRounding.AwayFromZero);

                Check.That(calculado).IsEqualTo(esperado);
            }
        }
    }
}
