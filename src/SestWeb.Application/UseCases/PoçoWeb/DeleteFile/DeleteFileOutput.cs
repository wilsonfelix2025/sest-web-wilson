using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoWeb.DeleteFile
{
    public class DeleteFileOutput : UseCaseOutput<DeleteFileStatus>
    {
        private DeleteFileOutput()
        {
        }

        public static DeleteFileOutput FileDeletedSuccesfully()
        {
            return new DeleteFileOutput
            {
                Status = DeleteFileStatus.FileDeleted,
                Mensagem = "Arquivo removido com sucesso."
            };
        }

        public static DeleteFileOutput FileNotDeleted(string mensagem)
        {
            return new DeleteFileOutput
            {
                Status = DeleteFileStatus.FileNotDeleted,
                Mensagem = $"Não foi possível remover o arquivo. {mensagem}"
            };
        }

        public static DeleteFileOutput FileNotFound(string id)
        {
            return new DeleteFileOutput
            {
                Status = DeleteFileStatus.FileNotFound,
                Mensagem = $"Não foi possível encontrar arquivo com id {id}."
            };
        }
    }
}