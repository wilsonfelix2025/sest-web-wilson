using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OilField;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class OilFieldWriteOnlyRepository : IOilFieldWriteOnlyRepository
    {
        private readonly Context _context;

        public OilFieldWriteOnlyRepository(Context context)
        {
            _context = context;
        }
        /// <inheritdoc />
        public async Task CreateOilField(OilField oilField)
        {
            var checkDuplicity = await _context.OilFields.Find(of => of.Name == oilField.Name).AnyAsync();
            if (checkDuplicity)
                throw new Exception($"Já existe um campo com o nome '{oilField.Name}'.");
            checkDuplicity = await _context.OilFields.Find(of => of.Id == oilField.Id).AnyAsync();
            if (checkDuplicity)
                throw new Exception($"Já existe um campo com o mesmo id '{oilField.Id}'.");

            await _context.OilFields.InsertOneAsync(oilField);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteOilField(string id)
        {
            var result = await _context.OilFields.DeleteOneAsync(oilField => oilField.Id == id);
            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateOilField(OilField oilField)
        {
            if (oilField == null)
                throw new ArgumentNullException(nameof(oilField), "Campo não pode ser nulo.");

            var result = await _context.OilFields.ReplaceOneAsync(of => of.Id == oilField.Id, oilField);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}
