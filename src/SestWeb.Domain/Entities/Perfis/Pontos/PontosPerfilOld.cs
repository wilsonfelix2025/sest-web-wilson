using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Perfis.Pontos
{
    [Obsolete]
    public class PontosPerfilOld
    {
        [Obsolete]
        [BsonElement]
        private List<PontoOld> _pontos;

        public PontosPerfilOld()
        {
            _pontos = new List<PontoOld>();
        }

        public IReadOnlyList<PontoOld> Pontos => _pontos.AsReadOnly();
        public PontoOld PrimeiroPonto => _pontos.Any() ? _pontos[0] : null;
        public int QuantidadeElementos => _pontos.Count;
        public PontoOld UltimoPonto => _pontos.Any() ? _pontos[QuantidadeElementos - 1] : null;

        [BsonIgnore]
        public IConversorProfundidade ConversorProfundidade { get; set; }

        [BsonIgnore]
        public ILitologia Litologia { get; set; }

        public void Clear()
        {
            _pontos.Clear();
        }

        public PontoOld GetMaximoPontoComPV()
        {
            if (!_pontos.Any())
            {
                throw new InvalidOperationException("Perfil não contém pontos.");
            }

            if (PrimeiroPonto.Pv == 0) return null;

            return _pontos.OrderBy(x => x.Pv).LastOrDefault(x => x.Pv != 0);
        }

        public PontoOld GetMaximoPontoComLitologia()
        {
            if (!_pontos.Any())
            {
                throw new InvalidOperationException("Perfil não contém pontos.");
            }

            if (PrimeiroPonto.TipoRocha == null) return null;

            return _pontos.LastOrDefault(x => x.TipoRocha != null);
        }

        public void Reset(IEnumerable<PontoOld> pontos)
        {
            _pontos = pontos.Distinct().OrderBy(x => x.Pm).ToList();

            CompletarPontos();
        }

        public bool TryGetPonto(double pm, out PontoOld pontoOld)
        {
            if (!_pontos.BuscaBinariaPorPmMenorIgual(pm, out var i, out var equal))
            {
                pontoOld = null;
                return false;
            }

            if (equal)
            {
                pontoOld = _pontos[i];
                return true;
            }

            var j = i + 1;
            var pontoAnterior = _pontos[i];
            var pontoProximo = _pontos[j];

            var x = ((pontoProximo.Valor - pontoAnterior.Valor) / (pontoProximo.Pm - pontoAnterior.Pm)) * pm +
                            pontoAnterior.Valor - (pontoProximo.Valor * pontoAnterior.Pm - pontoAnterior.Pm * pontoAnterior.Valor) / (pontoProximo.Pm - pontoAnterior.Pm);

            pontoOld = new PontoOld(pm, x, OrigemPonto.Interpolado);

            ComplementarPonto(pontoOld);

            return true;
        }

        private void ComplementarPonto(PontoOld pontoOld)
        {
            if (ConversorProfundidade != null && ConversorProfundidade.TryGetTVDFromMD(pontoOld.Pm, out var pv))
            {
                pontoOld.Pv = pv;
            }

            //if (Litologia != null && Litologia.TryGetPontoLitologia(pontoOld.Pm, out var pontoLito))
            //{
            //    pontoOld.TipoRocha = pontoLito.TipoRocha;
            //}
        }

        public void CompletarPontosLitologia()
        {
            Parallel.ForEach(_pontos, ComplementarPontoLitologia);
        }

        private void ComplementarPontoLitologia(PontoOld pontoOld)
        {
            //if (Litologia != null && Litologia.TryGetPontoLitologia(pontoOld.Pm, out var pontoLito))
            //{
            //    pontoOld.TipoRocha = pontoLito.TipoRocha;
            //}
        }

        public void CompletarPontos()
        {
            Parallel.ForEach(_pontos, ComplementarPonto);
        }

        public void SobrescreverPontos(double pmTopo, double pmBase, List<PontoOld> pontosRecebidos)
        {
            List<PontoOld> novosPontos = new List<PontoOld>();

            var qtdAtéTopo = _pontos.Count(c => c.Pm < pmTopo);
            var qtdAtéBase = _pontos.Count(c => c.Pm <= pmBase);
            
            pontosRecebidos.BuscaBinariaPorPmMenorIgual(pmTopo, out var índiceTopoPontosRecebidos, out var _2);
            pontosRecebidos.BuscaBinariaPorPmMenorIgual(pmBase, out var índiceBasePontosRecebidos, out var _3);

            if (índiceTopoPontosRecebidos < 0)
            {
                // Pontos já começam em profundidade inferior ao PM do topo
                índiceTopoPontosRecebidos = 0;
            }
            else if (índiceBasePontosRecebidos < 0)
            {
                // Pontos terminam em profundidade inferior ao PM da base
                índiceBasePontosRecebidos = pontosRecebidos.Count - 1;
            }

            // Pega os k primeiros elementos já existentes no poço, até o topo do trecho a ser sobrescrito
            novosPontos.AddRange(_pontos.Take(qtdAtéTopo));
            // Extrai o intervalo de pontos a ser inserido no perfil entre os PMs topo e base
            novosPontos.AddRange(pontosRecebidos.Skip(índiceTopoPontosRecebidos).Take(índiceBasePontosRecebidos - índiceTopoPontosRecebidos + 1));
            // Adiciona os pontos restantes já existentes no perfil
            novosPontos.AddRange(_pontos.Skip(qtdAtéBase));

            Reset(novosPontos);

        }

    }
}
