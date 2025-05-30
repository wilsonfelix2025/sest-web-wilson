using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.PontosEntity.Factory;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Entities.Trend;
using SestWeb.Domain.EstilosVisuais;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.Perfis.Base
{
    public abstract class PerfilBase : PontoFactory<Ponto>, ISupportInitialize
    {
        #region Constructor

        private protected PerfilBase(string nome, IConversorProfundidade conversorProfundidade, ILitologia litologia) : base(conversorProfundidade, litologia)
        {
            Nome = nome;
            Litologia = litologia;
            EstiloVisual = new EstiloVisual();
            _pontosDePerfil = new Pontos<Ponto>(this, conversorProfundidade, litologia);
            ConversorProfundidade = conversorProfundidade;
        }

        #endregion

        #region Fields

        private Pontos<Ponto> _pontosDePerfil;

        #endregion

        #region Properties

        public ObjectId Id { get; private set; }

        public string IdPoço { get; set; }

        public string IdCálculo { get; set; }

        public string Nome { get; private set; }

        public ILitologia Litologia { get; private set; }

        public IReadOnlyList<Ponto> Pontos => GetPontos();

        public IConversorProfundidade ConversorProfundidade
        {
            get => _pontosDePerfil.GetCoversor();
            set => _pontosDePerfil.SetConversor(value);
        }

        #region Abstract Properties

        public abstract string Mnemonico { get; private protected set; }

        public abstract TipoPerfil TipoPerfil { get; }

        public abstract string Descrição { get; private protected set; }

        public abstract GrupoPerfis GrupoPerfis { get; private protected set; }

        public abstract GrupoUnidades GrupoDeUnidades { get; private protected set; }

        public abstract string UnidadePadrão { get; }

        #endregion

        #region Pontos proxy Properties

        public int Count => _pontosDePerfil == null ? 0 : _pontosDePerfil.Count;

        public double ValorMínimo => _pontosDePerfil == null ? 0 : _pontosDePerfil.ValorMínimo;

        public double ValorMáximo => _pontosDePerfil == null ? 0 : _pontosDePerfil.ValorMáximo;

        public TipoProfundidade ProfundidadeExibição => _pontosDePerfil == null ? 0 : _pontosDePerfil.TipoProfundidade;

        public Profundidade PvMáximo => _pontosDePerfil == null ? null : _pontosDePerfil.GetPvMáximo();

        public Profundidade PmMáximo => _pontosDePerfil == null ? null : _pontosDePerfil.GetPmMáximo();

        public Profundidade PvMínimo => _pontosDePerfil == null ? null : _pontosDePerfil.GetPvMínimo();

        public Profundidade PmMínimo => _pontosDePerfil == null ? null : _pontosDePerfil.GetPmMínimo();

        public Ponto PrimeiroPonto => _pontosDePerfil == null ? null : _pontosDePerfil.PrimeiroPonto;

        public Ponto UltimoPonto => _pontosDePerfil == null ? null : _pontosDePerfil.UltimoPonto;

        #endregion

        #region Visual Properties

        public EstiloVisual EstiloVisual { get; private protected set; }

        #endregion

        #region Cálculo Properties

        public virtual bool PodeSerEntradaCálculoPerfis { get; private protected set; } = true;

        public virtual bool PodeSerConvertidoParaTensão { get; private protected set; } = false;
        public virtual bool PodeSerConvertidoParaGradiente { get; private protected set; } = false;

        #endregion

        #region Trecho Properties

        public virtual bool PodeSerUsadoParaComplementarTrecho { get; private protected set; } = false;

        #endregion

        #region Trend Properties

        public virtual bool PodeTerTrendCompactacao { get; private protected set; } = false;

        public virtual bool PodeTerTrendBaseFolhelho { get; private protected set; } = false;

        public Trend.Trend Trend { get; set; }

        public bool TemTrendLBF => Trend != null && Trend.TipoTrend == TipoTrendEnum.LBF;

        public bool TemTrendCompactação => Trend != null && Trend.TipoTrend == TipoTrendEnum.Compactação;

        #endregion

        #endregion

        #region Methods

        #region Override Methods

        public override string ToString()
        {
            return $"{Nome} ({Mnemonico}) - {_pontosDePerfil.Count} pontos";
        }

        #endregion

        #region Base Methods

        public void SetNewId()
        {
            Id = new ObjectId();
        }

        public void EditarNome(string nomeNovo)
        {
            Nome = nomeNovo;
        }

        public void EditarId(ObjectId id)
        {
            Id = id;
        }

        public void EditarDescrição(string novaDescrição)
        {
            Descrição = novaDescrição;
        }

        public void EditarEstiloVisual(EstiloVisual novoEstiloVisual)
        {
            EstiloVisual = novoEstiloVisual;
        }

        public void EditarPodeSerEntradaCálculoPerfis(bool podeSerUsado)
        {
            PodeSerEntradaCálculoPerfis = podeSerUsado;
        }

        #endregion

        #region Trend Methods

        public void RemoverTrend()
        {
            this.Trend = null;
        }

        #endregion

        #region Pontos Methods

        #region Add Methods

        #region Add Ponto

        public void AddPonto(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade pvProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            Criar(conversorProfundidade, pmProf, pvProf, valor, tipoProfundidade, origem, Litologia, out Ponto ponto);
            _pontosDePerfil.AddPonto(ponto);
        }

        public void AddPonto(IConversorProfundidade conversorProfundidade, double pm, double pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            Criar(conversorProfundidade, pm, pv, valor, tipoProfundidade, origem, Litologia, out Ponto ponto);
            _pontosDePerfil.AddPonto(ponto);
        }

        public void AddPontoEmPm(IConversorProfundidade conversorProfundidade, Profundidade pmProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia = null)
        {
            CriarEmPm(conversorProfundidade, pmProf, valor, tipoProfundidade, origem, litologia, out Ponto ponto);
            _pontosDePerfil.AddPonto(ponto);
        }

        public void AddPontoEmPm(IConversorProfundidade conversorProfundidade, double pm, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarEmPm(conversorProfundidade, pm, valor, tipoProfundidade, origem, Litologia, out Ponto ponto);
            _pontosDePerfil.AddPonto(ponto);
        }

        public void AddPontoEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarEmPv(conversorProfundidade, pvProf, valor, tipoProfundidade, origem, Litologia, out Ponto ponto);
            _pontosDePerfil.AddPonto(ponto);
        }

        public void AddPontoEmPv(IConversorProfundidade conversorProfundidade, double pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarEmPv(conversorProfundidade, pv, valor, tipoProfundidade, origem, Litologia, out Ponto ponto);
            _pontosDePerfil.AddPonto(ponto);
        }

        #endregion

        #region Add Pontos

        public void AddPontosEmPm(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarPontosEmPm(conversorProfundidade, pms, valores, tipoProfundidade, origem, out IList<Ponto> pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        public void AddPontosEmPm(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia)
        {
            CriarPontosEmPm(conversorProfundidade, pms, valores, tipoProfundidade, origem, litologia, out IList<Ponto> pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        public void AddPontosEmPm(IConversorProfundidade conversorProfundidade, IEnumerable<double> pms, IEnumerable<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarPontosEmPm(conversorProfundidade, pms, valores, tipoProfundidade, origem, out IList<Ponto> pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        public void AddPontosEmPm(IConversorProfundidade conversorProfundidade, IEnumerable<Profundidade> pms, IEnumerable<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            CriarPontosEmPm(conversorProfundidade, pms, valores, tipoProfundidade, origem, out IList<Ponto> pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        public void AddPontosEmPv(IConversorProfundidade conversorProfundidade, IList<double> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem,
            out IList<Ponto> pontos)
        {
            CriarPontosEmPv(conversorProfundidade, pvs, valores, tipoProfundidade, origem, out pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        public void AddPontosEmPv(IConversorProfundidade conversorProfundidade, IList<Profundidade> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem,
            out IList<Ponto> pontos)
        {
            CriarPontosEmPv(conversorProfundidade, pvs, valores, tipoProfundidade, origem, out pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        public void AddPontosEmPv(IConversorProfundidade conversorProfundidade, IEnumerable<double> pvs, IEnumerable<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem,
            out IList<Ponto> pontos)
        {
            CriarPontosEmPv(conversorProfundidade, pvs, valores, tipoProfundidade, origem, out pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        public void AddPontosEmPv(IConversorProfundidade conversorProfundidade, IEnumerable<Profundidade> pvs, IEnumerable<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem,
            out IList<Ponto> pontos)
        {
            CriarPontosEmPv(conversorProfundidade, pvs, valores, tipoProfundidade, origem, out pontos);
            _pontosDePerfil.AddPontos(pontos);
        }

        #endregion

        #endregion

        #region Get Methods

        /// <summary>
        /// Retorna a coleção de profundidades.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Profundidade> GetProfundidades()
        {
            return _pontosDePerfil.GetProfundidades();
        }

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
        public IReadOnlyList<Ponto> GetPontos()
        {
            return ImmutableList.CreateRange(_pontosDePerfil.GetPontos());
        }

        /// <summary>
        /// Tenta obter um ponto em uma dada profundidade medida.
        /// </summary>
        /// <param name="pm">Profundidade Medida</param>
        /// <param name="ponto">Ponto de saída</param>
        /// <returns>True caso encontre o ponto, False caso contrário</returns>
        public bool TryGetPontoEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out Ponto ponto,
            GrupoCálculo grupoCálculo = GrupoCálculo.Indefinido)
        {
            // tratamento para buracos em DTS ou RHOB OBS: Somente no Cálculo de Perfis  !!!!! 
            // Passar essa ´logica para o Método chamador no cálculo de perfis
            if ((Mnemonico == nameof(RHOB) || Mnemonico == nameof(DTS)) && _pontosDePerfil.EstáEmBuraco(pm) &&
                grupoCálculo == GrupoCálculo.Perfis && _pontosDePerfil.EstáEmBuraco(pm))
            {
                ponto = default(Ponto);
                return false;
            }

            if (!_pontosDePerfil.TryGetPontoEmPm(conversorProfundidade,this, pm, out ponto))
                return false;

            if (ponto.Origem == OrigemPonto.Interpolado)
            {
                AjusteValorInterpoladoPelaLitologia(pm, ponto);
            }

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return  Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor, ponto.TipoProfundidade, ponto.Origem, Litologia, out ponto);
        }

        public bool TryGetValueEmPm(Profundidade pm, out double value)
        {
            return _pontosDePerfil.TryGetValueEmPm(pm, out value);
        }

        /// <summary>
        /// Obtém os pontos compreendidos no trecho estabelecido pelos parâmetros.
        /// </summary>
        /// <param name="pmTopo"></param>
        /// <param name="pmBase"></param>
        /// <param name="pontosNoTrecho"></param>
        /// <returns></returns>
        public bool TryGetPontosEmPm(Profundidade pmTopo, Profundidade pmBase, out IReadOnlyList<Ponto> pontosNoTrecho)
        {
            if (!_pontosDePerfil.TryGetPontosEmPm(pmTopo, pmBase, out IList<Ponto> pontos))
            {
                pontosNoTrecho =  new List<Ponto>();
                return false;
            }

            pontosNoTrecho = ImmutableList.CreateRange(pontos);
            return true;
        }

        /// <summary>
        /// Obtém o ponto na profundidade passada caso haja ponto nessa profundidade.
        /// Caso contrário, obtém o ponto anterior. 
        /// </summary>
        /// <param name="profundidade"></param>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public bool TryGetPontoOrPreviousEmPm(IConversorProfundidade conversorProfundidade, Profundidade profundidade, out Ponto ponto)
        {
            if (!_pontosDePerfil.TryGetPontoOrPreviousEmPm(profundidade, out ponto))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor, ponto.TipoProfundidade, ponto.Origem, Litologia, out ponto);
        }

        /// <summary>
        /// Obtém o ponto na profundidade passada caso haja ponto nessa profundidade.
        /// Caso contrário, obtém o próximo ponto. 
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public bool TryGetPontoOrNextEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out Ponto ponto)
        {
            if (!_pontosDePerfil.TryGetPontoOrNextEmPm(pm, out ponto))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor, ponto.TipoProfundidade, ponto.Origem, Litologia, out ponto);
        }

        /// <summary>
        /// Obtém o ponto anterior e o ponto posterior.
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="previousPoint"></param>
        /// <param name="nextPoint"></param>
        /// <returns></returns>
        public bool TryGetPreviousAndNextPointEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out Ponto previousPoint, out Ponto nextPoint)
        {
            if (!_pontosDePerfil.TryGetPreviousAndNextPointEmPm(pm, out previousPoint, out nextPoint))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            Criar(conversorProfundidade, previousPoint.Pm, previousPoint.Pv, previousPoint.Valor, previousPoint.TipoProfundidade, previousPoint.Origem, Litologia, out previousPoint);
            Criar(conversorProfundidade, nextPoint.Pm, nextPoint.Pv, nextPoint.Valor, nextPoint.TipoProfundidade, nextPoint.Origem, Litologia, out nextPoint);
            return true;
        }

        /// <summary>
        /// Obtém o ponto anterior.
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="previousPoint"></param>
        /// <returns></returns>
        public bool TryGetPreviousPointEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out Ponto previousPoint)
        {
            if (!_pontosDePerfil.TryGetPreviousPointEmPm(pm, out previousPoint))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, previousPoint.Pm, previousPoint.Pv, previousPoint.Valor, previousPoint.TipoProfundidade, previousPoint.Origem, Litologia, out previousPoint);
        }

        /// <summary>
        /// Obtém o próximo ponto .
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="nextPoint"></param>
        /// <returns></returns>
        public bool TryGetNextPointEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out Ponto nextPoint)
        {
            if (!_pontosDePerfil.TryGetNextPointEmPm(pm, out nextPoint))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return Criar(conversorProfundidade, nextPoint.Pm, nextPoint.Pv, nextPoint.Valor, nextPoint.TipoProfundidade, nextPoint.Origem, Litologia, out nextPoint);
        }

        /// <summary>
        /// Tenta obter ponto a partir de uma profundidade PV passada.
        /// </summary>
        /// <param name="pv">Profundidade em PV.</param>
        /// <param name="ponto">Ponto obtido.</param>
        /// <returns></returns>
        public bool TryGetPontoEmPv(IConversorProfundidade conversorProfundidade, PerfilBase perfil, Profundidade pv, out Ponto ponto)
        {
            if (!_pontosDePerfil.TryGetPontoEmPv(conversorProfundidade,this, pv, out ponto))
                return false;

            // Roberto Miyoshi OBS: Retorna uma cópia do ponto, não permitindo que o ponto verdadeiro seja acidentamente modificado no fluxo chamador.
            return perfil.Criar(conversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor, ponto.TipoProfundidade, ponto.Origem, Litologia, out ponto);
        }

        /// <summary>
        /// Obtém pontos existentes com a profundidade vertical fornecida.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pointsAtSamePv"></param>
        /// <returns></returns>
        public bool TryGetPontosAtSamePv(Profundidade pv, out IList<Ponto> pointsAtSamePv)
        {
            if (!_pontosDePerfil.TryGetPointsAtSamePv(pv, out pointsAtSamePv)) 
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
        public bool TryGetPontosEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase, out IList<Ponto> pontos, bool inserirTopoBase = false)
        {
            pontos = default;

            if (!_pontosDePerfil.TryGetPontosEmPv(conversorProfundidade, pvTopo, pvBase, out var pontosNoTrecho))
                return false;

            if (inserirTopoBase)
            {
                if (!pontosNoTrecho.Any(p=>p.Pv.Equals(pvTopo)))
                {
                    var gotIt = TryGetPontoEmPv(conversorProfundidade,this, pvTopo, out Ponto pontoTopo);

                    if (gotIt && pontoTopo != null)
                        pontosNoTrecho.Add(pontoTopo);
                }

                if (!pontosNoTrecho.Any(p=>p.Pv.Equals(pvBase)))
                {
                    var gotIt = TryGetPontoEmPv(conversorProfundidade, this,pvBase, out Ponto pontoBase);

                    if (gotIt && pontoBase != null)
                        pontosNoTrecho.Add(pontoBase);
                }
            }

            pontos = ImmutableList.CreateRange(pontosNoTrecho);

            return true;
        }

        public bool TryGetPvsNoTrecho(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase, out IList<Ponto> pvsNoTrecho)
        {
            return _pontosDePerfil.TryGetPvsNoTrecho(conversorProfundidade, pvTopo, pvBase,out pvsNoTrecho);
        }

        private void AjusteValorInterpoladoPelaLitologia(Profundidade pm, Ponto ponto)
        {
            //throw new NotImplementedException();
            //var pontosObtidos =
            //    _pontos.GetPontoAnteriorEPosteriorBuscaBinária(pm, out Ponto pontoAnterior, out Ponto pontoPosterior);

            //if (pontosObtidos && Litologia != null)
            //{
            //    var obteveLitoPontoAnterior = Litologia.TryGetTipoLitologia(pontoAnterior.Profundidade, TipoProfundidade.PM, out TipoLitologia litologiaPontoAnterior);
            //    var obteveLitoPontoPosterior = Litologia.TryGetTipoLitologia(pontoPosterior.Profundidade, TipoProfundidade.PM, out TipoLitologia litologiaPontoPosterior);

            //    if (!obteveLitoPontoAnterior || !obteveLitoPontoPosterior ||
            //        litologiaPontoAnterior.Mnemônico == litologiaPontoPosterior.Mnemônico) return;

            //    Litologia.TryGetTipoLitologia(ponto.Profundidade, TipoProfundidade.PM, out TipoLitologia litologiaPontoAtual);

            //    if (litologiaPontoAtual == litologiaPontoAnterior)
            //        ponto.Valor = pontoAnterior.Valor;
            //    else if (litologiaPontoAtual == litologiaPontoPosterior)
            //        ponto.Valor = pontoPosterior.Valor;
            //    else
            //    {
            //        // ponto interpolado está numa litologia intermediária, diferente das extremidades. 
            //    }
            //}
        }

        /// <summary>
        /// Obtém uma coleção de pontos que estão em trechos horizontais.
        /// </summary>
        /// <param name="pvTopo"></param>
        /// <param name="pvBase"></param>
        /// <param name="pontosEmTrechosHorizontais"></param>
        /// <returns></returns>
        public bool TryGetTrechosHorizontaisEmPv(Profundidade pvTopo, Profundidade pvBase, out IList<Ponto> pontosEmTrechosHorizontais)
        {
            if (!_pontosDePerfil.TryGetTrechosHorizontaisEmPv(pvTopo, pvBase, out pontosEmTrechosHorizontais))
                return false;

            pontosEmTrechosHorizontais = ImmutableList.CreateRange(pontosEmTrechosHorizontais);
            return true;
        }

        public IList GetTodasProfundidadesEmPv()
        {
            return _pontosDePerfil.GetTodasProfundidadesEmPv();
        }

        /// <summary>
        /// Obtém Pontos com a profundidade em pv passada, caso haja pontos nessa profundidade.
        /// Caso contrário, obtém os pontos do próximo pv. 
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pontosTvd"></param>
        /// <returns></returns>
        public bool TryGetPointsOrNextPointsEmPv(Profundidade pv, out IList<Ponto> pontosTvd)
        {
            if (!_pontosDePerfil.TryGetPointsOrNextPointsEmPv(pv, out pontosTvd))
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
            return _pontosDePerfil.TryGetNextPv(pv, out nextPv);
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
            return _pontosDePerfil.TryGetPreviousAndNextEmPv(pv, out previousPv, out nextPv);
        }


        public IOrderedEnumerable<Ponto> GetPvPontosNoTrecho(IConversorProfundidade conversorProfundidade, Profundidade pvTopoCorrelação, Profundidade pvBaseCorrelação, Trajetória trajetória, bool inserirTopoBase = false)
        {
            var achouPontos = _pontosDePerfil.TryGetPontosEmPv(conversorProfundidade, pvTopoCorrelação, pvBaseCorrelação, out var pontosPvTrechoCorrelação);


            if (inserirTopoBase)
            {
                // Garantir que o ponto do topo seja inserido --> Garantir a corretude da proporção 
                if (pontosPvTrechoCorrelação.All(p => p.Pv != pvTopoCorrelação))
                {
                    var gotIt = TryGetPontoEmPv(trajetória, this, pvTopoCorrelação, out Ponto pontoTopoCorrelação);

                    if (gotIt && pontoTopoCorrelação != null)
                        pontosPvTrechoCorrelação.Add(pontoTopoCorrelação);
                }

                // (RCM) Garantir que o ponto da base seja inserido --> Garantir a corretude da proporção 
                if (pontosPvTrechoCorrelação.All(p => p.Pv != pvBaseCorrelação))
                {
                    var gotIt = TryGetPontoEmPv(trajetória, this, pvBaseCorrelação, out Ponto pontoBaseCorrelação);

                    if (gotIt && pontoBaseCorrelação != null)
                        pontosPvTrechoCorrelação.Add(pontoBaseCorrelação);
                }
            }

            return pontosPvTrechoCorrelação.OrderBy(p => p.Pv);
        }

        #endregion

        #region Verification Methods

        /// <summary>
        /// Verifica se há pontos na coleção
        /// </summary>
        /// <returns></returns>
        public bool ContémPontos()
        {
            return _pontosDePerfil.Count > 0;
        }

        public bool ContémPonto(Ponto ponto)
        {
            return _pontosDePerfil.ContémPonto(ponto);
        }

        /// <summary>
        /// Verifica se a coleção contém ponto e uma determinada profundidade de medida
        /// </summary>
        /// <param name="pmProf">Profundidade de Medida</param>
        /// <returns>True se a coleção contém um ponto na profundidade, False caso contrário</returns>
        public bool ContémPontoNoPm(Profundidade pmProf)
        {
            return _pontosDePerfil.ContémPontoNoPm(pmProf);
        }

        // Responderá negativamente caso não contenha pontos, porém se o perfil está sem pontos
        // a operação resultante do resultado já não seria aplicada independentemente da origem do perfil.
        public bool ÉPerfilMontado()
        {
            if (!ContémPontos())
                return false;

            var pontos = _pontosDePerfil.GetPontos();

            bool éPerfilMontado = false;

            Parallel.For(0, pontos.Count, (index, state) =>
            {
                if (pontos[index].Origem == OrigemPonto.Montado)
                {
                    éPerfilMontado = true;
                    state.Stop();
                }
            });

            //for (int index = 0; index < pontos.Count; index++)
            //{
            //    if (pontos[index].Origem == OrigemPonto.Montado)
            //        return true;
            //}

            return éPerfilMontado;
        }

        // Responderá negativamente caso não contenha pontos, porém se o perfil está sem pontos
        // a operação resultante do resultado já não seria aplicada independentemente da origem do perfil.
        public bool ÉPerfilCalculado()
        {
            if (!ContémPontos())
                return false;

            var pontos = _pontosDePerfil.GetPontos();

            bool éPerfilCalculado = false;

            if (!string.IsNullOrWhiteSpace(IdCálculo))
                éPerfilCalculado = true;
           

            //Parallel.For(0, pontos.Count, (index, state) =>
            //{
            //    if (pontos[index].Origem == OrigemPonto.Calculado)
            //    {
            //        éPerfilCalculado = true;
            //        state.Stop();
            //    }
            //});

            //for (int index = 0; index < pontos.Count; index++)
            //{
            //    if (pontos[index].Origem == OrigemPonto.Calculado)
            //        return true;
            //}
            //return false;

            return éPerfilCalculado;
        }

        // Responderá negativamente caso não contenha pontos, porém se o perfil está sem pontos
        // a operação resultante do resultado já não seria aplicada independentemente da origem do perfil.
        public bool ÉPerfilFiltrado()
        {
            if (!ContémPontos())
                return false;

            var pontos = _pontosDePerfil.GetPontos();

            bool éPerfilFiltrado = false;

            Parallel.For(0, pontos.Count, (index, state) =>
            {
                if (pontos[index].Origem == OrigemPonto.Filtrado)
                {
                    éPerfilFiltrado = true;
                    state.Stop();
                }
            });

            return éPerfilFiltrado;

            //for (int index = 0; index < pontos.Count; index++)
            //{
            //    if (pontos[index].Origem == OrigemPonto.Filtrado)
            //        return true;
            //}

            //return false;
        }

        #endregion

        #region Remove Methods

        public void Clear()
        {
            _pontosDePerfil.Clear();
        }

        public bool RemovePontoEmPm(Profundidade pm)
        {
            return _pontosDePerfil.RemovePontoEmPm(pm);
        }

        public bool RemovePonto(Ponto ponto)
        {
            return _pontosDePerfil.RemovePonto(ponto);
        }

        public int RemovePontosEmPm(Profundidade pmTopo, Profundidade pmBase)
        {
            return _pontosDePerfil.RemovePontosEmPm(pmTopo, pmBase);
        }

        public void RemovePontosEmPm(IList<Profundidade> pms)
        {
            _pontosDePerfil.RemovePontosEmPm(pms);
        }

        public void Clear(IList<Ponto> pontos)
        {
            _pontosDePerfil.RemovePontos(pontos);
        }

        public void RemovePontosEmPv(Profundidade pv)
        {
            _pontosDePerfil.RemovePontosEmPv(pv);
        }

        public void RemovePontosEmPv(IList<Profundidade> pvs)
        {
            _pontosDePerfil.RemovePontosEmPv(pvs);
        }

        public void RemovePontosEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase)
        {
            _pontosDePerfil.RemovePontosEmPv(conversorProfundidade, pvTopo, pvBase);
        }

        #endregion

        #region Edition Methods

        public void EditPonto(Ponto editingPoint, double newValue)
        {
            _pontosDePerfil.EditPonto(editingPoint, newValue);
        }

        public void EditPontoEmPm(Profundidade pm, double newValue)
        {
            _pontosDePerfil.EditPontoEmPm(pm, newValue);
        }

        public bool ReplacePonto(Ponto newPoint)
        {
            return _pontosDePerfil.ReplacePoint(newPoint);
        }

        public void ReplacePontos(IList<Ponto> newPoints)
        {
            _pontosDePerfil.ReplacePontos(newPoints);
        }       

        public void EditPontos(IList<Ponto> editingPoints, IList<double> newValues)
        {
            _pontosDePerfil.EditPontos(editingPoints,newValues);
        }

        /// <summary>
        /// Edita pontos caso existam e adiciona caso não existam.
        /// </summary>
        /// <param name="conversorProfundidade"></param>
        /// <param name="pms"></param>
        /// <param name="newValues"></param>
        public void EditPontosEmPm(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<double> newValues)
        {
            if(pms == null || pms.Count == 0)
                return;

            var oldPmsNoPerfil = GetProfundidades().ToDictionary(p => p, p => p);

            for (int index = 0; index < pms.Count; index++)
            {
                if (!ContémPontoNoPm(pms[index]))
                {
                    AddPontoEmPm(conversorProfundidade,pms[index],newValues[index],TipoProfundidade.PM,OrigemPonto.Editado, Litologia);
                }

                _pontosDePerfil.EditPontoEmPm(pms[index], newValues[index]);

                if (oldPmsNoPerfil.ContainsKey(pms[index]))
                    oldPmsNoPerfil.Remove(pms[index]);
            }

            foreach (var removingProfs in oldPmsNoPerfil)
            {
                RemovePontoEmPm(removingProfs.Key);
            }
        }

        public void EditPontosEmPv(Profundidade pv, double newValue)
        {
            _pontosDePerfil.EditPontosEmPv(pv, newValue);
        }

        public void EditPontosEmPv(IConversorProfundidade conversorProfundidade, IList<Profundidade> pvs, IList<double> valores)
        {
            if (pvs == null || pvs.Count == 0)
            {
                return;
            }

            _pontosDePerfil.TryGetPvsNoTrecho(conversorProfundidade, _pontosDePerfil.PvMínimo, _pontosDePerfil.PvMáximo, out var listaCompletaPvs);
            var pvsDict = new Dictionary<double, double>();

            foreach (var pontoPv in listaCompletaPvs)
            {
                pvsDict.Add(pontoPv.Pv.Valor, pontoPv.Valor);
            }

            for (int index = 0; index < pvs.Count(); index++)
            {
                if (!pvsDict.ContainsKey(pvs[index].Valor))
                {
                    AddPontoEmPv(conversorProfundidade, pvs[index], valores[index], TipoProfundidade.PV, OrigemPonto.Interpretado);
                } 
                else
                {
                    pvsDict.Remove(pvs[index].Valor);
                    _pontosDePerfil.EditPontosEmPv(pvs[index], valores[index]);
                }
            }

            foreach (var pontoRestante in pvsDict)
            {
                RemovePontosEmPv(new Profundidade(pontoRestante.Key));
            }
        }

        public bool SobrescreverPontosEmPm(IConversorProfundidade conversorProfundidade, Profundidade profTopo, Profundidade profBase, //TODO(Gabriel Pinheiro) : Verificar corretude da mudança
            IList<Profundidade> pms, IList<double> valores, OrigemPonto origem)
        {
            CriarPontosEmPm(conversorProfundidade, pms, valores, TipoProfundidade.PM, origem, out IList<Ponto> pontos);
            return _pontosDePerfil.SobrescreverEmPm(profTopo, profBase, pontos.ToList());
        }

        #endregion

        #region Convertion Methods

        public void AtualizarPvs(IConversorProfundidade conversorProfundidade)
        {
            ConversorProfundidade = conversorProfundidade;
            _pontosDePerfil.AtualizarPvs(conversorProfundidade);
        }

        public void ConverterParaPv()
        {
            _pontosDePerfil.ConverterParaPv();
        }

        public void ConverterParaPm()
        {
            _pontosDePerfil.ConverterParaPm();
        }

        public void Shift(double delta)
        {
            _pontosDePerfil.Shift(delta, GrupoPerfis == GrupoPerfis.Gradientes);
        }

        #endregion

        #endregion

        #endregion

        #region Map

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PerfilBase)))
                return;

            BsonClassMap.RegisterClassMap<PerfilBase>(perfilBase =>
            {
                perfilBase.AutoMap();
                perfilBase.MapMember(perfil => perfil.Id);
                perfilBase.SetIdMember(perfilBase.GetMemberMap(perfil => perfil.Id));
                perfilBase.MapMember(perfil => perfil.IdPoço);
                perfilBase.MapMember(perfil => perfil.IdCálculo);
                perfilBase.MapMember(perfil => perfil.Nome);
                perfilBase.MapMember(perfil => perfil._pontosDePerfil).SetSerializer(new PontosSerializer()); ;
                perfilBase.MapMember(perfil => perfil.Mnemonico);
                perfilBase.MapMember(perfil => perfil.PodeSerUsadoParaComplementarTrecho);
                perfilBase.MapMember(perfil => perfil.PodeTerTrendBaseFolhelho);
                perfilBase.MapMember(perfil => perfil.PodeTerTrendCompactacao);
                perfilBase.MapMember(perfil => perfil.PodeSerEntradaCálculoPerfis);
                perfilBase.UnmapMember(perfil => perfil.GrupoDeUnidades);
                perfilBase.UnmapMember(perfil => perfil.ConversorProfundidade);
                perfilBase.UnmapMember(perfil => perfil.Litologia);
                perfilBase.UnmapMember(perfil => perfil.TipoPerfil);
                perfilBase.UnmapMember(perfil => perfil.Descrição);
                perfilBase.MapMember(perfil => perfil.GrupoPerfis);
                perfilBase.UnmapMember(perfil => perfil.UnidadePadrão);
                perfilBase.UnmapMember(perfil => perfil.TemTrendCompactação);
                perfilBase.UnmapMember(perfil => perfil.TemTrendLBF);

                perfilBase.SetIsRootClass(true);
                perfilBase.SetIgnoreExtraElements(true);
                perfilBase.SetDiscriminator(nameof(PerfilBase));

            });

            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(PerfilBase)))
                .ToList()
                .ForEach(perfilDerived =>
                {
                    RuntimeHelpers.RunClassConstructor(perfilDerived.TypeHandle);
                    
                    var classMapDefinition = typeof(BsonClassMap<>);
                    var classMapType = classMapDefinition.MakeGenericType(perfilDerived);
                    var classMap = (BsonClassMap)Activator.CreateInstance(classMapType);
                    classMap.SetDiscriminator(perfilDerived.Name);
                    BsonClassMap.RegisterClassMap(classMap);
                });

            Pontos<Ponto>.Map();
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            GrupoDeUnidades =  GrupoUnidades.GetGrupoUnidades(Mnemonico);
            GrupoPerfis = GrupoPerfis.GetGrupoPerfis(Mnemonico);
        }

        #endregion

    }

    
}
