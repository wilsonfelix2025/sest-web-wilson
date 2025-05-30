using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Sapatas.ObterSapatas;
using SestWeb.Application.UseCases.SapataUseCases.ObterSapatas;
using SestWeb.Domain.Entities.Poço.Sapatas;

namespace SestWeb.Api.Tests.UseCases.ObterSapatas
{
    [TestFixture]
    public class ObterSapatasTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(PoçosController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(PoçosController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-sapatas");
        }

        [Test]
        public void ObterSapatasDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterSapatas)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterSapatasDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterSapatas)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterSapatasDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var obterSapatasUseCase = A.Fake<IObterSapatasUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterSapatasUseCase, presenter);

            A.CallTo(() => obterSapatasUseCase.Execute(A<string>.Ignored)).Returns(ObterSapatasOutput.SapatasObtidas(new List<Sapata>()));

            // Act
            var result = await controller.ObterSapatas(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ObterSapatasDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";

            var obterSapatasUseCase = A.Fake<IObterSapatasUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterSapatasUseCase, presenter);

            A.CallTo(() => obterSapatasUseCase.Execute(id)).Returns(ObterSapatasOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.ObterSapatas(id);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
        }

        [Test]
        public async Task ObterSapatasDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterSapatasUseCase = A.Fake<IObterSapatasUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterSapatasUseCase, presenter);

            A.CallTo(() => obterSapatasUseCase.Execute(id)).Returns(ObterSapatasOutput.SapatasNãoObtidas(mensagemDeErro));

            // Act
            var result = await controller.ObterSapatas(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}