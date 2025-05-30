using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.File;

namespace SestWeb.Application.UseCases.PoçoWeb.RenameFile
{
    public class RenameFileOutput : UseCaseOutput<RenameFileStatus>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string FileType { get; set; }
        public string Schema { get; set; }

        private RenameFileOutput(RenameFileStatus status)
        {
            Status = status;
        }

        public static RenameFileOutput FileRenamedSuccesfully(File file)
        {
            return new RenameFileOutput(RenameFileStatus.FileRenamed)
            {
                Name = file.Name,
                Description = file.Description,
                Comment = file.Comment,
                FileType = file.FileType,
                Schema = file.Schema,
                Mensagem = "Arquivo renomeado com sucesso."
            };
        }

        public static RenameFileOutput FileNotRenamed(string message)
        {
            return new RenameFileOutput(RenameFileStatus.FileNotRenamed)
            {
                Mensagem = $"Não foi possível renomear o arquivo. {message}"
            };
        }
    }
}
