using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateFile
{
    internal class CreateFileUseCase : ICreateFileUseCase
    {
        private readonly IFileWriteOnlyRepository _fileWriteOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;
        private readonly IWellWriteOnlyRepository _wellWriteOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public CreateFileUseCase(IFileWriteOnlyRepository fileWriteOnlyRepository, IWellReadOnlyRepository wellReadOnlyRepository, IWellWriteOnlyRepository wellWriteOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _fileWriteOnlyRepository = fileWriteOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
            _wellWriteOnlyRepository = wellWriteOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<CreateFileOutput> Execute(string authorization, string name, string wellId, string description, string fileType)
        {
            try
            {
                Well well = await _wellReadOnlyRepository.GetWell(wellId);
                FileRequest file = new FileRequest(name, description, fileType, well.Url);

                File newFile = await _fileWriteOnlyRepository.CreateFile(file, authorization);
                if (newFile == null)
                {
                    return CreateFileOutput.FileNotCreated("Erro ao contatar banco do PoçoWeb.");
                }

                var novoPoço = PoçoFactory.CriarPoço(newFile.Id, newFile.Name, Tipo.GetTipo(newFile.FileType));
                await _poçoWriteOnlyRepository.CriarPoço(novoPoço);

                well.Files.Add(newFile.Id);
                await _wellWriteOnlyRepository.UpdateWell(well);

                return CreateFileOutput.FileCreatedSuccesfully(newFile);
            }
            catch (Exception ex)
            {
                return CreateFileOutput.FileNotCreated(ex.Message);
            }
        }
    }
}
