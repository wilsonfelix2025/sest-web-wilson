using System.Collections.Generic;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.SestTR1.Deep;

namespace SestWeb.Domain.Tests.ImportaçãoTests.SestTR1.Deep
{
    [TestFixture]
    public class LeitorDeepDadosGeraisTests
    {
        [Test]
        public void DeveLerSeExistemDadosGerais_Sucesso()
        {
            var dadosSelecionados = new List<DadosSelecionadosEnum>() { DadosSelecionadosEnum.DadosGerais };
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoCorreto.xsrt",
                dadosSelecionados,
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorDadosGerais.DadosGerais;

            Check.That(result).IsNotNull();
            Check.That(result.Identificação).IsNotNull();
            Check.That(result.Identificação.Bacia).Equals("Santos");
            Check.That(result.Geometria).IsNotNull();
            Check.That(result.Geometria.OnShore).IsNotNull();
            Check.That(result.Geometria.OffShore).IsNotNull();
            Check.That(result.Geometria.OffShore.LaminaDagua).Equals("2250");
            Check.That(result.Area).IsNotNull();
            Check.That(result.Area.SonicoSuperficie).Equals("180");
        }

        [Test]
        public void DeveLerSeExistemDadosGerais_Falha()
        {
            var dadosSelecionados = new List<DadosSelecionadosEnum>() { DadosSelecionadosEnum.DadosGerais };
            var leitor = new LeitorDeepSestTR1(
                "ArquivosEntradaParaTestes/SestTR1/ArquivoSemSônicoSuperficie.xsrt",
                dadosSelecionados,
                new List<PerfilParaImportarDTO>(),
                new List<LitologiaParaImportarDTO>(),
                new Dictionary<string, object>()
                );

            var result = leitor.LeitorDadosGerais.DadosGerais;

            Check.That(result).IsNotNull();
            Check.That(result.Area).IsNotNull();
            Check.That(result.Area.SonicoSuperficie).IsNull();
        }
    }
}