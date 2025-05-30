using NFluent;
using NUnit.Framework;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.SestTR1.Deep;
using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Deep
{
    public class LeitorDeepPerfisTest
    {
        [Test]
        public void DeveLerListaDePerfis_Sucesso()
        {
            var perfisParaImportar = new List<PerfilParaImportarDTO>() { 
                new PerfilParaImportarDTO { Nome = "Projeto_GCOLI" },
                new PerfilParaImportarDTO { Nome = "Projeto_GSOBR" },
                new PerfilParaImportarDTO { Nome = "Projeto_GECD" },
                new PerfilParaImportarDTO { Nome = "Projeto_YOUNG" },
                new PerfilParaImportarDTO { Nome = "Projeto_BIOT" },
            };
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                new List<DadosSelecionadosEnum>(),
                perfisParaImportar,
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorPerfis.Perfis;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(5);
        }

        [Test]
        public void DeveLerListaDePerfis_PerfilInexistente_Falha()
        {
            var perfisParaImportar = new List<PerfilParaImportarDTO>() {
                new PerfilParaImportarDTO { Nome = "Perfil inexistente" },
            };
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                new List<DadosSelecionadosEnum>(),
                perfisParaImportar,
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorPerfis.Perfis;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(0);
        }

        [Test]
        public void DeveLerListaDePerfis_PerfilTipoDesconhecido_Falha()
        {
            Check.ThatCode(() =>
            {
                var leitor = new LeitorDeepSestTR1(
                    "ArquivosEntradaParaTestes/SestTR1/ArquivoComTipoDePerfilInexistente.xsrt",
                    new List<DadosSelecionadosEnum>(),
                    new List<PerfilParaImportarDTO>() { new PerfilParaImportarDTO { Nome = "Projeto_GSOBR" } },
                    new List<LitologiaParaImportarDTO>(),
                    new Dictionary<string, object>()
                );
            }).Throws<Exception>().WithMessage("O tipo de perfil não está mapeado para importações do SEST TR1: Inexistente");
        }

        [Test]
        public void DeveLerListaDePerfis_PerfilPontoInválido_Falha()
        {
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoComPontoDePerfilInválido.xsrt",
                new List<DadosSelecionadosEnum>(),
                new List<PerfilParaImportarDTO>() { new PerfilParaImportarDTO { Nome = "Projeto_GSOBR" } },
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>()
            );
            var result = leitor.LeitorPerfis.Perfis;

            Check.That(result).IsNotNull();
            Check.That(result.Count).Equals(1);
            Check.That(result[0].PontosDTO[0].Pm).IsNotEqualTo("2274");
        }
    }
}
