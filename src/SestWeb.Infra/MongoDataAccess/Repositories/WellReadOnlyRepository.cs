using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class WellReadOnlyRepository : IWellReadOnlyRepository
    {
        private readonly Context _context;
        public WellReadOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Well> GetWell(string id)
        {
            Well result = await _context.Wells.Find(well => well.Id == id).Limit(1).FirstOrDefaultAsync();
            if (result != null)
            {
                return result;
            }

            throw new InfrastructureException($"Well de id {id} não encontrado.");
        }

        /// <inheritdoc />
        public async Task<List<Well>> GetWells()
        {
            return await _context.Wells.Find(FilterDefinition<Well>.Empty).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasWell(string id)
        {
            return await _context.Wells.Find(well => well.Id == id).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasWellWithTheSameName(string name)
        {
            return await _context.Wells.Find(well => well.Name == name).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<Well> GetWellByName(string name, string oilFieldId)
        {
            var result = await _context.Wells.Find(well => well.Name == name && well.OilFieldId == oilFieldId).Limit(1).FirstOrDefaultAsync();

            if (result != null)
            {
                return result;
            }

            throw new InfrastructureException($"Well de nome: {name} não encontrado.");
        }
    }
}
