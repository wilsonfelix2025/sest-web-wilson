using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    public class RelacionamentoCorrsPropMecReadyOnlyRepository : IRelacionamentoCorrsPropMecReadyOnlyRepository
    {
        private readonly Context _context;

        public RelacionamentoCorrsPropMecReadyOnlyRepository(Context context)
        {
            _context = context;
        }

        public Task<bool> ExisteRelacionamento(string nome)
        {
            return _context.RelacionamentosCorrsPropMec.Find(rel => rel.Nome == nome).AnyAsync();
        }

        public async Task<RelacionamentoUcsCoesaAngatPorGrupoLitológico> ObterRelacionamentoPeloNome(string nome)
        {
            return await _context.RelacionamentosCorrsPropMec.Find(rel => rel.Nome == nome).Limit(1).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentosPorNomes(List<string> listaNomes)
        {
            return await _context.RelacionamentosCorrsPropMec.Find(rel => listaNomes.Contains(rel.Nome)).ToListAsync();
        }

        public async Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentosFixos()
        {
            return await _context.RelacionamentosCorrsPropMec.Find(rel => rel.Origem == Origem.Fixa).ToListAsync();
        }

        public async Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentosDeUsuários()
        {
            return await _context.RelacionamentosCorrsPropMec.Find(rel => rel.Origem == Origem.Usuário).ToListAsync();
        }

        public async Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterTodosRelacionamentos()
        {
            return await _context.RelacionamentosCorrsPropMec.Find(Builders<RelacionamentoUcsCoesaAngatPorGrupoLitológico>.Filter.Empty).ToListAsync();
        }

        public async Task<bool> RelacionamentosSistemaCarregados()
        {
            return await _context.RelacionamentosCorrsPropMec.Find(rel => rel.Origem == Origem.Fixa).AnyAsync();
        }

        public async Task<bool> ExistemRelacionamentos()
        {
            return await _context.RelacionamentosCorrsPropMec.Find(Builders<RelacionamentoUcsCoesaAngatPorGrupoLitológico>.Filter.Empty).AnyAsync();
        }
    }
}
