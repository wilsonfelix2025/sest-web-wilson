using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho
{
    public class FiltroLinhaBaseFolhelho : Filtro, IFiltroLinhaBaseFolhelho
    {
        public double? LimiteInferior { get; }
        public double? LimiteSuperior { get; }
        public TipoCorteEnum? TipoCorte { get; }
        public PerfilBase PerfilLBF { get; }
        public string IdPerfilLBF { get; }

        private FiltroLinhaBaseFolhelho(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, PerfilBase perfilLBF) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
            PerfilLBF = perfilLBF;
            IdPerfilLBF = PerfilLBF.Id.ToString();
        }

        private FiltroLinhaBaseFolhelho(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, string idPerfilLBF) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
            IdPerfilLBF = idPerfilLBF;
        }

        public static void RegisterFiltroLinhaBaseFolhelhoCtor()
        {
            FiltroLinhaBaseFolhelhoFactory.RegisterFiltroLinhaBaseFolhelhoCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, perfilLBF) => new FiltroLinhaBaseFolhelho(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, perfilLBF));
        }

        public sealed override void Execute(bool chamadaPelaPipeline)
        {
            if (TipoCorte.HasValue && LimiteSuperior.HasValue && LimiteInferior.HasValue)
                AplicarLimites(PerfisEntrada, PerfisSaída, TipoCorte.Value, LimiteInferior.Value, LimiteSuperior.Value);

            AplicarLinhaBaseFolhelho();
        }

        private void AplicarLinhaBaseFolhelho()
        {
            var pontosFiltradosPM = new List<Profundidade>();
            var pontosFiltradosValor = new List<double>();
            var perfilSaída = PerfisSaída.Perfis.First();
            var perfil = PerfisEntrada.Perfis.First();
            var trend = PerfilLBF.Trend;
            var pontos = perfilSaída.ContémPontos() ? perfilSaída.GetPontos() : perfil.GetPontos();

            foreach (var ponto in pontos)
            {
                var profPm = ponto.Pm;
                var valor = ponto.Valor;

                double valorTrend = 0.0;

                PerfilLBF.TryGetPontoEmPm(ConversorProfundidade, profPm, out Ponto pontoGray, GrupoCálculo.FiltroLinhaBaseFolhelho);

                var trecho = trend.Trechos.Find(t => pontoGray.Pm.Valor >= t.PvTopo && pontoGray.Pm.Valor <= t.PvBase);

                if (trecho == null) continue;

                valorTrend = trecho.CalcularValor(trecho.PvBase, pontoGray.Pm.Valor, trecho.Inclinação,
                    trecho.ValorBase, trecho.ValorTopo);

                if (Math.Abs(pontoGray.Valor - valorTrend) < 0.01)
                {
                    pontosFiltradosPM.Add(profPm);
                    pontosFiltradosValor.Add(valor);
                    continue;
                }

                if (pontoGray.Valor > valorTrend)
                {
                    pontosFiltradosPM.Add(profPm);
                    pontosFiltradosValor.Add(valor);
                }

            }

            perfilSaída.Clear();
            perfilSaída.AddPontosEmPm(ConversorProfundidade, pontosFiltradosPM, pontosFiltradosValor, TipoProfundidade.PM,
                OrigemPonto.Filtrado, Litologia);
        }

        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<FiltroLinhaBaseFolhelho>(filtro =>
            {
                filtro.AutoMap();
                filtro.UnmapMember(f => f.PerfilLBF);
                filtro.MapMember(f => f.IdPerfilLBF);
                filtro.MapMember(f => f.LimiteInferior);
                filtro.MapMember(f => f.LimiteSuperior);
                filtro.MapMember(f => f.TipoCorte);
                filtro.MapCreator(f => new FiltroLinhaBaseFolhelho(f.Nome, f.GrupoCálculo, f.PerfisEntrada, f.PerfisSaída, null, null, f.LimiteInferior, f.LimiteSuperior, f.TipoCorte, f.IdPerfilLBF));
            });
        }
    }
}
