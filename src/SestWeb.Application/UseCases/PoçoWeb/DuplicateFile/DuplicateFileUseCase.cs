using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.PoçoWeb.DuplicateFile
{
    internal class DuplicateFileUseCase : IDuplicateFileUseCase
    {
        private readonly IFileWriteOnlyRepository _fileWriteOnlyRepository;
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;
        private readonly IWellWriteOnlyRepository _wellWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public DuplicateFileUseCase(IFileWriteOnlyRepository fileWriteOnlyRepository, IFileReadOnlyRepository fileReadOnlyRepository, IWellReadOnlyRepository wellReadOnlyRepository, IWellWriteOnlyRepository wellWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _fileWriteOnlyRepository = fileWriteOnlyRepository;
            _fileReadOnlyRepository = fileReadOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
            _wellWriteOnlyRepository = wellWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<DuplicateFileOutput> Execute(string authorization, string id)
        {
            try
            {
                File originalFile = await _fileReadOnlyRepository.GetFile(id);
                Poço originalPoço = await _poçoReadOnlyRepository.ObterPoço(id);
                Well well = await _wellReadOnlyRepository.GetWell(originalFile.WellId);
                FileRequest file = new FileRequest(originalFile.Name, originalFile.Description, originalFile.FileType, well.Url);

                File duplicatedFile = await _fileWriteOnlyRepository.DuplicateFile(file, authorization);
                if (duplicatedFile == null)
                {
                    return DuplicateFileOutput.FileNotDuplicated("Erro ao contatar banco do PoçoWeb.");
                }
                var novoPoço = PoçoFactory.CriarPoço(duplicatedFile.Id, duplicatedFile.Name, Tipo.GetTipo(duplicatedFile.FileType));
                novoPoço.DadosGerais = originalPoço.DadosGerais;
                novoPoço.Sapatas = originalPoço.Sapatas;
                novoPoço.Objetivos = originalPoço.Objetivos;
                novoPoço.Trajetória = originalPoço.Trajetória;
                novoPoço.Estratigrafia = originalPoço.Estratigrafia;
                novoPoço.Litologias = originalPoço.Litologias;
                novoPoço.State = originalPoço.State;
                novoPoço.Perfis = originalPoço.Perfis;
                novoPoço.Perfis.ForEach(p => p.SetNewId());


                foreach (var tree in novoPoço.State.Tree)
                {
                    if (tree.Name == "Perfis")
                    {
                        tree.Data.Clear();
                    }
                }

                await _poçoWriteOnlyRepository.DuplicarPoço(novoPoço);

                well.Files.Add(duplicatedFile.Id);
                await _wellWriteOnlyRepository.UpdateWell(well);

                return DuplicateFileOutput.FileDuplicatedSuccesfully(duplicatedFile);
            }
            catch (Exception ex)
            {
                return DuplicateFileOutput.FileNotDuplicated(ex.Message);
            }
        }
    }
}
