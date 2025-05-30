using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Objetivos.RemoverObjetivo;
using SestWeb.Application.UseCases.ObjetivoUseCases.RemoverObjetivo;

namespace SestWeb.Api.Tests.UseCases.RemoverObjetivo
{
    [TestFixture]
    public class RemoverObjetivoTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/remover-objetivo");
        }

        [Test]
        public void RemoverObjetivoDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverObjetivo)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(204)]
        [TestCase(400)]
        [TestCase(404)]
        public void RemoverObjetivoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverObjetivo)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverObjetivoDeveRetornarNoContentResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var removerObjetivoUseCase = A.Fake<IRemoverObjetivoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerObjetivoUseCase, presenter);

            A.CallTo(() => removerObjetivoUseCase.Execute(A<string>.Ignored, A<double>.Ignored)).Returns(RemoverObjetivoOutput.ObjetivoRemovido());

            // Act
            var result = await controller.RemoverObjetivo(id, new RemoverObjetivoRequest { ProfundidadeMedida = pm });

            // Assert
            Check.That(result).IsInstanceOf<NoContentResult>();
        }

        [Test]
        public async Task RemoverObjetivoDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var removerObjetivoUseCase = A.Fake<IRemoverObjetivoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerObjetivoUseCase, presenter);

            A.CallTo(() => removerObjetivoUseCase.Execute(A<string>.Ignored, A<double>.Ignored)).Returns(RemoverObjetivoOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.RemoverObjetivo(id, new RemoverObjetivoRequest { ProfundidadeMedida = pm });

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
            Check.That(((NotFoundObjectResult)result).Value).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task RemoverObjetivoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;
            const string mensagemDeErro = "Mensagem de erro.";

            var removerObjetivoUseCase = A.Fake<IRemoverObjetivoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerObjetivoUseCase, presenter);

            A.CallTo(() => removerObjetivoUseCase.Execute(A<string>.Ignored, A<double>.Ignored)).Returns(RemoverObjetivoOutput.ObjetivoNãoRemovido(mensagemDeErro));

            // Act
            var result = await controller.RemoverObjetivo(id, new RemoverObjetivoRequest { ProfundidadeMedida = pm });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
            Check.That(((BadRequestObjectResult)result).Value).IsEqualTo($"Não foi possível remover o objetivo. {mensagemDeErro}");
        }
    }
}