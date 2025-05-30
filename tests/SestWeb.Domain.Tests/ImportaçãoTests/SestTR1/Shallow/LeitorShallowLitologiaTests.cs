using System.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.SestTR1.Shallow;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Shallow
{
    [TestFixture]
    public static class LeitorShallowLitologiaTests
    {
        [Test]
        public static void DeveLerSeExisteLitologia_Sucesso()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.Litologias.Count()).Equals(2);
        }

        [Test]
        public static void DeveLerSeExistemLitologia_Falha()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoSemLitologias.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.Litologias.Count()).Equals(0);
        }
    }
}