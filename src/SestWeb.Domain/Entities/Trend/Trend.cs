
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Trend
{
    public class Trend
    {

        private Trend(TipoTrendEnum tipoTrend, PerfilBase Perfil)
        {
            TipoTrend = tipoTrend;
            Nome = CriarNomeTrend(tipoTrend, Perfil);
            PodeSerUsadoEmCálculo = false; //TODO: quando implementar perfil filtrado precisa trocar aqui
            CriarTrechoInicial(Perfil, tipoTrend);

            if (Id == ObjectId.Empty)
            {
                Id = ObjectId.GenerateNewId();
            }
        }

        [BsonConstructor]
        private Trend(TipoTrendEnum tipoTrend, string nome, ObjectId id, bool podeSerUsadoEmCálculo, List<TrechoTrend> trechos)
        {
            TipoTrend = tipoTrend;
            Nome = nome;
            Id = id;
            PodeSerUsadoEmCálculo = podeSerUsadoEmCálculo;
            Trechos = trechos;
        }

        [BsonId]
        public ObjectId Id { get; }
        [BsonElement]
        public TipoTrendEnum TipoTrend { get; }
        [BsonElement]
        public List<TrechoTrend> Trechos { get; private set; } = new List<TrechoTrend>();
        [BsonElement]
        public string Nome { get; private set; }
        //setado na criação do trend, só vai ser true quando o Perfil do Trend for perfil filtrado
        [BsonElement]
        public bool PodeSerUsadoEmCálculo { get; }
        public int QuantidadeElementos => Trechos.Count;


        #region Methods

        //cria os dois pontos iniciais do trend, no inicio do perfil e no final do perfil
        public void CriarTrechoInicial(PerfilBase PerfilBase, TipoTrendEnum tipoTrend)
        {
            if (tipoTrend == TipoTrendEnum.Compactação)
            {
                var trecho = new TrechoTrend(PerfilBase.PrimeiroPonto.Pv.Valor, PerfilBase.PrimeiroPonto.Valor,
                    PerfilBase.UltimoPonto.Pv.Valor, PerfilBase.UltimoPonto.Valor);

                Trechos.Add(trecho);
            }
            else if (tipoTrend == TipoTrendEnum.LBF)
            {
                var trecho = new TrechoTrend(PerfilBase.PrimeiroPonto.Pm.Valor, PerfilBase.PrimeiroPonto.Valor
                    , PerfilBase.UltimoPonto.Pm.Valor, PerfilBase.PrimeiroPonto.Valor, 0);

                Trechos.Add(trecho);
            }

        }

        public void ResetTrechos(List<TrechoTrend> trechos)
        {
            Trechos = trechos.OrderBy(t => t.PvTopo).ToList();
        }

        public void SetNomeTrend(string nomeTrend)
        {
            Nome = nomeTrend;
        }

        public TrechoTrend ÚltimoTrecho()
        {
            return Trechos.Any() ? Trechos[QuantidadeElementos - 1] : null;
        }

        public TrechoTrend PrimeiroTrecho()
        {
            return Trechos.Any() ? Trechos[0] : null;
        }

        private string CriarNomeTrend(TipoTrendEnum tipoTrend, PerfilBase perfil)
        {
            var nomeTrend = string.Empty;

            if (tipoTrend == TipoTrendEnum.Compactação)
                nomeTrend = "Trend_" + perfil.Nome;
            else if (tipoTrend == TipoTrendEnum.LBF)
                nomeTrend = "LBF_" + perfil.Nome;


            return nomeTrend;
        }

        public List<Profundidade> ObterProfundidadesPm(IConversorProfundidade conversorProfundidade)
        {
            var pvs = Trechos.Select(t => t.PvTopo).ToList();
            var pms = new List<Profundidade>();

            for (var index = 0; index < pvs.Count; index++)
            {
                var pv = pvs[index];

                if (conversorProfundidade.TryGetMDFromTVD(pv, out double pm))
                {
                    pms.Add(new Profundidade(pm));
                }
            }

            return pms;
        }

        public bool TryGetValor(Profundidade pv, out double valor)
        {
            var erroAceitavel = 0.001;

            if (FastMath.SafeEquals(PrimeiroTrecho().PvTopo, pv.Valor, erroAceitavel))
            {
                valor = PrimeiroTrecho().ValorTopo;
                return true;
            }

            foreach (var trecho in Trechos)
            {
                if (FastMath.SafeLessThan(trecho.PvTopo, pv.Valor, erroAceitavel) &&
                    FastMath.SafeGreaterThan(trecho.PvBase, pv.Valor, erroAceitavel))
                {
                    valor = trecho.CalcularValor(trecho.PvBase, pv.Valor, trecho.Inclinação,
                    trecho.ValorBase, trecho.ValorTopo);
                    return true;
                }
            }

            if (FastMath.SafeEquals(ÚltimoTrecho().PvBase, pv.Valor, erroAceitavel))
            {
                valor = ÚltimoTrecho().ValorBase;
                return true;
            }

            valor = -1;
            return false;
        }
        #endregion
    }
}
