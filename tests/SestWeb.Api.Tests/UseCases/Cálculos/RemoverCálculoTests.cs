
using NFluent;
using NUnit.Framework;
using System;
using SestWeb.Api.UseCases.Cálculos.RemoverCálculo;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using SestWeb.Application.UseCases.CálculosUseCase.RemoverCálculo;

namespace SestWeb.Api.Tests.UseCases.Cálculos
{
    [TestFixture]
    public class RemoverCálculoTests
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(CálculoController), typeof(ApiControllerAttribute))).IsTrue();

        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(CálculoController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/remover-calculo");
        }

        [Test]
        public void RemoverCálculoDeveTerAtributoHttpDelete()
        {
            var attribute = (HttpDeleteAttribute)Attribute.GetCustomAttribute(typeof(CálculoController).GetMethod(nameof(CálculoController.RemoverCálculo)), typeof(HttpDeleteAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(204)]
        [TestCase(400)]
        [TestCase(404)]
        public void RemoverCálculolDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(CálculoController).GetMethod(nameof(CálculoController.RemoverCálculo)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverCálculoDeveRetornarNoContentResultEmCasoDeSucesso()
        {
            // Arrange

            var useCase = A.Fake<IRemoverCálculoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new CálculoController(useCase, presenter);

            A.CallTo(() => useCase.Execute("1","1")).Returns(RemoverCálculoOutput.CálculoRemovido());

            // Act
            var result = await controller.RemoverCálculo("1","1");

            // Assert
            Check.That(result as NoContentResult).IsNotNull();
        }

        [Test]
        public async Task RemoverCálculoDeveRetornarNotFoundObjectResultCasoNãoEncontreCálculo()
        {
            // Arrange
            
            var useCase = A.Fake<IRemoverCálculoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new CálculoController(useCase, presenter);

            A.CallTo(() => useCase.Execute("1","1")).Returns(RemoverCálculoOutput.CálculoNãoEncontrado());

            // Act
            var result = await controller.RemoverCálculo("1","1");

            // Assert
            Check.That((result as NotFoundObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task RemoverCálculoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var useCase = A.Fake<IRemoverCálculoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new CálculoController(useCase, presenter);

            A.CallTo(() => useCase.Execute("1","1")).Returns(RemoverCálculoOutput.CálculoNãoRemovido(mensagemDeErro));

            // Act
            var result = await controller.RemoverCálculo("1","1");

            // Assert
            Check.That((result as BadRequestObjectResult)?.Value).IsNotNull();
        }

    }
}
