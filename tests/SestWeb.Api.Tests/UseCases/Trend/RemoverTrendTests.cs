
using NFluent;
using NUnit.Framework;
using System;
using SestWeb.Api.UseCases.Trend.RemoverTrend;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using SestWeb.Application.UseCases.TrendUseCases.RemoverTrend;

namespace SestWeb.Api.Tests.UseCases.Trend
{
    [TestFixture]
    public class RemoverTrendTests
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(TrendController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(TrendController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/remover-trend");
        }

        [Test]
        public void RemoverTrendDeveTerAtributoHttpDelete()
        {
            var attribute = (HttpDeleteAttribute)Attribute.GetCustomAttribute(typeof(TrendController).GetMethod(nameof(TrendController.RemoverTrend)), typeof(HttpDeleteAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(204)]
        [TestCase(400)]
        public void RemoverTrendDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(TrendController).GetMethod(nameof(TrendController.RemoverTrend)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverTrendDeveRetornarNoContentResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var useCase = A.Fake<IRemoverTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);

            A.CallTo(() => useCase.Execute(id)).Returns(RemoverTrendOutput.TrendRemovido());

            // Act
            var result = await controller.RemoverTrend(id);

            // Assert
            Check.That(result as NoContentResult).IsNotNull();
        }

        [Test]
        public async Task RemoverTrendDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var useCase = A.Fake<IRemoverTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);

            A.CallTo(() => useCase.Execute(id)).Returns(RemoverTrendOutput.TrendNãoRemovido(mensagemDeErro));

            // Act
            var result = await controller.RemoverTrend(id);

            // Assert
            Check.That((result as BadRequestObjectResult)?.Value).IsNotNull();
        }
    }
}
