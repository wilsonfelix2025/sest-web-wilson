
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Litologia
{
    public class FiltroLitologia : Filtro, IFiltroLitologia
    {
        public double? LimiteInferior { get; }
        public double? LimiteSuperior { get; }
        public TipoCorteEnum? TipoCorte { get; }
        public List<string> Litologias { get; }

        private FiltroLitologia(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, List<string> litologias) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
            Litologias = litologias;
        }

        public static void RegisterFiltroLitologiaCtor()
        {
            FiltroLitologiaFactory.RegisterFiltroLitologiaCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, litologias) => new FiltroLitologia(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, litologias));
        }

        public sealed override void Execute(bool chamadaPelaPipeline)
        {
            if (chamadaPelaPipeline)
                PerfisSaída.Perfis.First().Clear();

            if (TipoCorte.HasValue && LimiteSuperior.HasValue && LimiteInferior.HasValue)
                AplicarLimites(PerfisEntrada, PerfisSaída, TipoCorte.Value, LimiteInferior.Value, LimiteSuperior.Value);

            AplicarLitologia();
        }

        private void AplicarLitologia()
        {
            var perfilSaída = PerfisSaída.Perfis.First();
            var perfil = PerfisEntrada.Perfis.First();
            var pontosDePerfilFiltrados = perfilSaída.ContémPontos() ? perfilSaída.GetPontos().Where(c => c.TipoRocha != null && Litologias.Contains(c.TipoRocha.Mnemonico)) : perfil.GetPontos().Where(c => c.TipoRocha != null && Litologias.Contains(c.TipoRocha.Mnemonico));

            var pms = pontosDePerfilFiltrados.Select(s => s.Pm).ToList();
            var valores = pontosDePerfilFiltrados.Select(s => s.Valor).ToList();
            perfilSaída.Clear();
            perfilSaída.AddPontosEmPm(ConversorProfundidade, pms, valores, TipoProfundidade.PM, OrigemPonto.Filtrado, Litologia);
        }

        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<FiltroLitologia>(filtro =>
            {
                filtro.AutoMap();
                filtro.MapMember(f => f.Litologias);
                filtro.MapMember(f => f.LimiteInferior);
                filtro.MapMember(f => f.LimiteSuperior);
                filtro.MapMember(f => f.TipoCorte);
                filtro.MapCreator(f => new FiltroLitologia(f.Nome, f.GrupoCálculo, f.PerfisEntrada, f.PerfisSaída, null, null, f.LimiteInferior, f.LimiteSuperior, f.TipoCorte, f.Litologias));

            });
        }
    }
}
