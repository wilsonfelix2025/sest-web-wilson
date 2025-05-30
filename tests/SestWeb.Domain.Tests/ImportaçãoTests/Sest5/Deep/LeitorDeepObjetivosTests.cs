using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Deep.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Deep
{
    [TestFixture]
    public class LeitorDeepObjetivosTests
    {
        [Test]
        public void DeveLerSeExistemObjetivos_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorObjetivosSest5.ObterObjetivos(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result).CountIs(2);
            Check.That(result[0].TipoObjetivo).IsEqualTo("Primário");
            Check.That(result[1].TipoObjetivo).IsEqualTo("Secundário");
        }

        [Test]
        public void DeveLerSeExistemObjetivos_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");
            arquivo.Root?.Element("Objetivo")?.Remove();

            var result = LeitorObjetivosSest5.ObterObjetivos(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result).IsEmpty();
        }
    }
}