using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Poço.CriarPoço;
using SestWeb.Application.UseCases.PoçoUseCases.CriarPoço;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Api.Tests.UseCases.CriarPoço
{
    [TestFixture]
    public class CriarPoçoTest
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
        public void CriarPoçoDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(PoçosController).GetMethod(nameof(PoçosController.CriarPoço)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(201)]
        [TestCase(400)]
        public void CriarPoçoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PoçosController).GetMethod(nameof(PoçosController.CriarPoço)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task CriarPoçoDeveRetornarCreatedAtRouteResultEmCasoDeSucesso()
        {
            // Arrange
            const string nomePoço = "NovoPoço";
            const TipoPoço tipoPoço = TipoPoço.Projeto;

            var poçoInput = new CriarPoçoRequest {Nome = nomePoço, TipoPoço = tipoPoço};

            var criarPoçoUseCase = A.Fake<ICriarPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarPoçoUseCase, presenter);

            A.CallTo(() => criarPoçoUseCase.Execute(nomePoço, tipoPoço)).Returns(CriarPoçoOutput.PoçoCriado(new PoçoOutput("id", nomePoço, tipoPoço)));

            // Act
            var result = await controller.CriarPoço(poçoInput);

            // Assert
            Check.That(result).IsInstanceOf<CreatedAtRouteResult>();

            var createdResult = (CreatedAtRouteResult) result;

            Check.That(createdResult.Value).IsInstanceOf<PoçoOutput>();
            var resultValue = (PoçoOutput) createdResult.Value;

            Check.That(createdResult.RouteName).IsEqualTo("ObterPoço");
            Check.That(createdResult.RouteValues).CountIs(1);
            Check.That(createdResult.RouteValues).ContainsPair("id", resultValue.Id);

            Check.That(resultValue.Nome).IsEqualTo(nomePoço);
            Check.That(resultValue.TipoPoço).IsEqualTo(tipoPoço);
        }

        [Test]
        public async Task CriarPoçoDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string nomePoço = "NovoPoço";
            const TipoPoço tipoPoço = TipoPoço.Projeto;
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoInput = new CriarPoçoRequest { Nome = nomePoço, TipoPoço = tipoPoço };

            var criarPoçoUseCase = A.Fake<ICriarPoçoUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PoçosController(criarPoçoUseCase, presenter);

            A.CallTo(() => criarPoçoUseCase.Execute(nomePoço, tipoPoço)).Returns(CriarPoçoOutput.PoçoNãoCriado(mensagemDeErro));

            // Act
            var result = await controller.CriarPoço(poçoInput);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}