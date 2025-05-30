using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class RegistrosEventosReadOnlyRepository : IRegistrosEventosReadOnlyRepository
    {
        private readonly Context _context;

        public RegistrosEventosReadOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<RegistroEvento> ObterRegistroEvento(string id)
        {
            if (ObjectId.TryParse(id, out var mongoId))
                return await _context.RegistrosEventos.Find(registroEvento => registroEvento.Id == mongoId).Limit(1).FirstOrDefaultAsync();

            throw new InfrastructureException($"O id {id} não está em um formato válido.");
        }

        public async Task<bool> ExisteRegistroEventoComId(string id)
        {
            return await _context.RegistrosEventos.Find(el => el.Id.Equals(new ObjectId(id))).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<RegistroEvento>> ObterRegistrosEventosDeUmPoço(string idPoço)
        {
            return await _context.RegistrosEventos.Find(el => el.IdPoço == idPoço).ToListAsync();
        }

        public async Task<IReadOnlyCollection<RegistroEvento>> ObterRegistrosEventosPorListaDeIds(List<string> listaIds)
        {
            var filter = Builders<RegistroEvento>.Filter
                .In(p => p.Id.ToString(), listaIds);

            var registrosEventos = _context.RegistrosEventos.Find(filter).ToList();

            return registrosEventos;

            //return await _context.Perfis.Find(perfil => listaIds.Contains(perfil.Id.ToString())).ToListAsync();
        }
    }
}
