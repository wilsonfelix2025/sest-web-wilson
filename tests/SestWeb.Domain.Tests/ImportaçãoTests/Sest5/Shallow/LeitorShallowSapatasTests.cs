using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Shallow
{
    [TestFixture]
    public class LeitorShallowSapatasTests
    {
        [Test]
        public void DeveLerSeExistemSapatas_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorShallowSapatas.LerSapatas(arquivo);

            Check.That(result).IsTrue();
        }

        [Test]
        public void DeveLerSeExistemSapatas_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_trajetoria.xml");

            var result = LeitorShallowSapatas.LerSapatas(arquivo);

            Check.That(result).IsFalse();
        }
    }
}