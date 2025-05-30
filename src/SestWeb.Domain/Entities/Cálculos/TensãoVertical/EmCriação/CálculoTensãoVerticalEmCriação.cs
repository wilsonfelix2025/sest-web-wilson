using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.TensãoVertical.EmCriação
{
    public class CálculoTensãoVerticalEmCriação: CálculoEmCriação
    {
        public CálculoTensãoVerticalEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, Geometria geometria, DadosGerais dadosGerais) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {

        }
    }
}
