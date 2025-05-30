using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    public class RelacionamentoCorrsPropMecWriteOnlyRepository : IRelacionamentoCorrsPropMecWriteOnlyRepository
    {
        private readonly Context _context;

        public RelacionamentoCorrsPropMecWriteOnlyRepository(Context context)
        {
            _context = context;
        }

        public async Task CriarRelacionamentoCorrsPropMec(RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamento)
        {
            if (relacionamento == null)
                throw new ArgumentNullException(nameof(RelacionamentoUcsCoesaAngatPorGrupoLitológico), "Relacionamento não pode ser null.");
            await _context.RelacionamentosCorrsPropMec.InsertOneAsync(relacionamento);
        }

        public async Task<bool> RemoverRelacionamentoCorrsPropMec(string nome)
        {
            var result = await _context.RelacionamentosCorrsPropMec.DeleteOneAsync(rel => rel.Nome == nome);

            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        public async Task<bool> UpdateRelacionamentoCorrsPropMec(RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamento)
        {
            if (relacionamento == null)
                throw new ArgumentNullException(nameof(RelacionamentoUcsCoesaAngatPorGrupoLitológico), "relacionamento não pode ser null.");

            var result = await _context.RelacionamentosCorrsPropMec.ReplaceOneAsync(rel => rel.Nome == relacionamento.Nome, relacionamento);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }

        public async Task<bool> InsertRelacionamentoCorrsPropMec(IList<RelacionamentoUcsCoesaAngatPorGrupoLitológico> relacionamentos)
        {
            var existemCorrs = await _context.RelacionamentosCorrsPropMec.Find(Builders<RelacionamentoUcsCoesaAngatPorGrupoLitológico>.Filter.Empty).AnyAsync();
            if (existemCorrs)
                return false;

            await _context.CreateRelacionamentosIndex();
            await _context.RelacionamentosCorrsPropMec.InsertManyAsync(relacionamentos);
            return true;
        }
    }
}
