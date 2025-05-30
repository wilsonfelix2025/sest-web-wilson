using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.File;

namespace SestWeb.Application.UseCases.PoçoWeb.RenameFile
{
    internal class RenameFileUseCase : IRenameFileUseCase
    {
        private readonly IFileWriteOnlyRepository _fileWriteOnlyRepository;
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RenameFileUseCase(IFileWriteOnlyRepository fileWriteOnlyRepository, IFileReadOnlyRepository fileReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _fileWriteOnlyRepository = fileWriteOnlyRepository;
            _fileReadOnlyRepository = fileReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }
        public async Task<RenameFileOutput> Execute(string id, string newName)
        {
            try
            {
                File file = await _fileReadOnlyRepository.GetFile(id);
                var result = await _fileWriteOnlyRepository.RenameFile(file, newName);

                if (result.isRenamed)
                {
                    var poço = await _poçoReadOnlyRepository.ObterPoço(id);
                    poço.DadosGerais.Identificação.Nome = result.newName;
                    await _poçoWriteOnlyRepository.AtualizarPoço(poço);
                    file.Name = result.newName;
                    return RenameFileOutput.FileRenamedSuccesfully(file);
                }

                return RenameFileOutput.FileNotRenamed("Tente outro nome para o arquivo.");
            }
            catch (Exception ex)
            {
                return RenameFileOutput.FileNotRenamed(ex.Message);
            }
        }
    }
}
