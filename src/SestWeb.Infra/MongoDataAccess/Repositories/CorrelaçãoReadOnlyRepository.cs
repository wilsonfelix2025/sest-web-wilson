using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class CorrelaçãoReadOnlyRepository : ICorrelaçãoReadOnlyRepository
    {
        private readonly Context _context;

        public CorrelaçãoReadOnlyRepository(Context context)
        {
            _context = context;
        }

        public Task<bool> ExisteCorrelação(string nome)
        {
            return _context.Correlações.Find(correlação => correlação.Nome == nome).AnyAsync();
        }

        public async Task<Correlação> ObterCorrelaçãoPeloNome(string nome)
        {
            return await _context.Correlações.Find(correlação => correlação.Nome == nome).Limit(1).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPorNomes(List<string> listaNomes)
        {
            return await _context.Correlações.Find(correlação => listaNomes.Contains(correlação.Nome)).ToListAsync();
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPorChaveUsuário(string userKey)
        {
            return await _context.Correlações.Find(correlação => correlação.Autor.Chave == userKey).ToListAsync();
        }
        
        public async Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPorMnemônico(string mnemônico)
        {
            return await _context.Correlações.Find(correlação => correlação.PerfisSaída.Tipos[0] == mnemônico).ToListAsync();
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesFixas()
        {
            return await _context.Correlações.Find(correlação => correlação.Origem == Origem.Fixa).ToListAsync();
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesDeUsuários()
        {
            return await _context.Correlações.Find(correlação => correlação.Origem == Origem.Usuário).ToListAsync();
        }

        public async Task<IReadOnlyCollection<Autor>> ObterAutoresCorrelações()
        {
            return
                await _context.Correlações
                    .Distinct<Autor>(new ExpressionFieldDefinition<Correlação, Autor>(c => (Autor)c.Autor), new ExpressionFilterDefinition<Correlação>(c => true)).ToListAsync(); 
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterTodasCorrelações()
        {
            return await _context.Correlações.Find(Builders<Correlação>.Filter.Empty).ToListAsync();
        }

        public async Task<bool> CorrelaçõesSistemaCarregadas()
        {
            return await _context.Correlações.Find(correlação => correlação.Origem == Origem.Fixa).AnyAsync();
        }

        public async Task<bool> ExistemCorrelações()
        {
            return await _context.Correlações.Find(Builders<Correlação>.Filter.Empty).AnyAsync();
        }
    }
}
