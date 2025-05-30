using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec.Factory;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec
{
    /// <summary>
    /// Relacionamento de correlações de prop mec associado a um caso de poço específico.
    /// </summary>
    public class RelacionamentoPropMecPoço : IRelacionamentoPropMecPoço
    {
        private RelacionamentoPropMecPoço(string idPoço, RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoPropMec)
        {
            IdPoço = idPoço;
            RelacionamentoPropMec = relacionamentoPropMec;
            Nome = relacionamentoPropMec.Nome;
        }

        public string IdPoço { get; }

        public string Nome { get; }

        public RelacionamentoUcsCoesaAngatPorGrupoLitológico RelacionamentoPropMec { get; }

        public static void RegisterRelacionamentoPropMecPoçoCtor()
        {
            RelacionamentoPropMecPoçoFactory.RegisterRelacionamentoPropMecPoçoCtor((idPoço, relacionamentoPropMec) => new RelacionamentoPropMecPoço(idPoço, relacionamentoPropMec));
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(RelacionamentoPropMecPoço)))
                return;

            BsonClassMap.RegisterClassMap<RelacionamentoPropMecPoço>(rel =>
            {
                rel.AutoMap();
                rel.MapCreator(r => new RelacionamentoPropMecPoço(r.IdPoço, r.RelacionamentoPropMec));
                rel.MapMember(r => r.Nome);
                rel.SetIdMember(rel.GetMemberMap(r => r.Nome));
                rel.MapMember(r => r.IdPoço);
                rel.MapMember(r => r.RelacionamentoPropMec);
                rel.SetIgnoreExtraElements(true);
                rel.SetDiscriminator("RelacionamentoPropMecPoço");
            });
        }
    }
}
