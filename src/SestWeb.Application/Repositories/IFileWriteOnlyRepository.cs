using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.Repositories
{
    public interface IFileWriteOnlyRepository
    {
        Task<File> CreateFile(FileRequest file, string authorization);
        Task<bool> UpdateFile(File file);
        Task<decimal> DeleteFile(string id, string authorization);
        Task MoveFile(Well well, File file, Well newWell);
        Task<(bool isRenamed, string newName)> RenameFile(File file, string newName);
        Task<File> DuplicateFile(FileRequest file, string authorization);
    }
}
