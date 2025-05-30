using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.SestTR1.Shallow;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Shallow
{
    [TestFixture]
    public class LeitorShallowDadosGeraisTests
    {
        [Test]
        public void DeveLerSeExistemDadosGerais_Sucesso()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.TemDadosGerais).IsTrue();
        }

        [Test]
        public void DeveLerSeExistemDadosGerais_Falha()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoSemDadosGerais.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.TemDadosGerais).IsFalse();
        }
    }
}