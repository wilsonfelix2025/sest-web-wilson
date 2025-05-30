using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Sapatas.RemoverSapata;
using SestWeb.Application.UseCases.SapataUseCases.RemoverSapata;

namespace SestWeb.Api.Tests.UseCases.RemoverSapata
{
    [TestFixture]
    public class RemoverSapataTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/remover-sapata");
        }

        [Test]
        public void RemoverSapataDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverSapata)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(204)]
        [TestCase(400)]
        [TestCase(404)]
        public void RemoverSapataDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverSapata)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverSapataDeveRetornarNoContentResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var removerSapataUseCase = A.Fake<IRemoverSapataUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerSapataUseCase, presenter);

            A.CallTo(() => removerSapataUseCase.Execute(A<string>.Ignored, A<double>.Ignored)).Returns(RemoverSapataOutput.SapataRemovida());

            // Act
            var result = await controller.RemoverSapata(id, new RemoverSapataRequest { ProfundidadeMedida = pm });

            // Assert
            Check.That(result).IsInstanceOf<NoContentResult>();
        }

        [Test]
        public async Task RemoverSapataDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var removerSapataUseCase = A.Fake<IRemoverSapataUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerSapataUseCase, presenter);

            A.CallTo(() => removerSapataUseCase.Execute(A<string>.Ignored, A<double>.Ignored)).Returns(RemoverSapataOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.RemoverSapata(id, new RemoverSapataRequest { ProfundidadeMedida = pm });

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
            Check.That(((NotFoundObjectResult)result).Value).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task RemoverSapataDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;
            const string mensagemDeErro = "Mensagem de erro.";

            var removerSapataUseCase = A.Fake<IRemoverSapataUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerSapataUseCase, presenter);

            A.CallTo(() => removerSapataUseCase.Execute(A<string>.Ignored, A<double>.Ignored)).Returns(RemoverSapataOutput.SapataNãoRemovida(mensagemDeErro));

            // Act
            var result = await controller.RemoverSapata(id, new RemoverSapataRequest { ProfundidadeMedida = pm });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
            Check.That(((BadRequestObjectResult)result).Value).IsEqualTo($"Não foi possível remover a sapata. {mensagemDeErro}");
        }
    }
}