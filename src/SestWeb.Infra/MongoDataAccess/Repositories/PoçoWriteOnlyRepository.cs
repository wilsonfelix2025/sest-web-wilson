using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using MathNet.Numerics;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class PoçoWriteOnlyRepository : IPoçoWriteOnlyRepository
    {
        private readonly Context _context;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPipelineUseCase _pipelineUseCase;

        public PoçoWriteOnlyRepository(Context context, IPerfilWriteOnlyRepository perfilWriteOnlyRepository, IPipelineUseCase pipelineUseCase)
        { 
            _context = context;
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _pipelineUseCase = pipelineUseCase;
        }

        #region Poço

        public async Task CriarPoço(Poço poço)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    await _context.Poços.InsertOneAsync(poço);
                    if (poço.Perfis != null && poço.Perfis.Count > 0)
                    {
                        await _context.Perfis.InsertManyAsync(poço.Perfis);
                    }
                    if (poço.RegistrosEventos != null && poço.RegistrosEventos.Count > 0)
                    {
                        await _context.RegistrosEventos.InsertManyAsync(poço.RegistrosEventos);
                    }

                    await session.CommitTransactionAsync();
                }

                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                }
            }
        }

        public async Task DuplicarPoço(Poço poço)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    await AtualizarTreeNaDuplicaçãoPoço(poço);

                    await _context.Poços.InsertOneAsync(poço);

                    await session.CommitTransactionAsync();
                }

                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                }
            }
        }

        private async Task AtualizarTreeNaDuplicaçãoPoço(Poço poço)
        {
            foreach (var perfil in poço.Perfis)
            {
                perfil.IdPoço = poço.Id;

                await _context.Perfis.InsertOneAsync(perfil);

                var grupo = perfil.GrupoPerfis == null ? "Perfis" : perfil.GrupoPerfis.Nome;
                //atualiza a árvore de perfis
                poço.State.Tree.ForEach(t =>
                {
                    if (t.Name == grupo)
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
        }


        public async Task<bool> RemoverPoço(string idPoço)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var result = await _context.Poços.DeleteOneAsync(poço => poço.Id == idPoço);

                    // Se removeu o poço, remover também os perfis do poço.
                    if (result.IsAcknowledged && result.DeletedCount == 1)
                    {
                        result = await _context.Perfis.DeleteManyAsync(perfil => perfil.IdPoço == idPoço);
                        result = await _context.RegistrosEventos.DeleteManyAsync(regEv => regEv.IdPoço == idPoço);
                        return result.IsAcknowledged;
                    }

                    await session.CommitTransactionAsync();

                    return false;
                }

                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                    return false;
                }
            }

        }

        public async Task<bool> AtualizarPoço(Poço poçoAtualizado)
        {
            if (poçoAtualizado == null)
                throw new ArgumentNullException(nameof(poçoAtualizado), "Poço não pode ser nulo.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    poçoAtualizado.RegistrosEventos = await _context.RegistrosEventos.Find(regEv => regEv.IdPoço == poçoAtualizado.Id).ToListAsync();

                    await AtualizarTreeNoUpdatePoço(poçoAtualizado);

                    var result = await _context.Poços.ReplaceOneAsync(poço => poço.Id == poçoAtualizado.Id, poçoAtualizado);
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

        private async Task AtualizarTreeNoUpdatePoço(Poço poçoAtualizado)
        {
            foreach (var perfil in poçoAtualizado.Perfis)
            {
                if (perfil.IdPoço == null)
                {
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
            }

            //atualizar a Tree da litografia, caso necessário
            poçoAtualizado.State.Tree.ForEach(t =>
            {
                if (t.Name == "Litologia")
                {
                    poçoAtualizado.Litologias.ForEach(l =>
                    {
                        if (t.Data.Count(x => x.Id == l.Id.ToString()) == 0)
                        {
                            var litologiaTree = new Tree
                            {
                                Id = l.Id.ToString(),
                                Fixa = false,
                                Name = l.Classificação.Nome,
                                Tipo = "2"
                            };
                            t.Data.Add(litologiaTree);
                        }
                    });
                }
            });
        }

        public async Task<bool> AtualizarDadosGerais(string idPoço, Poço entity, bool atualizaTrajetória)
        {
            if (entity.DadosGerais == null)
                throw new ArgumentNullException(nameof(entity.DadosGerais), "Dados gerais não pode ser nulo.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.Set(poço => poço.DadosGerais, entity.DadosGerais);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

                    if (atualizaTrajetória)
                    {
                        update = Builders<Poço>.Update.Set(poço => poço.Trajetória, entity.Trajetória);
                        result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                    }

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

        public async Task<bool> AtualizarDados(string idPoço, Poço entity, List<DadosSelecionadosEnum> dadosSelecionados)
        {
            if (entity.DadosGerais == null)
                throw new ArgumentNullException(nameof(entity.DadosGerais), "Dados gerais não pode ser nulo.");


            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    if (dadosSelecionados.Contains(DadosSelecionadosEnum.Sapatas))
                    {
                        var update = Builders<Poço>.Update.Set(poço => poço.Sapatas, entity.Sapatas);
                        var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                        VerificaResultadoAtualização(result);
                    }

                    if (dadosSelecionados.Contains(DadosSelecionadosEnum.Objetivos))
                    {
                        var update = Builders<Poço>.Update.Set(poço => poço.Objetivos, entity.Objetivos);
                        var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                        VerificaResultadoAtualização(result);
                    }

                    if (dadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
                    {
                        var update = Builders<Poço>.Update.Set(poço => poço.DadosGerais, entity.DadosGerais);
                        var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                        VerificaResultadoAtualização(result);
                    }

                    if (dadosSelecionados.Contains(DadosSelecionadosEnum.Estratigrafia))
                    {
                        var update = Builders<Poço>.Update.Set(poço => poço.Estratigrafia, entity.Estratigrafia);
                        var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                        VerificaResultadoAtualização(result);
                    }

                    if (dadosSelecionados.Contains(DadosSelecionadosEnum.Trajetória) || dadosSelecionados.Contains(DadosSelecionadosEnum.MesaRotativa))
                    {
                        var update = Builders<Poço>.Update.Set(poço => poço.Trajetória, entity.Trajetória);
                        update = update.Set(poço => poço.Litologias, entity.Litologias);
                        update = update.Set(poço => poço.Estratigrafia, entity.Estratigrafia);
                        update = update.Set(poço => poço.Objetivos, entity.Objetivos);
                        update = update.Set(poço => poço.Sapatas, entity.Sapatas);
                        
                        await _perfilWriteOnlyRepository.AtualizarPerfis(idPoço, entity);

                        var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                        VerificaResultadoAtualização(result);
                    }

                    await session.CommitTransactionAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                    return false;
                }

            }

        }

        public async Task<bool> AtualizarListaPoçoApoio(string idPoço, List<string> lista)
        {
            if (lista == null)
                throw new ArgumentNullException(nameof(lista), "Lista não pode ser vazia.");

            var update = Builders<Poço>.Update.Set(poço => poço.IdsPoçosApoio, lista);
            var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

            return result.IsAcknowledged && result.ModifiedCount == 1;
        }


        #endregion

        #region Sapata

        public async Task<bool> CriarSapata(string idPoço, Sapata sapata)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.PushEach(poço => poço.Sapatas, new[] { sapata }, sort: Builders<Sapata>.Sort.Ascending("Pm"));
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        public async Task<bool> RemoverSapata(string idPoço, double profundidadeMedida)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.PullFilter(poço => poço.Sapatas, sapata => sapata.Pm == profundidadeMedida);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);
                    
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

        public async Task<bool> AtualizarSapatas(string idPoço, List<Sapata> sapatas)
        {
            if (sapatas == null || !sapatas.Any())
                throw new ArgumentNullException(nameof(sapatas), "Sapatas não pode ser vazia.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.Set(poço => poço.Sapatas, sapatas);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        #endregion

        #region Objetivo

        public async Task<bool> CriarObjetivo(string idPoço, Objetivo objetivo)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.PushEach(poço => poço.Objetivos, new[] { objetivo }, sort: Builders<Objetivo>.Sort.Ascending("Pm"));
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        public async Task<bool> RemoverObjetivo(string idPoço, double profundidadeMedida)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.PullFilter(poço => poço.Objetivos, objetivo => objetivo.Pm == profundidadeMedida);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        public async Task<bool> AtualizarObjetivos(string idPoço, List<Objetivo> objetivos)
        {
            if (objetivos == null || !objetivos.Any())
                throw new ArgumentNullException(nameof(objetivos), "Objetivos não pode ser vazio.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.Set(poço => poço.Objetivos, objetivos);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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
        #endregion

        #region Litologia

        public async Task<bool> CriarLitologia(string idPoço, Litologia litologia)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.Push(poço => poço.Litologias, litologia);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        public async Task<bool> RemoverLitologia(string idPoço, string idLitologia)
        {
            if (!ObjectId.TryParse(idLitologia, out var mongoIdLitologia))
                throw new InfrastructureException($"O idLitologia {idLitologia} não está em um formato válido.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.PullFilter(poço => poço.Litologias, litologia => litologia.Id == mongoIdLitologia);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        #endregion

        #region Estratigrafia

        public async Task<bool> AtualizarEstratigrafia(string idPoço, Estratigrafia estratigrafia)
        {
            if (estratigrafia == null)
                throw new ArgumentNullException(nameof(estratigrafia), "Estratigrafia não pode ser nula.");

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.Set(poço => poço.Estratigrafia, estratigrafia);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        #endregion

        #region Trajetória

        public async Task<bool> AtualizarTrajetória(string idPoço, Trajetória trajetória)
        {
            if (trajetória == null)
                return true;

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var update = Builders<Poço>.Update.Set(poço => poço.Trajetória, trajetória);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        #endregion

        #region Litologia

        public async Task<bool> AtualizarLitologias(string idPoço, Poço poçoAtualizado)
        {
            var lista = poçoAtualizado.Litologias;

            if (lista == null)
                return true;

            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    await AtualizarTreeNoUpdateDasLitologias(poçoAtualizado);

                    var update = Builders<Poço>.Update.Set(poço => poço.Litologias, lista).Set(poço => poço.State, poçoAtualizado.State);
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == idPoço, update);

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

        private async Task AtualizarTreeNoUpdateDasLitologias(Poço poçoAtualizado)
        {
            //atualizar a Tree da litografia, caso necessário
            poçoAtualizado.State.Tree.ForEach(t =>
            {
                if (t.Name == "Litologia")
                {
                    poçoAtualizado.Litologias.ForEach(l =>
                    {
                        if (t.Data.Count(x => x.Id == l.Id.ToString()) == 0)
                        {
                            var litologiaTree = new Tree
                            {
                                Id = l.Id.ToString(),
                                Fixa = false,
                                Name = l.Classificação.Nome,
                                Tipo = "2"
                            };
                            t.Data.Add(litologiaTree);
                        }
                    });
                }
            });
        }

        #endregion

        #region Cálculo
        public async Task<bool> CriarCálculo(Poço poço, Cálculo cálculo, string grupoCálculo, bool perfuracao = false)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    await AtualizarTreeNaCriaçãoCálculo(poço, cálculo, grupoCálculo, perfuracao);

                    var update = Builders<Poço>.Update.Set(p => p.State, poço.State)
                        .Push(p => p.Cálculos, cálculo);
                    var result = await _context.Poços.UpdateOneAsync(p => p.Id == poço.Id, update);

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

        private async Task AtualizarTreeNaCriaçãoCálculo(Poço poço, Cálculo cálculo, string grupoCálculo, bool perfuracao)
        {
            cálculo.PerfisSaída.IdPerfis = new List<string>();

            bool criarPastaCálculos = true;
            bool criarPastaGrupoCálculos = true;

            var cálculosTree = new Tree
            {
                Id = cálculo.Id.ToString(),
                Fixa = false,
                Name = cálculo.Nome,
                Tipo = "1"
            };

            foreach (var perfil in cálculo.PerfisSaída.Perfis)
            {
                if (perfil.IdPoço == null)
                {
                    perfil.IdPoço = poço.Id;
                    perfil.IdCálculo = cálculo.Id.ToString();
                    await _context.Perfis.InsertOneAsync(perfil);

                    cálculo.PerfisSaída.IdPerfis.Add(perfil.Id.ToString());

                    var perfilTree = new Tree
                    {
                        Id = perfil.Id.ToString(),
                        Fixa = false,
                        Name = perfil.Nome,
                        Tipo = "0"
                    };

                    //atualiza a árvore de perfis

                    poço.State.Tree.ForEach(t =>
                    {
                        // Se está sendo adicionado um filtro e o cursor t está na pasta de filtros
                        if (t.Name == grupoCálculo && grupoCálculo == "Filtros")
                        {
                            t.Data.Add(perfilTree);
                        }
                        // Se o cursor t estiver na pasta de cálculos ou na pasta de perfuração
                        else if ((!perfuracao && t.Name == "Cálculos") || (perfuracao && t.Name == "Perfuração"))
                        {
                            // Itera sobre as pastas de cálculos
                            foreach (var tipoCálculo in t.Data)
                            {
                                // Se achar a pasta do grupo de cálculo
                                if (tipoCálculo.Name == grupoCálculo)
                                {
                                    criarPastaGrupoCálculos = false;

                                    // Busca pela pasta do cálculo sendo adicionado
                                    tipoCálculo.Data.ForEach(c =>
                                    {
                                        // Se achar
                                        if (c.Name == cálculo.Nome)
                                        {
                                            criarPastaCálculos = false;
                                            c.Data.Add(perfilTree);
                                        }
                                    });

                                    if (criarPastaCálculos)
                                    {
                                        cálculosTree.Data.Add(perfilTree);
                                        tipoCálculo.Data.Add(cálculosTree);
                                        criarPastaCálculos = false;
                                    }
                                }
                            }

                            // Se mesmo depois de percorrer os tipos, ainda não tiver criado a pasta do cálculo,
                            // não existe a pasta do grupo de cálculo
                            if (criarPastaGrupoCálculos)
                            {
                                var pastaGrupo = new Tree
                                {
                                    Fixa = false,
                                    Name = grupoCálculo,
                                    Tipo = "1"
                                };

                                cálculosTree.Data.Add(perfilTree);
                                pastaGrupo.Data.Add(cálculosTree);
                                t.Data.Add(pastaGrupo);
                                criarPastaGrupoCálculos = false;
                            }
                        }
                    });
                }
            }
        }

        public async Task<bool> EditarCálculo(Poço poço, Cálculo cálculo, string idCálculoAntigo, string grupoCálculoState)
        {
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    await AtualizarTreeNaEdiçãoCálculo(poço, cálculo, idCálculoAntigo, grupoCálculoState);

                    var cálculos = poço.Cálculos.ToList();
                    var index = cálculos.IndexOf(cálculos.Single(x => x.Id.ToString() == idCálculoAntigo));
                    cálculos[index] = cálculo;

                    var update = Builders<Poço>.Update.Set(p => p.Cálculos, cálculos)
                                                    .Set(p => p.State, poço.State);
                    var result = await _context.Poços.UpdateOneAsync(p => p.Id == poço.Id, update);

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

        private async Task AtualizarTreeNaEdiçãoCálculo(Poço poço, Cálculo cálculo, string idCálculoAntigo,
            string grupoCálculoState)
        {
            foreach (var perfil in cálculo.PerfisSaída.Perfis)
            {
                //perfis litológicos são adicionados ao final da criação do cálculo e não precisam ser adicionados novamente
                if (perfil.Id == ObjectId.Empty) continue;

                perfil.IdPoço = poço.Id;
                perfil.IdCálculo = cálculo.Id.ToString();
                await _context.Perfis.ReplaceOneAsync(f => f.Id == perfil.Id, perfil);

                //atualiza a árvore de Filtros
                poço.State.Tree.ForEach(t =>
                {
                    if (t.Name == grupoCálculoState && grupoCálculoState == "Filtros")
                    {
                        foreach (var d in t.Data)
                        {
                            if (d.Id == perfil.Id.ToString())
                            {
                                d.Name = perfil.Nome;
                                break;
                            }
                        }
                    }
                    else if (t.Name == "Cálculos")
                    {
                        foreach (var tipoCálculo in t.Data)
                        {
                            if (tipoCálculo.Name == grupoCálculoState)
                            {
                                tipoCálculo.Data.ForEach(c =>
                                {
                                    if (c.Id == idCálculoAntigo)
                                    {
                                        c.Id = cálculo.Id.ToString();
                                        c.Name = cálculo.Nome;
                                    }

                                    foreach (var perfilTree in c.Data)
                                    {
                                        if (perfilTree.Id == perfil.Id.ToString())
                                        {
                                            perfilTree.Name = perfil.Nome;
                                            break;
                                        }
                                    }
                                });
                            }
                        }
                    }
                });
            }
        }

        public async Task<bool> RemoverCálculo(string idPoço, string idCálculo)
        {
            //atualiza os dados no banco dentro de uma transação (coleção de perfis e poço)
            using (var session = await _context._mongoClient.StartSessionAsync())
            {
                // Begin transaction
                session.StartTransaction();

                try
                {
                    var poçoAtual = await _context.Poços.Find(p => p.Id == idPoço).SingleAsync();
                    var calculo = poçoAtual.Cálculos.First(c => c.Id.ToString() == idCálculo);

                    await AtualizarTreeNaRemoçãoCálculo(idCálculo, calculo, poçoAtual);

                    await _context.Perfis.DeleteManyAsync(p => p.IdCálculo == idCálculo && p.IdPoço == poçoAtual.Id);

                    var update = Builders<Poço>.Update.Set(poço => poço.State, poçoAtual.State)
                            .PullFilter(poço => poço.Cálculos, calc => calc.Id == calculo.Id)
                        ;
                    var result = await _context.Poços.UpdateOneAsync(poço => poço.Id == poçoAtual.Id, update);

                    await session.CommitTransactionAsync();

                    return result.IsAcknowledged && result.ModifiedCount == 1;
                }
                catch (Exception)
                {
                    await session.AbortTransactionAsync();
                    return false;
                }
            }
        }

        private async Task AtualizarTreeNaRemoçãoCálculo(string idCálculo, Cálculo calculo, Poço poçoAtual)
        {
            //atualiza a árvore de perfis
            calculo.PerfisSaída.IdPerfis.ForEach(id => { poçoAtual.State.RemoveFromTree(id); });
            poçoAtual.State.RemoveFromTree(idCálculo);
        }

        #endregion

        private void VerificaResultadoAtualização(UpdateResult result)
        {
            if ((result.IsAcknowledged && result.ModifiedCount == 1) == false)
            {
                throw new Exception("Erro ao atualizar os dados - operação cancelada");
            }
        }
    }
}
