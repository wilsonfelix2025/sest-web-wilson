using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class FileReadOnlyRepository : IFileReadOnlyRepository
    {
        private readonly Context _context;

        public FileReadOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<File> GetFile(string id)
        {
            File result = await _context.Files.Find(file => file.Id == id).Limit(1).FirstOrDefaultAsync();
            if (result != null)
            {
                return result;
            }

            throw new InfrastructureException($"File de id {id} não encontrado.");
        }

        /// <inheritdoc />
        public async Task<List<File>> GetFiles()
        {
            return await _context.Files.Find(FilterDefinition<File>.Empty).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<File>> GetFilesByWell(string wellId)
        {
            return await _context.Files.Find(file => file.WellId == wellId).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasFile(string id)
        {
            return await _context.Files.Find(file => file.Id == id).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasFileWithTheSameName(string name)
        {
            return await _context.Files.Find(file => file.Name == name).AnyAsync();
        }
    }
}