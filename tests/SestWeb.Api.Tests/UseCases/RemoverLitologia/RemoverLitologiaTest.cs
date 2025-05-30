using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Litologia.RemoverLitologia;
using SestWeb.Application.UseCases.LitologiaUseCases.RemoverLitologia;

namespace SestWeb.Api.Tests.UseCases.RemoverLitologia
{
    [TestFixture]
    public class RemoverLitologiaTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/remover-litologia/{idLitologia}");
        }

        [Test]
        public void RemoverSapataDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverLitologia)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(204)]
        [TestCase(400)]
        [TestCase(404)]
        public void RemoverSapataDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverLitologia)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverLitologiaDeveRetornarNoContentResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";

            var removerLitologiaUseCase = A.Fake<IRemoverLitologiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerLitologiaUseCase, presenter);

            A.CallTo(() => removerLitologiaUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(RemoverLitologiaOutput.LitologiaRemovida());

            // Act
            var result = await controller.RemoverLitologia(id, idLitologia);

            // Assert
            Check.That(result).IsInstanceOf<NoContentResult>();
        }

        [Test]
        public async Task RemoverSapataDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";

            var removerLitologiaUseCase = A.Fake<IRemoverLitologiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerLitologiaUseCase, presenter);

            A.CallTo(() => removerLitologiaUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(RemoverLitologiaOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.RemoverLitologia(id, idLitologia);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
        }

        [Test]
        public async Task RemoverSapataDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";
            const string mensagemDeErro = "Mensagem de erro.";

            var removerSapataUseCase = A.Fake<IRemoverLitologiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(removerSapataUseCase, presenter);

            A.CallTo(() => removerSapataUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(RemoverLitologiaOutput.LitologiaNãoRemovida(mensagemDeErro));

            // Act
            var result = await controller.RemoverLitologia(id, idLitologia);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}