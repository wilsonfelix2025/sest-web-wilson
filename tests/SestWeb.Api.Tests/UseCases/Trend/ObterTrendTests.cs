
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Trend.ObterTrend;
using SestWeb.Application.UseCases.TrendUseCases.ObterTrend;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.Tests.UseCases.Trend
{
    [TestFixture]
    public class ObterTrendTests
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
            Check.That(attribute.Template).IsEqualTo("api/obter-trend");
        }

        [Test]
        public void EditarTrendDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(TrendController).GetMethod(nameof(TrendController.ObterTrend)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void ObterTrendDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(TrendController).GetMethod(nameof(TrendController.ObterTrend)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterTrendDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var useCase = A.Fake<IObterTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);

            A.CallTo(() => useCase.Execute(id)).Returns(ObterTrendOutput.TrendObtido(null));

            // Act
            var result = await controller.ObterTrend(id);

            // Assert
            Check.That((result as OkObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task ObterTrendDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var useCase = A.Fake<IObterTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);

            A.CallTo(() => useCase.Execute(id)).Returns(ObterTrendOutput.TrendNãoObtido(mensagemDeErro));

            // Act
            var result = await controller.ObterTrend(id);

            // Assert
            Check.That((result as BadRequestObjectResult)?.Value).IsNotNull();
        }
    }

}

