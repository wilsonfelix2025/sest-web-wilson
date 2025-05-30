using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Simples
{
    public class FiltroSimples : Filtro, IFiltroSimples
    {
        public double? LimiteInferior { get; }
        public double? LimiteSuperior { get; }
        public TipoCorteEnum? TipoCorte { get; }
        public double DesvioMáximo { get; }

        private FiltroSimples(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, double desvioMáximo) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
            DesvioMáximo = desvioMáximo;
        }

        public static void RegisterFiltroSimplesCtor()
        {
            FiltroSimplesFactory.RegisterFiltroSimplesCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, desvioMáximo) => new FiltroSimples(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, desvioMáximo));
        }

        public sealed override void Execute(bool chamadaPelaPipeline)
        {
            if (TipoCorte.HasValue && LimiteSuperior.HasValue && LimiteInferior.HasValue)
                AplicarLimites(PerfisEntrada,PerfisSaída,TipoCorte.Value, LimiteInferior.Value, LimiteSuperior.Value);

            AplicarDesvioMáximo();
        }

        private void AplicarDesvioMáximo()
        {
            var perfil = PerfisEntrada.Perfis.First();
            var perfilSaída = PerfisSaída.Perfis.First();
            var valorAnterior = 0.0;
            var pontos = perfilSaída.ContémPontos() ? perfilSaída.GetPontos() : perfil.GetPontos();
            var novosPontosPM = new List<Profundidade>();
            var novosPontosValor = new List<double>();

            if (pontos.Count <= 1) return;

            for (var index = 0; index < pontos.Count; index++)
            {
                if (index == 0)
                {
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(pontos[index].Valor);
                    valorAnterior = pontos[index].Valor;
                    continue;
                }

                var diferença = pontos[index].Valor - valorAnterior;
                var diferençaAbsoluta = Math.Abs(diferença);

                if (!(diferençaAbsoluta > DesvioMáximo))
                {
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(pontos[index].Valor);
                    valorAnterior = pontos[index].Valor;
                    continue;
                }

                if (diferença > 0)
                {
                    var valor = Math.Round(valorAnterior + DesvioMáximo, 2);
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(valor);
                    valorAnterior = valor;
                }
                else if (diferença < 0)
                {
                    var valor = Math.Round(valorAnterior - DesvioMáximo, 2);
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(valor);
                    valorAnterior = valor;
                }
            }

            perfilSaída.Clear();
            perfilSaída.AddPontosEmPm(ConversorProfundidade, novosPontosPM,novosPontosValor,TipoProfundidade.PM,OrigemPonto.Filtrado, Litologia);
        }

        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<FiltroSimples>(filtro =>
            {
                filtro.AutoMap();
                filtro.MapMember(f => f.DesvioMáximo);
                filtro.MapMember(f => f.LimiteInferior);
                filtro.MapMember(f => f.LimiteSuperior);
                filtro.MapMember(f => f.TipoCorte);
                filtro.MapCreator(f => new FiltroSimples(f.Nome, f.GrupoCálculo, f.PerfisEntrada, f.PerfisSaída, null, null, f.LimiteInferior, f.LimiteSuperior, f.TipoCorte, f.DesvioMáximo));
            });

        }
    }
}
