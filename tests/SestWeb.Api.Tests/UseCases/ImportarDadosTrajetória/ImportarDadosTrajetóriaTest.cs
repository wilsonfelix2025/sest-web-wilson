
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
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarTrajetóriaUseCase;

namespace SestWeb.Api.Tests.UseCases.ImportarDadosTrajetória
{
    [TestFixture]
    public class ImportarDadosTrajetóriaTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(ImportarDadosTrajetóriaController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(ImportarDadosTrajetóriaController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/importar-dados/trajetoria");
        }

        [Test]
        public void ImportarDadosTrajetóriaDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(ImportarDadosTrajetóriaController).GetMethod(nameof(ImportarDadosTrajetóriaController.ImportarDadosTrajetoria)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status400BadRequest)]
        public void ImportarDadosTrajetóriaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(ImportarDadosTrajetóriaController).GetMethod(nameof(ImportarDadosTrajetóriaController.ImportarDadosTrajetoria)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ImportarDadosTrajetóriaDeveRetornarOkResultEmCasoDeSucesso()
        {
            var lista = A.Fake<List<PontoTrajetóriaRequest>>();

            var importarDadosTrajetóriaUseCase = A.Fake<IImportarDadosTrajetóriaUseCase>();
            var presenter = A.Fake<ImportarDadosPresenter>();
            A.CallTo(() => importarDadosTrajetóriaUseCase.Execute(A<string>.Ignored, A<TrajetóriaInput>.Ignored)).Returns(ImportarDadosOutput.ImportadoComSucesso());

            var controller = new ImportarDadosTrajetóriaController(importarDadosTrajetóriaUseCase, presenter);

            // Act
            var result = await controller.ImportarDadosTrajetoria(new ImportarDadosTrajetóriaRequest
            {
                PoçoId = "0",
                PontosTrajetória = lista
            });

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ImportarDadosTrajetóriaDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            var lista = A.Fake<List<PontoTrajetóriaRequest>>();

            var importarDadosTrajetóriaUseCase = A.Fake<IImportarDadosTrajetóriaUseCase>();
            var presenter = A.Fake<ImportarDadosPresenter>();
            A.CallTo(() => importarDadosTrajetóriaUseCase.Execute(A<string>.Ignored, A<TrajetóriaInput>.Ignored)).Returns(ImportarDadosOutput.ImportaçãoComFalha());

            var controller = new ImportarDadosTrajetóriaController(importarDadosTrajetóriaUseCase, presenter);

            // Act
            var result = await controller.ImportarDadosTrajetoria(new ImportarDadosTrajetóriaRequest
            {
                PoçoId = "0",
                PontosTrajetória = lista
            });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

    }
}
