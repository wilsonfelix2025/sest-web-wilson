using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Shallow
{
    [TestFixture]
    public static class LeitorShallowLitologiaTests
    {
        [Test]
        public static void DeveLerSeExisteLitologia_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorShallowLitologia.LerLitologia(arquivo);

            Check.That(result).Not.IsNullOrEmpty();
        }

        [Test]
        public static void DeveLerSeExistemLitologia_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_trajetoria.xml");

            var result = LeitorShallowLitologia.LerLitologia(arquivo);

            Check.That(result).IsNullOrEmpty();
        }
    }
}