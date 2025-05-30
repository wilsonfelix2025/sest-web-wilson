using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Estratigrafia.AdicionarItemEstratigrafia;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia;

namespace SestWeb.Api.Tests.UseCases.AdicionarItemEstratigrafia
{
    [TestFixture]
    public class AdicionarItemEstratigrafiaTests
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/adicionar-item-estratigrafia");
        }

        [Test]
        public void AdicionarItemEstratigrafiaDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(
                typeof(PoçosController).GetMethod(nameof(PoçosController.AdicionarItemEstratigrafia)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void AdicionarItemEstratigrafiaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(
                typeof(PoçosController).GetMethod(nameof(PoçosController.AdicionarItemEstratigrafia)),
                typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task AdicionarItemEstratigrafiaUseCaseDeveRetornarOkResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const double pm = 1000.0;
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string tipo = "FM";
            const string idade = "idade";

            var adicionarItemEstratigrafiaRequest = new AdicionarItemEstratigrafiaRequest
            {
                PM = pm,
                Sigla = sigla,
                Descrição = descrição,
                Tipo = tipo,
                Idade = idade
            };

            var adicionarItemEstratigrafiaUseCase = A.Fake<IAdicionarItemEstratigrafiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(adicionarItemEstratigrafiaUseCase, presenter);

            A.CallTo(() => adicionarItemEstratigrafiaUseCase.Execute(A<AdicionarItemEstratigrafiaInput>.Ignored))
                .Returns(AdicionarItemEstratigrafiaOutput.ItemEstratigrafiaAdicionado());

            // Act
            var result = await controller.AdicionarItemEstratigrafia(id, adicionarItemEstratigrafiaRequest);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task AdicionarItemEstratigrafiaDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string id = "id";
            const double pm = 1000.0;
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string tipo = "FM";
            const string idade = "idade";

            var adicionarItemEstratigrafiaRequest = new AdicionarItemEstratigrafiaRequest
            {
                PM = pm,
                Sigla = sigla,
                Descrição = descrição,
                Tipo = tipo,
                Idade = idade
            };

            var adicionarItemEstratigrafiaUseCase = A.Fake<IAdicionarItemEstratigrafiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(adicionarItemEstratigrafiaUseCase, presenter);

            A.CallTo(() => adicionarItemEstratigrafiaUseCase.Execute(A<AdicionarItemEstratigrafiaInput>.Ignored))
                .Returns(AdicionarItemEstratigrafiaOutput.ItemEstratigrafiaNãoAdicionado());

            // Act
            var result = await controller.AdicionarItemEstratigrafia(id, adicionarItemEstratigrafiaRequest);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}