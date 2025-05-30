using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Filtros.Corte.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Corte
{
    public class FiltroCorte : Filtro, IFiltroCorte
    {
        public double? LimiteInferior { get; }
        public double? LimiteSuperior { get; }
        public TipoCorteEnum? TipoCorte { get; }
        
        private FiltroCorte(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
        }

        public static void RegisterFiltroCorteCtor()
        {
            FiltroCorteFactory.RegisterFiltroCorteCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte) => new FiltroCorte(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte));
        }

        public sealed override void Execute(bool chamadaPelaPipeline)
        {
            if (TipoCorte.HasValue && LimiteSuperior.HasValue && LimiteInferior.HasValue)
                AplicarLimites(PerfisEntrada,PerfisSaída,TipoCorte.Value, LimiteInferior.Value, LimiteSuperior.Value);

        }

        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<FiltroCorte>(filtro =>
            {
                filtro.AutoMap();
                filtro.MapMember(f => f.LimiteInferior);
                filtro.MapMember(f => f.LimiteSuperior);
                filtro.MapMember(f => f.TipoCorte);
                filtro.MapCreator(f => new FiltroCorte(f.Nome, f.GrupoCálculo, f.PerfisEntrada, f.PerfisSaída, null, null, f.LimiteInferior, f.LimiteSuperior, f.TipoCorte));
            });

        }
    }
}
