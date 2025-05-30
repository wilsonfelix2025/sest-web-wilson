
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.DTOs.Cálculo.TensõesInSitu
{
    public class DepleçãoDTO
    {
        public PerfilBase GPOROOriginal { get; set; }
        public string GPOROOriginalId { get; set; }
        public PerfilBase GPORODepletada { get; set; }
        public string GPORODepletadaId { get; set; }
        public PerfilBase Poisson { get; set; }
        public string PoissonId { get; set; }
        public PerfilBase Biot { get; set; }
        public string BiotId { get; set; }

        public bool ContémPerfil(string perfilId)
        {
            if (Biot != null && Biot.Id.ToString() == perfilId)
                return true;
            if (Poisson != null && Poisson.Id.ToString() == perfilId)
                return true;
            if (GPOROOriginal != null && GPOROOriginal.Id.ToString() == perfilId)
                return true;
            if (GPORODepletada != null && GPORODepletada.Id.ToString() == perfilId)
                return true;
            return false;
        }

        public void Invalidate(ObjectId perfilId)
        {
            if (Biot.Id == perfilId)
                Biot = null;
            else if (Poisson.Id == perfilId)
                Poisson = null;
            else if (GPOROOriginal.Id == perfilId)
                GPOROOriginal = null;
            else if (GPORODepletada.Id == perfilId)
                GPORODepletada = null;
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(DepleçãoDTO)))
                return;

            BsonClassMap.RegisterClassMap<DepleçãoDTO>(pe =>
            {
                pe.AutoMap();
                pe.UnmapMember(p => p.GPOROOriginal);
                pe.UnmapMember(p => p.GPORODepletada);
                pe.UnmapMember(p => p.Poisson);
                pe.UnmapMember(p => p.Biot);
                pe.MapMember(p => p.GPOROOriginalId);
                pe.MapMember(p => p.GPORODepletadaId);
                pe.MapMember(p => p.PoissonId);
                pe.MapMember(p => p.BiotId);
                pe.SetDiscriminator("Depleção");
            });
        }

    }
}
