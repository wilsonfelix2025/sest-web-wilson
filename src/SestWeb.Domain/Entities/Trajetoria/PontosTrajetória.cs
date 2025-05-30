using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.PontosEntity.InternalCollections;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Trajetoria
{

    internal class PontosTrajetória : PontoDeTrajetóriaFactory, ISupportInitialize
    {
        private const double DistanciaEntrePontosParaTrajetóriaVertical = 1;

        private ConcurrentSortedListWrapper<Profundidade, PontoDeTrajetória> _pontos;

        private ConcurrentSortedListWrapper<Profundidade, ConcurrentSortedListWrapper<Profundidade, PontoDeTrajetória>> _pvPoints;

        public PontosTrajetória(MétodoDeCálculoDaTrajetória métodoDeCálculoDaTrajetória = MétodoDeCálculoDaTrajetória.RaioCurvatura)
        {
            _pontos = new ConcurrentSortedListWrapper<Profundidade, PontoDeTrajetória>();
            _pvPoints = new ConcurrentSortedListWrapper<Profundidade, ConcurrentSortedListWrapper<Profundidade, PontoDeTrajetória>>();
            MétodoDeCálculoDaTrajetória = métodoDeCálculoDaTrajetória;
        }

        #region Propriedades

        public bool ÉVertical => TrajetóriaVertical();

        public bool ContémPontos => _pontos.Values.Count > 0;

        public int Count => _pontos.Values.Count;

        public PontoDeTrajetória PrimeiroPonto => (PontoDeTrajetória)(ContémPontos ? _pontos.GetValueList()[0] : CriarPontoInicial());

        public PontoDeTrajetória ÚltimoPonto => (PontoDeTrajetória)(ContémPontos ? _pontos.GetValueList()[_pontos.Values.Count - 1] : CriarPontoInicial());

        public Profundidade PvMáximo => ContémPontos ? (Profundidade)_pvPoints.GetKeyList()[_pvPoints.Keys.Count - 1] : new Profundidade(0d);

        public Profundidade PmMáximo => ContémPontos ? (Profundidade)_pontos.GetKeyList()[_pontos.Keys.Count - 1] : new Profundidade(0d);

        public Profundidade PmMínimo => ContémPontos ? (Profundidade)_pontos.GetKeyList()[0] : new Profundidade(0d);

        public Profundidade PvMínimo => ContémPontos ? (Profundidade)_pvPoints.GetKeyList()[0] : new Profundidade(0d);

        public IList<PontoDeTrajetória> Pontos
        {
            get => GetPontos();
            set => Reset(value);
        }

        public MétodoDeCálculoDaTrajetória MétodoDeCálculoDaTrajetória { get; private set; }

        #endregion

        #region Add

        public void AdicionarPontoTrajetória(double pm, double inclinação, double azimute)
        {
            AdicionaPrimeiroPontoCasoNecessário();

            if (CriarPontoPorPm(pm, inclinação, azimute, MétodoDeCálculoDaTrajetória, out PontoDeTrajetória novoPonto))
            {
                AdicionarPontoTrajetóriaInternal(novoPonto);
            }
            AddPv(novoPonto);
        }

        private void AdicionaPrimeiroPontoCasoNecessário()
        {
            if (_pontos.ContainsKey(new Profundidade(0d)))
                return;

            var primeiroPonto = CriarPontoInicial();
            AdicionarPontoTrajetóriaInternal(primeiroPonto);
            AddPv(primeiroPonto);
        }

        public void Reset(IList<PontoDeTrajetória> pontos)
        {
            pontos.Remove(CriarPontoInicial());

            Clear();

            if (!pontos.Any())
            {
                return;
            }

            AdicionaPrimeiroPontoCasoNecessário();
            AdicionarPontosInternal(pontos);
            AtualizarDadosDosPontosDeTrajetória();
        }

        public void AdicionarPontosTrajetória(IList<PontoDeTrajetória> pontos)
        {
            if (!pontos.Any()) return;

            AdicionaPrimeiroPontoCasoNecessário();
            AdicionarPontosInternal(pontos);
            AtualizarDadosDosPontosDeTrajetória();
        }

        private void AdicionarPontoTrajetóriaInternal(PontoDeTrajetória pontoDeTrajetória)
        {
            var profPm = pontoDeTrajetória.Pm;

            if (_pontos.ContainsKey(profPm)) return;

            _pontos.Add(profPm, pontoDeTrajetória);
        }

        private void GerarDadosPontoTrajetória(PontoDeTrajetória pontoAnterior, PontoDeTrajetória pontoAtual)
        {
            GeradorPontosTrajetória.GerarDadosPontoTrajetória(pontoAnterior, pontoAtual, MétodoDeCálculoDaTrajetória);
            AddPv(pontoAtual);
        }

        private void AdicionarPontosInternal(IEnumerable<PontoDeTrajetória> pontos)
        {
            foreach (var ponto in pontos)
            {
                AdicionarPontoTrajetóriaInternal(ponto);
            }
        }

        public void CriarTrajetóriaVertical(double pmFinal)
        {
            if (ÉVertical && Equals(pmFinal, PmMáximo))
                return;

            var pm = 0.0;

            while (pm <= pmFinal)
            {
                AdicionarPontoTrajetória(pm, 0, 0);
                pm += DistanciaEntrePontosParaTrajetóriaVertical;
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Obtêm todos os pontos.
        /// </summary>
        /// <returns></returns>
        public IList<PontoDeTrajetória> GetPontos()
        {
            PontoDeTrajetória[] pontos = new PontoDeTrajetória[_pontos.Count];

            var sortedPoints = _pontos.GetValueList();
            for (int index = 0; index < _pontos.Count; index++)
            {
                pontos[index] = (PontoDeTrajetória)sortedPoints[index];
            }

            return pontos;
        }

        private bool TrajetóriaVertical()
        {
            if (_pontos.Count == 0)
                return false;

            var pontos = _pontos.GetValueList();
            for (int index = 0; index < pontos.Count; index++)
            {
                if (((PontoDeTrajetória)pontos[index]).Inclinação > 3.0)
                    return false;
            }

            return true;
        }

        public bool TryGetTVDFromMD(double md, out double tvd)
        {
            // temporariamente desativado
            const bool salvarTabelaDinÂmica = false;

            if (md > ((Profundidade)_pontos.GetKeyList()[_pontos.Keys.Count - 1]).Valor)
            {
                tvd = default;
                return false;
            }

            var pmProf = new Profundidade(md);
            if (ContainsProfundidade(pmProf))
            {
                tvd = _pontos[pmProf].Pv.Valor;
                return true;
            }
            else if (ÉVertical)
            {
                tvd = md;
                return true;
            }
            else if (CriarPontoPorPm(pmProf.Valor, MétodoDeCálculoDaTrajetória, out PontoDeTrajetória ponto))
            {
                if (salvarTabelaDinÂmica)
                    AddToDinamicTable(ponto);

                tvd = (double)Math.Truncate((decimal)ponto.Pv.Valor * 10000000) / 10000000;
                //tvd = ponto.Pv.Valor;
                return true;

            }

            tvd = default;
            return false;
        }

        public bool TryGetPonto(Profundidade profPm, out PontoDeTrajetória ponto)
        {
            ponto = default;

            if (_pontos.Count < 2)
            {
                return false;
            }

            if (ContainsProfundidade(profPm))
            {
                ponto = _pontos[profPm];
                return true;
            }
            else if (CriarPontoPorPm(profPm.Valor, MétodoDeCálculoDaTrajetória, out ponto))
            {
                return true;
            }

            return false;
        }

        public bool ContainsProfundidade(Profundidade profPm)
        {
            return _pontos.ContainsKey(profPm);
        }

        public bool GetPreviousAndNextPmBuscaBinária(Profundidade profundidade, out Profundidade previousPm, out Profundidade nextPm)
        {
            if (_pontos == null)
                throw new InvalidOperationException("_pontos Null");

            var profundidades = _pontos.GetKeyList();

            if (profundidades.Count < 2 || profundidade.Valor <= ((Profundidade)profundidades[0]).Valor || profundidade.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                previousPm = null;
                nextPm = null;
                return false;
            }

            if (profundidade.Valor > ((Profundidade)profundidades[profundidades.Count - 2]).Valor &&
                profundidade.Valor < ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                previousPm = (Profundidade)profundidades[profundidades.Count - 2];
                nextPm = (Profundidade)profundidades[profundidades.Count - 1];
                return true;
            }

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > profundidade.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < profundidade.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int indexAnterior = 0;
            int indexPosterior = 0;
            if (Math.Abs(profundidade.Valor - ((Profundidade)profundidades[meio]).Valor) < 0.09)
            {
                indexAnterior = meio - 1;
                indexPosterior = meio + 1;
            }
            else if (profundidade.Valor > ((Profundidade)profundidades[meio]).Valor)
            {
                indexAnterior = meio;
                indexPosterior = meio + 1;
            }
            else if (profundidade.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                indexPosterior = meio;
                indexAnterior = meio - 1;

            }
            if (indexAnterior < 0 || indexPosterior > profundidades.Count - 1)
            {
                previousPm = null;
                nextPm = null;
                return false;
            }

            previousPm = (Profundidade)profundidades[indexAnterior];
            nextPm = (Profundidade)profundidades[indexPosterior];
            return true;
        }

        public override double GetDefaultAngle()
        {
            double angle = 0;

            if (_pontos.Count > 0)
            {
                var maxPm = (PontoDeTrajetória)_pontos.GetValueList()[_pontos.Count - 1];

                angle = maxPm.PolCoordDirec;

                if (angle < 0)
                {
                    angle += 360;
                }
            }

            return angle;
        }

        public override PontoDeTrajetória GetÚltimoPonto()
        {
            return ÚltimoPonto;
        }

        public override PontoDeTrajetória GetPrimeiroPonto()
        {
            return PrimeiroPonto;
        }

        public bool ContémTrechoSenoidal()
        {
            for (int index = 0; index < _pontos.Count; index++)
            {
                if (((PontoDeTrajetória)_pontos.Values[index]).Inclinação > 90)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Remove

        public void ApagarPontos()
        {
            Clear();
        }

        private void Clear()
        {
            _pontos.Clear();
            _pvPoints.Clear();
        }

        public bool RemoverPontoTrajetória(Profundidade pm)
        {
            if (!_pontos.ContainsKey(pm))
                return false;

            RemoverPontoTrajetóriaInternal(_pontos[pm]);

            return true;
        }

        private void RemoverPontoTrajetóriaInternal(PontoDeTrajetória ponto)
        {
            _pontos.Remove(ponto.Pm);
            RemovePvPoint(ponto);
            AtualizarDadosDosPontosDeTrajetória();
        }

        #endregion

        #region Edit

        public bool EditarPontoTrajetória(Profundidade pm, double inclinação, double azimute)
        {
            PontoDeTrajetória ponto;

            if (!_pontos.ContainsKey(pm))
                return false;

            EditarPontoTrajetóriaInternal(_pontos[pm], inclinação, azimute);

            return true;
        }

        private void AtualizarDadosDosPontosDeTrajetória()
        {
            var pontos = _pontos.GetValueList();

            for (var i = 1; i < pontos.Count; i++)
            {
                var pontoAnterior = (PontoDeTrajetória)pontos[i - 1];
                var pontoAtual = (PontoDeTrajetória)pontos[i];

                GerarDadosPontoTrajetória(pontoAnterior, pontoAtual);
            }
        }

        private void EditarPontoTrajetóriaInternal(PontoDeTrajetória ponto, double inclinação, double azimute)
        {
            ponto.AlterarAzimute(azimute);
            ponto.AlterarInclinação(inclinação);

            AtualizarDadosDosPontosDeTrajetória();
        }

        #endregion

        #region Pv

        private void AddPv(PontoDeTrajetória ponto)
        {
            var pvProf = new Profundidade(ponto.Pv.Valor);
            if (!_pvPoints.ContainsKey(pvProf))
            {
                _pvPoints.Add(pvProf, new ConcurrentSortedListWrapper<Profundidade, PontoDeTrajetória>());
            }

            var pmProf = new Profundidade(ponto.Pm.Valor);
            if (!_pvPoints[pvProf].ContainsKey(pmProf))
                _pvPoints[pvProf].Add(pmProf, ponto);
        }

        private void RemovePvPoint(PontoDeTrajetória ponto)
        {
            var pvProf = new Profundidade(ponto.Pv.Valor);
            if (!_pvPoints.ContainsKey(pvProf))
                return;

            var pmProf = new Profundidade(ponto.Pm.Valor);
            if (!_pvPoints[pvProf].ContainsKey(pmProf))
                return;

            _pvPoints[pvProf].Remove(pmProf);
        }

        public IList<PontoDeTrajetória> GetPointsByPv(Profundidade pv)
        {
            if (!_pvPoints.ContainsKey(pv))
                return new List<PontoDeTrajetória>();

            var pvPoints = _pvPoints[pv].GetValueList();
            PontoDeTrajetória[] pontos = new PontoDeTrajetória[pvPoints.Count];

            for (int index = 0; index < pvPoints.Count; index++)
            {
                pontos[index] = (PontoDeTrajetória)pvPoints[index];
            }

            return pontos;
        }

        public bool ContainsProfundidadePv(Profundidade pv)
        {
            return _pvPoints.ContainsKey(pv);
        }

        public IList<Profundidade> GetAllProfundidadesPv()
        {
            return _pvPoints.GetKeyList() as IList<Profundidade>;
        }

        public IList<Profundidade> GetAllProfundidadesPm()
        {
            return _pontos.GetKeyList() as IList<Profundidade>;
        }

        public bool EstáEmTrechoHorizontal(PontoDeTrajetória ponto)
        {
            var pvProf = new Profundidade(ponto.Pv.Valor);
            return EstáEmTrechoHorizontal(pvProf);
        }

        public bool EstáEmTrechoHorizontal(Profundidade pvProf)
        {
            if (_pvPoints.ContainsKey(pvProf))
            {
                if (_pvPoints[pvProf].Count > 1)
                {
                    return true;
                }

                if (_pvPoints[pvProf].Count == 1 && ((PontoDeTrajetória)_pvPoints[pvProf].GetValueList()[0]).Inclinação.Equals(90))
                    return true;
            }
            return false;
        }

        public bool TryGetUniquePms(Profundidade pv, out IList<Profundidade> pms)
        {
            pms = new List<Profundidade>();

            if (!_pvPoints.ContainsKey(pv))
                return false;

            if (_pvPoints[pv].Count == 1)
            {
                pms.Add((Profundidade)_pvPoints[pv].GetKeyList()[0]);
                return true;
            }

            var pontos = GetPointsByPv(pv);
            if (!pontos[0].Inclinação.Equals(90))
                pms.Add(pontos[0].Pm);

            for (var index = 1; index < pontos.Count; index++)
            {
                var ponto = pontos[index];
                var pontoAnterior = pontos[index - 1];

                // pegar os pms dos pontos fora de trechos horizontais, i.e. dos pontos senoidais
                if (!ponto.Inclinação.Equals(90))
                {
                    pms.Add(ponto.Pm);

                    // acrescentar os pms finais dos trechos horizontais
                    if (pontoAnterior.Inclinação.Equals(90))
                        pms.Add(pontoAnterior.Pm);
                }

                // pegar o último ponto caso ele estela no trecho horizontal
                if (index == pontos.Count - 1 && ponto.Inclinação.Equals(90))
                    pms.Add(ponto.Pm);
            }

            return true;
        }

        public bool GetPreviousAndNextPvBuscaBinária(Profundidade profundidade, ref Profundidade previousPv, ref Profundidade nextPv)
        {
            if (_pvPoints == null)
                throw new InvalidOperationException("_pvPoints Null");

            var profundidades = _pvPoints.GetKeyList();

            if (profundidades.Count < 2 || profundidade.Valor <= ((Profundidade)profundidades[0]).Valor || profundidade.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
                return false;

            if (profundidade.Valor > ((Profundidade)profundidades[profundidades.Count - 2]).Valor &&
                profundidade.Valor < ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                previousPv = (Profundidade)profundidades[profundidades.Count - 2];
                nextPv = (Profundidade)profundidades[profundidades.Count - 1];
                return true;
            }

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > profundidade.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < profundidade.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int indexAnterior = 0;
            int indexPosterior = 0;
            if (Math.Abs(profundidade.Valor - ((Profundidade)profundidades[meio]).Valor) < 0.09)
            {
                indexAnterior = meio - 1;
                indexPosterior = meio + 1;
            }
            else if (profundidade.Valor > ((Profundidade)profundidades[meio]).Valor)
            {
                indexAnterior = meio;
                indexPosterior = meio + 1;
            }
            else if (profundidade.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                indexPosterior = meio;
                indexAnterior = meio - 1;
            }

            if (indexAnterior < 0 || indexPosterior > profundidades.Count - 1)
                return false;

            previousPv = (Profundidade)profundidades[indexAnterior];
            nextPv = (Profundidade)profundidades[indexPosterior];
            return true;
        }

        public bool GetNextPvBuscaBinária(Profundidade profundidade, out Profundidade profundidadePosterior)
        {
            if (_pvPoints == null)
                throw new InvalidOperationException("_pvPoints Null");

            var profundidades = _pvPoints.GetKeyList();

            if (profundidades.Count < 2 || profundidade.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                profundidadePosterior = null;
                return false;
            }

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > profundidade.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < profundidade.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int index;
            if (profundidade.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                index = meio;
            }
            else
            {
                index = meio + 1;
            }

            profundidadePosterior = (Profundidade)profundidades[index];
            return true;
        }

        public bool GetNextPmBuscaBinária(Profundidade profundidade, out Profundidade profundidadePosterior)
        {
            if (_pontos == null)
                throw new InvalidOperationException("_pontos Null");

            var profundidades = _pontos.GetKeyList();

            if (profundidades.Count < 2 || profundidade.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                profundidadePosterior = null;
                return false;
            }

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > profundidade.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < profundidade.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int index;
            if (profundidade.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                index = meio;
            }
            else
            {
                index = meio + 1;
            }

            profundidadePosterior = (Profundidade)profundidades[index];
            return true;
        }

        private SortedList<Profundidade, IList<PontoDeTrajetória>> GetTrechosHorizontais()
        {
            SortedList<Profundidade, IList<PontoDeTrajetória>> pontosNoTrecho = new SortedList<Profundidade, IList<PontoDeTrajetória>>();

            var pvs = _pvPoints.GetKeyList();
            for (int index = 0; index < pvs.Count; index++)
            {
                var profundidade = (Profundidade)pvs[index];

                if (TryGetPontosNoTrechoHorizontal(profundidade, out IList<PontoDeTrajetória> pontosNoTrechoHorizontal))
                {
                    pontosNoTrecho.Add(profundidade, pontosNoTrechoHorizontal);
                }
            }

            return pontosNoTrecho;
        }

        public IList<Profundidade> GetPvsDeTrechosHorizontais()
        {
            return GetTrechosHorizontais().Keys;
        }

        public bool TryGetPontosNoTrechoHorizontal(Profundidade PvProf, out IList<PontoDeTrajetória> pontosNoTrechoHorizontal)
        {
            var pontosNoTrecho = GetPointsByPv(PvProf);

            if (pontosNoTrecho != null && pontosNoTrecho.Count < 2)
            {
                pontosNoTrechoHorizontal = null;
                return false;
            }

            pontosNoTrechoHorizontal = pontosNoTrecho;
            return true;
        }


        public bool TryGetMDFromTVD(double tvd, out double md)
        {
            var pvProf = new Profundidade(tvd);
            if (EstáEmTrechoHorizontal(pvProf))
            {
                var pvPoints = GetPointsByPv(pvProf);
                md = pvPoints[pvPoints.Count - 1].Pm.Valor;
                return true;
            }
            if (ContainsProfundidadePv(pvProf))
            {
                var pvPoints = GetPointsByPv(pvProf);
                md = pvPoints[pvPoints.Count - 1].Pm.Valor;
                return true;
            }

            if (CriarPontoPorPv(pvProf.Valor, MétodoDeCálculoDaTrajetória, out PontoDeTrajetória ponto))
            {
                md = ponto.Pm.Valor;
                //AddToDinamicTable(ponto);
                return true;
            }

            md = default;
            return false;
        }

        public override bool TryGetSessãoTrajetória(Profundidade pm, out SessãoTrajetória sessãoTrajetória)
        {
            if (GetPreviousAndNextPmBuscaBinária(pm, out Profundidade previous, out Profundidade next))
            {
                TryGetPonto(previous, out PontoDeTrajetória pontoInicial);
                TryGetPonto(next, out PontoDeTrajetória pontoFinal);
                sessãoTrajetória = new SessãoTrajetória(pontoInicial, pontoFinal);
                return true;
            }

            sessãoTrajetória = default;
            return false;
        }

        private void AddToDinamicTable(PontoDeTrajetória ponto)
        {
            AdicionarPontoTrajetóriaInternal(ponto);
            AddPv(ponto);
        }

        public override bool TryGetSessãoTrajetóriaEmPv(Profundidade pv, out SessãoTrajetória sessãoTrajetória)
        {
            Profundidade previous = null, next = null;
            if (GetPreviousAndNextPvBuscaBinária(pv, ref previous, ref next))
            {
                var previousPoints = GetPointsByPv(previous);
                PontoDeTrajetória pontoInicial = previousPoints[previousPoints.Count - 1];

                var nextPoints = GetPointsByPv(next);
                PontoDeTrajetória pontoFinal = nextPoints[0];
                sessãoTrajetória = new SessãoTrajetória(pontoInicial, pontoFinal);
                return true;
            }

            sessãoTrajetória = default;
            return false;
        }

        public IList<Profundidade> GetPvsDeTrechosHorizontais(Profundidade profundidadeTopoPv, Profundidade profundidadeBasePv)
        {
            return GetTrechosHorizontais(profundidadeTopoPv, profundidadeBasePv).Keys;
        }

        public SortedList<Profundidade, IList<PontoDeTrajetória>> GetPvsDeTrechosHorizontaisPontos(Profundidade profundidadeTopoPv, Profundidade profundidadeBasePv)
        {
            return GetTrechosHorizontais(profundidadeTopoPv, profundidadeBasePv);
        }

        public IList<Profundidade> GetProfundidadesNoTrechoPm(Profundidade profundidadeTopoPm, Profundidade profundidadeBasePm)
        {
            if (_pontos == null)
                throw new InvalidOperationException("_pontos null");

            List<Profundidade> profundidadesNoTrecho = new List<Profundidade>();
            int topoIndex = -1;

            if (_pontos.ContainsKey(profundidadeTopoPm))
            {
                topoIndex = _pontos.IndexOfKey(profundidadeTopoPm);
            }
            else
            {
                var achouNextPm = GetNextPmBuscaBinária(profundidadeTopoPm, out var nextPm);

                if (achouNextPm && nextPm != null)
                {
                    topoIndex = _pontos.IndexOfKey(nextPm);
                }
            }

            if (topoIndex >= 0)
            {
                var pms = _pontos.GetKeyList();
                for (int index = topoIndex; index < pms.Count; index++)
                {
                    var profundidade = (Profundidade)pms[index];

                    if (profundidade >= profundidadeTopoPm && profundidade <= profundidadeBasePm)
                    {
                        profundidadesNoTrecho.Add(profundidade);
                    }
                    else if (profundidade > profundidadeBasePm)
                    {
                        break;
                    }
                }
            }

            return profundidadesNoTrecho;
        }

        private SortedList<Profundidade, IList<PontoDeTrajetória>> GetTrechosHorizontais(Profundidade profundidadeTopoPv, Profundidade profundidadeBasePv)
        {
            if (_pvPoints == null)
                throw new InvalidOperationException("_pvPoints null");

            SortedList<Profundidade, IList<PontoDeTrajetória>> pontosNoTrecho = new SortedList<Profundidade, IList<PontoDeTrajetória>>();
            //SortedList pontosNoTrecho = new SortedList();
            int topoIndex = -1;

            if (_pvPoints.ContainsKey(profundidadeTopoPv))
            {
                topoIndex = _pvPoints.IndexOfKey(profundidadeTopoPv);
            }
            else
            {
                var achouNextPv = GetNextPvBuscaBinária(profundidadeTopoPv, out var nextPv);

                if (achouNextPv && nextPv != null)
                {
                    topoIndex = _pvPoints.IndexOfKey(nextPv);
                }
            }

            if (topoIndex >= 0)
            {
                var pvs = _pvPoints.GetKeyList();
                for (int index = topoIndex; index < pvs.Count; index++)
                {
                    var profundidade = (Profundidade)pvs[index];

                    if (profundidade >= profundidadeTopoPv && profundidade <= profundidadeBasePv)
                    {
                        if (TryGetPontosNoTrechoHorizontal(profundidade, out IList<PontoDeTrajetória> pontosNoTrechoHorizontal))
                        {
                            pontosNoTrecho.Add(profundidade, pontosNoTrechoHorizontal);
                        }
                    }
                    else if (profundidade > profundidadeBasePv)
                    {
                        break;
                    }
                }
            }

            return pontosNoTrecho;
        }

        #endregion

        #region Map

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PontosTrajetória)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Profundidade)))
            {
                Profundidade.Map();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PontoDeTrajetória)))
            {
                PontoDeTrajetória.Map();
            }

            BsonClassMap.RegisterClassMap<PontoDeTrajetóriaFactory>(pontoDeTrajetóriaFactory => {
                pontoDeTrajetóriaFactory.AutoMap();
                pontoDeTrajetóriaFactory.SetIsRootClass(true);
            });

            BsonClassMap.RegisterClassMap<PontosTrajetória>(pontosTrajetória =>
            {
                pontosTrajetória.AutoMap();
                pontosTrajetória.MapMember(pontos => pontos.Pontos);
                pontosTrajetória.MapMember(pontos => pontos.MétodoDeCálculoDaTrajetória).SetSerializer(new EnumSerializer<MétodoDeCálculoDaTrajetória>(BsonType.String));

                pontosTrajetória.UnmapMember(pontos => pontos.ÉVertical);
                pontosTrajetória.UnmapMember(pontos => pontos.ContémPontos);
                pontosTrajetória.UnmapMember(pontos => pontos.Count);
                pontosTrajetória.UnmapMember(pontos => pontos.PrimeiroPonto);
                pontosTrajetória.UnmapMember(pontos => pontos.ÚltimoPonto);
                pontosTrajetória.UnmapMember(pontos => pontos.PvMáximo);
                pontosTrajetória.UnmapMember(pontos => pontos.PmMáximo);
                pontosTrajetória.UnmapMember(pontos => pontos.PmMínimo);
                pontosTrajetória.UnmapMember(pontos => pontos.PvMínimo);

                pontosTrajetória.SetIgnoreExtraElements(true);
                pontosTrajetória.SetDiscriminator(nameof(Trajetória));
            });
        }

        public void BeginInit()
        {
            _pontos = new ConcurrentSortedListWrapper<Profundidade, PontoDeTrajetória>();
            _pvPoints = new ConcurrentSortedListWrapper<Profundidade, ConcurrentSortedListWrapper<Profundidade, PontoDeTrajetória>>();
        }

        public void EndInit()
        {
            var pontos = _pontos.GetValueList();
            for (var index = 0; index < pontos.Count; index++)
            {
                var ponto = (PontoDeTrajetória)pontos[index];
                ponto.RegisterGetAngleFunc(GetDefaultAngle);
            }
        }

        #endregion
    }
}
