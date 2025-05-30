using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Poço.RenomearPoço;
using SestWeb.Application.UseCases.PoçoUseCases.RenomearPoço;

namespace SestWeb.Api.Tests.UseCases.RenomearPoço
{
    [TestFixture]
    public class RenomearPoçoTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/renomear-poco");
        }

        [Test]
        public void RenomearPoçoDeveTerAtributoHttpPut()
        {
            var attribute = (HttpPutAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.RenomearPoço)), typeof(HttpPutAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void RenomearPoçoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.RenomearPoço)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RenomearPoçoDeveRetornarOkResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string nomePoço = "xpto";

            var renomearPoçoUseCase = A.Fake<IRenomearPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(renomearPoçoUseCase, presenter);

            A.CallTo(() => renomearPoçoUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(RenomearPoçoOutput.PoçoRenomeado());

            // Act
            var result = await controller.RenomearPoço(id, new RenomearPoçoRequest {NomePoço = nomePoço});

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task RenomearPoçoDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            const string nomePoço = "xpto";

            var renomearPoçoUseCase = A.Fake<IRenomearPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(renomearPoçoUseCase, presenter);

            A.CallTo(() => renomearPoçoUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(RenomearPoçoOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.RenomearPoço(id, new RenomearPoçoRequest { NomePoço = nomePoço });

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
        }

        [Test]
        public async Task RenomearPoçoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string nomePoço = "xpto";
            const string mensagemDeErro = "Mensagem de erro.";

            var renomearPoçoUseCase = A.Fake<IRenomearPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(renomearPoçoUseCase, presenter);

            A.CallTo(() => renomearPoçoUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(RenomearPoçoOutput.PoçoNãoRenomeado(mensagemDeErro));

            // Act
            var result = await controller.RenomearPoço(id, new RenomearPoçoRequest { NomePoço = nomePoço });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}