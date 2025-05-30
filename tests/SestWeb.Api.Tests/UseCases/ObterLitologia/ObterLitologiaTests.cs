using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Litologia.ObterLitologia;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologia;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Api.Tests.UseCases.ObterLitologia
{
    [TestFixture]
    public class ObterLitologiaTests
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-litologia/{idLitologia}");
        }

        [Test]
        public void ObterLitologiaDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(
                typeof(PoçosController).GetMethod(nameof(PoçosController.ObterLitologia)),
                typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterLitologiaDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(
                typeof(PoçosController).GetMethod(nameof(PoçosController.ObterLitologia)),
                typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList())
                .HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterLitologiaDeveRetornarNotFoundObjectResultCasoNãoEncontreLitologia()
        {
            // Arrange
            const string idPoço = "idPoço";
            const string idLitologia = "idLitologia";
            var mensagemDeErro = $"Não foi possível encontrar poço com id {idLitologia}.";

            var obterLitologiaUseCase = A.Fake<IObterLitologiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterLitologiaUseCase, presenter);

            A.CallTo(() => obterLitologiaUseCase.Execute(A<string>.Ignored, A<string>.Ignored))
                .Returns(ObterLitologiaOutput.PoçoNãoEncontrado(idLitologia));

            // Act
            var result = await controller.ObterLitologia(idPoço, idLitologia);

            // Assert
            Check.That(result).IsInstanceOf<NotFoundObjectResult>();
            Check.That(((NotFoundObjectResult) result).Value).IsEqualTo(mensagemDeErro);
        }

        [Test]
        public async Task ObterLitologiaDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string idPoço = "idPoço";
            const string idLitologia = "idLitologia";

            var obterLitologiaUseCase = A.Fake<IObterLitologiaUseCase>();
            var presenter = A.Fake<Presenter>();
            var traj = A.Fake<IConversorProfundidade>();
            var controller = new PoçosController(obterLitologiaUseCase, presenter);

            A.CallTo(() => obterLitologiaUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(
                ObterLitologiaOutput.LitologiaObtida(new Litologia(TipoLitologia.Adaptada, traj)));

            // Act
            var result = await controller.ObterLitologia(idPoço, idLitologia);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();

            var okResult = (OkObjectResult) result;

            Check.That(okResult.Value).IsNotNull();

            var valueResult = (Litologia) okResult.Value;

            Check.That(valueResult).IsNotNull();
        }
    }
}