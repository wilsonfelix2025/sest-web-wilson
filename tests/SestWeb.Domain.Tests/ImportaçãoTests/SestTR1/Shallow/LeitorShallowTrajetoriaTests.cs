using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.SestTR1.Shallow;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Shallow
{
    [TestFixture]
    public class LeitorShallowTrajetoriaTests
    {
        [Test]
        public void DeveLerSeExistemTrajetorias_Sucesso()
        {
            var leitor = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt").LerDados();
            var result = leitor.Trajetórias;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(2);
        }

        [Test]
        public void DeveLerSeExistemTrajetoria_Falha()
        {
            var leitor = new LeitorShallowSestTR1("ArquivosEntradaParaTestes/SestTR1/ArquivoSemTrajetórias.xsrt").LerDados();
            var result = leitor.Trajetórias;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(0);
        }
    }
}