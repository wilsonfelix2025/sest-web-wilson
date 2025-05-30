using NFluent;
using NUnit.Framework;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.SestTR1.Deep;
using System.Collections.Generic;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Deep
{
    [TestFixture]
    public class LeitorDeepTrajetoriaTests
    {
        [Test]
        public void DeveLerSeExisteTrajetoria_Sucesso()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>() { { "TrajetóriaSelecionada", "Projeto_Trajectory" } }
                );
            var result = leitor.LeitorTrajetória.Trajetória;

            Check.That(result).IsNotNull();
            Check.That(result.Pontos).IsNotNull();
            Check.That(result.Pontos.Count).IsNotZero();
        }

        [Test]
        public void DeveLerSeExisteTrajetoria_TrajetóriaInexistente_Falha()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>() { { "TrajetóriaSelecionada", "Trajetória_Inexistente" } }
                );

            Check.That(leitor.LeitorTrajetória).IsNull();
        }

        [Test]
        public void DeveLerSeExisteTrajetoria_TrajetóriaNãoInformada_Falha()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>()
                );

            Check.That(leitor.LeitorTrajetória).IsNull();
        }
    }
}