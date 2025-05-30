using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel
{
    public class FiltroMédiaMóvel : Filtro, IFiltroMédiaMóvel
    {
        public double? LimiteInferior { get; }
        public double? LimiteSuperior { get; }
        public TipoCorteEnum? TipoCorte { get; }
        public int NúmeroPontos { get; }

        private FiltroMédiaMóvel(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, int númeroPontos) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            LimiteInferior = limiteInferior;
            LimiteSuperior = limiteSuperior;
            TipoCorte = tipoCorte;
            NúmeroPontos = númeroPontos;
        }

        public static void RegisterFiltroMédiaMóvelCtor()
        {
            FiltroMédiaMóvelFactory.RegisterFiltroMédiaMóvelCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, númeroPontos) => new FiltroMédiaMóvel(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, númeroPontos));
        }

        public sealed override void Execute(bool chamadaPelaPipeline)
        {
            if (TipoCorte.HasValue && LimiteSuperior.HasValue && LimiteInferior.HasValue)
                AplicarLimites(PerfisEntrada, PerfisSaída, TipoCorte.Value, LimiteInferior.Value, LimiteSuperior.Value);

            AplicarMédiaMóvel();
        }

        private void AplicarMédiaMóvel()
        {
            var perfilSaída = PerfisSaída.Perfis.First();
            var perfil = PerfisEntrada.Perfis.First();
            var pontos = perfilSaída.ContémPontos() ? perfilSaída.GetPontos() : perfil.GetPontos();

            var raio = (NúmeroPontos - 1) / 2;
            var indexDoPrimeiro = raio;
            var indexDoÚltimo = pontos.Count - raio;

            var pontosPerfilFiltradoPM = new List<Profundidade>();
            var pontosPerfilFiltradoValor = new List<double>();

            TratamentoDasExtremidadesMédiaMóvel(pontosPerfilFiltradoPM, pontosPerfilFiltradoValor, indexDoPrimeiro, indexDoÚltimo, raio, pontos);

            for (var index = indexDoPrimeiro; index < indexDoÚltimo; index++)
            {
                var soma = 0.0;
                for (var frameIndex = index - raio; frameIndex <= index + raio; frameIndex++)
                    soma += pontos[frameIndex].Valor;
                var média = Math.Round(soma / NúmeroPontos, 2);

                var pontoOriginal = pontos[index];

                pontosPerfilFiltradoPM.Add(pontoOriginal.Pm);
                pontosPerfilFiltradoValor.Add(média);
            }

            perfilSaída.Clear();
            perfilSaída.AddPontosEmPm(ConversorProfundidade, pontosPerfilFiltradoPM, pontosPerfilFiltradoValor, TipoProfundidade.PM, OrigemPonto.Filtrado, Litologia);
        }

        private void TratamentoDasExtremidadesMédiaMóvel(List<Profundidade> pontosPerfilFiltradosPM, List<double> pontosPerfilFiltradosValor,  int indexDoPrimeiro, int indexDoÚltimo, int raio, IReadOnlyList<Ponto> pontos)
        {
            for (var index = 0; index < indexDoPrimeiro; index++)
            {
                var soma = 0.0;
                var qtdItens = 0;
                for (var frameIndex = index - raio; frameIndex <= index + raio; frameIndex++)
                {
                    if (frameIndex >= 0)
                    {
                        soma += pontos[frameIndex].Valor;
                        qtdItens += 1;
                    }
                }

                var média = Math.Round(soma / qtdItens, 2);
                var pontoOriginal = pontos[index];

                pontosPerfilFiltradosPM.Add(pontoOriginal.Pm);
                pontosPerfilFiltradosValor.Add(média);
            }

            for (var index = indexDoÚltimo; index < indexDoÚltimo + raio; index++)
            {
                var soma = 0.0;
                var qtdItens = 0;
                for (var frameIndex = index - raio; frameIndex <= index + raio; frameIndex++)
                {
                    if (frameIndex < indexDoÚltimo + raio)
                    {
                        soma += pontos[frameIndex].Valor;
                        qtdItens += 1;
                    }
                }

                var média = Math.Round(soma / qtdItens, 2);
                var pontoOriginal = pontos[index];

                pontosPerfilFiltradosPM.Add(pontoOriginal.Pm);
                pontosPerfilFiltradosValor.Add(média);
            }
        }

        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<FiltroMédiaMóvel>(filtro =>
            {
                filtro.AutoMap();
                filtro.MapMember(f => f.NúmeroPontos);
                filtro.MapMember(f => f.LimiteInferior);
                filtro.MapMember(f => f.LimiteSuperior);
                filtro.MapMember(f => f.TipoCorte);
                filtro.MapCreator(f => new FiltroMédiaMóvel(f.Nome, f.GrupoCálculo, f.PerfisEntrada, f.PerfisSaída, null, null, f.LimiteInferior, f.LimiteSuperior, f.TipoCorte, f.NúmeroPontos));
            });
        }
    }
}
