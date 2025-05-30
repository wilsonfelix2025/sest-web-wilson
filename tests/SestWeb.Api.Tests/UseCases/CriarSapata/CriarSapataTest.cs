using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Sapatas.CriarSapata;
using SestWeb.Application.UseCases.SapataUseCases.CriarSapata;

namespace SestWeb.Api.Tests.UseCases.CriarSapata
{
    [TestFixture]
    public class CriarSapataTest
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
                (RouteAttribute) Attribute.GetCustomAttribute(typeof(PoçosController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/criar-sapata");
        }

        [Test]
        public void CriarSapataDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(
                typeof(PoçosController).GetMethod(nameof(PoçosController.CriarSapata)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(201)]
        [TestCase(400)]
        public void CriarSapataDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(
                typeof(PoçosController).GetMethod(nameof(PoçosController.CriarSapata)),
                typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList())
                .HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task CriarSapataDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string id = "id";
            const double pm = double.NaN;
            const string diâmetro = "aaa";

            var criarSapataInput = new CriarSapataRequest {ProfundidadeMedida = pm, DiâmetroSapata = diâmetro};

            var criarSapataUseCase = A.Fake<ICriarSapataUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarSapataUseCase, presenter);

            A.CallTo(() => criarSapataUseCase.Execute(A<string>.Ignored, A<double>.Ignored, A<string>.Ignored))
                .Returns(CriarSapataOutput.SapataNãoCriada());

            // Act
            var result = await controller.CriarSapata(id, criarSapataInput);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

        [Test]
        public async Task CriarSapataDeveRetornarCreatedResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000;
            const string diâmetro = "12 1/2";

            var criarSapataInput = new CriarSapataRequest {ProfundidadeMedida = pm, DiâmetroSapata = diâmetro};

            var criarSapataUseCase = A.Fake<ICriarSapataUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarSapataUseCase, presenter);

            A.CallTo(() => criarSapataUseCase.Execute(id, pm, diâmetro)).Returns(CriarSapataOutput.SapataCriada());

            // Act
            var result = await controller.CriarSapata(id, criarSapataInput);

            // Assert
            Check.That(result).IsInstanceOf<CreatedResult>();
        }
    }
}