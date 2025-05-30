using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.EstilosVisuais;

namespace SestWeb.Domain.Entities.Trajetoria
{
    public class Trajetória : IConversorProfundidade, ISupportInitialize
    {
        public Trajetória(MétodoDeCálculoDaTrajetória método)
        {
            _pontosTrajetória = new PontosTrajetória(método);

            EstiloVisual = new EstiloVisual();
            ProfundidadeExibição = TipoProfundidade.PM;
        }

        #region Fields

        private readonly double _defaultProf = 10000;

        private List<PontoProjeção> _pontosDeProjeção = new List<PontoProjeção>();

        private PontosTrajetória _pontosTrajetória;

        #endregion

        #region Properties

        #region Visual Properties

        public EstiloVisual EstiloVisual { get; private set; }

        public IReadOnlyList<PontoProjeção> PontosProjeção => _pontosDeProjeção;

        public TipoProfundidade ProfundidadeExibição { get; private set; }

        #endregion

        #region Pontos Proxy

        public PontoDeTrajetória PrimeiroPonto => _pontosTrajetória.PrimeiroPonto;

        public PontoDeTrajetória ÚltimoPonto => _pontosTrajetória.ÚltimoPonto;

        public Profundidade PmInicial => _pontosTrajetória.PmMínimo;

        public Profundidade PmFinal => _pontosTrajetória.PmMáximo;

        public Profundidade PvInicial => _pontosTrajetória.PvMínimo;

        public Profundidade PvFinal => _pontosTrajetória.PvMáximo;

        public MétodoDeCálculoDaTrajetória MétodoDeCálculoDaTrajetória => _pontosTrajetória.MétodoDeCálculoDaTrajetória;

        public bool ÉVertical => _pontosTrajetória.ÉVertical;

        public int Count => _pontosTrajetória.Count;

        public IReadOnlyList<PontoDeTrajetória> Pontos => GetPontos();

        #endregion

        #endregion

        #region Methods

        #region Add

        public void CriarTrajetóriaVertical(double pmFinal)
        {
            _pontosTrajetória.CriarTrajetóriaVertical(pmFinal);
            //CriarPontosDeProjeção();
        }

        public void AddPonto(double pm, double inclinação, double azimute)
        {
            _pontosTrajetória.AdicionarPontoTrajetória(pm, inclinação, azimute);
            //CriarPontosDeProjeção();
        }

        public void AddPontos(IList<PontoDeTrajetória> pontosDeTrajetória)
        {
            _pontosTrajetória.AdicionarPontosTrajetória(pontosDeTrajetória);
            //CriarPontosDeProjeção();
        }

        public void Reset(IList<PontoDeTrajetória> pontos)
        {
            _pontosTrajetória.Reset(pontos);
            //CriarPontosDeProjeção();
        }

        public void GerarTrajetóriaPadrão()
        {
            CriarTrajetóriaVertical(_defaultProf);
            //CriarPontosDeProjeção();
        }

        #endregion

        #region Get

        /* Roberto Miyoshi
         *
         * IReadOnlyList<string> myReadOnlyList = myList;. This works because List<string> implements IReadOnlyList<string>.
         * It does not prevent others from casting myReadOnlyList back to List<string>.
         *
         * IReadOnlyList<string> myReadOnlyList = myList.AsReadOnly();. This creates a read-only proxy for the original list.
         * Although it does prevent others from casting myReadOnlyList back to List<string>,
         * the contents of myReadOnlyList may still change as a result of modifications on myList.
         *
         * IReadOnlyList<string> myReadOnlyList = ImmutableList.CreateRange(myList);
         * This creates an ImmutableList<string> which contains copies of the original list's contents and does not allow any modification.
         * Changes to myList won't be visible in myReadOnlyList.
        */
        /// <summary>
        /// Obtém todos os pontos.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<PontoDeTrajetória> GetPontos()
        {
            return ImmutableList.CreateRange(_pontosTrajetória.GetPontos());
        }

        public bool ContainsProfundidade(Profundidade profPm)
        {
            return _pontosTrajetória.ContainsProfundidade(profPm);
        }

        public bool TryGetTVDFromMD(double md, out double tvd)
        {
            return _pontosTrajetória.TryGetTVDFromMD(md, out tvd);
        }

        public bool TryGetPonto(Profundidade pm, out PontoDeTrajetória pontoDeTrajetória)
        {
            return _pontosTrajetória.TryGetPonto(pm, out pontoDeTrajetória);
        }

        public bool ContémDados()
        {
            return _pontosTrajetória.ContémPontos;
        }

        public bool ContémTrechoSenoidal()
        {
            return _pontosTrajetória.ContémTrechoSenoidal();
        }

        #endregion

        #region Remove

        public void Clear()
        {
            _pontosTrajetória.ApagarPontos();
            //CriarPontosDeProjeção();
        }

        public void RemoverPontoDeTrajetória(double pm)
        {
            _pontosTrajetória.RemoverPontoTrajetória(new Profundidade(pm));
            //CriarPontosDeProjeção();
        }

        #endregion

        #region Edit

        public bool EditarPontoDeTrajetória(double pm, double inclinação, double azimute)
        {
            var result = _pontosTrajetória.EditarPontoTrajetória(new Profundidade(pm), inclinação, azimute);

            if (result)
            {
                //CriarPontosDeProjeção();
            }
            return result;
        }

        public void Shift(double delta)
        {
            var pontos = _pontosTrajetória.Pontos;
            List<PontoDeTrajetória> novosPontos = new List<PontoDeTrajetória>(pontos.Count);

            for (var index = 0; index < pontos.Count; index++)
            {
                var ponto = pontos[index];
                if (Math.Abs(ponto.Pm.Valor) < 0.009)
                {
                    novosPontos.Add(ponto);
                    continue;
                }

                _pontosTrajetória.CriarPontoPorPm(ponto.Pm.Valor + delta, ponto.Inclinação, ponto.Azimute, _pontosTrajetória.MétodoDeCálculoDaTrajetória, out PontoDeTrajetória novoPonto);
                novosPontos.Add(novoPonto);
            }

            _pontosTrajetória.Reset(novosPontos);
            //CriarPontosDeProjeção();
        }

        public void SobrescreverPontos(double pmTopo, double pmBase, IList<double> pms, IList<double> inclinações, IList<double> azimutes)
        {
            if (pms == null || inclinações == null || azimutes == null || pms.Count != inclinações.Count ||
                pms.Count != azimutes.Count || inclinações.Count != azimutes.Count)
                return;

            var pmsAnteriores = _pontosTrajetória.GetProfundidadesNoTrechoPm(PmInicial, new Profundidade(pmTopo));
            var pmsPosteriores = _pontosTrajetória.GetProfundidadesNoTrechoPm(new Profundidade(pmBase), PmFinal);

            List<PontoDeTrajetória> pontos = new List<PontoDeTrajetória>(pmsAnteriores.Count);

            foreach (var pm in pmsAnteriores)
            {
                if (pm.Valor < pmTopo)
                {
                    _pontosTrajetória.TryGetPonto(pm, out PontoDeTrajetória pontoAnterior);
                    pontos.Add(pontoAnterior);
                }
            }

            for (var index = 0; index < pms.Count; index++)
            {
                var pm = pms[index];
                if (pm >= pmTopo && pm <= pmBase)
                {
                    if (_pontosTrajetória.CriarPontoPorPm(pms[index], inclinações[index], azimutes[index],
                        MétodoDeCálculoDaTrajetória, out PontoDeTrajetória novoPonto))
                    {
                        pontos.Add(novoPonto);
                    }
                }
            }

            foreach (var pm in pmsPosteriores)
            {
                if (pm.Valor > pmBase)
                {
                    _pontosTrajetória.TryGetPonto(pm, out PontoDeTrajetória pontoAnterior);
                    pontos.Add(pontoAnterior);
                }
            }

            Reset(pontos);
        }

        #endregion

        #region Exibition

        public void ExibirEmPM()
        {
            if (ProfundidadeExibição == TipoProfundidade.PM)
                return;

            ProfundidadeExibição = TipoProfundidade.PM;
        }

        public void ExibirEmPV()
        {
            if (ProfundidadeExibição == TipoProfundidade.PV)
                return;

            ProfundidadeExibição = TipoProfundidade.PV;
        }

        #endregion

        #region Pv

        public bool TryGetMDFromTVD(double tvd, out double md)
        {
            return _pontosTrajetória.TryGetMDFromTVD(tvd, out md);
        }

        public IList<Profundidade> GetPvsDeTrechosHorizontais(Profundidade profundidadeTopoPv,
            Profundidade profundidadeBasePv)
        {
            return _pontosTrajetória.GetPvsDeTrechosHorizontais(profundidadeTopoPv, profundidadeBasePv);
        }

        public SortedList<Profundidade, IList<PontoDeTrajetória>> GetPvsDeTrechosHorizontaisPontos(Profundidade profundidadeTopoPv,
            Profundidade profundidadeBasePv)
        {
            return _pontosTrajetória.GetPvsDeTrechosHorizontaisPontos(profundidadeTopoPv, profundidadeBasePv);
        }

        public bool GetNextPmBuscaBinária(Profundidade profundidade, out Profundidade profundidadePosterior)
        {
            return _pontosTrajetória.GetNextPmBuscaBinária(profundidade, out profundidadePosterior);
        }

        public IList<Profundidade> GetProfundidadesNoTrechoPm(Profundidade profundidadeTopoPm,
            Profundidade profundidadeBasePm)
        {
            return _pontosTrajetória.GetProfundidadesNoTrechoPm(profundidadeTopoPm, profundidadeBasePm);
        }

        public bool TryGetPontosNoTrechoHorizontal(Profundidade PvProf,
            out IList<PontoDeTrajetória> pontosNoTrechoHorizontal)
        {
            return _pontosTrajetória.TryGetPontosNoTrechoHorizontal(PvProf, out pontosNoTrechoHorizontal);
        }

        public bool EstáEmTrechoHorizontal(Profundidade pm, Profundidade pv)
        {
            return _pontosTrajetória.EstáEmTrechoHorizontal(pv);
        }

        public bool EstáEmTrechoHorizontal(PontoDeTrajetória ponto)
        {
            return _pontosTrajetória.EstáEmTrechoHorizontal(ponto);
        }

        public bool EstáEmTrechoHorizontal(Profundidade pvProf)
        {
            return _pontosTrajetória.EstáEmTrechoHorizontal(pvProf);
        }

        public bool TryGetLastPmHorizontal(Profundidade pv, out Profundidade pm)
        {
            pm = default;

            if (!EstáEmTrechoHorizontal(pv))
            {
                return false;
            }

            var pontos = _pontosTrajetória.GetPointsByPv(pv);

            return true;
        }

        public bool TryGetUniquePms(Profundidade pv, out IList<Profundidade> pms)
        {
            return _pontosTrajetória.TryGetUniquePms(pv, out pms);
        }

        public bool ContainsProfundidadePv(Profundidade pvProf)
        {
            return _pontosTrajetória.ContainsProfundidadePv(pvProf);
        }

        public IList<PontoDeTrajetória> GetPointsByPv(Profundidade pv)
        {
            List<PontoDeTrajetória> pontosDeTrajetória = new List<PontoDeTrajetória>();

            TryGetUniquePms(pv, out IList<Profundidade> pms);

            foreach (var pm in pms)
            {
                if (TryGetPonto(pm, out PontoDeTrajetória ponto))
                {
                    pontosDeTrajetória.Add(ponto);
                }
            }

            return pontosDeTrajetória;
        }

        public IList<Profundidade> GetProfundidadesPm()
        {
            return _pontosTrajetória.GetAllProfundidadesPm();
        }

        public IList<Profundidade> GetProfundidadesPv()
        {
            return _pontosTrajetória.GetAllProfundidadesPv();
        }

        #endregion

        #endregion

        #region Map

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Trajetória)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(PontosTrajetória)))
            {
                PontosTrajetória.Map();
            }

            BsonClassMap.RegisterClassMap<Trajetória>(trajetória =>
            {
                trajetória.AutoMap();
                trajetória.MapMember(trajet => trajet._pontosTrajetória);
                trajetória.MapMember(trajet => trajet.ProfundidadeExibição);

                trajetória.UnmapMember(trajet => trajet.PrimeiroPonto);
                trajetória.UnmapMember(trajet => trajet.ÚltimoPonto);
                trajetória.UnmapMember(trajet => trajet.PmInicial);
                trajetória.UnmapMember(trajet => trajet.PmFinal);
                trajetória.UnmapMember(trajet => trajet.PvInicial);
                trajetória.UnmapMember(trajet => trajet.PvFinal);
                trajetória.UnmapMember(trajet => trajet.MétodoDeCálculoDaTrajetória);
                trajetória.UnmapMember(trajet => trajet.ÉVertical);
                trajetória.UnmapMember(trajet => trajet.Count);
                trajetória.UnmapMember(trajet => trajet.ÉVertical);

                trajetória.SetIgnoreExtraElements(true);
                trajetória.SetDiscriminator(nameof(Trajetória));
            });
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

        #endregion

    }
}
