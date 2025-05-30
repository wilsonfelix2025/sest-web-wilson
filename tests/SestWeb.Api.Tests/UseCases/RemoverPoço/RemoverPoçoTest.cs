using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Poço.RemoverPoço;
using SestWeb.Application.UseCases.PoçoUseCases.RemoverPoço;

namespace SestWeb.Api.Tests.UseCases.RemoverPoço
{
    [TestFixture]
    public class RemoverPoçoTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos");
        }

        [Test]
        public void RemoverPoçoDeveTerAtributoHttpDelete()
        {
            var attribute = (HttpDeleteAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverPoço)), typeof(HttpDeleteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("{id}");
        }

        [TestCase(204)]
        [TestCase(400)]
        [TestCase(404)]
        public void RemoverPoçoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverPoço)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverPoçoDeveRetornarNoContentResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var removerPoçoUseCase = A.Fake<IRemoverPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerPoçoUseCase, presenter);

            A.CallTo(() => removerPoçoUseCase.Execute(id)).Returns(RemoverPoçoOutput.PoçoRemovido());

            // Act
            var result = await controller.RemoverPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<NoContentResult>();
        }

        [Test]
        public async Task RemoverPoçoDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";

            var removerPoçoUseCase = A.Fake<IRemoverPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerPoçoUseCase, presenter);

            A.CallTo(() => removerPoçoUseCase.Execute(id)).Returns(RemoverPoçoOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.RemoverPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
            Check.That(((NotFoundObjectResult)result).Value).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task RemoverPoçoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var removerPoçoUseCase = A.Fake<IRemoverPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerPoçoUseCase, presenter);

            A.CallTo(() => removerPoçoUseCase.Execute(id)).Returns(RemoverPoçoOutput.PoçoNãoRemovido(mensagemDeErro));

            // Act
            var result = await controller.RemoverPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
            Check.That(((BadRequestObjectResult)result).Value).IsEqualTo($"Não foi possível remover poço. {mensagemDeErro}");
        }
    }
}