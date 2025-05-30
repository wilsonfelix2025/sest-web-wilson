using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.LoaderCorrelaçõesSistema;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class CorrelaçãoWriteOnlyRepository : ICorrelaçãoWriteOnlyRepository
    {
        private readonly Context _context;

        public CorrelaçãoWriteOnlyRepository(Context context)
        {
            _context = context;
        }

        public async Task CriarCorrelação(Correlação correlação)
        {
            if (correlação == null)
                throw new ArgumentNullException(nameof(correlação), "Correlação não pode ser nula.");
            await _context.Correlações.InsertOneAsync(correlação);
        }

        //public async Task CriarCorrelaçãoPoço(CorrelaçãoPoço correlaçãoPoço)
        //{
        //    if (correlaçãoPoço == null)
        //        throw new ArgumentNullException(nameof(correlaçãoPoço), "Correlação não pode ser nula.");
        //    await _context.CorrelaçõesPoço.InsertOneAsync(correlaçãoPoço);
        //}

        public async Task<bool> RemoverCorrelação(string nome)
        {
            var result = await _context.Correlações.DeleteOneAsync(correlação => correlação.Nome == nome);

            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        public async Task<bool> UpdateCorrelação(Correlação correlação)
        {
            if (correlação == null)
                throw new ArgumentNullException(nameof(correlação), "Correlação não pode ser nula.");

            var result = await _context.Correlações.ReplaceOneAsync(corr => corr.Nome == correlação.Nome, correlação);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }

        public async Task<bool> InsertCorrelaçõesSistema(IList<Correlação> correlações)
        {

            if (correlações == null)
                throw new ArgumentNullException("correlações", "correlações não pode ser nulo.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var existemCorrs = await _context.Correlações.Find(Builders<Correlação>.Filter.Empty).AnyAsync();

                    if (!existemCorrs)
                    {
                        await _context.CreateCorrelaçõesIndex();
                    }

                    foreach (var correlação in correlações)
                    {
                        var existeCorr = await _context.Correlações.Find(corr => corr.Nome == correlação.Nome).AnyAsync();

                        if (existeCorr)
                            await UpdateCorrelação(correlação);
                        else
                            await CriarCorrelação(correlação);
                    }

                    await session.CommitTransactionAsync();

                    return true;
                }

                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                    return false;
                }
            }
        }
    }
}
