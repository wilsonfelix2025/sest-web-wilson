using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Objetivos.ObterObjetivos;
using SestWeb.Application.UseCases.ObjetivoUseCases.ObterObjetivos;
using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Api.Tests.UseCases.ObterObjetivos
{
    [TestFixture]
    public class ObterObjetivosTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-objetivos");
        }

        [Test]
        public void ObterObjetivosDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterObjetivos)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterObjetivosDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterObjetivos)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterObjetivosDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var obterObjetivosUseCase = A.Fake<IObterObjetivosUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterObjetivosUseCase, presenter);

            A.CallTo(() => obterObjetivosUseCase.Execute(A<string>.Ignored)).Returns(ObterObjetivosOutput.ObjetivosObtidos(new List<Objetivo>()));

            // Act
            var result = await controller.ObterObjetivos(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();

            var okResult = (OkObjectResult)result;

            Check.That(okResult.Value).IsNotNull();
        }

        [Test]
        public async Task ObterObjetivosDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            var mensagemDeErro = $"Não foi possível encontrar poço com id {id}.";

            var obterObjetivosUseCase = A.Fake<IObterObjetivosUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterObjetivosUseCase, presenter);

            A.CallTo(() => obterObjetivosUseCase.Execute(id)).Returns(ObterObjetivosOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.ObterObjetivos(id);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
            Check.That(((NotFoundObjectResult)result).Value).IsEqualTo(mensagemDeErro);
        }

        [Test]
        public async Task ObterObjetivosDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterObjetivosUseCase = A.Fake<IObterObjetivosUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterObjetivosUseCase, presenter);

            A.CallTo(() => obterObjetivosUseCase.Execute(id)).Returns(ObterObjetivosOutput.ObjetivosNãoObtidos(mensagemDeErro));

            // Act
            var result = await controller.ObterObjetivos(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
            Check.That(((BadRequestObjectResult)result).Value).IsEqualTo($"Não foi possível obter objetivos. {mensagemDeErro}");
        }
    }
}