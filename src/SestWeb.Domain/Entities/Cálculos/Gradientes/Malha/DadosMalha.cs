using MongoDB.Bson.Serialization;
using System;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    public class DadosMalha
    {
        public double AnguloTotal { get; } = 180;
        public int AnguloDivisao { get; } = 20;
        public int RaioDivisao { get; } = 20;
        public double AnguloInternoPorExterno { get; } = 2.0;
        public double AnguloMaxMinIncremento { get; } = 100.0;
        public double PorcentagemMalha { get; }

        public DadosMalha()
        {
            this.AnguloDivisao = this.AnguloDivisao * 2;
        }

        public DadosMalha(int anguloDivisao, int raioDivisao, double anguloInternoPorExterno, double anguloMaxMinIncremento)
        {
            // anguloDivisao * 2 para malha de 180
            this.AnguloDivisao = anguloDivisao * 2;
            this.RaioDivisao = raioDivisao;
            this.AnguloInternoPorExterno = anguloInternoPorExterno;
            this.AnguloMaxMinIncremento = anguloMaxMinIncremento;
            this.PorcentagemMalha = (Math.Pow(anguloInternoPorExterno, 2) - 1) * 100;
            Validar();
        }

        private void Validar()
        {
            ValidaLimites(AnguloInternoPorExterno, nameof(AnguloInternoPorExterno));
            ValidaLimites(AnguloDivisao, nameof(AnguloDivisao));
            ValidaLimites(RaioDivisao, nameof(RaioDivisao));
            ValidaLimites(AnguloMaxMinIncremento, nameof(AnguloMaxMinIncremento));
        }

        private void ValidaLimites(double value, string nome)
        {
            const double limiteInferior = 1;
            const double limiteSuperior = 999;

            if (!(value >= limiteInferior && value <= limiteSuperior))
                throw new ArgumentException($"{nome} está fora dos limites aceitáveis.");
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(DadosMalha)))
                return;
            
            BsonClassMap.RegisterClassMap<DadosMalha>(calc =>
            {
                calc.AutoMap();
                calc.MapMember(p => p.AnguloInternoPorExterno);
                calc.MapMember(p => p.RaioDivisao);
                calc.MapMember(p => p.AnguloDivisao);
                calc.MapMember(p => p.AnguloMaxMinIncremento);
                calc.SetDiscriminator("Dadosmalha");
                calc.MapCreator(p => new DadosMalha(p.AnguloDivisao/2, p.RaioDivisao, p.AnguloInternoPorExterno, p.AnguloMaxMinIncremento));

            });

        }
    }
}
