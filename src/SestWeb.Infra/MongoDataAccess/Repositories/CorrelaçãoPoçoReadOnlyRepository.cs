using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    public class CorrelaçãoPoçoReadOnlyRepository : ICorrelaçãoPoçoReadOnlyRepository
    {
        private readonly Context _context;

        public CorrelaçãoPoçoReadOnlyRepository(Context context)
        {
            _context = context;
        }


        public Task<bool> ExisteCorrelaçãoPoço(string idPoço, string nome)
        {
            return _context.CorrelaçõesPoço.Find(correlação => correlação.IdPoço == idPoço && correlação.Nome == nome).AnyAsync();
        }

        public async Task<Correlação> ObterCorrelaçãoPoçoPeloNome(string idPoço, string nome)
        {
            return await _context.CorrelaçõesPoço.Find(correlação => correlação.IdPoço == idPoço && correlação.Nome == nome)
                .Project(corr => (Correlação)corr.Correlação)
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPoçoPorChaveUsuário(string idPoço, string userKey)
        {
            return await _context.CorrelaçõesPoço
                .Find(correlação => correlação.IdPoço == idPoço && correlação.Correlação.Autor.Chave == userKey)
                .Project(corr => (Correlação)corr.Correlação)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPoçoPorMnemônico(string idPoço, string mnemônico)
        {
            return await _context.CorrelaçõesPoço.Find(correlação => correlação.IdPoço == idPoço && correlação.Correlação.PerfisSaída.Tipos[0] == mnemônico)
                .Project(corr => (Correlação)corr.Correlação)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Autor>> ObterAutoresCorrelaçõesPoço(string idPoço)
        {
            return
                await _context.CorrelaçõesPoço
                    .Distinct<Autor>(new ExpressionFieldDefinition<CorrelaçãoPoço, Autor>(c => (Autor)c.Correlação.Autor), new ExpressionFilterDefinition<CorrelaçãoPoço>(c => c.IdPoço == idPoço && c.Correlação.Origem == Origem.Poço)).ToListAsync();
        }

        public async Task<IReadOnlyCollection<Correlação>> ObterTodasCorrelaçõesPoço(string idPoço)
        {
            return await _context.CorrelaçõesPoço.Find(correlação => correlação.IdPoço == idPoço)
                .Project(corr => (Correlação)corr.Correlação)
                .ToListAsync();
        }

        public async Task<bool> ExistemCorrelaçõesPoço(string idPoço)
        {
            return await _context.CorrelaçõesPoço.Find(correlação => correlação.IdPoço == idPoço).AnyAsync();
        }
    }
}
