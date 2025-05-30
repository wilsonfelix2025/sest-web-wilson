using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção.Casos
{
    public class DepleçãoCaso05 : DepleçãoBase
    {
        public DepleçãoCaso05(DadosDepleção dados) : base(dados)
        {
        }
        public override Profundidade ObterD(Profundidade pvi)
        {
            return new Profundidade(pvi.Valor - Dados.PvCorrelação.Valor);
        }

        public override Profundidade ObterF(Profundidade pvi)
        {
            return new Profundidade(0);
        }

        public override Profundidade ObterH(Profundidade pvi)
        {
            return new Profundidade(0);
        }
    }
}
