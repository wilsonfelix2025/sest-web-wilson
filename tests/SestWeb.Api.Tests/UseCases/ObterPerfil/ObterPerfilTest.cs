using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Perfil.ObterPerfil;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfil;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Api.Tests.UseCases.ObterPerfil
{
    [TestFixture]
    public class ObterPerfilTest
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
        public void ObterPerfilDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PerfisController).GetMethod(nameof(PerfisController.ObterPerfil)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("{id}");
            Check.That(attribute.Name).IsEqualTo("ObterPerfil");
        }

        [TestCase(200)]
        [TestCase(400)]
        [TestCase(404)]
        public void ObterPerfilDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PerfisController).GetMethod(nameof(PerfisController.ObterPerfil)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterPerfilDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil = "DTC";

            //var perfilFactory = PerfilOld.GetFactory(A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            //var perfil = perfilFactory.CriarPerfil<DTC>(nomePerfil);
            var perfil = PerfisFactory.Create(nomePerfil, nomePerfil, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());

            var obterPerfilUseCase = A.Fake<IObterPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(obterPerfilUseCase, presenter);

            A.CallTo(() => obterPerfilUseCase.Execute(id)).Returns(ObterPerfilOutput.PerfilObtido(perfil));

            // Act
            var result = await controller.ObterPerfil(id);

            // Assert
            Check.That((result as OkObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task ObterPerfilDeveRetornarNotFoundObjectResultCasoNãoEncontrePerfil()
        {
            // Arrange
            const string id = "id";

            var obterPerfilUseCase = A.Fake<IObterPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(obterPerfilUseCase, presenter);

            A.CallTo(() => obterPerfilUseCase.Execute(id)).Returns(ObterPerfilOutput.PerfilNãoEncontrado(id));

            // Act
            var result = await controller.ObterPerfil(id);

            // Assert
            Check.That((result as NotFoundObjectResult)?.Value).IsNotNull();
        }

        [Test]
        public async Task ObterPerfilDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var obterPerfilUseCase = A.Fake<IObterPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(obterPerfilUseCase, presenter);

            A.CallTo(() => obterPerfilUseCase.Execute(id)).Returns(ObterPerfilOutput.PerfilNãoObtido(mensagemDeErro));

            // Act
            var result = await controller.ObterPerfil(id);

            // Assert
            Check.That((result as BadRequestObjectResult)?.Value).IsNotNull();
        }
    }
}