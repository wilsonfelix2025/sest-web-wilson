using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Importação.ImportarDados;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarPerfilUseCase;

namespace SestWeb.Api.Tests.UseCases.ImportarDadosPerfil
{
    [TestFixture]
    public class ImportarDadosPerfilTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(ImportarDadosPerfilController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(ImportarDadosPerfilController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/importar-dados/perfil");
        }

        [Test]
        public void ImportarDadosPerfilDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(ImportarDadosPerfilController).GetMethod(nameof(ImportarDadosPerfilController.ImportarDadosPerfil)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status400BadRequest)]
        public void ImportarDadosPerfilDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(ImportarDadosPerfilController).GetMethod(nameof(ImportarDadosPerfilController.ImportarDadosPerfil)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task ImportarDadosPerfilDeveRetornarOkResultEmCasoDeSucesso()
        {
            var lista = A.Fake<List<PontoPerfilRequest>>();
            var model = A.Fake<PerfilRequest>();
            model.PontosPerfil = lista;
            var listaPerfil = A.Fake<List<PerfilRequest>>();
            listaPerfil.Add(model);

            var importarDadosPerfilUseCase = A.Fake<IImportarDadosPerfilUseCase>();
            var presenter = A.Fake<ImportarDadosPresenter>();
            A.CallTo(() => importarDadosPerfilUseCase.Execute(A<string>.Ignored, A<List<PerfilInput>>.Ignored)).Returns(ImportarDadosOutput.ImportadoComSucesso());

            var controller = new ImportarDadosPerfilController(importarDadosPerfilUseCase, presenter);

            // Act
            var result = await controller.ImportarDadosPerfil(new ImportarDadosPerfilRequest
            {
                PoçoId = "0",
                Perfis = listaPerfil
            });

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task ImportarDadosPerfilDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            var lista = A.Fake<List<PontoPerfilRequest>>();
            var model = A.Fake<PerfilRequest>();
            model.PontosPerfil = lista;
            var listaPerfil = A.Fake<List<PerfilRequest>>();
            listaPerfil.Add(model);

            var importarDadosPerfilUseCase = A.Fake<IImportarDadosPerfilUseCase>();
            var presenter = A.Fake<ImportarDadosPresenter>();
            A.CallTo(() => importarDadosPerfilUseCase.Execute(A<string>.Ignored, A<List<PerfilInput>>.Ignored)).Returns(ImportarDadosOutput.ImportaçãoComFalha());

            var controller = new ImportarDadosPerfilController(importarDadosPerfilUseCase, presenter);

            // Act
            var result = await controller.ImportarDadosPerfil(new ImportarDadosPerfilRequest
            {
                PoçoId = "0",
                Perfis = listaPerfil
            });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

    }
}
