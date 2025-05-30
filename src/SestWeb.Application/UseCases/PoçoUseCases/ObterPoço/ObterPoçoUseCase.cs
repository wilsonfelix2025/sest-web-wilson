using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Services;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.PoçoWeb.Tree;
using Newtonsoft.Json;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterPoço
{
    internal class ObterPoçoUseCase : IObterPoçoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;
        private readonly IOilFieldReadOnlyRepository _oilFieldReadOnlyRepository;
        private readonly IOpUnitReadOnlyRepository _opUnitReadOnlyRepository;
        private readonly IPocoWebService _pocoWebService;


        public ObterPoçoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IFileReadOnlyRepository fileReadOnlyRepository
            , IWellReadOnlyRepository wellReadOnlyRepository, IOilFieldReadOnlyRepository oilFieldReadOnlyRepository
            , IOpUnitReadOnlyRepository opUnitReadOnlyRepository, IPocoWebService pocoWebService)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _fileReadOnlyRepository = fileReadOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
            _oilFieldReadOnlyRepository = oilFieldReadOnlyRepository;
            _opUnitReadOnlyRepository = opUnitReadOnlyRepository;
            _pocoWebService = pocoWebService;
        }

        public async Task<ObterPoçoOutput> Execute(string id, string token)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(id);

                if (poço == null)
                    return ObterPoçoOutput.PoçoNãoEncontrado(id);

                var file = await _fileReadOnlyRepository.GetFile(id);
                var well = await _wellReadOnlyRepository.GetWell(file.WellId);
                var oilField = await _oilFieldReadOnlyRepository.GetOilField(well.OilFieldId);
                var opUnit = await _opUnitReadOnlyRepository.GetOpUnit(oilField.OpUnitId);

                Node caminho = new Node(opUnit.Id, opUnit.Name, opUnit.Url);
                Node of = new Node(oilField.Id, oilField.Name, oilField.Url);
                Node w = new Node(well.Id, well.Name, well.Url);
                Leaf f = new Leaf(file.Id, file.Name, file.Url, file.FileType);
                w.Children.Add(f);
                of.Children.Add(w);
                caminho.Children.Add(of);

                if (!string.IsNullOrWhiteSpace(poço.DadosGerais.Identificação.PoçoWebUrlRevisões))
                {
                    var json = await _pocoWebService.GetFileRevisions(poço.DadosGerais.Identificação.PoçoWebUrlRevisões, token);
                    var revisoes = JsonConvert.DeserializeObject<List<PoçoWebRevisionsDTO>>(json);
                    var fileUrl = poço.DadosGerais.Identificação.PoçoWebUrlRevisões.Substring(0, poço.DadosGerais.Identificação.PoçoWebUrlRevisões.Length - 10);

                    var ultimaRevisaoUrl = revisoes.Last().Url.AbsoluteUri;
                    var ultimaRevisao = ultimaRevisaoUrl.Remove(ultimaRevisaoUrl.Length - 1).Split('/').Last();
                    var ultimaRevisaoUrlFile = fileUrl + "?rev=" + ultimaRevisao;

                    if (ultimaRevisaoUrlFile != poço.DadosGerais.Identificação.PoçoWebRevisãoUrl)
                    {
                        poço.DadosGerais.Identificação.PoçoWebAtualizado = false;
                        poço.DadosGerais.Identificação.PoçoWebUrlÚltimaRevisão = ultimaRevisaoUrlFile;
                    }
                    else
                    {
                        poço.DadosGerais.Identificação.PoçoWebAtualizado = true;
                    }
                }

                return ObterPoçoOutput.PoçoObtido(poço, caminho);
            }
            catch (Exception e)
            {
                return ObterPoçoOutput.PoçoNãoObtido(e.Message);
            }
        }
    }
}