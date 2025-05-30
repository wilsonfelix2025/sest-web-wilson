using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Deep.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Deep
{
    [TestFixture]
    public class LeitorDeepDadosGeraisTests
    {
        [Test]
        public void DeveLerSeExistemDadosGerais_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorDadosGeraisSest5.ObterDadosGerais(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result.Identificação).IsNotNull();
            Check.That(result.Geometria).IsNotNull();
            Check.That(result.Area).IsNotNull();
        }

        [Test]
        public void DeveLerSeExistemDadosGerais_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");
            arquivo.Root?.Element("Identificacao")?.Remove();

            var result = LeitorDadosGeraisSest5.ObterDadosGerais(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result.Identificação).IsNull();
        }
    }
}