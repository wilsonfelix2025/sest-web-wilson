using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoWeb.MoveFile
{
    public class MoveFileOutput : UseCaseOutput<MoveFileStatus>
    {
        public string FileName { get; set; }
        public string WellName { get; set; }

        private MoveFileOutput(MoveFileStatus status)
        {
            Status = status;
        }

        internal static MoveFileOutput FileNotMoved(string message)
        {
            return new MoveFileOutput(MoveFileStatus.FileNotMoved)
            {
                Mensagem = $"Não foi possível mover o arquivo. {message}"
            };
        }

        internal static MoveFileOutput FileMovedSuccesfully(string fileName, string wellName)
        {
            return new MoveFileOutput(MoveFileStatus.FileMoved)
            {
                FileName = fileName,
                WellName = wellName,
                Mensagem = $"O arquivo {fileName} foi movido para o poço {wellName}"
            };
        }

        internal static MoveFileOutput FileAlreadyExistInWell(string fileName, string wellName)
        {
            return new MoveFileOutput(MoveFileStatus.FileNotMoved)
            {
                Mensagem = $"O arquivo {fileName} já existe no poço {wellName}"
            };
        }
    }
}
