using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Poço.ObterPoço;
using SestWeb.Application.UseCases.PoçoUseCases.ObterPoço;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Factories;

namespace SestWeb.Api.Tests.UseCases.ObterPoço
{
    [TestFixture]
    public class ObterPoçoTest
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
        public void ObterPoçoDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPoço)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("{id}");
            Check.That(attribute.Name).IsEqualTo("ObterPoço");
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterPoçoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPoço)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterPoçoDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string nomePoço = "NovoPoço";
            const TipoPoço tipoPoço = TipoPoço.Projeto;

            var obterPoçoUseCase = A.Fake<IObterPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPoçoUseCase, presenter);

            A.CallTo(() => obterPoçoUseCase.Execute(id,A<string>.Ignored)).Returns(ObterPoçoOutput.PoçoObtido(PoçoFactory.CriarPoço(nomePoço, nomePoço, tipoPoço)));

            // Act
            var result = await controller.ObterPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ObterPoçoDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";

            var obterPoçoUseCase = A.Fake<IObterPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPoçoUseCase, presenter);

            A.CallTo(() => obterPoçoUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(ObterPoçoOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.ObterPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
        }

        [Test]
        public async Task ObterPoçoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterPoçoUseCase = A.Fake<IObterPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPoçoUseCase, presenter);

            A.CallTo(() => obterPoçoUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(ObterPoçoOutput.PoçoNãoObtido(mensagemDeErro));

            // Act
            var result = await controller.ObterPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}