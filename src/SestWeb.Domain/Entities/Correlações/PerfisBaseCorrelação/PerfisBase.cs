using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;

namespace SestWeb.Domain.Entities.Correlações.PerfisBaseCorrelação
{
    public abstract class PerfisBase : IPerfisBase
    {
        public PerfisBase(List<string> tipos)
        {
            Tipos = tipos;
        }

        #region Properties

        public List<string> Tipos { get; }

        #endregion

        #region Methods

        private void LoadPerfis(string expressão)
        {
            Tipos.Clear();

            foreach (var perfil in GetPerfis(expressão))
            {
                if (Tipos.Find(p => p.Equals(perfil)) == null)
                    Tipos.Add(perfil);
            }
        }

        protected internal abstract IEnumerable<string> GetPerfis(string expression);

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PerfisBase)))
                return;

            BsonClassMap.RegisterClassMap<PerfisBase>(pb =>
            {
                pb.AddKnownType(typeof(PerfisSaída));
                pb.AddKnownType(typeof(PerfisEntrada));
                //pb.AutoMap();
                pb.MapMember(p => p.Tipos);
                pb.SetIgnoreExtraElements(true);
                pb.SetDiscriminator("PerfisBase");
            });
            BsonClassMap.RegisterClassMap<PerfisSaída>(ps =>
            {
                ps.AutoMap();
                //ps.MapCreator(p => new PerfisSaída(p.Tipos));
                ps.SetDiscriminator("PerfisSaída");
            });
            BsonClassMap.RegisterClassMap<PerfisEntrada>(pe =>
            {
                pe.AutoMap();
                //pe.MapCreator(p => new PerfisEntrada(p.Tipos));
                pe.SetDiscriminator("PerfisEntrada");
            });
        }

        #endregion
    }
}
