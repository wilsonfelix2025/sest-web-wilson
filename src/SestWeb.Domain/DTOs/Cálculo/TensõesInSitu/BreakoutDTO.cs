using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Domain.DTOs.Cálculo.TensõesInSitu
{
    public class BreakoutDTO
    {
        public PerfilBase PerfilUCS { get; set; }
        public string PerfilUCSId { get; set; }
        public PerfilBase PerfilANGAT { get; set; }
        public string PerfilANGATId { get; set; }
        public PerfilBase PerfilGPORO { get; set; }
        public string PerfilGPOROId { get; set; }
        public List<BreakoutValoresDTO> TrechosBreakout { get; set; }
        public double Azimuth { get; set; }


        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(BreakoutDTO)))
                return;

            BsonClassMap.RegisterClassMap<BreakoutDTO>(pe =>
            {
                pe.AutoMap();
                pe.UnmapMember(p => p.PerfilUCS);
                pe.UnmapMember(p => p.PerfilANGAT);
                pe.UnmapMember(p => p.PerfilGPORO);
                pe.MapMember(p => p.PerfilUCSId);
                pe.MapMember(p => p.PerfilANGATId);
                pe.MapMember(p => p.PerfilGPOROId);
                pe.MapMember(p => p.Azimuth);
                pe.MapMember(p => p.TrechosBreakout);
                pe.SetDiscriminator("Breakout");
            });
        }

    }

    public class BreakoutValoresDTO
    {
        public double PM { get; set; }
        public double Azimute { get; set; }
        public double Largura { get; set; }
        public double PesoFluido { get; set; }
    }
}
