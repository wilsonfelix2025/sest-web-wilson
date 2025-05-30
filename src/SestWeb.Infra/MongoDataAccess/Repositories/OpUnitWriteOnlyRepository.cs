using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class OpUnitWriteOnlyRepository : IOpUnitWriteOnlyRepository
    {
        private readonly Context _context;

        public OpUnitWriteOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task CreateOpUnit(OpUnit opUnit)
        {
            var checkDuplicity = await _context.OpUnits.Find(op => op.Name == opUnit.Name).AnyAsync();
            if (checkDuplicity)
                throw new Exception($"Já existe uma unidade operacional com o nome '{opUnit.Name}'.");
            checkDuplicity = await _context.OpUnits.Find(op => op.Id == opUnit.Id).AnyAsync();
            if (checkDuplicity)
                throw new Exception($"Já existe uma unidade operacional com o mesmo id '{opUnit.Id}'.");

            await _context.OpUnits.InsertOneAsync(opUnit);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteOpUnit(string id)
        {
            var result = await _context.OpUnits.DeleteOneAsync(opUnit => opUnit.Id == id);
            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateOpUnit(OpUnit opUnit)
        {
            if (opUnit == null)
                throw new ArgumentNullException(nameof(opUnit), "Unidade operacional não pode ser nulo.");

            var result = await _context.OpUnits.ReplaceOneAsync(ou => ou.Id == opUnit.Id, opUnit);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}
