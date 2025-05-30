using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.EmCriação
{
    public class CálculoGradienteSobrecargaEmCriação : CálculoEmCriação
    {
        public CálculoGradienteSobrecargaEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
        }
    }
}
