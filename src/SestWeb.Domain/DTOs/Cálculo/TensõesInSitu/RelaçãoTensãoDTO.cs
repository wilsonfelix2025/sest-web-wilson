using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.DTOs.Cálculo.TensõesInSitu
{
    public class RelaçãoTensãoDTO
    {
        public PerfilBase PerfilRelaçãoTensão { get; set; }
        public string PerfilRelaçãoTensãoId { get; set; }
        public TipoRelaçãoEntreTensãoEnum TipoRelação { get; set; }
        public double? AZTHMenor { get; set; }
        public PerfilBase PerfilTVERT { get; set; }
        public string PerfilTVERTId { get; set; }
        public PerfilBase PerfilGPORO { get; set; }
        public string PerfilGPOROId { get; set; }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(RelaçãoTensãoDTO)))
                return;
            
            BsonClassMap.RegisterClassMap<RelaçãoTensãoDTO>(pe =>
            {
                pe.AutoMap();
                pe.UnmapMember(p => p.PerfilRelaçãoTensão);
                pe.UnmapMember(p => p.PerfilGPORO);
                pe.UnmapMember(p => p.PerfilTVERT);
                pe.MapMember(p => p.PerfilTVERTId);
                pe.MapMember(p => p.PerfilRelaçãoTensãoId);
                pe.MapMember(p => p.PerfilGPOROId);
                pe.MapMember(p => p.AZTHMenor);
                pe.MapMember(p => p.TipoRelação).SetSerializer(new EnumSerializer<TipoRelaçãoEntreTensãoEnum>(BsonType.String));
                pe.SetDiscriminator("RelaçãoTensão");
            });
        }

    }
}
