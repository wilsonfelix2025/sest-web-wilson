using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação.Factory;

namespace SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação
{
    public class PerfisSaída : IPerfisSaída//: PerfisBase, IPerfisSaída
    {
        private PerfisSaída(List<string> tipos)
        {
            Tipos = tipos.Distinct().ToList();
        }// : base(tipos) { }

        public List<string> Tipos { get; }

        //protected internal override IEnumerable<string> GetPerfis(string expression)
        //{
        //    return Identify(expression);
        //}

        public static IEnumerable<string> Identify(string expression)
        {
            return TiposVálidos.Perfis.Where(tipo =>
            {
                string pattern = $@"\b{tipo}\b\s*=\s*";
                var regex = new Regex(pattern);
                return regex.IsMatch(expression);
            }).Distinct();
        }

        public static IPerfisSaídaFactory GetFactory()
        {
            return new PerfisSaídaFactory(tipos => new PerfisSaída(tipos));
        }

        public new static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PerfisSaída)))
                return;

            BsonClassMap.RegisterClassMap<PerfisSaída>(ps =>
            {
                ps.AutoMap();
                ps.MapCreator(p => new PerfisSaída(p.Tipos));
                ps.MapMember(p => p.Tipos);
                ps.SetDiscriminator("PerfisSaída");
            });
        }
    }
}
