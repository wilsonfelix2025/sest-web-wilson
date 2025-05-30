using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.File;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateFile
{
    public class CreateFileOutput : UseCaseOutput<CreateFileStatus>
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Well { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string FileType { get; set; }
        public string Schema { get; set; }

        private CreateFileOutput(CreateFileStatus status)
        {
            Status = status;
        }

        public static CreateFileOutput FileCreatedSuccesfully(File file)
        {
            return new CreateFileOutput(CreateFileStatus.FileCreated)
            {
                Name = file.Name,
                Id = file.Id,
                Well = file.WellId,
                Description = file.Description,
                Comment = file.Comment,
                FileType = file.FileType,
                Schema = file.Schema,
                Mensagem = "Arquivo criado com sucesso."
            };
        }

        public static CreateFileOutput FileNotCreated(string message)
        {
            return new CreateFileOutput(CreateFileStatus.FileNotCreated)
            {
                Mensagem = $"Não foi possível criar o arquivo. {message}"
            };
        }
    }
}
