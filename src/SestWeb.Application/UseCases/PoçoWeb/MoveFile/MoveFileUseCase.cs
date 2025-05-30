using SestWeb.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.PoçoWeb.MoveFile
{
    public class MoveFileUseCase : IMoveFileUseCase
    {
        private readonly IFileWriteOnlyRepository _fileWriteOnlyRepository;
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;

        public MoveFileUseCase(IFileWriteOnlyRepository fileWriteOnlyRepository, IFileReadOnlyRepository fileReadOnlyRepository, IWellReadOnlyRepository wellReadOnlyRepository)
        {
            _fileReadOnlyRepository = fileReadOnlyRepository;
            _fileWriteOnlyRepository = fileWriteOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
        }
        public async Task<MoveFileOutput> Execute(string fileId, string wellId)
        {
            try
            {
                File file = await _fileReadOnlyRepository.GetFile(fileId);
                Well newWell = await _wellReadOnlyRepository.GetWell(wellId);
                List<Well> wells = await _wellReadOnlyRepository.GetWells();

                Well well = wells.Find(_ => _.Files.Exists(f => f == file.Id));

                if (newWell.Files.Contains(file.Id))
                    return MoveFileOutput.FileAlreadyExistInWell(file.Name, newWell.Name);

                await _fileWriteOnlyRepository.MoveFile(well, file, newWell);
                return MoveFileOutput.FileMovedSuccesfully(file.Name, newWell.Name);
            }
            catch (Exception ex)
            {
                return MoveFileOutput.FileNotMoved(ex.Message);
            }
        }
    }
}
