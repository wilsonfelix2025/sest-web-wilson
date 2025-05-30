using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.EmCriação
{
    public class FiltroLitologiaEmCriação : CálculoEmCriação
    {
        public double? LimiteInferior { get; }
        public double? LimiteSuperior { get; }
        public TipoCorteEnum? TipoCorte { get; }
        public List<string> Litologias { get; }

        public FiltroLitologiaEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, List<string> litologias) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
            Litologias = litologias;
        }
    }
}
