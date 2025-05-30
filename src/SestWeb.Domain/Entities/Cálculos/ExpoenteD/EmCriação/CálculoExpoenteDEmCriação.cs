using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.EmCriação
{

    public class CálculoExpoenteDEmCriação : CálculoEmCriação
    {
        public string Correlação { get; }

        public CálculoExpoenteDEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada
            , IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, string correlação) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            Correlação = correlação;
        }
    }
}
