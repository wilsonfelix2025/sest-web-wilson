using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Shallow
{
    [TestFixture]
    public class LeitorShallowTrajetoriaTests
    {
        [Test]
        public void DeveLerSeExistemTrajetoria_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorShallowTrajetoria.LerTrajetoria(arquivo);

            Check.That(result).IsTrue();
        }

        [Test]
        public void DeveLerSeExistemTrajetoria_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");
            arquivo.Root?.Element("Trajetoria")?.Remove();

            var result = LeitorShallowTrajetoria.LerTrajetoria(arquivo);

            Check.That(result).IsFalse();
        }
    }
}