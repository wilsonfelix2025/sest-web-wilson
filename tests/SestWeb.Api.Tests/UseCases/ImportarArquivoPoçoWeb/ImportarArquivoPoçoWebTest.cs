using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Importação.ImportarArquivo;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase;
using SestWeb.Domain.Enums;

namespace SestWeb.Api.Tests.UseCases.ImportarArquivoPoçoWeb
{
    [TestFixture]
    public class ImportarArquivoPoçoWebTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(ImportarArquivoController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(ImportarArquivoController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/[controller]");
        }
        [Test]
        public void ImportarArquivoPoçoWebDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(ImportarArquivoController).GetMethod(nameof(ImportarArquivoController.ImportarArquivo)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status400BadRequest)]
        public void ImportarArquivoPoçoWebDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(ImportarArquivoController).GetMethod(nameof(ImportarArquivoController.ImportarArquivo)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ImportarArquivoPoçoWebDeveRetornarOkResultEmCasoDeSucesso()
        {
            var dadosSelecionado = A.Fake<List<DadosSelecionadosEnum>>();
            var dadosExtras = A.Fake<Dictionary<string, object>>();
            var listaLitologia = A.Fake<List<LitologiaParaImportar>>();
            var listaPerfis = A.Fake<List<PerfilParaImportar>>();

            var useCase = A.Fake<IImportarArquivoUseCase>();
            var presenter = A.Fake<ImportarArquivoPresenter>();
            A.CallTo(() => useCase.Execute(A<List<DadosSelecionadosEnum>>.Ignored
                    ,A<string>.Ignored, A<List<PerfilModel>>.Ignored
                    ,A<List<LitologiaModel>>.Ignored, A<string>.Ignored, A<string>.Ignored
                    , A<Dictionary<string, object>>.Ignored)).Returns(ImportarArquivoOutput.ImportadoComSucesso());

            var controller = new ImportarArquivoController(useCase, presenter);

            // Act
            var result = await controller.ImportarArquivo(new DadoParaImportarRequest()
            {
                PoçoId = "0",
                CaminhoDoArquivo = "caminho",
                CorreçãoMesaRotativa = "0",
                DadosSelecionados = dadosSelecionado,
                Extras = dadosExtras,
                ListaLitologias = listaLitologia,
                ListaPerfis = listaPerfis
            });

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ImportarArquivoPoçoWebDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            var dadosSelecionado = A.Fake<List<DadosSelecionadosEnum>>();
            var dadosExtras = A.Fake<Dictionary<string, object>>();
            var listaLitologia = A.Fake<List<LitologiaParaImportar>>();
            var listaPerfis = A.Fake<List<PerfilParaImportar>>();

            var useCase = A.Fake<IImportarArquivoUseCase>();
            var presenter = A.Fake<ImportarArquivoPresenter>();
            A.CallTo(() => useCase.Execute(A<List<DadosSelecionadosEnum>>.Ignored
                , A<string>.Ignored, A<List<PerfilModel>>.Ignored
                , A<List<LitologiaModel>>.Ignored, A<string>.Ignored, A<string>.Ignored
                , A<Dictionary<string, object>>.Ignored)).Returns(ImportarArquivoOutput.ImportaçãoComFalha());

            var controller = new ImportarArquivoController(useCase, presenter);

            // Act
            var result = await controller.ImportarArquivo(new DadoParaImportarRequest()
            {
                PoçoId = "0",
                CaminhoDoArquivo = "caminho",
                CorreçãoMesaRotativa = "0",
                DadosSelecionados = dadosSelecionado,
                Extras = dadosExtras,
                ListaLitologias = listaLitologia,
                ListaPerfis = listaPerfis
            });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

        [Test]
        public async Task ImportarArquivoPoçoWebDeveRetornarBadRequestObjectResultCasoOcorraErroDeValidação()
        {
            var dadosSelecionado = A.Fake<List<DadosSelecionadosEnum>>();
            var dadosExtras = A.Fake<Dictionary<string, object>>();
            var listaLitologia = A.Fake<List<LitologiaParaImportar>>();
            var listaPerfis = A.Fake<List<PerfilParaImportar>>();

            var useCase = A.Fake<IImportarArquivoUseCase>();
            var presenter = A.Fake<ImportarArquivoPresenter>();
            A.CallTo(() => useCase.Execute(A<List<DadosSelecionadosEnum>>.Ignored
                , A<string>.Ignored, A<List<PerfilModel>>.Ignored
                , A<List<LitologiaModel>>.Ignored, A<string>.Ignored, A<string>.Ignored
                , A<Dictionary<string, object>>.Ignored)).Returns(ImportarArquivoOutput.ImportaçãoComFalhasDeValidação());

            var controller = new ImportarArquivoController(useCase, presenter);

            // Act
            var result = await controller.ImportarArquivo(new DadoParaImportarRequest()
            {
                PoçoId = "0",
                CaminhoDoArquivo = "caminho",
                CorreçãoMesaRotativa = "0",
                DadosSelecionados = dadosSelecionado,
                Extras = dadosExtras,
                ListaLitologias = listaLitologia,
                ListaPerfis = listaPerfis
            });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

        [Test]
        public async Task ImportarArquivoPoçoWebDeveRetornarBadRequestObjectResultCasoTenhaPerfilIncompleto()
        {
            var dadosSelecionado = A.Fake<List<DadosSelecionadosEnum>>();
            var dadosExtras = A.Fake<Dictionary<string, object>>();
            var listaLitologia = A.Fake<List<LitologiaParaImportar>>();
            var listaPerfis = A.Fake<List<PerfilParaImportar>>();

            var useCase = A.Fake<IImportarArquivoUseCase>();
            var presenter = A.Fake<ImportarArquivoPresenter>();
            A.CallTo(() => useCase.Execute(A<List<DadosSelecionadosEnum>>.Ignored
                , A<string>.Ignored, A<List<PerfilModel>>.Ignored
                , A<List<LitologiaModel>>.Ignored, A<string>.Ignored, A<string>.Ignored
                , A<Dictionary<string, object>>.Ignored)).Returns(ImportarArquivoOutput.ImportaçãoCanceladaPerfisIncompletos());

            var controller = new ImportarArquivoController(useCase, presenter);

            // Act
            var result = await controller.ImportarArquivo(new DadoParaImportarRequest()
            {
                PoçoId = "0",
                CaminhoDoArquivo = "caminho",
                CorreçãoMesaRotativa = "0",
                DadosSelecionados = dadosSelecionado,
                Extras = dadosExtras,
                ListaLitologias = listaLitologia,
                ListaPerfis = listaPerfis
            });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}



