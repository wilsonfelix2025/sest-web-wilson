using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação.Factory;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;

namespace SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação
{
    public class PerfisEntrada : IPerfisEntrada //: PerfisBase, IPerfisEntrada
    {
        private PerfisEntrada(List<string> tipos) //: base(tipos)
        {
            Tipos = tipos.Distinct().ToList();
        }

        public List<string> Tipos { get; }

        //protected internal override IEnumerable<string> GetPerfis(string expression)
        //{
        //    return Identify(expression);
        //}

        public static IEnumerable<string> Identify(string expression)
        {
            return Enumerable.Where(TiposVálidos.Perfis, tipo =>
            {
                string pattern = $@"\b{tipo}\b";
                var regex = new Regex(pattern);
                var perfisDeSaída = PerfisSaída.Identify(expression);
                return regex.IsMatch(expression) && !perfisDeSaída.Contains(tipo);
            }).Distinct();
        }

        public static IPerfisEntradaFactory GetFactory()
        {
            return new PerfisEntradaFactory(tipos => new PerfisEntrada(tipos));
        }

        public new static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PerfisEntrada)))
                return;

            BsonClassMap.RegisterClassMap<PerfisEntrada>(pe =>
            {
                pe.AutoMap();
                pe.MapCreator(p => new PerfisEntrada(p.Tipos));
                pe.MapMember(p => p.Tipos);
                pe.SetDiscriminator("PerfisEntrada");
            });
        }
    }
}
