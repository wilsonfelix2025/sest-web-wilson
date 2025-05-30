
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Trend.CriarTrend;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using SestWeb.Application.UseCases.TrendUseCases.CriarTrend;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Api.Tests.UseCases.Trend
{
    [TestFixture]
    public class CriarTrendTests
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(TrendController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(TrendController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/criar-trend");
        }

        [Test]
        public void CriarTrendDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(TrendController).GetMethod(nameof(TrendController.CriarTrend)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(201)]
        [TestCase(400)]
        public void CriarTrendDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(TrendController).GetMethod(nameof(TrendController.CriarTrend)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task CriarTrendDeveRetornarCreatedEmCasoDeSucesso()
        {
            const string mnemônico = "DTC";
            const string nomePerfil = "Perfil1";

            var useCase = A.Fake<ICriarTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);
            var trajetória = new Trajetória(MétodoDeCálculoDaTrajetória.MinimaCurvatura);
            var litologias = new List<Litologia> { new Litologia(TipoLitologia.Adaptada, trajetória) };
            var litologia = litologias.Single(x => x.Classificação == TipoLitologia.Adaptada);
            
            //var factory = PerfilOld.GetFactory(trajetória, litologia);
            //var tipoPerfil = PerfilOld.ObterTipoPerfilPorMnemônico(mnemônico);
            //var perfil = factory.CriarPerfil(tipoPerfil, nomePerfil);

            var perfil = PerfisFactory.Create(mnemônico, nomePerfil, trajetória, litologia);

            A.CallTo(() => useCase.Execute(A<string>.Ignored)).Returns(CriarTrendOutput.TrendCriado(perfil.Trend));

            // Act
            var result = await controller.CriarTrend("1234");

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();

        }

        [Test]
        public async Task CriarTrendDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string mensagem = "Mensagem de erro.";

            var useCase = A.Fake<ICriarTrendUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new TrendController(useCase, presenter);

            A.CallTo(() => useCase.Execute(A<string>.Ignored)).Returns(CriarTrendOutput.TrendNãoCriado(mensagem));

            // Act
            var result = await controller.CriarTrend("1234");

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();

            var badRequestResult = (BadRequestObjectResult)result;

            Check.That(badRequestResult.Value).IsEqualTo($"Não foi possível criar trend. {mensagem}");
        }

    }
}
