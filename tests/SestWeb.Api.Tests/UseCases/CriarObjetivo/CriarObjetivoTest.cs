using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Objetivos.CriarObjetivo;
using SestWeb.Application.UseCases.ObjetivoUseCases.CriarObjetivo;
using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Api.Tests.UseCases.CriarObjetivo
{
    [TestFixture]
    public class CriarObjetivoTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/criar-objetivo");
        }

        [Test]
        public void CriarObjetivocDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute) Attribute.GetCustomAttribute(
                typeof(PoçosController).GetMethod(nameof(PoçosController.CriarObjetivo)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(201)]
        [TestCase(400)]
        public void CriarObjetivoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(
                typeof(PoçosController).GetMethod(nameof(PoçosController.CriarObjetivo)),
                typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList())
                .HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task CriarObjetivoDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string id = "id";
            const double pm = double.NaN;
            const TipoObjetivo tipoObjetivo = TipoObjetivo.Primário;

            var criarObjetivoRequest = new CriarObjetivoRequest {ProfundidadeMedida = pm, TipoObjetivo = tipoObjetivo};

            var criarObjetivoUseCase = A.Fake<ICriarObjetivoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarObjetivoUseCase, presenter);

            A.CallTo(() => criarObjetivoUseCase.Execute(A<string>.Ignored, A<double>.Ignored, A<TipoObjetivo>.Ignored))
                .Returns(CriarObjetivoOutput.ObjetivoNãoCriado());

            // Act
            var result = await controller.CriarObjetivo(id, criarObjetivoRequest);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

        [Test]
        public async Task CriarObjetivoDeveRetornarCreatedResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000;
            const TipoObjetivo tipoObjetivo = TipoObjetivo.Primário;

            var criarObjetivoRequest = new CriarObjetivoRequest {ProfundidadeMedida = pm, TipoObjetivo = tipoObjetivo};

            var criarObjetivoUseCase = A.Fake<ICriarObjetivoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarObjetivoUseCase, presenter);

            A.CallTo(() => criarObjetivoUseCase.Execute(id, pm, tipoObjetivo))
                .Returns(CriarObjetivoOutput.ObjetivoCriado());

            // Act
            var result = await controller.CriarObjetivo(id, criarObjetivoRequest);

            // Assert
            Check.That(result).IsInstanceOf<CreatedResult>();
        }
    }
}