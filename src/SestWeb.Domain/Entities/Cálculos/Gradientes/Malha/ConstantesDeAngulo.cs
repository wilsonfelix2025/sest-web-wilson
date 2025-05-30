using System;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    public class ConstantesDeAngulo
    {
        /// <summary>
        /// Classe para fazer cache de seno e cosseno da malha, que será igual para todas as malhas do poço.
        /// Usado para melhorar o desempenho do cálculo de área plastificada.
        /// </summary>
        /// <param name="anguloEmRadiano">Ângulo em graus.</param>
        public ConstantesDeAngulo(double anguloEmRadiano)
        {
            Angulo = anguloEmRadiano;
            CossenoAnguloRadiano = Math.Cos(anguloEmRadiano);
            CossenoAnguloRadiano2 = Math.Cos(2 * anguloEmRadiano);
            SenoAnguloRadiano = Math.Sin(anguloEmRadiano);
            SenoAnguloRadiano2 = Math.Sin(2 * anguloEmRadiano);
        }

        public double Angulo { get; }
        public double CossenoAnguloRadiano { get; }
        public double CossenoAnguloRadiano2 { get; }
        public double SenoAnguloRadiano { get; }
        public double SenoAnguloRadiano2 { get; }
    }
}
