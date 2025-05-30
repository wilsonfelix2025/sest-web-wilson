using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.Tree;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.PoçoWeb.GetTree
{
    internal class GetTreeUseCase : IGetTreeUseCase
    {
        private readonly IOpUnitReadOnlyRepository _opUnitReadOnlyRepository;
        private readonly IOilFieldReadOnlyRepository _oilFieldReadOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;

        public GetTreeUseCase(IOpUnitReadOnlyRepository opUnitReadOnlyRepository, IOilFieldReadOnlyRepository oilFieldReadOnlyRepository, IWellReadOnlyRepository wellReadOnlyRepository, IFileReadOnlyRepository fileReadOnlyRepository)
        {
            _opUnitReadOnlyRepository = opUnitReadOnlyRepository;
            _oilFieldReadOnlyRepository = oilFieldReadOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
            _fileReadOnlyRepository = fileReadOnlyRepository;
        }
        public async Task<GetTreeOutput> Execute()
        {
            try
            {
                List<Node> tree = new List<Node>();

                List<OpUnit> opUnits = await _opUnitReadOnlyRepository.GetOpUnits();
                foreach (OpUnit opUnit in opUnits)
                {
                    Node op = new Node(opUnit.Id, opUnit.Name, opUnit.Url);

                    foreach (string oilFieldId in opUnit.OilFields)
                    {
                        OilField oilField = await _oilFieldReadOnlyRepository.GetOilField(oilFieldId);

                        Node of = new Node(oilField.Id, oilField.Name, oilField.Url);

                        foreach (string wellId in oilField.Wells)
                        {
                            Well well = await _wellReadOnlyRepository.GetWell(wellId);

                            Node w = new Node(well.Id, well.Name, well.Url);

                            foreach (string fileId in well.Files)
                            {
                                File file = await _fileReadOnlyRepository.GetFile(fileId);

                                Leaf f = new Leaf(file.Id, file.Name, file.Url, file.FileType);

                                w.Children.Add(f);
                                w.FilesCount += 1;
                            }
                            of.Children.Add(w);
                            of.FilesCount += w.FilesCount;
                        }
                        op.Children.Add(of);
                        op.FilesCount += of.FilesCount;
                    }
                    tree.Add(op);
                }
                // List<OilField> oilFields = await _oilFieldReadOnlyRepository.GetOilFields();
                // List<Well> wells = await _wellReadOnlyRepository.GetWells();

                return GetTreeOutput.TreeFoundSuccessfully(tree);
            }
            catch (Exception ex)
            {
                return GetTreeOutput.TreeNotObtained(ex.Message);
            }
        }
    }
}
