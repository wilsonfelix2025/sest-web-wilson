using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using System.Collections.Generic;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Simples.EmCriação
{
    public class FiltroSimplesEmCriação : CálculoEmCriação
    {

        public double? LimiteInferior { get; }
        public double? LimiteSuperior { get; }
        public TipoCorteEnum? TipoCorte { get; }
        public double? DesvioMáximo { get; }

        public FiltroSimplesEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, double? desvioMáximo) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
            DesvioMáximo = desvioMáximo;
        }
    }
}
