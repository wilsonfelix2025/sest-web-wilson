using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    public class RelacionamentoCorrsPropMecPoçoReadOnlyRepository : IRelacionamentoCorrsPropMecPoçoReadOnlyRepository
    {
        private readonly Context _context;

        public RelacionamentoCorrsPropMecPoçoReadOnlyRepository(Context context)
        {
            _context = context;
        }


        public Task<bool> ExisteRelacionamentoCorrsPropMecPoço(string idPoço, string nome)
        {
            return _context.RelacionamentosCorrsPropMecPoço.Find(rel => rel.IdPoço == idPoço && rel.Nome == nome).AnyAsync();
        }

        public async Task<RelacionamentoUcsCoesaAngatPorGrupoLitológico> ObterRelacionamentoCorrsPropMecPoçoPeloNome(string idPoço, string nome)
        {
            return await _context.RelacionamentosCorrsPropMecPoço.Find(rel => rel.IdPoço == idPoço && rel.Nome == nome)
                .Project(rel => rel.RelacionamentoPropMec)
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterRelacionamentoCorrsPropMecPoçoPorChaveUsuário(string idPoço, string userKey)
        {
            return await _context.RelacionamentosCorrsPropMecPoço
                .Find(rel => rel.IdPoço == idPoço && rel.RelacionamentoPropMec.Autor.Chave == userKey)
                .Project(rel => rel.RelacionamentoPropMec)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Autor>> ObterAutores(string idPoço)
        {
            return
                await _context.RelacionamentosCorrsPropMecPoço
                    .Distinct<Autor>(new ExpressionFieldDefinition<RelacionamentoPropMecPoço, Autor>(r => (Autor)r.RelacionamentoPropMec.Autor), new ExpressionFilterDefinition<RelacionamentoPropMecPoço>(c => c.IdPoço == idPoço && c.RelacionamentoPropMec.Origem == Origem.Poço)).ToListAsync();
        }

        public async Task<IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>> ObterTodosRelacionamentosCorrsPropMecPoço(string idPoço)
        {
            return await _context.RelacionamentosCorrsPropMecPoço.Find(rel => rel.IdPoço == idPoço)
                .Project(rel => rel.RelacionamentoPropMec)
                .ToListAsync();
        }

        public async Task<bool> ExistemRelacionamentosCorrsPropMecPoço(string idPoço)
        {
            return await _context.RelacionamentosCorrsPropMecPoço.Find(rel => rel.IdPoço == idPoço).AnyAsync();
        }
    }
}
