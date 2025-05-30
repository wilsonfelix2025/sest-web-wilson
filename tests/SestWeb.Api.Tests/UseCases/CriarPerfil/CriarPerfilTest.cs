using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Perfil.CriarPerfil;
using SestWeb.Application.UseCases.PerfilUseCases.CriarPerfil;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Api.Tests.UseCases.CriarPerfil
{
    [TestFixture]
    public class CriarPerfilTest
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
        public void CriarPerfilDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(PerfisController).GetMethod(nameof(PerfisController.CriarPerfil)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(201)]
        [TestCase(400)]
        public void CriarPerfilDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(PerfisController).GetMethod(nameof(PerfisController.CriarPerfil)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task CriarPerfilDeveRetornarCreatedAtRouteResultEmCasoDeSucesso()
        {
            // Arrange
            const string idPoço = "id";
            const string nomePerfil = "NovoPoço";
            const string mnemônico = "DTC";

            var criarPerfilRequest = new CriarPerfilRequest { IdPoço = idPoço, Nome = nomePerfil, Mnemônico = mnemônico };

            var criarPoçoUseCase = A.Fake<ICriarPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(criarPoçoUseCase, presenter);

            var trajetória = new Trajetória(MétodoDeCálculoDaTrajetória.MinimaCurvatura);
            var litologias = new List<Litologia> {new Litologia(TipoLitologia.Adaptada, trajetória)};
            var litologia = litologias.Single(x => x.Classificação == TipoLitologia.Adaptada);
            
            //var factory = PerfilOld.GetFactory(trajetória, litologia);
            //var tipoPerfil = PerfilOld.ObterTipoPerfilPorMnemônico(mnemônico);
            //var perfil = factory.CriarPerfil(tipoPerfil, nomePerfil);
            var perfil = PerfisFactory.Create(mnemônico, nomePerfil, trajetória, litologia);

            A.CallTo(() => criarPoçoUseCase.Execute(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(CriarPerfilOutput.PerfilCriado(perfil));

            // Act
            var result = await controller.CriarPerfil(criarPerfilRequest);

            // Assert
            Check.That(result).IsInstanceOf<CreatedAtRouteResult>();

            var createdResult = (CreatedAtRouteResult)result;

            Check.That(createdResult.Value).IsInstanceOf<DTC>();
            var resultValue = (DTC)createdResult.Value;

            Check.That(createdResult.RouteName).IsEqualTo("ObterPerfil");
            Check.That(createdResult.RouteValues).CountIs(1);
            Check.That(createdResult.RouteValues).ContainsPair("id", resultValue.Id);

            Check.That(resultValue.Nome).IsEqualTo(nomePerfil);
            Check.That(resultValue.Mnemonico).IsEqualTo(mnemônico);
        }

        [Test]
        public async Task CriarPerfilDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string idPoço = "id";
            const string nomePerfil = "NovoPoço";
            const string mnemônico = "DTC";
            const string mensagem = "Mensagem de erro.";

            var criarPerfilRequest = new CriarPerfilRequest { IdPoço = idPoço, Nome = nomePerfil, Mnemônico = mnemônico };

            var criarPoçoUseCase = A.Fake<ICriarPerfilUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new PerfisController(criarPoçoUseCase, presenter);

            A.CallTo(() => criarPoçoUseCase.Execute(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(CriarPerfilOutput.PerfilNãoCriado(mensagem));

            // Act
            var result = await controller.CriarPerfil(criarPerfilRequest);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
            
            var badRequestResult = (BadRequestObjectResult)result;

            Check.That(badRequestResult.Value).IsEqualTo($"Não foi possível criar perfil. {mensagem}");
        }
    }
}