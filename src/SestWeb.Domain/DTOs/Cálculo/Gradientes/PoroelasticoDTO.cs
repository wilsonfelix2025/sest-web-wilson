using MongoDB.Bson.Serialization;
using System.Collections.Generic;

namespace SestWeb.Domain.DTOs.Cálculo.Gradientes
{
    public class PoroelasticoDTO
    {
        public double Kf { get; set; }
        public double Viscosidade { get; set; }
        public double CoeficienteReflexão { get; set; }
        public double CoeficienteInchamento { get; set; }
        public double CoeficienteDifusãoSoluto { get; set; }
        public double DensidadeFluidoFormação { get; set; }
        public double TemperaturaFormação { get; set; }
        public double TemperaturaFormaçãoFisicoQuimica { get; set; }

        public double ConcentraçãoSolFluidoPerfuração { get; set; }
        public double ConcentraçãoSolutoRocha { get; set; }
        public string TipoSal { get; set; }
        public double CoeficienteDissociaçãoSoluto { get; set; }
        public double MassaMolarSoluto { get; set; }
        public double ExpansãoTérmicaVolumeFluidoPoros { get; set; }
        public double TemperaturaPoço { get; set; }
        public double PropriedadeTérmicaTemperaturaFormação { get; set; }
        public double DifusidadeTérmica { get; set; }
        public double ExpansãoTérmicaRocha { get; set; }
        public List<string> Litologias { get; set; } = new List<string>();

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PoroelasticoDTO)))
                return;

            BsonClassMap.RegisterClassMap<PoroelasticoDTO>(calc =>
            {
                calc.AutoMap();
                calc.MapMember(p => p.CoeficienteDifusãoSoluto);
                calc.MapMember(p => p.CoeficienteDissociaçãoSoluto);
                calc.MapMember(p => p.CoeficienteInchamento);
                calc.MapMember(p => p.CoeficienteReflexão);
                calc.MapMember(p => p.ConcentraçãoSolFluidoPerfuração);
                calc.MapMember(p => p.ConcentraçãoSolutoRocha);
                calc.MapMember(p => p.DensidadeFluidoFormação);
                calc.MapMember(p => p.DifusidadeTérmica);
                calc.MapMember(p => p.ExpansãoTérmicaRocha);
                calc.MapMember(p => p.ExpansãoTérmicaVolumeFluidoPoros);
                calc.MapMember(p => p.Kf);
                calc.MapMember(p => p.Litologias);
                calc.MapMember(p => p.MassaMolarSoluto);
                calc.MapMember(p => p.PropriedadeTérmicaTemperaturaFormação);
                calc.MapMember(p => p.TemperaturaFormação);
                calc.MapMember(p => p.TemperaturaFormaçãoFisicoQuimica);
                calc.MapMember(p => p.TemperaturaPoço);
                calc.MapMember(p => p.Viscosidade);
                calc.MapMember(p => p.TipoSal);
                calc.SetDiscriminator("PoroElastico");
            });

        }
    }
}
