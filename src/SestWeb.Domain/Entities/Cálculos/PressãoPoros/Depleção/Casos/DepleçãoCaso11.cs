using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção.Casos
{
    public class DepleçãoCaso11 : DepleçãoBase
    {
        public DepleçãoCaso11(DadosDepleção dados) : base(dados)
        {
        }
        public override Profundidade ObterD(Profundidade pvi)
        {
            return new Profundidade(Dados.PvCorrelação.Valor - pvi.Valor);
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
