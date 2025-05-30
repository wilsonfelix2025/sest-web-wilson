using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;

namespace SestWeb.Application.Repositories
{
    public interface IRelacionamentoCorrsPropMecPoçoReadOnlyRepository
    {
        Task<bool> ExisteRelacionamentoCorrsPropMecPoço(string idPoço, string nome);
        Task<RelacionamentoUcsCoesaAngatPorGrupoLitológico> ObterRelacionamentoCorrsPropMecPoçoPeloNome(string idPoço, string nome);
        Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentoCorrsPropMecPoçoPorChaveUsuário(string idPoço, string userKey);
        Task<IReadOnlyCollection<Autor>> ObterAutores(string idPoço);
        Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterTodosRelacionamentosCorrsPropMecPoço(string idPoço);
        Task<bool> ExistemRelacionamentosCorrsPropMecPoço(string idPoço);
    }
}
