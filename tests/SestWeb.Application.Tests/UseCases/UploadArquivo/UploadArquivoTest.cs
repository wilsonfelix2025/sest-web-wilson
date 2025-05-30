using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Services;
using SestWeb.Application.UseCases.UploadUseCase;
using SestWeb.Domain.Importadores.Shallow;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Application.Tests.UseCases.UploadArquivo
{
    [TestFixture]
    public class UploadArquivoTest
    {
        [Test]
        public async Task DeveRetornarUploadRealizado()
        {
            // Arrange
            const string caminho = "NovoNome";
            const string extensão = ".xml";

            var fileService = A.Fake<IFileService>();
            var leitorShallowArquivos = A.Fake<ILeitorShallowArquivos>();
            A.CallTo(() => fileService.SalvarArquivo(A<string>.Ignored, A<byte[]>.Ignored)).Returns(caminho);
            A.CallTo(() => leitorShallowArquivos.LerArquivo(A<TipoArquivo>.Ignored, A<string>.Ignored, null)).Returns(new DadosLidos());

            var uploadArquivoUseCase = new UploadArquivoUseCase(fileService, leitorShallowArquivos);

            // Act
            var result = await uploadArquivoUseCase.Execute(extensão, Array.Empty<byte>());

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(UploadArquivoStatus.UploadRealizado);
            Check.That(result.Mensagem).IsEqualTo("Upload realizado com sucesso.");
        }

        [Test]
        public async Task DeveRetornarUploadNãoRealizado()
        {
            // Arrange
            const string extensão = ".xml";
            const string mensagemErro = "Mensagem de erro.";

            var fileService = A.Fake<IFileService>();
            var leitorShallowArquivos = A.Fake<ILeitorShallowArquivos>();
            A.CallTo(() => fileService.SalvarArquivo(A<string>.Ignored, A<byte[]>.Ignored)).ThrowsAsync(new Exception(mensagemErro));

            var uploadArquivoUseCase = new UploadArquivoUseCase(fileService, leitorShallowArquivos);

            // Act
            var result = await uploadArquivoUseCase.Execute(extensão, Array.Empty<byte>());

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(UploadArquivoStatus.UploadNãoRealizado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível realizar upload. {mensagemErro}");
        }
    }
}