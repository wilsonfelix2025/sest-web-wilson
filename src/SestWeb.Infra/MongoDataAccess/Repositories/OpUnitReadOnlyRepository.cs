using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Exceptions;
using System.Linq;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class OpUnitReadOnlyRepository : IOpUnitReadOnlyRepository
    {
        private readonly Context _context;

        public OpUnitReadOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<OpUnit> GetOpUnit(string id)
        {
            if (id.Contains("pocoweb"))
            {
                var urlParts = id.Split("/");
                id = urlParts[urlParts.Length - 2];
            }

            OpUnit result = await _context.OpUnits.Find(opUnit => opUnit.Id == id).Limit(1).FirstOrDefaultAsync();
            if (result != null)
            {
                return result;
            }

            throw new InfrastructureException($"OpUnit de id {id} não encontrado.");
        }

        /// <inheritdoc />
        public async Task<List<OpUnit>> GetOpUnits()
        {
            return await _context.OpUnits.Find(FilterDefinition<OpUnit>.Empty).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasOpUnit(string id)
        {
            return await _context.OpUnits.Find(opUnit => opUnit.Id == id).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasOpUnitWithTheSameName(string name)
        {
            return await _context.OpUnits.Find(opUnit => opUnit.Name == name).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<OpUnit> GetOpUnitByName(string name)
        {
            OpUnit result = await _context.OpUnits.Find(opUnit => opUnit.Name == name)
                .Limit(1)
                .FirstOrDefaultAsync();

            if (result != null)
            {
                return result;
            }

            throw new InfrastructureException($"OpUnit de nome: {name} não encontrado.");
        }

        public string GetLastId()
        {
            var listIds = _context.OpUnits.AsQueryable().Select(s => s.Id).ToList();
            return listIds.Last();
        }
    }
}
