using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public abstract class RelacionamentoUcsCoesaAngat 
    {
        public RelacionamentoUcsCoesaAngat(Correlação ucs, Correlação coesa, Correlação angat, Correlação restr)
        {
            Ucs = ucs;
            Coesa = coesa;
            Angat = angat;
            Restr = restr;
        }

        public Correlação Ucs { get; private set; }
       
        public Correlação Coesa { get; private set; }
        
        public Correlação Angat { get; private set; }
        
        public Correlação Restr { get; private set; }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(RelacionamentoUcsCoesaAngat)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Correlação)))
                Correlação.Map();

            BsonClassMap.RegisterClassMap<RelacionamentoUcsCoesaAngat>(rel =>
            {
                rel.AutoMap();
                rel.MapMember(r => r.Ucs);
                rel.MapMember(r => r.Coesa);
                rel.MapMember(r => r.Angat);
                rel.MapMember(r => r.Restr);
                rel.SetIsRootClass(true);
                rel.SetIgnoreExtraElements(true);
                rel.SetDiscriminator("RelacionamentoUcsCoesaAngat");
            });

            RelacionamentoUcsCoesaAngatPorGrupoLitológico.Map();
        }


    }
}
