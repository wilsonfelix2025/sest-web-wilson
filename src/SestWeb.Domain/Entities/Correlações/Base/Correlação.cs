using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;

namespace SestWeb.Domain.Entities.Correlações.Base
{
    public class Correlação : ICorrelação, ISupportInitialize
    {
        private Correlação(string nome, IAutor autor, string descrição, Origem origem, IExpressão expressão)
        {
            Nome = nome;
            Autor = autor;
            Descrição = descrição;
            Origem = origem;
            Expressão = expressão;
        }

        #region Properties

        public string Nome { get; }

        public string Descrição { get; }

        public IAutor Autor { get; }

        public Origem Origem { get; }

        public IExpressão Expressão { get; }

        public IPerfisEntrada PerfisEntrada => Expressão.PerfisEntrada;

        public IPerfisSaída PerfisSaída => Expressão.PerfisSaída;

        #endregion

        #region Methods

        

        public static void RegisterCorrelaçãoCtor()
        {
            CorrelaçãoFactory.RegisterCorrelaçãoCtor((nome, autor, descrição, origem, expressão) => new Correlação(nome, autor, descrição, origem, expressão));
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Correlação)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Autor)))
                AutorCorrelação.Autor.Map();

            if (!BsonClassMap.IsClassMapRegistered(typeof(Expressão)))
                ExpressãoCorrelação.Expressão.Map();

            if (!BsonClassMap.IsClassMapRegistered(typeof(PerfisEntrada)))
                PerfisEntradaCorrelação.PerfisEntrada.Map();

            if (!BsonClassMap.IsClassMapRegistered(typeof(PerfisSaída)))
                PerfisSaídaCorrelação.PerfisSaída.Map();

            BsonClassMap.RegisterClassMap<Correlação>(corr =>
            {
                corr.AutoMap();
                corr.MapCreator(c =>
                    new Correlação(c.Nome, c.Autor, c.Descrição, c.Origem, c.Expressão));
                corr.MapMember(c => c.Nome);
                corr.SetIdMember(corr.GetMemberMap(c => c.Nome));
                corr.MapMember(c => c.Autor).SetSerializer(new ImpliedImplementationInterfaceSerializer<IAutor, Autor>());
                corr.MapMember(c => c.Descrição);
                corr.MapMember(c => c.Origem).SetSerializer(new EnumSerializer<Origem>(BsonType.String));
                corr.MapMember(c => c.Expressão);
                corr.UnmapMember(c => c.PerfisEntrada);
                corr.UnmapMember(c => c.PerfisSaída);
                corr.SetIgnoreExtraElements(true);
                corr.SetDiscriminator("Correlação");
            });
        }


        #region ISupportInitialize members

        public void BeginInit() { }

        public void EndInit() { }

        #endregion

        #endregion
    }
}
