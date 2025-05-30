using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Perfil.ObterQtdPontosPerfil;
using SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Enums;

namespace SestWeb.Api.Tests.UseCases.ObterQtdPontosPerfil
{
    [TestFixture]
    public class ObterQtdPontosPerfilTests
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(PerfisController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(PerfisController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/perfis/obter-qtd-ponto");
        }

        [Test]
        public void ObterPerfilDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PerfisController).GetMethod(nameof(PerfisController.ObterQtdPontosPerfil)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void ObterQtdPontosPerfilDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PerfisController).GetMethod(nameof(PerfisController.ObterQtdPontosPerfil)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterQtdPontosPerfilDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";

            var useCase = A.Fake<IObterQtdPontosPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(useCase, presenter);

            A.CallTo(() => useCase.Execute(A<string>.Ignored, A<ObterQtdPontosPerfilInput>.Ignored)).Returns(ObterQtdPontosPerfilOutput.QtdObtida(1));

            // Act
            var result = await controller.ObterQtdPontosPerfil(id,10,null,TipoDeTrechoEnum.Inicial);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ObterQtdPontosPerfilDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var useCase = A.Fake<IObterQtdPontosPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(useCase, presenter);

            A.CallTo(() => useCase.Execute(A<string>.Ignored, A<ObterQtdPontosPerfilInput>.Ignored)).Returns(ObterQtdPontosPerfilOutput.QtdNãoObtida(mensagemDeErro));

            // Act
            var result = await controller.ObterQtdPontosPerfil(id,10,null,TipoDeTrechoEnum.Inicial);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

    }
}
