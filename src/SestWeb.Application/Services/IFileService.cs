using System.Threading.Tasks;

namespace SestWeb.Application.Services
{
    public interface IFileService
    {
        Task<string> SalvarArquivo(string extensão, byte[] arquivo);

        string SalvarArquivoPúblico();
    }
}