using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Cálculos.ObterCálculo;
using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.CálculosUseCase.ObterCálculo;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Api.Tests.UseCases.Cálculos
{
    [TestFixture]
    public class ObterCálculoTests
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
            Check.That(attribute.Template).IsEqualTo("api/obter-calculo");
        }

        [Test]
        public void ObterCálculolDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(CálculoController).GetMethod(nameof(CálculoController.ObterCálculo)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void ObterCálculoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(CálculoController).GetMethod(nameof(CálculoController.ObterCálculo)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterCálculolDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil = "DTC";

         var calc = A.Fake<ICálculo>();

            var useCase = A.Fake<IObterCálculoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new CálculoController(useCase, presenter);

            A.CallTo(() => useCase.Execute("1","1")).Returns(ObterCálculoOutput.CálculoObtido(calc));

            // Act
            var result = await controller.ObterCálculo("1","2");

            // Assert
            Check.That((result as OkObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task ObterCálculolDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var useCase = A.Fake<IObterCálculoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new CálculoController(useCase, presenter);

            A.CallTo(() => useCase.Execute(A<string>.Ignored,A<string>.Ignored)).Returns(ObterCálculoOutput.CálculoNãoObtido(mensagemDeErro));

            // Act
            var result = await controller.ObterCálculo("1","1");

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();

            var badRequestResult = (BadRequestObjectResult)result;

            Check.That(badRequestResult.Value).IsEqualTo($"Não foi possível obter cálculo. {mensagemDeErro}");

        }

    }
}
