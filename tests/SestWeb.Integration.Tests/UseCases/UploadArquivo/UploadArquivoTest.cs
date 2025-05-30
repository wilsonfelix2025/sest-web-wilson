using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.UploadArquivo
{
    [TestFixture]
    public class UploadArquivoTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeFezUploadDoArquivoComSucesso()
        {
            // Arrange
            HttpResponseMessage response;
            
            using (var file = File.OpenRead(@"filename.txt")) // TODO: ajustar caminho do arquivo
            using (var streamContent = new StreamContent(file))
            using (var formData = new MultipartFormDataContent())
            {
                // Add file (file, field name, file name)
                formData.Add(streamContent, "file", "filename.txt"); // TODO: ajustar nome do arquivo

                // Act
                response = await Client.PostAsync("api/upload", formData);
            }
            
            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Upload realizado com sucesso.");
        }
    }
}