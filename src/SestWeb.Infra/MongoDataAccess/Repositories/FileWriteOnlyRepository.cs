using MongoDB.Driver;
using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class FileWriteOnlyRepository : IFileWriteOnlyRepository
    {
        private readonly Context _context;
        private readonly IFileReadOnlyRepository _fileReadOnlyRepository;

        public FileWriteOnlyRepository(Context context, IFileReadOnlyRepository fileReadOnlyRepository)
        {
            _context = context;
            _fileReadOnlyRepository = fileReadOnlyRepository;
        }
        /// <inheritdoc />
        public async Task<File> CreateFile(FileRequest file, string authorization)
        {
            var checkDuplicity = await _fileReadOnlyRepository.HasFileWithTheSameName(file.name);
            if (checkDuplicity)
                throw new InfrastructureException($"Já existe um arquivo com o nome '{file.name}'.");

            int id = await _context.findIdNoUsed<File>(_context.Files);
            File newFile = new File("https://pocoweb.petro.intelie.net/api/public/file/" + id + "/?rev=8159", file.name, file.well, file.file_type);
            // File newFile = await _context._pocoWebService.createFile(file, authorization);
            if (newFile == null)
            {
                return null;
            }

            await _context.Files.InsertOneAsync(newFile);

            return newFile;
        }

        /// <inheritdoc />
        public async Task<decimal> DeleteFile(string id, string authorization)
        {
            // bool deleted = await _context._pocoWebService.deleteFile(id, authorization);
            // if (!deleted)
            // {
            //     return -1;
            // }

            var result = await _context.Files.DeleteOneAsync(file => file.Id == id);
            return (result.IsAcknowledged && result.DeletedCount == 1) ? 1 : 0;
        }

        private async Task<string> findNameNoUsed(string fileName)
        {
            var newName = fileName;
            var count = 1;
            do
            {
                newName = $"{fileName} ({count})";
                count++;
            } while (await _fileReadOnlyRepository.HasFileWithTheSameName(newName));
            return newName;
        }

        /// <inheritdoc />
        public async Task<File> DuplicateFile(FileRequest file, string authorization)
        {
            if (await _fileReadOnlyRepository.HasFileWithTheSameName(file.name))
            {
                file.name = await findNameNoUsed(file.name);
            }
            return await CreateFile(file, authorization);
        }

        /// <inheritdoc />
        public async Task MoveFile(Well well, File file, Well newWell)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "Arquivo não pode ser nulo.");

            if (well == null)
                throw new ArgumentNullException(nameof(well), "Poço não pode ser nulo.");

            if (newWell == null)
                throw new ArgumentNullException(nameof(newWell), "Novo poço não pode ser nulo.");

            newWell.Files.Add(file.Id);
            well.Files.RemoveAll(f => f == file.Id);
            file.WellId = newWell.Id;

            var updateDefinition = Builders<Well>.Update.Set(w => w.Files, well.Files);
            await _context.Wells.UpdateOneAsync(w => w.Id == well.Id, updateDefinition);

            updateDefinition = Builders<Well>.Update.Set(w => w.Files, newWell.Files);
            await _context.Wells.UpdateOneAsync(w => w.Id == newWell.Id, updateDefinition);

            await UpdateFile(file);
        }

        /// <inheritdoc />
        public async Task<(bool, string)> RenameFile(File file, string newName)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "Arquivo não pode ser nulo.");

            if (newName == null)
                throw new ArgumentNullException(nameof(newName), "O novo nome não pode ser nulo.");

            if (await _fileReadOnlyRepository.HasFileWithTheSameName(newName))
            {
                newName = await findNameNoUsed(newName);
            }

            file.Name = newName;

            var result = await _context.Files.ReplaceOneAsync(f => f.Id == file.Id, file);
            return ((result.IsAcknowledged && result.ModifiedCount == 1), newName);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateFile(File file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "Arquivo não pode ser nulo.");

            var result = await _context.Files.ReplaceOneAsync(f => f.Id == file.Id, file);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}
