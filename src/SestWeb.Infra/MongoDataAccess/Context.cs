using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MongoDB.Driver;
using SestWeb.Domain.DTOs.Cálculo.Gradientes;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;
using SestWeb.Domain.Entities.Correlações.VariablesCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.SistemasUnidades.Base;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Infra.Exceptions;
using SestWeb.Infra.MongoDataAccess.Entities;
using SestWeb.Infra.MongoDataAccess.Seed;
using SestWeb.Infra.Services;
using PerfisEntrada = SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação.PerfisEntrada;
using PerfisSaída = SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação.PerfisSaída;

namespace SestWeb.Infra.MongoDataAccess
{
    public class Context
    {
        private readonly IMongoDatabase _database;
        private readonly string _databaseName;
        internal readonly MongoClient _mongoClient;

        public Context(string connectionString, string databaseName)
        {
            _databaseName = databaseName;
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(databaseName);

            RegistrarClassesAbstratas();
            //CreateCorrelaçõesIndex();
            //LoadSystemCorrs();

            //#if DEBUG
            //            Seed();
            //#endif
        }

        internal IMongoCollection<Poço> Poços => _database.GetCollection<Poço>("poços");
        internal IMongoCollection<PerfilBase> Perfis => _database.GetCollection<PerfilBase>("perfis");
        internal IMongoCollection<RegistroEvento> RegistrosEventos => _database.GetCollection<RegistroEvento>("registrosEventos");
        internal IMongoCollection<Correlação> Correlações => _database.GetCollection<Correlação>("correlações");
        internal IMongoCollection<CorrelaçãoPoço> CorrelaçõesPoço => _database.GetCollection<CorrelaçãoPoço>("correlaçõesPoço");
        internal IMongoCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico> RelacionamentosCorrsPropMec =>
            _database.GetCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>("relacionamentosCorrsPropMec");
        internal IMongoCollection<RelacionamentoPropMecPoço> RelacionamentosCorrsPropMecPoço =>
            _database.GetCollection<RelacionamentoPropMecPoço>("relacionamentosCorrsPropMecPoço");
        internal IMongoCollection<OpUnit> OpUnits => _database.GetCollection<OpUnit>("opunits");
        internal IMongoCollection<OilField> OilFields => _database.GetCollection<OilField>("oilfields");
        internal IMongoCollection<Well> Wells => _database.GetCollection<Well>("wells");
        internal IMongoCollection<File> Files => _database.GetCollection<File>("files");

        public IMongoCollection<ApplicationUser> Usuarios => _database.GetCollection<ApplicationUser>("users");

        private static void RegistrarClassesAbstratas()
        {
            RuntimeHelpers.RunClassConstructor(typeof(TiposPerfil).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(GrupoUnidades).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(UnidadeMedida).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(GerenciadorUcsCoesaAngat).TypeHandle);

            MapTraJetória();
            MapExtratigrafia();
            MapPerfis();
            MapRegistrosEventos();
            MapLitologia();
            MapCorrelações();
            MapCálculos();
            MaprelacionamentoPropMec();
        }

        private static void MaprelacionamentoPropMec()
        {
            RelacionamentoUcsCoesaAngat.Map();
            RelacionamentoPropMecPoço.Map();
        }

        private static void MapTraJetória()
        {
            Trajetória.Map();
        }

        private static void MapExtratigrafia()
        {
            Estratigrafia.Map();
        }

        private static void MapLitologia()
        {
            Litologia.Map();
        }

        private static void MapPerfis()
        {
            PerfilBase.Map();
        }

        private static void MapRegistrosEventos()
        {
            RegistroEvento.Map();
        }

        private static void MapCorrelações()
        {
            Correlação.Map();
            CorrelaçãoPoço.Map();
            Autor.Map();
            Expressão.Map();
            Variables.Map();
            PerfisSaída.Map();
            PerfisEntrada.Map();
        }

        private static void MapCálculos()
        {
            Cálculo.Map();
            Domain.Entities.Cálculos.Base.PerfisDeEntrada.PerfisEntrada.Map();
            Domain.Entities.Cálculos.Base.PerfisDeSaída.PerfisSaída.Map();
            RelaçãoTensãoDTO.Map();
            DepleçãoDTO.Map();
            BreakoutDTO.Map();
            FraturasTrechosVerticaisDTO.Map();
            EntradasColapsosDTO.Map();
            PoroelasticoDTO.Map();
            DadosMalha.Map();
        }

        public Task<string> CreateCorrelaçõesIndex()
        {
            var correlaçãoBuilder = Builders<Correlação>.IndexKeys;
            var indexModel = new CreateIndexModel<Correlação>(correlaçãoBuilder.Ascending(corr => corr.Nome));
            return _database.GetCollection<Correlação>("correlações").Indexes.CreateOneAsync(indexModel);
        }

        public Task<string> CreateRelacionamentosIndex()
        {
            var relacionamentoBuilder = Builders<RelacionamentoUcsCoesaAngatPorGrupoLitológico>.IndexKeys;
            var indexModel = new CreateIndexModel<RelacionamentoUcsCoesaAngatPorGrupoLitológico>(relacionamentoBuilder.Ascending(rel => rel.Nome));
            return _database.GetCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico>("relacionamentosCorrsPropMec").Indexes.CreateOneAsync(indexModel);
        }

        //private void Seed()
        //{
        //    if (Poços.Find(_ => true).CountDocuments() != 0) return;

        //    Task.Run(async () => { await AlimentarBancoPoçoInicial(); }).Wait();
        //}

        internal async Task DeletarBancoDadosAsync()
        {
            try
            {
                await _mongoClient.DropDatabaseAsync(_databaseName);
            }
            catch (Exception e)
            {
                throw new DatabaseOffLineException(e.Message);
            }
        }

        private async Task<string> AlimentarBancoPoçoInicial()
        {
            try
            {
                var seeder = new Seeder();
                return await seeder.SeedAsync(this);
            }
            catch (Exception e)
            {
                throw new DatabaseOffLineException(e.Message);
            }
        }

        public async Task<string> AlimentarBancoPoçoWeb(List<OpUnit> opUnits, List<OilField> oilFields, List<Well> wells)
        {
            try
            {
                await CargaInicial(opUnits, oilFields, wells);
                return await AlimentarBancoPoçoInicial();
            }
            catch (Exception e)
            {
                throw new DatabaseOffLineException(e.Message);
            }
        }

        public async Task CargaInicial(List<OpUnit> opUnits, List<OilField> oilFields, List<Well> wells)
        {
            try
            {
                if (opUnits.Count > 0)
                    await OpUnits.InsertManyAsync(opUnits);
                if (oilFields.Count > 0)
                    await OilFields.InsertManyAsync(oilFields);
                if (wells.Count > 0)
                    await Wells.InsertManyAsync(wells);
            }
            catch (Exception e)
            {
                throw new DatabaseOffLineException(e.Message);
            }
        }

        public async Task<int> findIdNoUsed<T>(IMongoCollection<T> col) where T : IPoçoWebResponseItem
        {
            int id = 0;
            while (await col.Find(el => el.Id == id.ToString()).AnyAsync())
            {
                id++;
            };
            return id;
        }
    }
}