using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação.Factory;

namespace SestWeb.Domain.Entities.Correlações.PoçoCorrelação
{
    /// <summary>
    /// Correlação associada a um caso de poço específico.
    /// </summary>
    public class CorrelaçãoPoço : ICorrelaçãoPoço
    {
        private CorrelaçãoPoço(string idPoço, ICorrelação correlação)
        {
            IdPoço = idPoço;
            Correlação = correlação;
            Nome = correlação.Nome;
        }

        public string IdPoço { get; }

        public string Nome { get; }

        public ICorrelação Correlação { get; }

        public static void RegisterCorrelaçãoPoçoCtor()
        {
            CorrelaçãoPoçoFactory.RegisterCorrelaçãoPoçoCtor((idPoço, correlação) => new CorrelaçãoPoço(idPoço, correlação));
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(CorrelaçãoPoço)))
                return;

            BsonClassMap.RegisterClassMap<CorrelaçãoPoço>(corr =>
            {
                corr.AutoMap();
                corr.MapCreator(c =>
                    new CorrelaçãoPoço(c.IdPoço, c.Correlação));
                corr.MapMember(c => c.Nome);
                corr.SetIdMember(corr.GetMemberMap(c => c.Nome));
                corr.MapMember(c => c.IdPoço);
                corr.MapMember(c => c.Correlação).SetSerializer(new ImpliedImplementationInterfaceSerializer<ICorrelação, Correlação>());
                corr.SetIgnoreExtraElements(true);
                corr.SetDiscriminator("CorrelaçãoPoço");
            });
        }
    }
}
