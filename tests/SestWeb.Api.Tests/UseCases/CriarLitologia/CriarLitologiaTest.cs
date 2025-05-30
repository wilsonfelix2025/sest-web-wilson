using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Litologia.CriarLitologia;
using SestWeb.Application.UseCases.LitologiaUseCases.CriarLitologia;

namespace SestWeb.Api.Tests.UseCases.CriarLitologia
{
    [TestFixture]
    public class CriarLitologiaTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(PoçosController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute =
                (RouteAttribute)Attribute.GetCustomAttribute(typeof(PoçosController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/criar-litologia");
        }

        [Test]
        public void CriarLitologiacDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.CriarLitologia)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(201)]
        [TestCase(400)]
        public void CriarLitologiaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.CriarLitologia)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task CriarLitologiaDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string id = "id";
            const string tipoLitologia = "Prevista";

            var criarLitologiaRequest = new CriarLitologiaRequest { TipoLitologia = tipoLitologia };

            var criarLitologiaUseCase = A.Fake<ICriarLitologiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarLitologiaUseCase, presenter);

            A.CallTo(() => criarLitologiaUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(CriarLitologiaOutput.LitologiaNãoCriada());

            // Act
            var result = await controller.CriarLitologia(id, criarLitologiaRequest);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

        [Test]
        public async Task CriarLitologiaDeveRetornarCreatedResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string tipoLitologia = "Prevista";

            var criarLitologiaRequest = new CriarLitologiaRequest { TipoLitologia = tipoLitologia };

            var criarLitologiaUseCase = A.Fake<ICriarLitologiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarLitologiaUseCase, presenter);

            A.CallTo(() => criarLitologiaUseCase.Execute(id, tipoLitologia)).Returns(CriarLitologiaOutput.LitologiaCriada());
            
            // Act
            var result = await controller.CriarLitologia(id, criarLitologiaRequest);

            // Assert
            Check.That(result).IsInstanceOf<CreatedResult>();
        }
    }
}