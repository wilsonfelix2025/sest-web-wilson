using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção.Casos
{
    public class DepleçãoCaso10 : DepleçãoBase
    {
        public DepleçãoCaso10(DadosDepleção dados) : base(dados)
        {
        }
        public override Profundidade ObterD(Profundidade pvi)
        {
            return new Profundidade(Dados.PvCorrelação.Valor - Dados.Reservatório.ContatoÓleoÁgua.Valor);
        }

        public override Profundidade ObterF(Profundidade pvi)
        {
            return new Profundidade(Dados.Reservatório.ContatoÓleoÁgua.Valor - pvi.Valor);
        }

        public override Profundidade ObterH(Profundidade pvi)
        {
            return new Profundidade(0);
        }
    }
}
