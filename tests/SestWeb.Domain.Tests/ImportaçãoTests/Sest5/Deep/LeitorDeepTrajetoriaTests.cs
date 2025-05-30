using System;
using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Importadores.Deep.Sest5;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Deep
{
    [TestFixture]
    public class LeitorDeepTrajetoriaTests
    {
        [Test]
        public void DeveLerSeExistemTrajetoriaRetronanálise_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");

            var result = LeitorTrajetóriaSest5.ObterTrajetória(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result.Pontos).CountIs(186);
        }

        [Test]
        public void DeveLerSeExistemTrajetoriaProjeto_Sucesso()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Poço_projeto.xml");

            var result = LeitorTrajetóriaSest5.ObterTrajetória(arquivo);

            Check.That(result).IsNotNull();
            Check.That(result.Pontos).CountIs(1);
        }

        [Test]
        public void DeveLerSeExistemTrajetoria_Falha()
        {
            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");
            arquivo.Root?.Element("Trajetoria")?.Remove();

            Check.ThatCode(() => LeitorTrajetóriaSest5.ObterTrajetória(arquivo)).Throws<InvalidOperationException>().WithMessage("Não foi possível encontrar método de cálculo de trajetória.");
        }
    }
}