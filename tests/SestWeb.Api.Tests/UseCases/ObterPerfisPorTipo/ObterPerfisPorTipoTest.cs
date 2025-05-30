using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Perfil.ObterPerfisPorTipo;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisPorTipo;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Api.Tests.UseCases.ObterPerfisPorTipo
{
    [TestFixture]
    public class ObterPerfisPorTipoTest
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
            Check.That(attribute.Template).IsEqualTo("api/perfis/obter-perfis-por-tipo");
        }

        [Test]
        public void ObterPerfilDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(PerfisController).GetMethod(nameof(PerfisController.ObterPerfisPorTipo)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void ObterPerfilDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PerfisController).GetMethod(nameof(PerfisController.ObterPerfisPorTipo)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterPerfisPorTipoDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil = "DTC";
            var request = new ObterPerfisPorTipoRequest {IdPoço = id, Mnemônico = nomePerfil};

            var obterPerfisPorTipoUseCase = A.Fake<IObterPerfisPorTipoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(obterPerfisPorTipoUseCase, presenter);

            A.CallTo(() => obterPerfisPorTipoUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(ObterPerfisPorTipoOutput.PerfisObtidos(new List<PerfilBase>()));

            // Act
            var result = await controller.ObterPerfisPorTipo(request);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ObterPerfisPorTipoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil = "DTC";
            const string mensagemDeErro = "Mensagem de erro.";
            var request = new ObterPerfisPorTipoRequest { IdPoço = id, Mnemônico = nomePerfil };

            var obterPerfisPorTipoUseCase = A.Fake<IObterPerfisPorTipoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(obterPerfisPorTipoUseCase, presenter);

            A.CallTo(() => obterPerfisPorTipoUseCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(ObterPerfisPorTipoOutput.PerfisNãoObtidos(mensagemDeErro));

            // Act
            var result = await controller.ObterPerfisPorTipo(request);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}