using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Importação.ImportarDados;
using SestWeb.Api.UseCases.Importação.ImportarDados.ImportarLitologia;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarLitologiaUseCase;

namespace SestWeb.Api.Tests.UseCases.ImportarDadosLitologia
{
    [TestFixture]
    public class ImportarDadosLitologiaTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(ImportarDadosLitologiaController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(ImportarDadosLitologiaController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/importar-dados/litologia");
        }

        [Test]
        public void ImportarDadosLitologiaDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(ImportarDadosLitologiaController).GetMethod(nameof(ImportarDadosLitologiaController.ImportarDadosLitologia)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status400BadRequest)]
        public void ImportarDadosLitologiaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(ImportarDadosLitologiaController).GetMethod(nameof(ImportarDadosLitologiaController.ImportarDadosLitologia)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ImportarDadosLitologiaDeveRetornarOkResultEmCasoDeSucesso()
        {
            var lista = A.Fake<List<PontoLitologiaRequest>>();
            var model = A.Fake<LitologiaRequest>();
            model.PontosLitologia = lista;

            var importarDadosLitologiaUseCase = A.Fake<IImportarDadosLitologiaUseCase>();
            var presenter = A.Fake<ImportarDadosPresenter>();
            A.CallTo(() => importarDadosLitologiaUseCase.Execute(A<string>.Ignored, A<LitologiaInput>.Ignored)).Returns(ImportarDadosOutput.ImportadoComSucesso());

            var controller = new ImportarDadosLitologiaController(importarDadosLitologiaUseCase, presenter);

            // Act
            var result = await controller.ImportarDadosLitologia(new ImportarDadosLitologiaRequest
            {
                PoçoId = "0",
                Litologia = model
            });

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ImportarDadosLitologiaDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            var lista = A.Fake<List<PontoLitologiaRequest>>();
            var model = A.Fake<LitologiaRequest>();
            model.PontosLitologia = lista;

            var importarDadosLitologiaUseCase = A.Fake<IImportarDadosLitologiaUseCase>();
            var presenter = A.Fake<ImportarDadosPresenter>();
            A.CallTo(() => importarDadosLitologiaUseCase.Execute(A<string>.Ignored, A<LitologiaInput>.Ignored)).Returns(ImportarDadosOutput.ImportaçãoComFalha());

            var controller = new ImportarDadosLitologiaController(importarDadosLitologiaUseCase, presenter);

            // Act
            var result = await controller.ImportarDadosLitologia(new ImportarDadosLitologiaRequest
            {
                PoçoId = "0",
                Litologia = model
            });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }


    }
}
