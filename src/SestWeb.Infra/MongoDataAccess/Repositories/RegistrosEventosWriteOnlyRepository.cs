using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class RegistrosEventosWriteOnlyRepository : IRegistrosEventosWriteOnlyRepository
    {
        private readonly Context _context;

        public RegistrosEventosWriteOnlyRepository(Context context)
        {
            _context = context;
        }

        private async Task AtualizarArvore(Poço poço, RegistroEvento registroEvento)
        {
            // Se tiver algum trecho
            if (registroEvento.Trechos.Count > 0)
            {
                // Adicionar na árvore

                //Encontrar pasta de perfuração
                var perfuracao = poço.State.Tree.Find(t => t.Name == "Perfuração");

                var folhaRegistro = new Tree
                {
                    Id = registroEvento.Id.ToString(),
                    Fixa = false,
                    Name = registroEvento.Nome,
                    Tipo = "4"
                };
                // Verificar tipo do Registro / Evento
                if (registroEvento.Tipo == TipoRegistroEvento.Registro)
                {
                    // Se tiver pasta de Registros
                    var i = perfuracao.Data.FindIndex(t => t.Name == "Registros");
                    if (i >= 0)
                    {
                        if (perfuracao.Data[i].Data.FindIndex(el => el.Id == registroEvento.Id.ToString()) < 0)
                        {
                            // Adiciona registro na pasta
                            perfuracao.Data[i].Data.Add(folhaRegistro);
                        }
                    }
                    else
                    {
                        // Cria pasta de registro
                        var pastaRegistro = new Tree
                        {
                            Fixa = false,
                            Name = "Registros",
                            Tipo = "1"
                        };
                        pastaRegistro.Data.Add(folhaRegistro);
                        perfuracao.Data.Add(pastaRegistro);
                    }
                }
                else
                {
                    // Se tiver pasta de Eventos
                    var i = perfuracao.Data.FindIndex(t => t.Name == "Eventos de Perfuração");
                    if (i >= 0)
                    {
                        if (perfuracao.Data[i].Data.FindIndex(el => el.Id == registroEvento.Id.ToString()) < 0)
                        {
                            // Adiciona evento na pasta
                            perfuracao.Data[i].Data.Add(folhaRegistro);
                        }
                    }
                    else
                    {
                        // Cria pasta de eventos
                        var pastaRegistro = new Tree
                        {
                            Fixa = false,
                            Name = "Eventos de Perfuração",
                            Tipo = "1"
                        };
                        pastaRegistro.Data.Add(folhaRegistro);
                        perfuracao.Data.Add(pastaRegistro);
                    }
                }
            }
            else
            {
                // Se não tem trecho, remove da árvore
                poço.State.RemoveFromTree(registroEvento.Id.ToString());
            }
        }

        public async Task<bool> AtualizarRegistroEvento(Poço poço, RegistroEvento registroEventoAtualizado)
        {
            if (registroEventoAtualizado == null)
                throw new Exception("Erro ao atualizar registro/evento.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var result = await _context.RegistrosEventos.ReplaceOneAsync(f => f.Id == registroEventoAtualizado.Id, registroEventoAtualizado);

                    await AtualizarArvore(poço, registroEventoAtualizado);

                    var update = Builders<Poço>.Update.Set(p => p.State, poço.State);
                    await _context.Poços.UpdateOneAsync(p => p.Id == poço.Id, update);

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

        public async Task AtualizarRegistrosEventos(Poço poçoAtualizado, IReadOnlyCollection<RegistroEvento> registrosEventos)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    foreach (var registroEvento in registrosEventos)
                    {
                        await _context.RegistrosEventos.ReplaceOneAsync(f => f.Id == registroEvento.Id, registroEvento);

                        // Atualiza a árvore de registro/eventos
                        await AtualizarArvore(poçoAtualizado, registroEvento);
                    }

                    var update = Builders<Poço>.Update.Set(poço => poço.State, poçoAtualizado.State);
                    await _context.Poços.UpdateOneAsync(poço => poço.Id == poçoAtualizado.Id, update);

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
