using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Perfil.ObterPerfisDeUmPoço;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Api.Tests.UseCases.ObterPerfisDeUmPoço
{
    [TestFixture]
    public class ObterPerfisTrechoTests
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
            Check.That(attribute.Template).IsEqualTo("api/pocos/{id}/obter-perfis");
        }

        [Test]
        public void ObterPerfilDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPerfisDeUmPoço)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void ObterPerfilDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPerfisDeUmPoço)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterPerfisPorTipoDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var obterPerfisPorTipoUseCase = A.Fake<IObterPerfisDeUmPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPerfisPorTipoUseCase, presenter);

            A.CallTo(() => obterPerfisPorTipoUseCase.Execute(A<string>.Ignored)).Returns(ObterPerfisDeUmPoçoOutput.PerfisObtidos(new List<PerfilBase>()));

            // Act
            var result = await controller.ObterPerfisDeUmPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ObterPerfisPorTipoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterPerfisPorTipoUseCase = A.Fake<IObterPerfisDeUmPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPerfisPorTipoUseCase, presenter);

            A.CallTo(() => obterPerfisPorTipoUseCase.Execute(A<string>.Ignored)).Returns(ObterPerfisDeUmPoçoOutput.PerfisNãoObtidos(mensagemDeErro));

            // Act
            var result = await controller.ObterPerfisDeUmPoço(id);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}