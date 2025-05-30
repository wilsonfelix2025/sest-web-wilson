using System;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class PropriedadesPetrofisicas
    {
        public PropriedadesPetrofisicas(double porosidade, double permeabilidade, double viscosidade, PropriedadesElasticas propElasticas)
        {
            Porosidade = porosidade;
            Permeabilidade = permeabilidade;
            Viscosidade = viscosidade;

            this.Mobilidade = this.Permeabilidade / this.Viscosidade;

            var G = propElasticas.G;
            var B = propElasticas.B;
            var Vu = propElasticas.Vu;
            var poisson = propElasticas.Poisson;

            this.Cf = (2 * Mobilidade * G * Math.Pow(B, 2) * (1 - poisson) * (Math.Pow(1 + Vu, 2))) / (9 * (1 - Vu) * (Vu - poisson));
        }

        public double Porosidade { get; }
        public double Permeabilidade { get; }
        public double Viscosidade { get; }
        public double Mobilidade { get; }
        public double Cf { get; }
    }
}
