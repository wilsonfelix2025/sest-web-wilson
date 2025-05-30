using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção.Casos
{
    public class DepleçãoCaso04 : DepleçãoBase
    {
        public DepleçãoCaso04(DadosDepleção dados) : base(dados)
        {
        }
        public override Profundidade ObterD(Profundidade pvi)
        {
            return new Profundidade(Dados.Reservatório.ContatoGásÓleo.Valor - Dados.PvCorrelação.Valor);
        }

        public override Profundidade ObterF(Profundidade pvi)
        {
            return new Profundidade(pvi.Valor - Dados.Reservatório.ContatoGásÓleo.Valor);
        }

        public override Profundidade ObterH(Profundidade pvi)
        {
            return new Profundidade(0);
        }
    }
}
