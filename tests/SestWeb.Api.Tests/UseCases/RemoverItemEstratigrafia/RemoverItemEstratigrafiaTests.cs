using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Estratigrafia.RemoverItemEstratigrafia;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.RemoverItemEstratigrafia;

namespace SestWeb.Api.Tests.UseCases.RemoverItemEstratigrafia
{
    [TestFixture]
    public class RemoverItemEstratigrafiaTests
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/remover-item-estratigrafia");
        }

        [Test]
        public void RemoverItemEstratigrafiaDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(
                typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverItemEstratigrafia)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void AdicionarItemEstratigrafiaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(
                typeof(PoçosController).GetMethod(nameof(PoçosController.RemoverItemEstratigrafia)),
                typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverItemEstratigrafiaUseCaseDeveRetornarOkResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string tipo = "FM";
            const double pm = 1000.0;

            var adicionarItemEstratigrafiaRequest = new RemoverItemEstratigrafiaRequest
            {
                Tipo = tipo,
                Pm = pm
            };

            var adicionarItemEstratigrafiaUseCase = A.Fake<IRemoverItemEstratigrafiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(adicionarItemEstratigrafiaUseCase, presenter);

            A.CallTo(() => adicionarItemEstratigrafiaUseCase.Execute(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored))
                .Returns(RemoverItemEstratigrafiaOutput.ItemEstratigrafiaRemovido());

            // Act
            var result = await controller.RemoverItemEstratigrafia(id, adicionarItemEstratigrafiaRequest);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task AdicionarItemEstratigrafiaDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string id = "id";
            const string tipo = "FM";
            const double pm = 1000.0;

            var adicionarItemEstratigrafiaRequest = new RemoverItemEstratigrafiaRequest
            {
                Tipo = tipo,
                Pm = pm
            };

            var adicionarItemEstratigrafiaUseCase = A.Fake<IRemoverItemEstratigrafiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(adicionarItemEstratigrafiaUseCase, presenter);

            A.CallTo(() => adicionarItemEstratigrafiaUseCase.Execute(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored))
                .Returns(RemoverItemEstratigrafiaOutput.ItemEstratigrafiaNãoRemovido());

            // Act
            var result = await controller.RemoverItemEstratigrafia(id, adicionarItemEstratigrafiaRequest);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}