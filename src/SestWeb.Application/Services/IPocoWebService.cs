using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.Services
{
    public interface IPocoWebService
    {
        Task<List<FileDTO>> GetFilesByTypeFromWeb(string token, string tipoArquivo);
        Task<string> GetPoçoWebDTOFromWebByFile(string urlArquivo, string authorization);
        Task<string> GetFileRevisions(string urlRevisions, string authorization);
        Task<List<OpUnit>> GetOpUnitsFromWeb(string authorization);
        Task<List<OilField>> GetOilFieldsFromWeb(string authorization, List<OpUnit> opUnits);
        Task<List<Well>> GetWellsFromWeb(string authorization, List<OilField> oilFields);
    }
}
