using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção.Casos
{
    public class DepleçãoCaso02 : DepleçãoBase
    {
        public DepleçãoCaso02(DadosDepleção dados) : base(dados)
        {
        }

        public override Profundidade ObterD(Profundidade pvi)
        {
            return new Profundidade(0);
        }

        public override Profundidade ObterF(Profundidade pvi)
        {
            return new Profundidade(Dados.Reservatório.ContatoÓleoÁgua.Valor - Dados.PvCorrelação.Valor);
        }

        public override Profundidade ObterH(Profundidade pvi)
        {
            return new Profundidade(pvi.Valor - Dados.Reservatório.ContatoÓleoÁgua.Valor);
        }


    }
}
