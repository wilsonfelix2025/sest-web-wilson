using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class CorrelaçãoPoçoWriteOnlyRepository : ICorrelaçãoPoçoWriteOnlyRepository
    {
        private readonly Context _context;

        public CorrelaçãoPoçoWriteOnlyRepository(Context context)
        {
            _context = context;
        }

        public async Task CriarCorrelaçãoPoço(CorrelaçãoPoço correlaçãoPoço)
        {
            if (correlaçãoPoço == null)
                throw new ArgumentNullException(nameof(correlaçãoPoço), "Correlação não pode ser nula.");
            await _context.CorrelaçõesPoço.InsertOneAsync(correlaçãoPoço);
        }

        public async Task<bool> RemoverCorrelaçãoPoço(string nome)
        {
            var result = await _context.CorrelaçõesPoço.DeleteOneAsync(correlação => correlação.Nome == nome);

            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        public async Task<bool> UpdateCorrelaçãoPoço(CorrelaçãoPoço correlação)
        {
            if (correlação == null)
                throw new ArgumentNullException(nameof(correlação), "Correlação não pode ser nula.");

            var result = await _context.CorrelaçõesPoço.ReplaceOneAsync(corr => corr.Nome == correlação.Nome, correlação);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}
