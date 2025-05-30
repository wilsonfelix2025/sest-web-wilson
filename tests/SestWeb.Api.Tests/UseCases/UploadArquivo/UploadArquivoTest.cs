using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Api.UseCases.Upload;
using SestWeb.Application.UseCases.UploadUseCase;
using SestWeb.Domain.Importadores.Shallow;

namespace SestWeb.Api.Tests.UseCases.UploadArquivo
{
    [TestFixture]
    public class UploadArquivoTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(UploadArquivoController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute = (RouteAttribute)Attribute.GetCustomAttribute(typeof(UploadArquivoController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/upload");
        }

        [Test]
        public void UploadArquivoDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(UploadArquivoController).GetMethod(nameof(UploadArquivoController.UploadArquivo)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status400BadRequest)]
        public void UploadArquivoDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(UploadArquivoController).GetMethod(nameof(UploadArquivoController.UploadArquivo)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task UploadArquivoDeveRetornarOkResultEmCasoDeSucesso()
        {
            // Arrange
            const string caminho = "caminho";

            var arquivo = A.Fake<IFormFile>();
            const string conteúdo = "Conteúdo do arquivo";
            const string nome = "testeUpload.xml";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(conteúdo);
            writer.Flush();
            ms.Position = 0;
            A.CallTo(() => arquivo.OpenReadStream()).Returns(ms);
            A.CallTo(() => arquivo.FileName).Returns(nome);
            A.CallTo(() => arquivo.Length).Returns(ms.Length);

            var uploadArquivoUseCase = A.Fake<IUploadArquivoUseCase>();
            var presenter = A.Fake<Presenter>();
            A.CallTo(() => uploadArquivoUseCase.Execute(A<string>.Ignored, A<byte[]>.Ignored)).Returns(UploadArquivoOutput.UploadRealizado(caminho, new DadosLidos()));

            var controller = new UploadArquivoController(uploadArquivoUseCase, presenter);
            
            // Act
            var result = await controller.UploadArquivo(new UploadArquivoRequest { Arquivo = arquivo });

            // Assert
            Check.That(result).IsInstanceOf<OkObjectResult>();
        }

        [Test]
        public async Task UploadArquivoDeveRetornarBadRequestObjectResultCasoOcorraErro()
        {
            // Arrange
            const string mensagemDeErro = "Mensagem de erro.";

            var arquivo = A.Fake<IFormFile>();
            const string conteúdo = "Conteúdo do arquivo";
            const string nome = "testeUpload.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(conteúdo);
            writer.Flush();
            ms.Position = 0;
            A.CallTo(() => arquivo.OpenReadStream()).Returns(ms);
            A.CallTo(() => arquivo.FileName).Returns(nome);
            A.CallTo(() => arquivo.Length).Returns(ms.Length);

            var uploadArquivoUseCase = A.Fake<IUploadArquivoUseCase>();
            var presenter = A.Fake<Presenter>();
            A.CallTo(() => uploadArquivoUseCase.Execute(A<string>.Ignored, A<byte[]>.Ignored)).Returns(UploadArquivoOutput.UploadNãoRealizado(mensagemDeErro));

            var controller = new UploadArquivoController(uploadArquivoUseCase, presenter);
            
            // Act
            var result = await controller.UploadArquivo(new UploadArquivoRequest { Arquivo = arquivo });

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }
    }
}