using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.Repositories
{
    public interface IRelacionamentoCorrsPropMecReadyOnlyRepository
    {
        Task<bool> ExisteRelacionamento(string nome);

        Task<RelacionamentoUcsCoesaAngatPorGrupoLitológico> ObterRelacionamentoPeloNome(string nome);

        Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentosPorNomes(
            List<string> listaNomes);

        Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentosFixos();

        Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentosDeUsuários();

        Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterTodosRelacionamentos();

        Task<bool> RelacionamentosSistemaCarregados();

        Task<bool> ExistemRelacionamentos();
    }
}
