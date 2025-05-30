using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.InserirTrecho;
using SestWeb.Application.UseCases.InserirTrechoUseCase;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Enums;

namespace SestWeb.Api.Tests.UseCases.InserirTrecho
{
    [TestFixture]
    public class InserirTrechoTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(InserirTrechoController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(InserirTrechoController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/inserir-trecho-inicial");
        }

        [Test]
        public void ImportarDadosTrajetóriaDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(InserirTrechoController).GetMethod(nameof(InserirTrechoController.InserirTrecho)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status400BadRequest)]
        public void InserirTrechoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(InserirTrechoController).GetMethod(nameof(InserirTrechoController.InserirTrecho)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task InserirTrechoDeveRetornarOkResultEmCasoDeSucesso()
        {
            var useCase = A.Fake<IInserirTrechoUseCase>();
            var presenter = A.Fake<Presenter>();

            var perfil = PerfisFactory.Create("DTC", "DTC", null, null);

            A.CallTo(() => useCase.Execute(A<string>.Ignored, A<InserirTrechoInput>.Ignored)).Returns(InserirTrechoOutput.InserirTrechoCriadoComSucesso(perfil));

            var controller = new InserirTrechoController(useCase, presenter);

            // Act
            var result = await controller.InserirTrecho(new InserirTrechoRequest
            { 
              LitologiasSelecionadas  = new List<string>(),
              NomeNovoPerfil = "novoNome",
              PerfilId = "perfilId",
              PMLimite = 1000,
              TipoDeTrecho = TipoDeTrechoEnum.Inicial,
              TipoTratamento = TipoTratamentoTrechoEnum.Linear
            });

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task InserirTrechoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            var useCase = A.Fake<IInserirTrechoUseCase>();
            var presenter = A.Fake<Presenter>();
            A.CallTo(() => useCase.Execute(A<string>.Ignored, A<InserirTrechoInput>.Ignored)).Returns(InserirTrechoOutput.InserirTrechoComFalha("falha"));

            var controller = new InserirTrechoController(useCase, presenter);

            // Act
            var result = await controller.InserirTrecho(new InserirTrechoRequest
            {
                LitologiasSelecionadas = new List<string>(),
                NomeNovoPerfil = "novoNome",
                PerfilId = "perfilid",
                PMLimite = 1000,
                TipoDeTrecho = TipoDeTrechoEnum.Inicial,
                TipoTratamento = TipoTratamentoTrechoEnum.Linear
            });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}
