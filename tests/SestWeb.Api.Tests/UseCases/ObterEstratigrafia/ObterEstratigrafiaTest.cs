using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Estratigrafia.ObterEstratigrafia;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.ObterEstratigrafia;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;

namespace SestWeb.Api.Tests.UseCases.ObterEstratigrafia
{
    [TestFixture]
    public class ObterEstratigrafiaTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-estratigrafia");
        }

        [Test]
        public void ObterEstratigrafiaDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterEstratigrafia)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterEstratigrafiaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterEstratigrafia)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterEstratigrafiaDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var obterEstratigrafiaUseCase = A.Fake<IObterEstratigrafiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterEstratigrafiaUseCase, presenter);

            A.CallTo(() => obterEstratigrafiaUseCase.Execute(id)).Returns(ObterEstratigrafiaOutput.EstratigrafiaObtida(new Estratigrafia()));

            // Act
            var result = await controller.ObterEstratigrafia(id);

            // Assert
            Check.That((result as OkObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task ObterEstratigrafiaDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";

            var obterEstratigrafiaUseCase = A.Fake<IObterEstratigrafiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterEstratigrafiaUseCase, presenter);

            A.CallTo(() => obterEstratigrafiaUseCase.Execute(id)).Returns(ObterEstratigrafiaOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.ObterEstratigrafia(id);

            // Assert
            Check.That((result as NotFoundObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task ObterEstratigrafiaDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterEstratigrafiaUseCase = A.Fake<IObterEstratigrafiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterEstratigrafiaUseCase, presenter);

            A.CallTo(() => obterEstratigrafiaUseCase.Execute(id)).Returns(ObterEstratigrafiaOutput.EstratigrafiaNãoObtida(mensagemDeErro));

            // Act
            var result = await controller.ObterEstratigrafia(id);

            // Assert
            Check.That((result as BadRequestObjectResult)?.Value).IsNotNull();
        }
    }
}