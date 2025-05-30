using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção.Casos
{
    public abstract class DepleçãoBase
    {
        protected readonly DadosDepleção Dados;
        //(kgf/cm²) - psi
        private const double FatorConversão = 14.2233439119029;

        protected DepleçãoBase(DadosDepleção dados)
        {
            Dados = dados;
        }

        public abstract Profundidade ObterD(Profundidade pvi);
        public abstract Profundidade ObterF(Profundidade pvi);
        public abstract Profundidade ObterH(Profundidade pvi);

        public double ObterPressãoDepletada(Profundidade pvi)
        {
            var d = ObterD(pvi);
            var f = ObterF(pvi);
            var h = ObterH(pvi);

            double pressãoDepletada;

            if (Dados.PvCorrelação < pvi)
            {
                pressãoDepletada = Dados.PressãoCorrelação + d.Valor * Dados.Reservatório.ValorGásGás + f.Valor * Dados.Reservatório.ValorGásÓleo + h.Valor * Dados.Reservatório.ValorGásÁgua;
            }
            else
            {
                pressãoDepletada = Dados.PressãoCorrelação - (d.Valor * Dados.Reservatório.ValorGásÁgua) - (f.Valor * Dados.Reservatório.ValorGásÓleo) - (h.Valor * Dados.Reservatório.ValorGásGás);
            }

            return pressãoDepletada * FatorConversão;
        }
    }
}
