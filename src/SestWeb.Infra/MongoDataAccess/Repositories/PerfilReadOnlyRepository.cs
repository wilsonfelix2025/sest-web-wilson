using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trend;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class PerfilReadOnlyRepository : IPerfilReadOnlyRepository
    {
        private readonly Context _context;

        public PerfilReadOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<PerfilBase> ObterPerfil(string id)
        {
            if (ObjectId.TryParse(id, out var mongoId))
                return await _context.Perfis.Find(perfil => perfil.Id == mongoId).Limit(1).FirstOrDefaultAsync();

            throw new InfrastructureException($"O id {id} não está em um formato válido.");
        }

        public async Task<bool> ExistePerfilComId(string idPerfil)
        {
            return await _context.Perfis.Find(p => p.Id.Equals(new ObjectId(idPerfil))).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<PerfilBase>> ObterPerfisDeUmPoço(string idPoço)
        {
            return await _context.Perfis.Find(perfil => perfil.IdPoço == idPoço).ToListAsync();
        }

        public async Task<IReadOnlyCollection<PerfilBase>> ObterPerfisTrechoDeUmPoço(string idPoço)
        {
            return await _context.Perfis.Find(perfil => perfil.IdPoço == idPoço && perfil.PodeSerUsadoParaComplementarTrecho == true).ToListAsync();
        }


        /// <inheritdoc />
        public async Task<IReadOnlyCollection<PerfilBase>> ObterPerfisPorTipo(string idPoço, string mnemônico)
        {
            return await _context.Perfis.Find(perfil => perfil.IdPoço == idPoço && perfil.Mnemonico == mnemônico).ToListAsync();
        }

        public async Task<bool> ExistePerfilComMesmoNome(string nome, string idPoço)
        {
            return await _context.Perfis.Find(p => p.Nome == nome && p.IdPoço == idPoço).AnyAsync();
        }

        public async Task<IReadOnlyCollection<PerfilBase>> ObterPerfisPorListaDeIds(List<string> listaIds)
        {
            var filter = Builders<PerfilBase>.Filter
                .In(p => p.Id.ToString(), listaIds);

            var perfis = _context.Perfis.Find(filter).ToList();

            return perfis;

            //return await _context.Perfis.Find(perfil => listaIds.Contains(perfil.Id.ToString())).ToListAsync();
        }

        public async Task<Trend> ObterTrendDoPerfil(string id)
        {
            if (ObjectId.TryParse(id, out var mongoId))
            {
                var result = await _context.Perfis.Find(perfil => perfil.Id == mongoId).Limit(1).FirstOrDefaultAsync();
                return result?.Trend;
            }

            throw new InfrastructureException($"O id {id} não está em um formato válido.");
        }
    }
}
