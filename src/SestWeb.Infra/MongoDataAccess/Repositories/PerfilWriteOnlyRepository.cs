using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class PerfilWriteOnlyRepository : IPerfilWriteOnlyRepository
    {
        private readonly Context _context;

        public PerfilWriteOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task CriarPerfil(string idPoço, PerfilBase perfil, Poço poçoDoPerfil = null)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    await AtualizaPerfilTreeState(poçoDoPerfil, perfil);
                    
                    var update = Builders<Poço>.Update.Set(poço => poço.State, poçoDoPerfil.State);
                    await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                    
                    await session.CommitTransactionAsync();
                }

                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoverPerfil(string idPerfil, Poço poçoDoPerfil)
        {
            if (!ObjectId.TryParse(idPerfil, out var mongoId))
                throw new InfrastructureException($"O id do perfil {idPerfil} não está em um formato válido.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    poçoDoPerfil.State.RemoveFromTree(idPerfil);

                    var update = Builders<Poço>.Update.Set(poço => poço.State, poçoDoPerfil.State);
                    await _context.Poços.UpdateOneAsync(poço => poço.Id == poçoDoPerfil.Id, update);

                    var result = await _context.Perfis.DeleteOneAsync(perfil => perfil.Id == mongoId);

                    await session.CommitTransactionAsync();
                    return result.IsAcknowledged && result.DeletedCount == 1;
                }
                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                    return false;
                }
            }
        }

        public async Task<bool> AtualizarPerfil(PerfilBase perfilOldAtualizado)
        {
            if (perfilOldAtualizado == null)
                throw new Exception("Erro ao atualizar perfil.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var result = await _context.Perfis.ReplaceOneAsync(f => f.Id == perfilOldAtualizado.Id, perfilOldAtualizado);

                    await session.CommitTransactionAsync();
                    return result.IsAcknowledged && result.ModifiedCount == 1;
                }
                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                    return false;
                }
            }
        }

        public async Task AtualizarPerfis(string idPoço, Poço poçoAtualizado)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var atualizaTree = await AtualizaTodosPerfisTreeState(poçoAtualizado);

                    if (atualizaTree)
                    {
                        var update = Builders<Poço>.Update.Set(poço => poço.State, poçoAtualizado.State);
                        await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                    }

                    await session.CommitTransactionAsync();
                }

                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                }
            }
        }

        private async Task<bool> AtualizaTodosPerfisTreeState(Poço poçoAtualizado)
        {
            if (poçoAtualizado == null)
                return false;

            bool atualizaTree = false;

            foreach (var perfil in poçoAtualizado.Perfis)
            {
                var atualizar = await AtualizaPerfilTreeState(poçoAtualizado, perfil);

                if (atualizar)
                    atualizaTree = true;
            }

            return atualizaTree;
        }

        private async Task<bool> AtualizaPerfilTreeState(Poço poçoAtualizado, PerfilBase perfil)
        {
            if (poçoAtualizado == null || perfil == null)
                return false;

            bool atualizaTree = false;

            if (perfil.IdPoço == null)
            {
                atualizaTree = true;
                perfil.IdPoço = poçoAtualizado.Id;
                await _context.Perfis.InsertOneAsync(perfil);

                //atualiza a árvore de perfis
                poçoAtualizado.State.Tree.ForEach(t =>
                {
                    if (t.Name == perfil.GrupoPerfis.Nome)
                    {
                        var perfilTree = new Tree
                        {
                            Id = perfil.Id.ToString(),
                            Fixa = false,
                            Name = perfil.Nome,
                            Tipo = "0"
                        };
                        t.Data.Add(perfilTree);
                    }
                });
            }
            else
            {
                await _context.Perfis.ReplaceOneAsync(f => f.Id == perfil.Id, perfil);
            }


            return atualizaTree;
        }



        public async Task AtualizarTrendDoPerfil(PerfilBase perfilOld)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var filter = Builders<PerfilBase>.Filter.Eq(x => x.Id, perfilOld.Id);
                    var update = Builders<PerfilBase>.Update.Set(x => x.Trend, perfilOld.Trend);
                    await _context.Perfis.UpdateOneAsync(filter, update);
                    await session.CommitTransactionAsync();
                }
                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                }
            }
        }


    }
}
