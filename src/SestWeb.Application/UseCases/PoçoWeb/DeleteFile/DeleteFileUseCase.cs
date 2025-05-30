using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.PoçoWeb.DeleteFile
{
    internal class DeleteFileUseCase : IDeleteFileUseCase
    {
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;
        private readonly IFileWriteOnlyRepository _fileWriteOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;
        private readonly IWellWriteOnlyRepository _wellWriteOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public DeleteFileUseCase(IFileReadOnlyRepository fileReadOnlyRepository, IFileWriteOnlyRepository fileWriteOnlyRepository, IWellReadOnlyRepository wellReadOnlyRepository, IWellWriteOnlyRepository wellWriteOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)

        {
            _fileReadOnlyRepository = fileReadOnlyRepository;
            _fileWriteOnlyRepository = fileWriteOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
            _wellWriteOnlyRepository = wellWriteOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<DeleteFileOutput> Execute(string authorization, string id)
        {
            try
            {
                File file = await _fileReadOnlyRepository.GetFile(id);
                Well well = await _wellReadOnlyRepository.GetWell(file.WellId);
                var result = await _fileWriteOnlyRepository.DeleteFile(id, authorization);

                well.Files.Remove(id);
                await _wellWriteOnlyRepository.UpdateWell(well);

                if (result == 1)
                {
                    await _poçoWriteOnlyRepository.RemoverPoço(id);
                    well.Files.Remove(id);
                    await _wellWriteOnlyRepository.UpdateWell(well);
                    return DeleteFileOutput.FileDeletedSuccesfully();
                }
                else if (result == -1)
                    return DeleteFileOutput.FileNotDeleted("Erro ao contatar banco do PoçoWeb.");

                return DeleteFileOutput.FileNotFound(id);
            }
            catch (Exception e)
            {
                return DeleteFileOutput.FileNotDeleted(e.Message);
            }
        }
    }
}
