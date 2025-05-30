using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Exceptions;
using System.Linq;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class OilFieldReadOnlyRepository : IOilFieldReadOnlyRepository
    {
        private readonly Context _context;

        public OilFieldReadOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<OilField> GetOilField(string id)
        {
            if (id.Contains("pocoweb"))
            {
                var urlParts = id.Split("/");
                id = urlParts[urlParts.Length - 2];
            }

            OilField result = await _context.OilFields.Find(oilField => oilField.Id == id).Limit(1).FirstOrDefaultAsync();
            if (result != null)
            {
                return result;
            }

            throw new InfrastructureException($"Oilfield de id {id} não encontrado.");
        }

        /// <inheritdoc />
        public async Task<List<OilField>> GetOilFields()
        {
            return await _context.OilFields.Find(FilterDefinition<OilField>.Empty).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasOilField(string id)
        {
            return await _context.OilFields.Find(oilField => oilField.Id == id).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasOilFieldWithTheSameName(string name)
        {
            return await _context.OilFields.Find(oilField => oilField.Name == name).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<OilField> GetOilFieldByName(string name, string optUnitId)
        {
            OilField result = await _context.OilFields.Find(oilField => oilField.Name == name && oilField.OpUnitId == optUnitId)
                .Limit(1)
                .FirstOrDefaultAsync();

            return result;
        }

        public string GetLastId()
        {
            var listIds = _context.OilFields.AsQueryable().Select(s => s.Id).ToList();
            return listIds.Last();
        }
    }
}
