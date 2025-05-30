using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class WellWriteOnlyRepository : IWellWriteOnlyRepository
    {
        private readonly Context _context;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;

        public WellWriteOnlyRepository(Context context, IWellReadOnlyRepository wellReadOnlyRepository)
        {
            _context = context;
            _wellReadOnlyRepository = wellReadOnlyRepository;
        }
        /// <inheritdoc />
        public async Task<Well> CreateWell(WellRequest well, string authorization)
        {
            var checkDuplicity = await _wellReadOnlyRepository.HasWellWithTheSameName(well.name);
            if (checkDuplicity)
                throw new InfrastructureException($"Já existe um poço com o nome '{well.name}'.");
            int id = await _context.findIdNoUsed<Well>(_context.Wells);
            Well newWell = new Well("https://pocoweb.petro.intelie.net/api/public/well/" + id + "/", well.name, well.oilfield);
            // Well newWell = await _context._pocoWebService.createWell(well, authorization);
            if (newWell == null)
            {
                return null;
            }

            await _context.Wells.InsertOneAsync(newWell);

            return newWell;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteWell(string id)
        {
            var result = await _context.Wells.DeleteOneAsync(well => well.Id == id);
            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateWell(Well well)
        {
            if (well == null)
                throw new ArgumentNullException(nameof(well), "Poço não pode ser nulo.");

            var result = await _context.Wells.ReplaceOneAsync(w => w.Id == well.Id, well);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}
