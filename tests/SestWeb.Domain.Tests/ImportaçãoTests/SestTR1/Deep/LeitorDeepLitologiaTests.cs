using NFluent;
using NUnit.Framework;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.SestTR1.Deep;
using System.Collections.Generic;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Deep
{
    [TestFixture]
    public static class LeitorDeepLitologiaTests
    {
        [Test]
        public static void DeveLerSeExisteLitologia_Sucesso()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>() { new LitologiaParaImportarDTO() { Nome = "Lito_acomp" } },
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorLitologias.Litologias;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(1);
            Check.That(result[0].Pontos.Count).IsStrictlyGreaterThan(0);
        }

        [Test]
        public static void DeveLerSeExisteLitologia_LitologiaInexistente_Falha()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>() { new LitologiaParaImportarDTO() { Nome = "Litologia inexistente" } },
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorLitologias.Litologias;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(0);
        }

        [Test]
        public static void DeveLerSeExisteLitologia_PontoInválido_Falha()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoComValorDeLitologiaInválido.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>() { new LitologiaParaImportarDTO() { Nome = "Lito_acomp" } },
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorLitologias.Litologias;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(1);
            Check.That(result[0].Pontos[0].Pm).IsNotEqualTo("2278");
        }

        [Test]
        public static void DeveLerSeExisteLitologia_PontoInexistente_Falha()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoComValorDeLitologiaInexistente.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>() { new LitologiaParaImportarDTO() { Nome = "Lito_acomp" } },
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorLitologias.Litologias;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(1);
            Check.That(result[0].Pontos[0].Pm).IsNotEqualTo("2278");
        }
    }
}