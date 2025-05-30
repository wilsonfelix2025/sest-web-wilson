namespace SestWeb.Domain.Entities.Cálculos.Base
{
    public static class OperaçõesDeConversão
    {
        private const double FatorConversão = 0.1704;

        public static double ObterGradiente(double profundidadeVertical, double valor)
        {
            return valor / (FatorConversão * profundidadeVertical);
        }

        public static double ObterPressão(double profundidadeVertical, double gradiente)
        {
            return gradiente * profundidadeVertical * FatorConversão;
        }
    }
}
