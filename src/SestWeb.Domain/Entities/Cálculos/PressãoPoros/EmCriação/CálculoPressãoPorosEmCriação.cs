using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.EmCriação
{
    public class CálculoPressãoPorosEmCriação : CálculoEmCriação
    {
        public CálculoPressãoPorosEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, CorrelaçãoPressãoPoros métodoCálculo, IList<ParâmetroCorrelação> parâmetrosCorrelação,
            Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
        }
    }
}
