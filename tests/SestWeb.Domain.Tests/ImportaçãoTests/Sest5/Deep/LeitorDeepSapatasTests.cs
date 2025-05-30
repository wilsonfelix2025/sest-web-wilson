using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Deep.Sest5.Leitores;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Deep
{
    [TestFixture]
    public class LeitorDeepSapatasTests
    {
        [Test]
        public void DeveLerSeExistemSapatas_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorSapatas.ObterSapatas(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result).CountIs(4);
        }

        [Test]
        public void DeveLerSeExistemSapatas_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");
            arquivo.Root?.Element("Sapatas")?.Remove();

            var result = LeitorSapatas.ObterSapatas(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result).IsEmpty();
        }
    }
}