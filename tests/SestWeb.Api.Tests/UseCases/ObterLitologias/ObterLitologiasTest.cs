using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Litologia.ObterLitologias;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologias;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Api.Tests.UseCases.ObterLitologias
{
    [TestFixture]
    public class ObterLitologiasTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-litologias");
        }

        [Test]
        public void ObterLitologiasDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute) Attribute.GetCustomAttribute(
                typeof(PoçosController).GetMethod(nameof(PoçosController.ObterLitologias)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterLitologiasDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(
                typeof(PoçosController).GetMethod(nameof(PoçosController.ObterLitologias)),
                typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[]) attribute).ToList())
                .HasElementThatMatches(at => at.StatusCode == statusCode);
        }
        
        [Test]
        public async Task ObterLitologiasDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var obterLitologiasUseCase = A.Fake<IObterLitologiasUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterLitologiasUseCase, presenter);
            var traj = A.Fake<IConversorProfundidade>();

            A.CallTo(() => obterLitologiasUseCase.Execute(A<string>.Ignored)).Returns(
                ObterLitologiasOutput.LitologiasObtidas(new List<Litologia> {new Litologia(TipoLitologia.Adaptada, traj)}));

            // Act
            var result = await controller.ObterLitologias(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();

            var okResult = (OkObjectResult) result;

            Check.That(okResult.Value).IsNotNull();

            var valueResult = (List<Litologia>) okResult.Value;

            Check.That(valueResult).CountIs(1);
        }

        [Test]
        public async Task ObterLitologiasDeveRetornarNotFoundObjectResultCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            var mensagemDeErro = $"Não foi possível encontrar poço com id {id}.";

            var obterLitologiasUseCase = A.Fake<IObterLitologiasUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterLitologiasUseCase, presenter);

            A.CallTo(() => obterLitologiasUseCase.Execute(id)).Returns(ObterLitologiasOutput.PoçoNãoEncontrado(id));

            // Act
            var result = await controller.ObterLitologias(id);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
            Check.That(((NotFoundObjectResult)result).Value).IsEqualTo(mensagemDeErro);
        }

        [Test]
        public async Task ObterLitologiasDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterLitologiasUseCase = A.Fake<IObterLitologiasUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterLitologiasUseCase, presenter);

            A.CallTo(() => obterLitologiasUseCase.Execute(id)).Returns(ObterLitologiasOutput.LitologiasNãoObtidas(mensagemDeErro));

            // Act
            var result = await controller.ObterLitologias(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
            Check.That(((BadRequestObjectResult)result).Value).IsEqualTo($"Litologias não obtidas. {mensagemDeErro}");
        }
    }
}