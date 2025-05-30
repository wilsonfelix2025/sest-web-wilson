using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.File;

namespace SestWeb.Application.UseCases.PoçoWeb.DuplicateFile
{
    public class DuplicateFileOutput : UseCaseOutput<DuplicateFileStatus>
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Well { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string FileType { get; set; }
        public string Schema { get; set; }

        private DuplicateFileOutput(DuplicateFileStatus status)
        {
            Status = status;
        }

        public static DuplicateFileOutput FileDuplicatedSuccesfully(File file)
        {
            return new DuplicateFileOutput(DuplicateFileStatus.FileDuplicated)
            {
                Name = file.Name,
                Id = file.Id,
                Well = file.WellId,
                Description = file.Description,
                Comment = file.Comment,
                FileType = file.FileType,
                Schema = file.Schema,
                Mensagem = "Arquivo duplicado com sucesso."
            };
        }

        public static DuplicateFileOutput FileNotDuplicated(string message)
        {
            return new DuplicateFileOutput(DuplicateFileStatus.FileNotDuplicated)
            {
                Mensagem = $"Não foi possível duplicar o arquivo. {message}"
            };
        }
    }
}
