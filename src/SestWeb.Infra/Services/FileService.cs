using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SestWeb.Application.Services;

namespace SestWeb.Infra.Services
{
    public class FileService : IFileService
    {
        IHostingEnvironment _env;

        public FileService(IHostingEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SalvarArquivo(string extensão, byte[] arquivo)
        {
            var caminhoArquivoTemp = Path.GetTempFileName();
            var caminhoCompleto = Path.ChangeExtension(caminhoArquivoTemp, extensão);

            using (var stream = File.Create(caminhoCompleto))
            {
                await stream.WriteAsync(arquivo);
            }

            return caminhoCompleto;
        }

        public string SalvarArquivoPúblico()
        {
            var uploadDirecotroy = "uploads";
            var uploadPath = Path.Combine(_env.WebRootPath, uploadDirecotroy);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            return uploadPath;
        }
    }
}
