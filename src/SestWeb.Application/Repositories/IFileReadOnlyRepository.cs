using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.File;

namespace SestWeb.Application.Repositories
{
    public interface IFileReadOnlyRepository
    {
        Task<bool> HasFile(string id);
        Task<List<File>> GetFiles();
        Task<File> GetFile(string id);
        Task<List<File>> GetFilesByWell(string wellId);
        Task<bool> HasFileWithTheSameName(string name);
    }
}
