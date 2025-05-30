using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Perfil.RemoverPerfil;
using SestWeb.Application.UseCases.PerfilUseCases.RemoverPerfil;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Api.Tests.UseCases.RemoverPerfil
{
    [TestFixture]
    public class RemoverPerfilTest
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
            Check.That(attribute.Template).IsEqualTo("api/perfis");
        }

        [Test]
        public void RemoverPerfilDeveTerAtributoHttpDelete()
        {
            var attribute = (HttpDeleteAttribute)Attribute.GetCustomAttribute(typeof(PerfisController).GetMethod(nameof(PerfisController.RemoverPerfil)), typeof(HttpDeleteAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(204)]
        [TestCase(400)]
        [TestCase(404)]
        public void RemoverPerfilDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PerfisController).GetMethod(nameof(PerfisController.RemoverPerfil)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RemoverPerfilDeveRetornarNoContentResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string idpoço = "poço";

            var removerPoçoUseCase = A.Fake<IRemoverPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(removerPoçoUseCase, presenter);

            A.CallTo(() => removerPoçoUseCase.Execute(id, idpoço)).Returns(RemoverPerfilOutput.PerfilRemovido());

            // Act
            var result = await controller.RemoverPerfil(id, idpoço);

            // Assert
            Check.That(result as NoContentResult).IsNotNull();
        }

        [Test]
        public async Task RemoverPerfilDeveRetornarNotFoundObjectResultCasoNãoEncontrePerfil()
        {
            // Arrange
            const string id = "id";
            const string idpoço = "id";

            var removerPerfilUseCase = A.Fake<IRemoverPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(removerPerfilUseCase, presenter);

            A.CallTo(() => removerPerfilUseCase.Execute(id, idpoço)).Returns(RemoverPerfilOutput.PerfilNãoEncontrado(id));

            // Act
            var result = await controller.RemoverPerfil(id, idpoço);

            // Assert
            Check.That((result as NotFoundObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task RemoverPerfilDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string idpoço = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var removerPerfilUseCase = A.Fake<IRemoverPerfilUseCase>();

            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(removerPerfilUseCase, presenter);

            A.CallTo(() => removerPerfilUseCase.Execute(id, idpoço)).Returns(RemoverPerfilOutput.PerfilNãoRemovido(mensagemDeErro));

            // Act
            var result = await controller.RemoverPerfil(id, idpoço);

            // Assert
            Check.That((result as BadRequestObjectResult)?.Value).IsNotNull();
        }
    }
}