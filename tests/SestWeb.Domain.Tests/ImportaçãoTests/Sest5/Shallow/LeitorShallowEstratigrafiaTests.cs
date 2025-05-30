using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Shallow
{
    [TestFixture]
    public class LeitorShallowEstratigrafiaTests
    {
        [Test]
        public void DeveLerSeExisteEstratigrafia_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorShallowEstratigrafia.LerEstratigrafia(arquivo);

            Check.That(result).IsTrue();
        }

        [Test]
        public void DeveLerSeExistemEstratigrafia_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_trajetoria.xml");

            var result = LeitorShallowEstratigrafia.LerEstratigrafia(arquivo);

            Check.That(result).IsFalse();
        }
    }
}