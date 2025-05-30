using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Shallow
{
    [TestFixture]
    public class LeitorShallowPerfisTests
    {
        [Test]
        public void DeveLerSeExistemPerfis_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorShallowPerfis.LerPerfis(arquivo);

            Check.That(result).Not.IsEmpty();
        }
    }
}