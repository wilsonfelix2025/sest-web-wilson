using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.PoçoWeb.GetFiles;
using SestWeb.Application.UseCases.PoçoWeb.GetFiles;
using SestWeb.Domain.Entities.PoçoWeb.File;

namespace SestWeb.Api.Tests.UseCases.GetFiles
{
    [TestFixture]
    public class GetFilesTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(FileController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(FileController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/pocoweb/files/get-files");
        }
        [Test]
        public void GetFilesDeveTerAtributoHttpGet()
        {
            var attribute = (HttpGetAttribute)Attribute.GetCustomAttribute(typeof(FileController).GetMethod(nameof(FileController.GetFiles)), typeof(HttpGetAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status400BadRequest)]
        public void GetFilesDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(FileController).GetMethod(nameof(FileController.GetFiles)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task GetFilesDeveRetornarOkResultEmCasoDeSucesso()
        {
            
            var useCase = A.Fake<IGetFilesUseCase>();
            var presenter = A.Fake<Presenter>();
            A.CallTo(() => useCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(GetFilesOutput.BuscaComSucesso(new List<FileDTO>()));

            var controller = new FileController(useCase, presenter);

            // Act
            var result = await controller.GetFiles("drilling");

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task GetFilesDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            var useCase = A.Fake<IGetFilesUseCase>();
            var presenter = A.Fake<Presenter>();
            A.CallTo(() => useCase.Execute(A<string>.Ignored, A<string>.Ignored)).Returns(GetFilesOutput.BuscaSemSucesso());

            var controller = new FileController(useCase, presenter);

            // Act
            var result = await controller.GetFiles("drilling");

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

       
    }
}
