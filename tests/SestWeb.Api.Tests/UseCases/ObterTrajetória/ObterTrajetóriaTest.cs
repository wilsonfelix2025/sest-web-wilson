using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Trajetória.ObterTrajetória;
using SestWeb.Application.UseCases.TrajetóriaUseCases.ObterTrajetória;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Api.Tests.UseCases.ObterTrajetória
{
    [TestFixture]
    public class ObterTrajetóriaTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-trajetoria");
        }

        [Test]
        public void ObterTrajetóriaDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterTrajetória)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterTrajetóriaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterTrajetória)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterTrajetóriaDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var obterTrajetóriaUseCase = A.Fake<IObterTrajetóriaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterTrajetóriaUseCase, presenter);

            A.CallTo(() => obterTrajetóriaUseCase.Execute(id)).Returns(ObterTrajetóriaOutput.TrajetóriaObtida(new Trajetória(MétodoDeCálculoDaTrajetória.MinimaCurvatura)));

            // Act
            var result = await controller.ObterTrajetória(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ObterTrajetóriaDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";

            var obterTrajetóriaUseCase = A.Fake<IObterTrajetóriaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterTrajetóriaUseCase, presenter);

            A.CallTo(() => obterTrajetóriaUseCase.Execute(id)).Returns(ObterTrajetóriaOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.ObterTrajetória(id);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
        }

        [Test]
        public async Task ObterTrajetóriaDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterTrajetóriaUseCase = A.Fake<IObterTrajetóriaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterTrajetóriaUseCase, presenter);

            A.CallTo(() => obterTrajetóriaUseCase.Execute(id)).Returns(ObterTrajetóriaOutput.TrajetóriaNãoObtida(mensagemDeErro));

            // Act
            var result = await controller.ObterTrajetória(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}