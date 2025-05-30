using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Deep.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Deep
{
    [TestFixture]
    public class LeitorDeepEstratigrafiaTests
    {
        [Test]
        public void DeveLerSeExistemEstratigrafia_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorEstratigrafiaSest5.ObterEstratigrafia(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result.Itens).CountIs(3);
            Check.That(result.Itens["FM"]).CountIs(3);
            Check.That(result.Itens["MB"]).CountIs(7);
            Check.That(result.Itens["CR"]).CountIs(4);
        }

        [Test]
        public void DeveLerSeExistemEstratigrafia_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");
            arquivo.Root?.Element("EspecieGeologicaPoco")?.Remove();

            var result = LeitorEstratigrafiaSest5.ObterEstratigrafia(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result.Itens).IsEmpty();
        }
    }
}