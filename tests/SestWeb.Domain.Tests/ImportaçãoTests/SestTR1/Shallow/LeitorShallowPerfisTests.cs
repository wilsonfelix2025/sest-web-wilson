using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.SestTR1.Shallow;
using System.Linq;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Shallow
{
    [TestFixture]
    public class LeitorShallowPerfisTests
    {
        [Test]
        public void DeveLerSeExistemPerfis_Sucesso()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.Perfis.Count()).IsNotZero();
        }

        [Test]
        public void DeveLerSeExistemPerfis_Genérico_Sucesso()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoComPerfilGenérico.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.Perfis.Count()).Equals(1);
        }

        [Test]
        public void DeveLerSeExistemPerfis_TipoInexistente_Falha()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoComTipoDePerfilInexistente.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.Perfis.Count()).Equals(0);
        }

        [Test]
        public void DeveLerSeExistemPerfis_SemUnidade_Falha()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoComPerfilSemUnidade.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.Perfis.Count()).Equals(1);
            Check.That(result.Perfis[0].Unidade).Equals("");
        }

        [Test]
        public void DeveLerSeExistemPerfis_Falha()
        {
            var result = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoSemPerfis.xsrt").LerDados();

            Check.That(result).IsNotNull();
            Check.That(result.Perfis.Count()).IsZero();
        }
    }
}