using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Perfil.ObterPerfisParaTrecho;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho;

namespace SestWeb.Api.Tests.UseCases.ObterPerfisParaTrecho
{
    [TestFixture]
    public class ObterPerfisTrechoTests
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-perfis-trecho");
        }

        [Test]
        public void ObterPerfilDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPerfisParaTrecho)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void ObterPerfilTrechoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPerfisParaTrecho)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterPerfisTrechoDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var useCase = A.Fake<IObterPerfisTrechoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(useCase, presenter);

            A.CallTo(() => useCase.Execute(A<string>.Ignored)).Returns(ObterPerfisTrechoOutput.PerfisObtidos(new ObterPerfisTrechoModel()));

            // Act
            var result = await controller.ObterPerfisParaTrecho(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ObterPerfisTrechoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var useCase = A.Fake<IObterPerfisTrechoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(useCase, presenter);

            A.CallTo(() => useCase.Execute(A<string>.Ignored)).Returns(ObterPerfisTrechoOutput.PerfisNãoObtidos(mensagemDeErro));

            // Act
            var result = await controller.ObterPerfisParaTrecho(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}