using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{

    internal class RelacionamentoCorrsPropMecPoçoWriteOnlyRepository : IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository
    {
        private readonly Context _context;

        public RelacionamentoCorrsPropMecPoçoWriteOnlyRepository(Context context)
        {
            _context = context;
        }

        public async Task CriarRelacionamentoCorrsPropMecPoço(RelacionamentoPropMecPoço relacionamentoPropMecPoço)
        {
            if (relacionamentoPropMecPoço == null)
                throw new ArgumentNullException(nameof(relacionamentoPropMecPoço), "Relacionamento do poço não pode ser null.");
            await _context.RelacionamentosCorrsPropMecPoço.InsertOneAsync(relacionamentoPropMecPoço);
        }

        public async Task<bool> RemoverRelacionamentoCorrsPropMecPoço(string nome)
        {
            var result = await _context.RelacionamentosCorrsPropMecPoço.DeleteOneAsync(rel => rel.Nome == nome);

            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        public async Task<bool> UpdateRelacionamentoCorrsPropMecPoço(RelacionamentoPropMecPoço relacionamentoPropMecPoço)
        {
            if (relacionamentoPropMecPoço == null)
                throw new ArgumentNullException(nameof(relacionamentoPropMecPoço), "Relacionamento do poço não pode ser null.");

            var result = await _context.RelacionamentosCorrsPropMecPoço.ReplaceOneAsync(rel => rel.Nome == relacionamentoPropMecPoço.Nome, relacionamentoPropMecPoço);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}
