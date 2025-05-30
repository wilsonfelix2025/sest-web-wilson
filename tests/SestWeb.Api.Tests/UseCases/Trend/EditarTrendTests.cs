
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Trend.EditarTrend;
using SestWeb.Application.UseCases.TrendUseCases.EditarTrend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.Tests.UseCases.Trend
{
    [TestFixture]
    public class EditarTrendTests
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
            Check.That(attribute.Template).IsEqualTo("api/editar-trend");
        }

        [Test]
        public void EditarTrendDeveTerAtributoHttpPut()
        {
            var attribute = (HttpPutAttribute)Attribute.GetCustomAttribute(typeof(TrendController).GetMethod(nameof(TrendController.EditarTrend)), typeof(HttpPutAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void EditarTrendDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(TrendController).GetMethod(nameof(TrendController.EditarTrend)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task EditarTrendDeveRetornarCreatedEmCasoDeSucesso()
        {
            const string mnemônico = "DTC";
            const string nomePerfil = "Perfil1";

            var useCase = A.Fake<IEditarTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);
            var request = new EditarTrendRequest
            {
                Trechos = new List<EditarTrechosItem>(),
                NomeTrend = "nome",
                IdPerfil = "1234"
            };


            A.CallTo(() => useCase.Execute(A<List<EditarTrechosInput>>.Ignored,A<string>.Ignored, A<string>.Ignored)).Returns(EditarTrendOutput.TrendEditado(null, null));

            // Act
            var result = await controller.EditarTrend(request);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();

        }

        [Test]
        public async Task EditarTrendDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string mensagem = "Mensagem de erro.";

            var useCase = A.Fake<IEditarTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);
            var request = new EditarTrendRequest
            {
                Trechos = new List<EditarTrechosItem>(),
                NomeTrend = "nome",
                IdPerfil = "1234"
            };

            A.CallTo(() => useCase.Execute(A<List<EditarTrechosInput>>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(EditarTrendOutput.TrendNãoEditado(mensagem));

            // Act
            var result = await controller.EditarTrend(request);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();

            var badRequestResult = (BadRequestObjectResult)result;

            Check.That(badRequestResult.Value).IsEqualTo($"Não foi possível editar trend. {mensagem}");
        }

    }
}
