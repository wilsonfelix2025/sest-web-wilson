using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.PontosEntity.Factory;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.LitologiaDoPoco
{
    public class Litologia : PontoFactory<PontoLitologia>, ILitologia, ISupportInitialize
    {
        public Litologia(TipoLitologia classificação, IConversorProfundidade conversorProfundidade) : base(conversorProfundidade)
        {
            Classificação = classificação;

            if (Id == ObjectId.Empty)
            {
                Id = ObjectId.GenerateNewId();
            }

            _pontosDeLitologia = new Pontos<PontoLitologia>(this, conversorProfundidade, this);
            ConversorProfundidade = conversorProfundidade;
            TipoProfundidade = TipoProfundidade.PM;
        }

        #region Fields

        private Pontos<PontoLitologia> _pontosDeLitologia;

        #endregion

        #region Properties

        public IConversorProfundidade ConversorProfundidade { get; private set; }

        public ObjectId Id { get; private set; }

        public TipoLitologia Classificação { get; private set; }

        public IReadOnlyList<PontoLitologia> Pontos => GetPontos();

        public Profundidade PmBase => _pontosDeLitologia?.PmMáximo;

        public Profundidade PvBase => _pontosDeLitologia?.PvMáximo;

        public Profundidade PmTopo => _pontosDeLitologia?.PmMínimo;

        public Profundidade PvTopo => _pontosDeLitologia?.PvMínimo;

        public PontoLitologia PrimeiroPonto => _pontosDeLitologia?.PrimeiroPonto;

        public PontoLitologia UltimoPonto => _pontosDeLitologia?.UltimoPonto;

        public TipoProfundidade ProfundidadeExibição => _pontosDeLitologia == null ? 0 : _pontosDeLitologia.TipoProfundidade;

        public int Count => _pontosDeLitologia.Count;

        public TipoProfundidade TipoProfundidade { get; private set; }

        #endregion

        #region Add Methods

        #region Add Ponto

        public void AddPonto(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade pvProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            Criar(conversorProfundidade, pmProf, pvProf, tipoRocha, tipoProfundidade, origem, out PontoLitologia ponto);
            _pontosDeLitologia.AddPonto(ponto);
        }

        public void AddPonto(IConversorProfundidade conversorProfundidade, double pm, double pv, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            Criar(conversorProfundidade, pm, pv, tipoRocha, tipoProfundidade, origem, out PontoLitologia ponto);
            _pontosDeLitologia.AddPonto(ponto);
        }

        public void AddPontoEmPm(IConversorProfundidade conversorProfundidade, Profundidade pmProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarEmPm(conversorProfundidade, pmProf, tipoRocha, tipoProfundidade, origem, out PontoLitologia ponto);
            _pontosDeLitologia.AddPonto(ponto);
        }

        public void AddPontoEmPm(IConversorProfundidade conversorProfundidade, double pm, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarEmPm(conversorProfundidade, pm, tipoRocha, tipoProfundidade, origem, out PontoLitologia ponto);
            _pontosDeLitologia.AddPonto(ponto);
        }

        public void AddPontoEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarEmPv(conversorProfundidade, pvProf, tipoRocha, tipoProfundidade, origem, out PontoLitologia ponto);
            _pontosDeLitologia.AddPonto(ponto);
        }

        public void AddPontoEmPv(IConversorProfundidade conversorProfundidade, double pv, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarEmPv(conversorProfundidade, pv, tipoRocha, tipoProfundidade, origem, out PontoLitologia ponto);
            _pontosDeLitologia.AddPonto(ponto);
        }

        #endregion

        #region Add PontosDeLitologia

        public void AddPontosEmPm(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarPontosEmPm(conversorProfundidade, pms, tiposRocha, tipoProfundidade, origem, out IList<PontoLitologia> pontos);
            _pontosDeLitologia.AddPontos(pontos);
        }

        public void AddPontosEmPm(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarPontosEmPm(conversorProfundidade, pms, tiposRocha, tipoProfundidade, origem, out IList<PontoLitologia> pontos);
            _pontosDeLitologia.AddPontos(pontos);
        }

        public void AddPontosEmPv(IConversorProfundidade conversorProfundidade, IList<double> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem,
            out IList<PontoLitologia> pontos)
        {
            CriarPontosEmPv(conversorProfundidade, pvs, valores, tipoProfundidade, origem, out pontos);
            _pontosDeLitologia.AddPontos(pontos);
        }


        #endregion

        #endregion

        #region Remove Methods

        public void Clear()
        {
            _pontosDeLitologia.Clear();
        }

        public bool RemovePontoEmPm(Profundidade pm)
        {
            return _pontosDeLitologia.RemovePontoEmPm(pm);
        }

        public bool RemovePonto(PontoLitologia ponto)
        {
            return _pontosDeLitologia.RemovePonto(ponto);
        }

        public int RemovePontosEmPm(Profundidade pmTopo, Profundidade pmBase)
        {
            return _pontosDeLitologia.RemovePontosEmPm(pmTopo, pmBase);
        }

        public void RemovePontosEmPm(IList<Profundidade> pms)
        {
            _pontosDeLitologia.RemovePontosEmPm(pms);
        }

        public void RemovePontos(IList<PontoLitologia> pontos)
        {
            _pontosDeLitologia.RemovePontos(pontos);
        }

        public void RemovePontosEmPv(Profundidade pv)
        {
            _pontosDeLitologia.RemovePontosEmPv(pv);
        }

        public void RemovePontosEmPv(IList<Profundidade> pvs)
        {
            _pontosDeLitologia.RemovePontosEmPv(pvs);
        }

        public void RemovePontosEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase)
        {
            _pontosDeLitologia.RemovePontosEmPv(conversorProfundidade, pvTopo, pvBase);
        }

        #endregion

        #region Edition Methods

        public void EditPonto(PontoLitologia editingPoint, double newValue)
        {
            _pontosDeLitologia.EditPonto(editingPoint, newValue);
        }

        public void EditPontoEmPm(Profundidade pm, double newValue)
        {
            _pontosDeLitologia.EditPontoEmPm(pm, newValue);
        }

        public bool ReplacePonto(PontoLitologia newPoint)
        {
            return _pontosDeLitologia.ReplacePoint(newPoint);
        }

        public void ReplacePontos(IList<PontoLitologia> newPoints)
        {
            _pontosDeLitologia.ReplacePontos(newPoints);
        }

        public void EditPontos(IList<PontoLitologia> editingPoints, IList<double> newValues)
        {
            _pontosDeLitologia.EditPontos(editingPoints, newValues);
        }

        public void EditPontosEmPm(IList<Profundidade> pms, IList<string> tiposRocha)
        {
            _pontosDeLitologia.EditPontosEmPm(pms, tiposRocha);
        }

        public void EditPontosEmPv(Profundidade pv, string tipoRocha)
        {
            _pontosDeLitologia.EditPontosEmPv(pv, tipoRocha);
        }

        public bool SobrescreverPontosEmPm(IConversorProfundidade conversor, Profundidade profTopo, Profundidade profBase, IList<Profundidade> pms, IList<string> tiposRocha, OrigemPonto origem)
        {
            if (ConversorProfundidade == null)
                ConversorProfundidade = conversor;

            CriarPontosEmPm(ConversorProfundidade, pms, tiposRocha, TipoProfundidade.PM, origem, out IList<PontoLitologia> pontos);
            return _pontosDeLitologia.SobrescreverEmPm(profTopo, profBase, pontos.ToList());
        }

        public void AtualizarLitologiaComLaminaDAgua(double laminaDAgua, IConversorProfundidade conversorProfundidade)
        {
            _pontosDeLitologia.TryGetPontosEmPm(new Profundidade(0d), new Profundidade(laminaDAgua), out var pontosNaLamina);
            _pontosDeLitologia.TryGetPontosEmPm(new Profundidade(laminaDAgua), PmBase, out var pontosForaLamina);

            if (pontosNaLamina.Count <= 0)
                return;

            var ponto = pontosNaLamina[pontosNaLamina.Count - 1];
            _pontosDeLitologia.Clear();
            AddPontoEmPm(conversorProfundidade, ponto.Pm, ponto.TipoRocha.Mnemonico, TipoProfundidade.PM, OrigemPonto.Importado);
            _pontosDeLitologia.AddPontos(pontosForaLamina);
        }

        #endregion

        #region Convertion Methods

        public void AtualizarPvs(Trajetória trajetória)
        {
            ConversorProfundidade = trajetória;
            _pontosDeLitologia.AtualizarPvs(trajetória);
        }

        public void ConverterParaPv()
        {
            if (TipoProfundidade == TipoProfundidade.PV)
                return;

            _pontosDeLitologia.ConverterParaPv();
            TipoProfundidade = TipoProfundidade.PV;
        }

        public void ConverterParaPm()
        {
            if (TipoProfundidade == TipoProfundidade.PV)
                return;

            _pontosDeLitologia.ConverterParaPm();
            TipoProfundidade = TipoProfundidade.PM;
        }

        public void Shift(double delta)
        {
            _pontosDeLitologia.Shift(delta, false);
        }

        public void CompletarTrechosSenoidais(Trajetória trajetória)
        {
            ConversorProfundidade = trajetória;

            if (!_pontosDeLitologia.ContémPontos())
                return;

            if (!trajetória.ContémTrechoSenoidal())
                return;

            for (var index = 0; index < trajetória.Pontos.Count; index++)
            {
                var trajPoint = trajetória.Pontos[index];

                if (trajPoint.Inclinação <= 90)
                    continue;

                var pmTopoTrajPoint = trajPoint;
                var pmBaseTrajPoint = trajPoint;

                while (++index < trajetória.Pontos.Count && trajetória.Pontos[index].Inclinação > 90)
                {
                    pmBaseTrajPoint = trajetória.Pontos[index];
                }

                InserirTrechoSenoideEmPm(pmTopoTrajPoint, pmBaseTrajPoint, trajetória);
            }
        }

        private void InserirTrechoSenoideEmPm(PontoDeTrajetória pmTopoTrajPoint, PontoDeTrajetória pmBaseTrajPoint, Trajetória trajetória)
        {
            //TryGetPontoOrPreviousEmPm(trajetória, pmTopoTrajPoint.Pm, out PontoLitologia pontoTopoAnterior);
            TryGetPointsOrNextPointsEmPv(pmTopoTrajPoint.Pv, out var pontoTopoPosterior);

            if (pontoTopoPosterior.Count == 0)
            {
                TryGetPointsOrPreviousPointsEmPv(pmTopoTrajPoint.Pv, out pontoTopoPosterior);
            }

            TryGetPointsOrPreviousPointsEmPv(pmBaseTrajPoint.Pv, out var pontoBaseAnterior);

            if (!ContémPontoNoPm(pmTopoTrajPoint.Pm))
                AddPonto(trajetória, pmTopoTrajPoint.Pm, pmTopoTrajPoint.Pv, pontoTopoPosterior[0].TipoRocha.Mnemonico, pontoTopoPosterior[0].TipoProfundidade, OrigemPonto.Montado);

            if (!ContémPontoNoPm(pmBaseTrajPoint.Pm))
                AddPonto(trajetória, pmBaseTrajPoint.Pm, pmBaseTrajPoint.Pv, pontoBaseAnterior[0].TipoRocha.Mnemonico, pontoBaseAnterior[0].TipoProfundidade, OrigemPonto.Montado);

            //// ajustar topo
            //var inclinação = pmTopoTrajPoint.Inclinação;
            //var pmTopo = pmTopoTrajPoint.Pm.Valor;
            //var pontoTrajTopo = pmTopoTrajPoint;

            //while (inclinação > 90)
            //{
            //    pmTopo -= 0.01;
            //    trajetória.TryGetPonto(new Profundidade(pmTopo), out PontoDeTrajetória pontoTrajetória);
            //    inclinação = pontoTrajetória.Inclinação;

            //    if (inclinação < 90)
            //        break;

            //    pontoTrajTopo = pontoTrajetória;
            //}



            //// ajustar base
            //inclinação = pmBaseTrajPoint.Inclinação;
            //var pmBase = pmBaseTrajPoint.Pm.Valor;
            //var pontoTrajBase = pmBaseTrajPoint;

            //while (inclinação > 90)
            //{
            //    pmBase += 0.01;
            //    trajetória.TryGetPonto(new Profundidade(pmBase), out PontoDeTrajetória pontoTrajetória);
            //    inclinação = pontoTrajetória.Inclinação;

            //    if (inclinação < 90)
            //        break;

            //    pontoTrajBase = pontoTrajetória;
            //}



            TryGetPontosEmPv(trajetória, pmBaseTrajPoint.Pv, pmTopoTrajPoint.Pv, out var pontosTrecho, true);

            if (pontosTrecho != null && pontosTrecho.Count > 2)
            {
                pontosTrecho = pontosTrecho.OrderBy(p => p.Pv).ToList();
                List<string> rochasSubida = new List<string>();
                rochasSubida.Add(pontosTrecho[0].TipoRocha.Mnemonico);

                for (var index = 1; index < pontosTrecho.Count; index++)
                {
                    var ponto = pontosTrecho[index];
                    var pontoAnterior = pontosTrecho[index-1];

                    if (ponto.TipoRocha.Mnemonico != pontoAnterior.TipoRocha.Mnemonico)
                    {
                        rochasSubida.Add(ponto.TipoRocha.Mnemonico);
                    }
                }

                if (rochasSubida.Count > 2)
                {
                    var delta = 0.01;
                    var rochaInicioTrecho = pontosTrecho[pontosTrecho.Count-2].TipoRocha.Mnemonico;
                    var pmInicioTrecho = pmTopoTrajPoint.Pm.Valor + delta;
                    var pmFinalTrecho = pmBaseTrajPoint.Pm.Valor;

                    var pm = pmInicioTrecho;
                    var pv = pmTopoTrajPoint.Pv.Valor;
                    var mnemonicoAnterior = rochaInicioTrecho;
                    PontoLitologia pontoLitoAnterior = pontosTrecho[pontosTrecho.Count - 2];
                    //var fronteirasEncontradas = 0;

                    while (pm < trajetória.PmFinal.Valor && pv >= pontoBaseAnterior[0].Pv.Valor && pv <= pontoTopoPosterior[0].Pv.Valor)
                    {
                        if (!trajetória.TryGetTVDFromMD(pm, out pv))
                        {
                            pm += delta;
                            continue;
                        }

                        if (pm + delta >= trajetória.PmFinal.Valor)
                        {
                            trajetória.TryGetTVDFromMD(trajetória.PmFinal.Valor, out var pvFinal);
                            AddPonto(trajetória, trajetória.PmFinal,
                                new Profundidade(pvFinal),
                                pontoLitoAnterior.TipoRocha.Mnemonico, pontoTopoPosterior[0].TipoProfundidade, OrigemPonto.Montado);
                            break;
                        }

                        if (TryGetPontoEmPv(trajetória, this, new Profundidade(pv), out PontoLitologia ponto))
                        {
                            if (ponto!= null && ponto.TipoRocha.Mnemonico != pontoLitoAnterior.TipoRocha.Mnemonico)
                            {
                                PontoLitologia pontoLitoReferência = pontosTrecho[0];
                                if (pv < pontosTrecho[0].Pv.Valor)
                                {
                                    pontoLitoReferência = pontosTrecho[0];
                                }
                                else if(pv > pontosTrecho[pontosTrecho.Count-1].Pv.Valor)
                                {
                                    pontoLitoReferência = pontosTrecho[pontosTrecho.Count - 1];
                                }
                                else
                                {
                                    for (var index = 1; index < pontosTrecho.Count; index++)
                                    {
                                        var pontoTrecho = pontosTrecho[index];
                                        var pontoTrechoAnterior = pontosTrecho[index - 1];

                                        if(pv >= pontosTrecho[index - 1].Pv.Valor && pv < pontosTrecho[index].Pv.Valor)
                                        {
                                            pontoLitoReferência = pontosTrecho[index - 1];
                                        }
                                    }
                                }                                

                                if (trajetória.TryGetPonto(new Profundidade(pm),
                                    out PontoDeTrajetória pontoDeTrajetória))
                                {
                                    if (pontoDeTrajetória.Inclinação > 90)
                                    {
                                        if(TryGetPointsOrNextPointsEmPv(pontoLitoReferência.Pv, out var pontosPv))
                                        {
                                            AddPonto(trajetória, new Profundidade(pm),
                                            new Profundidade(pontosPv[0].Pv.Valor),
                                            pontosPv[0].TipoRocha.Mnemonico, ponto.TipoProfundidade, OrigemPonto.Montado);
                                        }
                                    }
                                    else
                                    {
                                        if (TryGetPointsOrPreviousPointsEmPv(pontoLitoReferência.Pv, out var pontosPv))
                                        {
                                            AddPonto(trajetória, new Profundidade(pm),
                                                new Profundidade(pontosPv[0].Pv.Valor),
                                                pontosPv[0].TipoRocha.Mnemonico, ponto.TipoProfundidade, OrigemPonto.Montado);
                                        }

                                    }

                                    mnemonicoAnterior = ponto.TipoRocha.Mnemonico;
                                }
                            }
                            pontoLitoAnterior = ponto;
                        }

                        pm += 0.01;
                    }
                }
            }


            if (!ContémPontoNoPm(trajetória.PmFinal) && trajetória.TryGetTVDFromMD(trajetória.PmFinal.Valor, out double lastPv))
            {
                if (lastPv >= pmBaseTrajPoint.Pv.Valor && lastPv <= pmTopoTrajPoint.Pv.Valor)
                {
                    AddPontoEmPm(trajetória, trajetória.PmFinal, UltimoPonto.TipoRocha.Mnemonico, TipoProfundidade.PM, OrigemPonto.Montado);
                }
            }
        }

        #endregion

        #region Perfis Litológicos

        public PerfilBase GetDTMC(double[] profundidadesReferência, TipoProfundidade tipoProfundidade, IConversorProfundidade trajetória, string nomeCálculo)
        {
            if (_pontosDeLitologia == null || !_pontosDeLitologia.ContémPontos())
                return null;

            var startIndex = GetStartIndex(PmTopo.Valor, profundidadesReferência);

            if (startIndex == profundidadesReferência.Length - 1)
                return null;

            var perfil = PerfisFactory.Create(nameof(DTMC), nameof(DTMC) + "_" + nomeCálculo, trajetória, this);

            // Não existe profundidade de referência prevista para o início da litologia.
            if (Math.Abs(profundidadesReferência[startIndex] - PmTopo.Valor) > 0.009)
            {
                var dtmc = _pontosDeLitologia.GetPontos()[0].TipoRocha.Dtmc;
                perfil.AddPontoEmPm(trajetória, PmTopo, dtmc, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            for (var index = startIndex; index < profundidadesReferência.Length; index++)
            {
                var pm = profundidadesReferência[index];

                if (!_pontosDeLitologia.BuscaBinariaPorPmMenorIgual(GetPontos().ToList(), new Profundidade(pm), out var idx, out var equal))
                {
                    continue;
                }
                var dtmc = _pontosDeLitologia.GetPontos()[idx].TipoRocha.Dtmc;
                perfil.AddPontoEmPm(trajetória, pm, dtmc, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            return perfil;
        }

        public PerfilBase GetRHOG(double[] profundidadesReferência, TipoProfundidade tipoProfundidade, IConversorProfundidade trajetória, string nomeCálculo)
        {
            if (_pontosDeLitologia == null || !_pontosDeLitologia.ContémPontos())
                return null;

            var startIndex = GetStartIndex(PmTopo.Valor, profundidadesReferência);

            if (startIndex == profundidadesReferência.Length - 1)
                return null;

            var rhog = PerfisFactory.Create(nameof(RHOG), nameof(RHOG) + "_" + nomeCálculo, trajetória, this);

            // Não existe profundidade de referência prevista para o início da litologia.
            if (Math.Abs(profundidadesReferência[startIndex] - PmTopo.Valor) > 0.009)
            {
                var rhogValue = _pontosDeLitologia.GetPontos()[0].TipoRocha.Rhog;
                rhog.AddPontoEmPm(trajetória, PmTopo, rhogValue, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            for (var index = startIndex; index < profundidadesReferência.Length; index++)
            {
                var pm = profundidadesReferência[index];

                if (!_pontosDeLitologia.BuscaBinariaPorPmMenorIgual(GetPontos().ToList(), new Profundidade(pm), out var idx, out var equal))
                {
                    continue;
                }
                var rhogValue = _pontosDeLitologia.GetPontos()[idx].TipoRocha.Rhog;
                rhog.AddPontoEmPm(trajetória, pm, rhogValue, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            return rhog;
        }

        public bool ObterGrupoLitológicoNessaProfundidade(double profundidade, out int valorGrupoLitológico)
        {
            if (_pontosDeLitologia.TryGetPontoOrPreviousEmPm(new Profundidade(profundidade), out PontoLitologia ponto))
            {
                valorGrupoLitológico = ponto.TipoRocha.Grupo.Valor;
                return true;
            }
            valorGrupoLitológico = GrupoLitologico.NãoIdentificado.Valor;
            return false;
        }

        public bool ObterTipoRochaNaProfundidade(double pm, out TipoRocha tipoRocha)
        {
            if (_pontosDeLitologia.TryGetPontoOrPreviousEmPm(new Profundidade(pm), out PontoLitologia ponto))
            {
                tipoRocha = ponto.TipoRocha;
                return true;
            }
            tipoRocha = TipoRocha.N_IDENT;
            return false;
        }


        //[Obsolete("Lógica aplicada nesse método faz busca binária para todos os pontos, já existentes ou não .")]
        //public bool ObterGrupoLitológicoNessaProfundidade(double profundidade, out int valorGrupoLitológico)
        //{
        //    if (!_pontosDeLitologia.BuscaBinariaPorPmMenorIgual(GetPontos().ToList(), new Profundidade(profundidade), out var idx, out var equal))
        //    {
        //        valorGrupoLitológico = GrupoLitologico.NãoIdentificado.Valor;
        //        return false;
        //    }

        //    valorGrupoLitológico = _pontosDeLitologia.GetPontos()[idx].TipoRocha.Grupo.Valor;
        //    return true;
        //}

        //[Obsolete("Lógica aplicada nesse método faz busca binária para todos os pontos, já existentes ou não .")]
        //public bool ObterTipoRochaNaProfundidade(double pm, out TipoRocha grupo)
        //{
        //    if (!_pontosDeLitologia.BuscaBinariaPorPmMenorIgual(GetPontos().ToList(), new Profundidade(pm), out var idx, out var equal))
        //    {
        //        grupo = TipoRocha.N_IDENT;
        //        return false;
        //    }

        //    grupo = _pontosDeLitologia.GetPontos()[idx].TipoRocha;
        //    return true;
        //}

        private int GetStartIndex(double pm, double[] profundidades)
        {
            if (profundidades.Length < 2 || pm >= profundidades[profundidades.Length - 1])
                return default;

            int menor = 0;
            int maior = profundidades.Length - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (profundidades[meio] > pm)
                {
                    maior = meio - 1;
                }
                else if (profundidades[meio] < pm)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int index;
            if (pm < profundidades[meio])
            {
                index = meio;
            }
            else
            {
                index = meio + 1;
            }

            return index;
        }

        #endregion

        #region Get Methods

        public bool ContémPontos()
        {
            return _pontosDeLitologia != null && _pontosDeLitologia.ContémPontos();
        }

        public bool ContémPontoNoPm(Profundidade pmProf)
        {
            return _pontosDeLitologia.ContémPontoNoPm(pmProf);
        }

        /// <summary>
        /// Retorna a coleção de profundidades.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Profundidade> GetProfundidades()
        {
            return _pontosDeLitologia.GetProfundidades();
        }

        /// <summary>
        /// Obtém todos os pontos.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<PontoLitologia> GetPontos()
        {
            return ImmutableList.CreateRange(_pontosDeLitologia.GetPontos());
        }

        public IList<GrupoLitologico> GetGrupoLitologicos()
        {
            SortedList<string, GrupoLitologico> gruposLitológicos = new SortedList<string, GrupoLitologico>();
            var pontos = _pontosDeLitologia.GetPontos();

            for (int index = 0; index < pontos.Count; index++)
            {
                var grupoLito = pontos[index].TipoRocha.Grupo;

                if (!gruposLitológicos.ContainsKey(grupoLito.Nome))
                    gruposLitológicos.Add(grupoLito.Nome, grupoLito);
            }

            return gruposLitológicos.Values;
        }

        /// <summary>
        /// Tenta obter um ponto em uma dada profundidade medida.
        /// </summary>
        /// <param name="pm">Profundidade Medida</param>
        /// <param name="ponto">Ponto de saída</param>
        /// <returns>True caso encontre o ponto, False caso contrário</returns>
        public bool TryGetPontoEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia ponto)
        {

            if (!_pontosDeLitologia.TryGetPontoEmPm(conversorProfundidade, this, pm, out ponto))
                return false;

            return Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor, ponto.TipoProfundidade, ponto.Origem, this, out ponto);
        }

        public bool TryGetLitoPontoEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia ponto)
        {

            if (!_pontosDeLitologia.TryGetLitoPointEmPm(pm, out ponto))
                return false;

            return true;
        }

        public bool TryGetValueEmPm(Profundidade pm, out double value)
        {
            return _pontosDeLitologia.TryGetValueEmPm(pm, out value);
        }

        /// <summary>
        /// Obtém os pontos compreendidos no trecho estabelecido pelos parâmetros.
        /// </summary>
        /// <param name="pmTopo"></param>
        /// <param name="pmBase"></param>
        /// <param name="pontosNoTrecho"></param>
        /// <returns></returns>
        public bool TryGetPontosEmPm(Profundidade pmTopo, Profundidade pmBase, out List<PontoLitologia> pontosNoTrecho)
        {
            if (!_pontosDeLitologia.TryGetPontosEmPm(pmTopo, pmBase, out IList<PontoLitologia> pontos))
            {
                pontosNoTrecho = new List<PontoLitologia>();
                return false;
            }

            pontosNoTrecho = ImmutableList.CreateRange(pontos).ToList();
            return true;
        }

        /// <summary>
        /// Obtém o ponto na profundidade passada caso haja ponto nessa profundidade.
        /// Caso contrário, obtém o ponto anterior. 
        /// </summary>
        /// <param name="profundidade"></param>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public bool TryGetPontoOrPreviousEmPm(IConversorProfundidade conversorProfundidade, Profundidade profundidade, out PontoLitologia ponto)
        {
            if (!_pontosDeLitologia.TryGetPontoOrPreviousEmPm(profundidade, out ponto))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.TipoRocha.Mnemonico, ponto.TipoProfundidade, ponto.Origem, out ponto);
        }

        /// <summary>
        /// Obtém o ponto na profundidade passada caso haja ponto nessa profundidade.
        /// Caso contrário, obtém o próximo ponto. 
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public bool TryGetPontoOrNextEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia ponto)
        {
            if (!_pontosDeLitologia.TryGetPontoOrNextEmPm(pm, out ponto))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor, ponto.TipoProfundidade, ponto.Origem, this, out ponto);
        }

        /// <summary>
        /// Obtém o ponto anterior e o ponto posterior.
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="previousPoint"></param>
        /// <param name="nextPoint"></param>
        /// <returns></returns>
        public bool TryGetPreviousAndNextPointEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia previousPoint, out PontoLitologia nextPoint)
        {
            if (!_pontosDeLitologia.TryGetPreviousAndNextPointEmPm(pm, out previousPoint, out nextPoint))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            Criar(conversorProfundidade, previousPoint.Pm, previousPoint.Pv, previousPoint.Valor, previousPoint.TipoProfundidade, previousPoint.Origem, this, out previousPoint);
            Criar(conversorProfundidade, nextPoint.Pm, nextPoint.Pv, nextPoint.Valor, nextPoint.TipoProfundidade, nextPoint.Origem, this, out nextPoint);
            return true;
        }

        /// <summary>
        /// Obtém o ponto anterior.
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="previousPoint"></param>
        /// <returns></returns>
        public bool TryGetPreviousPointEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia previousPoint)
        {
            if (!_pontosDeLitologia.TryGetPreviousPointEmPm(pm, out previousPoint))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, previousPoint.Pm, previousPoint.Pv, previousPoint.Valor, previousPoint.TipoProfundidade, previousPoint.Origem, this, out previousPoint);
        }

        /// <summary>
        /// Obtém o próximo ponto .
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="nextPoint"></param>
        /// <returns></returns>
        public bool TryGetNextPointEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia nextPoint)
        {
            if (!_pontosDeLitologia.TryGetNextPointEmPm(pm, out nextPoint))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, nextPoint.Pm, nextPoint.Pv, nextPoint.Valor, nextPoint.TipoProfundidade, nextPoint.Origem, this, out nextPoint);
        }

        /// <summary>
        /// Tenta obter ponto a partir de uma profundidade PV passada.
        /// </summary>
        /// <param name="pv">Profundidade em PV.</param>
        /// <param name="ponto">Ponto obtido.</param>
        /// <returns></returns>
        public bool TryGetPontoEmPv(IConversorProfundidade conversorProfundidade, Litologia litologia, Profundidade pv, out PontoLitologia ponto, bool montagemLitologia = false)
        {
            if (!_pontosDeLitologia.TryGetPontoEmPv(conversorProfundidade, this, pv, montagemLitologia, out ponto))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return litologia.Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.TipoRocha.Mnemonico, TipoProfundidade.PV, ponto.Origem, out ponto);
        }

        /// <summary>
        /// Obtém pontos existentes com a profundidade vertical fornecida.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pointsAtSamePv"></param>
        /// <returns></returns>
        public bool TryGetPontosAtSamePv(Profundidade pv, out IList<PontoLitologia> pointsAtSamePv)
        {
            if (!_pontosDeLitologia.TryGetPointsAtSamePv(pv, out pointsAtSamePv))
                return false;

            pointsAtSamePv = ImmutableList.CreateRange(pointsAtSamePv);
            return true;
        }

        /// <summary>
        /// Obtém pontos com profundidade vertical dentro do range fornecido.
        /// </summary>
        /// <param name="pvTopo"></param>
        /// <param name="pvBase"></param>
        /// <param name="pontos"></param>
        /// <param name="inserirTopoBase"></param>
        /// <returns></returns>
        public bool TryGetPontosEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase, out IList<PontoLitologia> pontos, bool inserirTopoBase = false, bool montagemLitologia = false)
        {
            pontos = default;

            if (!_pontosDeLitologia.TryGetPontosEmPv(conversorProfundidade, pvTopo, pvBase, out var pontosNoTrecho) && !inserirTopoBase)
                return false;

            if (inserirTopoBase)
            {
                if (!pontosNoTrecho.Any(p => p.Pv.Equals(pvTopo)))
                {
                    var gotIt = TryGetPontoEmPv(conversorProfundidade, this, pvTopo, out PontoLitologia pontoTopo, montagemLitologia);

                    if (gotIt && pontoTopo != null)
                        pontosNoTrecho.Add(pontoTopo);

                }

                if (!pontosNoTrecho.Any(p => p.Pv.Equals(pvBase)))
                {
                    var gotIt = TryGetPontoEmPv(conversorProfundidade, this, pvBase, out PontoLitologia pontoBase, montagemLitologia);

                    if (gotIt && pontoBase != null)
                        pontosNoTrecho.Add(pontoBase);
                }
            }

            pontos = ImmutableList.CreateRange(pontosNoTrecho);

            return true;
        }

        public bool TryGetPvsNoTrecho(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase, out IList<PontoLitologia> pvsNoTrecho)
        {
            return _pontosDeLitologia.TryGetPvsNoTrecho(conversorProfundidade, pvTopo, pvBase, out pvsNoTrecho);
        }


        /// <summary>
        /// Obtém uma coleção de pontos que estão em trechos horizontais.
        /// </summary>
        /// <param name="pvTopo"></param>
        /// <param name="pvBase"></param>
        /// <param name="pontosEmTrechosHorizontais"></param>
        /// <returns></returns>
        public bool TryGetTrechosHorizontaisEmPv(Profundidade pvTopo, Profundidade pvBase, out IList<PontoLitologia> pontosEmTrechosHorizontais)
        {
            if (!_pontosDeLitologia.TryGetTrechosHorizontaisEmPv(pvTopo, pvBase, out pontosEmTrechosHorizontais))
                return false;

            pontosEmTrechosHorizontais = ImmutableList.CreateRange(pontosEmTrechosHorizontais);
            return true;
        }

        public IList GetTodasProfundidadesEmPv()
        {
            return _pontosDeLitologia.GetTodasProfundidadesEmPv();
        }

        /// <summary>
        /// Obtém PontosDeLitologia com a profundidade em pv passada, caso haja pontos nessa profundidade.
        /// Caso contrário, obtém os pontos do próximo pv. 
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pontosTvd"></param>
        /// <returns></returns>
        public bool TryGetPointsOrNextPointsEmPv(Profundidade pv, out IList<PontoLitologia> pontosTvd)
        {
            if (!_pontosDeLitologia.TryGetPointsOrNextPointsEmPv(pv, out pontosTvd))
                return false;

            pontosTvd = ImmutableList.CreateRange(pontosTvd);
            return true;
        }

        /// <summary>
        /// Obtém PontosDeLitologia com a profundidade em pv passada, caso haja pontos nessa profundidade.
        /// Caso contrário, obtém os pontos do pv anterior. 
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pontosTvd"></param>
        /// <returns></returns>
        public bool TryGetPointsOrPreviousPointsEmPv(Profundidade pv, out IList<PontoLitologia> pontosTvd)
        {
            if (!_pontosDeLitologia.TryGetPointsOrPreviousPointsEmPv(pv, out pontosTvd))
                return false;

            pontosTvd = ImmutableList.CreateRange(pontosTvd);
            return true;
        }

        /// <summary>
        /// Obtém o pv posterior ao pv do parâmetro.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="nextPv"></param>
        /// <returns></returns>
        public bool TryGetNextPv(Profundidade pv, out Profundidade nextPv)
        {
            return _pontosDeLitologia.TryGetNextPv(pv, out nextPv);
        }

        /// <summary>
        /// Obtém o pv anterior e o pv posterior ao pv do parâmetro.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="previousPv"></param>
        /// <param name="nextPv"></param>
        /// <returns></returns>
        bool TryGetPreviousAndNextPv(Profundidade pv, out Profundidade previousPv, out Profundidade nextPv)
        {
            return _pontosDeLitologia.TryGetPreviousAndNextEmPv(pv, out previousPv, out nextPv);
        }

        #endregion

        #region Map

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Litologia)))
                return;

            BsonClassMap.RegisterClassMap<Litologia>(lito =>
            {
                lito.AutoMap();
                lito.MapMember(litologia => litologia.Id);
                lito.SetIdMember(lito.GetMemberMap(litologia => litologia.Id));
                lito.MapMember(litologia => litologia._pontosDeLitologia).SetSerializer(new PontosLitologiaSerializer()); ;
                lito.MapMember(litologia => litologia.Classificação);
                lito.UnmapMember(litologia => litologia.ConversorProfundidade);
                lito.UnmapMember(litologia => litologia.ProfundidadeExibição);
                lito.SetIgnoreExtraElements(true);
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