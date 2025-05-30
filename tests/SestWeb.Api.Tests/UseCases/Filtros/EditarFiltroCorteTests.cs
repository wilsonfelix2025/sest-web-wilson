using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Api.UseCases.Filtros.EditarFiltro;
using SestWeb.Api.UseCases.Filtros.EditarFiltro.FiltroCorte;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroCorte;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Api.Tests.UseCases.Filtros
{
    [TestFixture]
    public class EditarFiltroCorteTests
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(FiltroController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(FiltroController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/editar-filtro-corte");
        }

        [Test]
        public void EditarFiltroCorteDeveTerAtributoHttpPut()
        {
            var attribute = (HttpPutAttribute)Attribute.GetCustomAttribute(typeof(FiltroController).GetMethod(nameof(FiltroController.EditarFiltro)), typeof(HttpPutAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(200)]
        [TestCase(400)]
        public void EditarFiltroCorteDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(FiltroController).GetMethod(nameof(FiltroController.EditarFiltro)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task EditarFiltroCorteDeveRetornarCreatedEmCasoDeSucesso()
        {
            const string mnemônico = "DTC";
            const string nomePerfil = "Perfil1";

            var useCase = A.Fake<IEditarFiltroCorteUseCase>();
            var presenter = A.Fake<Presenter>();
            var request = A.Fake<FiltroCorteRequest>();
            var traj = A.Fake<IConversorProfundidade>();
            var controller = new FiltroController(useCase, presenter);
            var litologias = new List<Litologia> { new Litologia(TipoLitologia.Adaptada, traj) };
            var litologia = litologias.Single(x => x.Classificação == TipoLitologia.Adaptada);
            var perfil = PerfisFactory.Create(mnemônico, nomePerfil, A.Fake<IConversorProfundidade>(), litologia);

            A.CallTo(() => useCase.Execute(A<EditarFiltroCorteInput>.Ignored)).Returns(EditarFiltroOutput.FiltroEditado(perfil, null, null));

            // Act
            var result = await controller.EditarFiltro(request);

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task EditarFiltroCorteDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string mensagem = "Mensagem de erro.";

            var useCase = A.Fake<IEditarFiltroCorteUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new FiltroController(useCase, presenter);
            var request = A.Fake<FiltroCorteRequest>();

            A.CallTo(() => useCase.Execute(A<EditarFiltroCorteInput>.Ignored)).Returns(EditarFiltroOutput.FiltroNãoEditado(mensagem));

            // Act
            var result = await controller.EditarFiltro(request);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();

            var badRequestResult = (BadRequestObjectResult)result;

            Check.That(badRequestResult.Value).IsEqualTo($"Não foi possível editar filtro. {mensagem}");
        }

    }
}
