using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Domain.DTOs.Cálculo.TensõesInSitu
{
    public class FraturasTrechosVerticaisDTO
    {
        public PerfilBase PerfilRESTR { get; set; }
        public string PerfilRESTRId { get; set; }
        public PerfilBase PerfilGPORO { get; set; }
        public string PerfilGPOROId { get; set; }
        public List<FraturaTrechosVerticaisValoresDTO> TrechosFratura { get; set; }
        public double Azimuth { get; set; }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(FraturasTrechosVerticaisDTO)))
                return;

            BsonClassMap.RegisterClassMap<FraturasTrechosVerticaisDTO>(pe =>
            {
                pe.AutoMap();
                pe.UnmapMember(p => p.PerfilRESTR);
                pe.UnmapMember(p => p.PerfilGPORO);
                pe.MapMember(p => p.PerfilRESTRId);
                pe.MapMember(p => p.PerfilGPOROId);
                pe.MapMember(p => p.TrechosFratura);
                pe.MapMember(p => p.Azimuth);
                pe.SetDiscriminator("FraturasTrechosVerticais");
            });
        }
    }

    public class FraturaTrechosVerticaisValoresDTO
    {
        public double PM { get; set; }
        public double PesoFluido { get; set; }
    }
}
