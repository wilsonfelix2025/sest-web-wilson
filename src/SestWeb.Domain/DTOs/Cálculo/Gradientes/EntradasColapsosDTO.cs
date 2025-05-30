using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.DTOs.Cálculo.Gradientes
{
    public class EntradasColapsosDTO
    {
        public bool? FluidoPenetrante { get; set; }
        public double AreaPlastificada { get; set; }
        public CritérioRupturaGradientesEnum TipoCritérioRuptura { get; set; }
        public int? Tempo { get; set; }
        public bool? HabilitarFiltroAutomatico { get; set; }
        public bool? IncluirEfeitosFísicosQuímicos { get; set; }
        public bool? IncluirEfeitosTérmicos { get; set; }
        public bool? CalcularFraturasColapsosSeparadamente { get; set; }
        public PoroelasticoDTO PoroElastico {get;set;}

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(EntradasColapsosDTO)))
                return;

            BsonClassMap.RegisterClassMap<EntradasColapsosDTO>(calc =>
            {
                calc.AutoMap();
                calc.MapMember(p => p.FluidoPenetrante);
                calc.MapMember(p => p.AreaPlastificada);
                calc.MapMember(p => p.TipoCritérioRuptura).SetSerializer(new EnumSerializer<CritérioRupturaGradientesEnum>(BsonType.String));
                calc.MapMember(p => p.Tempo);
                calc.MapMember(p => p.HabilitarFiltroAutomatico);
                calc.MapMember(p => p.IncluirEfeitosFísicosQuímicos);
                calc.MapMember(p => p.IncluirEfeitosTérmicos);
                calc.MapMember(p => p.CalcularFraturasColapsosSeparadamente);
                calc.MapMember(p => p.PoroElastico);
                calc.SetDiscriminator("EntradaColapsos");
            });

        }
    }

    
}
