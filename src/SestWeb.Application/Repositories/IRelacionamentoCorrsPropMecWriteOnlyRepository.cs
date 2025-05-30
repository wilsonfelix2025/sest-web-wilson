using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.Repositories
{
    public interface IRelacionamentoCorrsPropMecWriteOnlyRepository
    {
        Task CriarRelacionamentoCorrsPropMec(RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamento);
        Task<bool> RemoverRelacionamentoCorrsPropMec(string nome);
        Task<bool> UpdateRelacionamentoCorrsPropMec(RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamento);
        Task<bool> InsertRelacionamentoCorrsPropMec(
            IList<RelacionamentoUcsCoesaAngatPorGrupoLitológico> relacionamentos);
    }
}
