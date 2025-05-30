using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Poço.ObterDadosGerais;
using SestWeb.Application.UseCases.PoçoUseCases.ObterDadosGerais;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Api.Tests.UseCases.ObterDadosGerais
{
    [TestFixture]
    public class ObterDadosGeraisTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-dados-gerais");
        }

        [Test]
        public void ObterDadosGeraisDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterDadosGerais)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterDadosGeraisDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterDadosGerais)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterDadosGeraisDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var obterPoçoUseCase = A.Fake<IObterDadosGeraisUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPoçoUseCase, presenter);

            A.CallTo(() => obterPoçoUseCase.Execute(id)).Returns(ObterDadosGeraisOutput.DadosGeraisObtidos(new DadosGerais()));

            // Act
            var result = await controller.ObterDadosGerais(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();

            var okResult = (OkObjectResult)result;

            Check.That(okResult.Value).IsNotNull();
            Check.That(okResult.Value).IsInstanceOf<DadosGerais>();
        }

        [Test]
        public async Task ObterDadosGeraisDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";

            var obterPoçoUseCase = A.Fake<IObterDadosGeraisUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPoçoUseCase, presenter);

            A.CallTo(() => obterPoçoUseCase.Execute(id)).Returns(ObterDadosGeraisOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.ObterDadosGerais(id);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
            Check.That(((NotFoundObjectResult)result).Value).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task ObterDadosGeraisDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterPoçoUseCase = A.Fake<IObterDadosGeraisUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPoçoUseCase, presenter);

            A.CallTo(() => obterPoçoUseCase.Execute(id)).Returns(ObterDadosGeraisOutput.DadosGeraisNãoObtidos(mensagemDeErro));

            // Act
            var result = await controller.ObterDadosGerais(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
            Check.That(((BadRequestObjectResult)result).Value).IsEqualTo($"Não foi possível obter dados gerais. {mensagemDeErro}");
        }
    }
}