using MongoDB.Bson.Serialization;

namespace SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação
{
    public class ParâmetroCorrelação
    {
        public string NomeParâmetro { get; private set; }
        public double Valor { get; private set; }

        public ParâmetroCorrelação(string nomeParâmetro, double valor)
        {
            NomeParâmetro = nomeParâmetro;
            Valor = valor;
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ParâmetroCorrelação)))
                return;

            BsonClassMap.RegisterClassMap<ParâmetroCorrelação>(param =>
            {
                param.AutoMap();
                param.SetIgnoreExtraElements(true);
                param.SetDiscriminator(nameof(ParâmetroCorrelação));
            });
        }
    }
}
