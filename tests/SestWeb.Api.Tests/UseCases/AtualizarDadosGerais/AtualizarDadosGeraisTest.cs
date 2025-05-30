using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Poço.AtualizarDadosGerais;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais;

namespace SestWeb.Api.Tests.UseCases.AtualizarDadosGerais
{
    [TestFixture]
    public class AtualizarDadosGeraisTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/atualizar-dados-gerais");
        }

        [Test]
        public void AtualizarDadosGeraisDeveTerAtributoHttpPut()
        {
            var attribute = (HttpPutAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.AtualizarDadosGerais)), typeof(HttpPutAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void AtualizarDadosGeraisDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.AtualizarDadosGerais)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task AtualizarDadosGeraisDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var atualizarDadosGeraisUseCase = A.Fake<IAtualizarDadosGeraisUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(atualizarDadosGeraisUseCase, presenter);

            A.CallTo(() => atualizarDadosGeraisUseCase.Execute(id, A<AtualizarDadosGeraisInput>.Ignored)).Returns(AtualizarDadosGeraisOutput.DadosGeraisAtualizados());

            // Act
            var result = await controller.AtualizarDadosGerais(id, new AtualizarDadosGeraisInput());

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task AtualizarDadosGeraisDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";

            var atualizarDadosGeraisUseCase = A.Fake<IAtualizarDadosGeraisUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(atualizarDadosGeraisUseCase, presenter);

            A.CallTo(() => atualizarDadosGeraisUseCase.Execute(id, A<AtualizarDadosGeraisInput>.Ignored)).Returns(AtualizarDadosGeraisOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.AtualizarDadosGerais(id, new AtualizarDadosGeraisInput());

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
        }

        [Test]
        public async Task AtualizarDadosGeraisDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";

            var atualizarDadosGeraisUseCase = A.Fake<IAtualizarDadosGeraisUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(atualizarDadosGeraisUseCase, presenter);

            A.CallTo(() => atualizarDadosGeraisUseCase.Execute(id, A<AtualizarDadosGeraisInput>.Ignored)).Returns(AtualizarDadosGeraisOutput.DadosGeraisNãoAtualizados());

            // Act
            var result = await controller.AtualizarDadosGerais(id, new AtualizarDadosGeraisInput());

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}