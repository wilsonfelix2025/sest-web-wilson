using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Poço.ObterPoços;
using SestWeb.Application.UseCases.PoçoUseCases.ObterPoços;

namespace SestWeb.Api.Tests.UseCases.ObterPoços
{
    [TestFixture]
    public class ObterPoçosTest
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
            Check.That(attribute.Template).IsEqualTo("api/pocos");
        }

        [Test]
        public void ObterPoçoDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPoços)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Name).IsEqualTo("ObterPoços");
        }

        [TestCase(200)]
        public void ObterPoçoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.ObterPoços)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ObterPoçoDeveRetornarOkObjectResultEmCasoDeSucesso()
        {
            // Arrange
            var obterPoçosUseCase = A.Fake<IObterPoçosUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(obterPoçosUseCase, presenter);

            A.CallTo(() => obterPoçosUseCase.Execute()).Returns(ObterPoçosOutput.PoçosObtidos(new List<PoçoOutput>()));

            // Act
            var result = await controller.ObterPoços();

            // Assert
            Check.That((result as OkObjectResult)?.Value).IsNotNull();
        }
    }
}