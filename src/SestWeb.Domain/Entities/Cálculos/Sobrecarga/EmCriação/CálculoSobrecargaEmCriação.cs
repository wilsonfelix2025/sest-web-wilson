using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.EmCriação
{
    public class CálculoSobrecargaEmCriação : CálculoEmCriação
    {
        public CálculoSobrecargaEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
        }
    }
}
