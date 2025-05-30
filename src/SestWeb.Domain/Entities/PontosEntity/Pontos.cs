using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.PontosEntity.Factory;
using SestWeb.Domain.Entities.PontosEntity.InternalCollections;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.PontosEntity
{
    public class Pontos<T> : IEquatable<Pontos<T>> where T : IPonto
    {
        #region Constructor

        public Pontos(IPontoFactory<T> pontoFactory, IConversorProfundidade conversorProfundidade, ILitologia litologia)
        {
            _conversorProfundidade = conversorProfundidade;
            _litologia = litologia;
            _pmPointsDict = new ConcurrentDictionaryWrapper<Profundidade, T>();
            _pmSortedPoints = new ConcurrentSortedListWrapper<Profundidade, T>();
            _pvPointsCache = new PontosCache<Profundidade, ConcurrentSortedListWrapper<Profundidade, T>>();
            _pvSortedProfs = new ConcurrentSortedListWrapper<Profundidade, Profundidade>();
           // _pontoFactory = pontoFactory;
            TipoProfundidade = TipoProfundidade.PM;

            //pontos = new List<PontoFront>();
            _pmPointsCache = new PontosCache<Profundidade, T>();
            _pmPointsCache.ItemAdded += OnItemAdded;
            _pmPointsCache.ItemRemoved += OnItemRemoved;
            
        }

        #endregion

        #region Fields

        /// <summary>
        /// Step identificado de buraco.
        /// </summary>
        private readonly double _stepMínimoIdentificadorBuraco = 10;

        /// <summary>
        /// Coversor de profundidades.
        /// </summary>
        private protected IConversorProfundidade _conversorProfundidade;

        /// <summary>
        /// Litologia do poço.
        /// </summary>
        private readonly ILitologia _litologia;

        /// <summary>
        /// Cache de carregamento dos pontos indexado por PM.
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
        public PontosCache<Profundidade, T> _pmPointsCache { get; private set; }


        /// <summary>
        /// Armazena os pontos por chave/valor. Acesso rápido.
        /// </summary>
        private readonly ConcurrentDictionaryWrapper<Profundidade, T> _pmPointsDict;

        /// <summary>
        /// Pontos ordenados por PM
        /// </summary>
        private readonly ConcurrentSortedListWrapper<Profundidade, T> _pmSortedPoints;

        /// <summary>
        /// Armazena os pontos ordenados por profundidade vertical. Para cada profundidade
        /// vertical, são armazenados os pontos ordenados por profundidade medida.
        /// </summary>
        private readonly PontosCache<Profundidade, ConcurrentSortedListWrapper<Profundidade, T>> _pvPointsCache;

        /// <summary>
        /// Profundidades em tvd ordenadas.
        /// </summary>
        private readonly ConcurrentSortedListWrapper<Profundidade, Profundidade> _pvSortedProfs;

        //private readonly IPontoFactory<T> _pontoFactory;

        //public List<PontoFront> pontos { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Tipo de profundidade de referência.
        /// </summary>
        public TipoProfundidade TipoProfundidade { get; private set; }

        /// <summary>
        /// Valor mínimo da série.
        /// </summary>
        public double ValorMínimo => Count > 0 ? _pmPointsCache.Min(p => p.Value.Valor) : 0;

        /// <summary>
        /// Valor máximo da série.
        /// </summary>
        public double ValorMáximo => Count > 0 ? _pmPointsCache.Max(p => p.Value.Valor) : 0;

        /// <summary>
        /// Quantidade de pontos.
        /// </summary>
        public int Count => _pmPointsDict.Count;

        /// <summary>
        /// Profundidade máxima da série.
        /// </summary>
        public Profundidade ProfundidadeMáxima => GetProfundidadeMáxima();

        /// <summary>
        /// Maior profundidade vertical na série.
        /// </summary>
        public Profundidade PvMáximo => GetPvMáximo();

        /// <summary>
        /// Maior profundidade medida na série.
        /// </summary>
        public Profundidade PmMáximo => GetPmMáximo();

        /// <summary>
        /// Menor profundidade vertical na série.
        /// </summary>
        public Profundidade PvMínimo => GetPvMínimo();

        /// <summary>
        /// Menor profundidade medida na série.
        /// </summary>
        public Profundidade PmMínimo => GetPmMínimo();

        /// <summary>
        /// Ponto com menor profundidade medida na série.
        /// </summary>
        public T PrimeiroPonto => Count > 0 ? (T)_pmSortedPoints.GetValueList()[0] : default(T);

        /// <summary>
        /// Ponto com maior profundidade medida na série.
        /// </summary>
        public T UltimoPonto => Count > 0 ? (T)_pmSortedPoints.GetValueList()[Count - 1] : default(T);


        #endregion

        #region Methods

        #region Adição 

        public void AddPonto(T ponto)
        {
            // TODO (GPC) Remover esse nullcheck
            if (ponto == null || !AddPontoInternal(ponto))
                return;

            //_itemsNotificator.ExecutarOnItemAdicionado(pontoDePerfil);
        }

        private bool AddPontoInternal(T ponto)
        {
            try
            {
                if (_pmPointsCache.ContainsKey(ponto.Pm))
                    return false;

                if (TipoProfundidade == TipoProfundidade.PV && ponto.TipoProfundidade == TipoProfundidade.PM)
                {
                    ponto.TrocarProfundidadeReferenciaParaPV();
                }

                if (_pmPointsCache.TryAdd(ponto.Pm, ponto))
                {
                    if (_conversorProfundidade != null)
                    {
                        if (ponto.Pm.Valor <= _conversorProfundidade.ÚltimoPonto.Pm.Valor)
                        {
                            if (ponto.Pv == null && _conversorProfundidade.ContémDados())
                                ponto.AtualizarPV();
                        }
                    }

                    if (ponto.Pv != null)
                    {
                        AddPontoEmPvInternal(ponto);
                    }
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private bool AddPontoEmPvInternal(T ponto)
        {
            if (!_pmPointsDict.ContainsKey(ponto.Pm))
                throw new InvalidOperationException("Ponto deve ser adicionado antes no dictionary com chaves em PM");

            if (ponto.Pv == null)
            {
                throw new InvalidOperationException("PV precisa ser calculado antes!");
                //return false;
            }

            var pv = new Profundidade(ponto.Pv.Valor);
            if (!_pvPointsCache.ContainsKey(pv))
            {
                if (_pvPointsCache.TryAdd(pv, new ConcurrentSortedListWrapper<Profundidade, T>()))
                {
                    _pvSortedProfs.Add(pv, pv);
                }
                else
                    return false;
            }

            if (!_pvPointsCache[pv].ContainsKey(ponto.Pm))
                _pvPointsCache[pv].Add(ponto.Pm, ponto);
            else
                return false;

            return true;
        }

        public void AddPontos(IEnumerable<T> pontosDePerfil)
        {
            if (pontosDePerfil == null || !pontosDePerfil.Any())
                return;

            var pontos = pontosDePerfil.ToList();
            for (int index = 0; index < pontosDePerfil.Count(); index++)
            {
                var ponto = pontos[index];
                if (ponto.Pv == null && _conversorProfundidade != null && _conversorProfundidade.ContémDados())
                    ponto.AtualizarPV();
                AddPontoInternal(pontos[index]);
            }

            //_itemsNotificator.ExecutarOnItensAdicionados(pontosDePerfil.ToList());
        }

        public void AddPontos(IList<T> pontosDePerfil)
        {
            if (pontosDePerfil == null || !pontosDePerfil.Any())
                return;

            for (int index = 0; index < pontosDePerfil.Count; index++)
            {
                AddPontoInternal(pontosDePerfil[index]);
            }

            //_itemsNotificator.ExecutarOnItensAdicionados(pontosDePerfil.ToList());
        }

        private void OnItemAdded(KeyValuePair<Profundidade, T> itemAdded)
        {
            ItemAddedHandle(itemAdded.Value);
        }

        private bool ItemAddedHandle(T ponto)
        {
            if (_pmPointsDict.ContainsKey(ponto.Pm))
                return false;

            _pmPointsDict.Add(ponto.Pm, ponto);
            _pmSortedPoints.Add(ponto.Pm, ponto);

            //AddPontosFront(ponto);

            return true;
        }

        //private void AddPontosFront(T ponto)
        //{
        //    var pto = new PontoFront
        //    {
        //        Valor = ponto.Valor,
        //        Origem = ponto.Origem.ToString(),
        //        TipoProfundidade = ponto.TipoProfundidade.ToString(),
        //        TipoRocha = ponto.TipoRocha,
        //        Pm = ponto.Pm.Valor,
        //        Pv = ponto.Pv.Valor
        //    };

        //    pontos.Add(pto);
        //}

        #endregion

        #region Obtenção

        #region Pm

        /// <summary>
        /// Retorna as profundidades de medida em ordem crescente
        /// </summary>
        /// <returns>Lista de profundidades em ordem crescente</returns>
        public IReadOnlyList<Profundidade> GetProfundidades()
        {
            Profundidade[] profundidades = new Profundidade[_pmSortedPoints.Count];

            var sortedProfs = _pmSortedPoints.GetKeyList();
            for (int index = 0; index < _pmSortedPoints.Count; index++)
            {
                profundidades[index] = (Profundidade)sortedProfs[index];
            }

            return profundidades; //ImmutableList.CreateRange(profundidades);
        }

        /// <summary>
        /// Obtêm todos os pontos.
        /// </summary>
        /// <returns></returns>
        public IList<T> GetPontos()
        {
            T[] pontos = new T[_pmSortedPoints.Count];

            var sortedPoints = _pmSortedPoints.GetValueList();
            for (int index = 0; index < _pmSortedPoints.Count; index++)
            {
                pontos[index] = (T)sortedPoints[index];
            }

            return pontos;
        }

        /// <summary>
        /// Tenta obter um ponto em uma dada profundidade medida.
        /// </summary>
        /// <param name="pm">Profundidade Medida</param>
        /// <param name="ponto">Ponto de saída</param>
        /// <returns>True caso encontre o ponto, False caso contrário</returns>
        public bool TryGetPontoEmPm(IConversorProfundidade conversorProfundidade, PerfilBase perfil, Profundidade pm, out T ponto)
        {
            try
            {
                if (_pmPointsDict.TryGetValue(pm, out ponto))
                {
                    return true;
                }

                if (pm < PmMínimo || pm > PmMáximo)
                {
                    ponto = default;
                    return false;
                }

                var pontosObtidos =
                    TryGetPreviousAndNextPointEmPm(pm, out T pontoAnterior, out T pontoPosterior);

                if (!pontosObtidos)
                {
                    ponto = default;
                    return false;
                }

                // Interpolação linear entre os pontos anterior e posterior
                double valor;
                if (Math.Abs(pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor) < 0.009)
                {
                    valor = (pontoAnterior.Valor + pontoPosterior.Valor) / 2;
                }

                valor = pontoAnterior.Valor + (pm.Valor - pontoAnterior.Pm.Valor) *
                        (pontoPosterior.Valor - pontoAnterior.Valor) /
                        (pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor);

                if (double.IsNaN(valor) || double.IsInfinity(valor))
                {
                    ponto = default;
                    return false;
                }

                var result = perfil.CriarEmPm(conversorProfundidade, pm, valor, TipoProfundidade.PM, OrigemPonto.Interpolado, null, out var temp);
                ponto = (T) (IPonto) temp;
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Pontos: TryGetPonto: Erro ao tentar obter ponto na profundidade Pm = {pm.Valor} Razão: {e.Message}",
                    e);
            }
        }

        public bool TryGetPontoEmPm(IConversorProfundidade conversorProfundidade, Litologia litologia, Profundidade pm, out T ponto)
        {
            try
            {
                
                if (_pmPointsDict.TryGetValue(pm, out ponto))
                {
                    return true;
                }

                
                var pontosObtidos =
                    TryGetPreviousAndNextPointEmPm(pm, out T pontoAnterior, out T pontoPosterior);

                if (!pontosObtidos)
                {
                    ponto = default;
                    return false;
                }

                // Interpolação linear entre os pontos anterior e posterior
                double valor;
                if (Math.Abs(pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor) < 0.009)
                {
                    valor = (pontoAnterior.Valor + pontoPosterior.Valor) / 2;
                }

                valor = pontoAnterior.Valor + (pm.Valor - pontoAnterior.Pm.Valor) *
                        (pontoPosterior.Valor - pontoAnterior.Valor) /
                        (pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor);

                if (double.IsNaN(valor) || double.IsInfinity(valor))
                {
                    ponto = default;
                    return false;
                }

                var result = litologia.CriarEmPm(conversorProfundidade, pm, valor, TipoProfundidade.PM, OrigemPonto.Interpolado, null, out var temp);
                ponto = (T)(IPonto)temp;
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Pontos: TryGetPonto: Erro ao tentar obter ponto na profundidade Pm = {pm.Valor} Razão: {e.Message}",
                    e);
            }
        }

        public bool TryGetValueEmPm(Profundidade profundidade, out double valor)
        {
            try
            {

                if (_pmPointsDict.TryGetValue(profundidade, out T ponto))
                {
                    valor = ponto.Valor;
                    return true;
                }

                if (profundidade < PmMínimo || profundidade > PmMáximo)
                {
                    valor = default;
                    return false;
                }

                var pontosObtidos =
                    TryGetPreviousAndNextPointEmPm(profundidade, out T pontoAnterior, out T pontoPosterior);

                if (!pontosObtidos)
                {
                    valor = default;
                    return false;
                }

                // Interpolação linear entre os pontos anterior e posterior
                if (Math.Abs(pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor) < 0.009)
                {
                    valor = (pontoAnterior.Valor + pontoPosterior.Valor) / 2;
                }

                valor = pontoAnterior.Valor + (profundidade.Valor - pontoAnterior.Pm.Valor) *
                        (pontoPosterior.Valor - pontoAnterior.Valor) /
                        (pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor);

                if (double.IsNaN(valor) || double.IsInfinity(valor))
                {
                    valor = default;
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Pontos: TryGetValue: Erro ao tentar obter valor na profundidade Pm = {profundidade.Valor} Razão: {e.Message}",
                    e);
            }
        }

        /// <summary>
        /// Obtêm os pontos com profundidades entre a porfundidade de Topo (inclusive)
        /// e a profundidade Base (exclusive). 
        /// </summary>
        /// <param name="pmTopo"></param>
        /// <param name="pmBase"></param>
        /// <param name="pontosNoTrecho"></param>
        /// <returns></returns>
        public bool TryGetPontosEmPm(Profundidade pmTopo, Profundidade pmBase, out IList<T> pontosNoTrecho)
        {
            pontosNoTrecho = new List<T>();

            if (!ContémPontos())
            {
                return false;
            }

            if (pmTopo > pmBase)
                throw new ArgumentException($"As profundidades topo: {pmTopo} e base: {pmBase} são inválidas. " +
                                            "A profundidade topo precisa ser sempre menor que a profundidade base, ou elas podem ser iguais.");

            var gotTopoTrecho = TryGetPontoOrNextEmPm(pmTopo, out var pontoTopo);

            if (gotTopoTrecho && pontoTopo != null)
            {
                int topoIndex = _pmSortedPoints.IndexOfKey(pontoTopo.Pm);

                var pontos = _pmSortedPoints.GetValueList();
                for (int index = topoIndex; index < _pmSortedPoints.Count; index++)
                {
                    var pontoNoTrecho = (T) pontos[index];
                    var profundidade = pontoNoTrecho.Pm;

                    if (profundidade >= pmTopo && profundidade <= pmBase)
                    {
                        pontosNoTrecho.Add(pontoNoTrecho);
                    }
                    else if (profundidade > pmBase)
                    {
                        break;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// // Interpolação linear entre os pontos anterior e posterior, realizada em pm.
        /// </summary>
        /// <param name="pmProv">Profundidade na qual se quer obter o valor.</param>
        /// <param name="pontoPosterior">Ponto anterior à profundidade passada.</param>
        /// <param name="pontoAnterior">Ponto posterior à profundidade passada.</param>
        /// <returns></returns>
        private double InterpolarEmPm(Profundidade pmProv, T pontoPosterior, T pontoAnterior)
        {
            double valor;
            if (Math.Abs(pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor) < 0.009)
            {
                valor = (pontoAnterior.Valor + pontoPosterior.Valor) / 2;
            }

            valor = pontoAnterior.Valor + (pmProv.Valor - pontoAnterior.Pm.Valor) *
                    (pontoPosterior.Valor - pontoAnterior.Valor) /
                    (pontoPosterior.Pm.Valor - pontoAnterior.Pm.Valor);
            return valor;
        }

        /// <summary>
        /// Obtém o ponto na profundidade passada caso haja ponto nessa profundidade.
        /// Caso contrário, obtém o ponto anterior. 
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public bool TryGetPontoOrPreviousEmPm(Profundidade pm, out T ponto)
        {
            if (_pmPointsDict.TryGetValue(pm, out ponto))
            {
                return true;
            }

            if (!TryGetPreviousPointEmPm(pm, out ponto))
                return false;

            return true;
        }

        /// <summary>
        /// Obtém o ponto na profundidade passada caso haja ponto nessa profundidade.
        /// Caso contrário, obtém o próximo ponto. 
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public bool TryGetPontoOrNextEmPm(Profundidade pm, out T ponto)
        {
            if (_pmPointsDict.TryGetValue(pm, out ponto))
            {
                return true;
            }

            if (!TryGetNextPointEmPm(pm, out ponto))
                return false;

            return true;
        }


        public bool TryGetLitoPointEmPm(Profundidade pm, out T litoPoint)
        {
            var profundidades = _pmSortedPoints.GetKeyList();

            if (profundidades.Count > 0)
            {
                var últimoIndice = profundidades.Count - 1;

                for (int i = 0; i < profundidades.Count - 1; i++)
                {
                    if (pm.Valor <= ((Profundidade) profundidades[i]).Valor ||
                        pm.Valor > ((Profundidade) profundidades[i]).Valor &&
                        pm.Valor < ((Profundidade) profundidades[i + 1]).Valor)
                    {
                        litoPoint = _pmSortedPoints[(Profundidade) profundidades[i]];
                        return true;
                    }
                }

                if (pm.Valor == ((Profundidade) profundidades[profundidades.Count - 1]).Valor)
                {
                    litoPoint = _pmSortedPoints[(Profundidade) profundidades[profundidades.Count - 1]];
                    return true;
                }
            }

            litoPoint = default;
            return false;
        }



        /// <summary>
        /// Obtém o ponto anterior e o ponto posterior.
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="previousPoint"></param>
        /// <param name="nextPoint"></param>
        /// <returns></returns>
        public bool TryGetPreviousAndNextPointEmPm(Profundidade pm, out T previousPoint, out T nextPoint)
        {
            var profundidades = _pmSortedPoints.GetKeyList();

            if (profundidades.Count < 2 || pm.Valor <= ((Profundidade)profundidades[0]).Valor ||
                pm.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                previousPoint = default;
                nextPoint = default;
                return false;
            }

            if (pm.Valor > ((Profundidade)profundidades[profundidades.Count - 2]).Valor &&
                pm.Valor < ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                previousPoint = _pmSortedPoints[(Profundidade)profundidades[profundidades.Count - 2]];
                nextPoint = _pmSortedPoints[(Profundidade)profundidades[profundidades.Count - 1]];
                return true;
            }

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > pm.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < pm.Valor)
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
            if (Math.Abs(pm.Valor - ((Profundidade)profundidades[meio]).Valor) < 0.09)
            {
                indexAnterior = meio - 1;
                indexPosterior = meio + 1;
            }
            else if (pm.Valor > ((Profundidade)profundidades[meio]).Valor)
            {
                indexAnterior = meio;
                indexPosterior = meio + 1;
            }
            else if (pm.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                indexPosterior = meio;
                indexAnterior = meio - 1;

            }

            if (indexAnterior < 0 || indexPosterior > profundidades.Count - 1)
            {
                previousPoint = default;
                nextPoint = default;
                return false;
            }

            previousPoint = _pmSortedPoints[(Profundidade)profundidades[indexAnterior]];
            nextPoint = _pmSortedPoints[(Profundidade)profundidades[indexPosterior]];
            return true;
        }

        /// <summary>
        /// Obtém o ponto anterior.
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="previousPoint"></param>
        /// <returns></returns>
        public bool TryGetPreviousPointEmPm(Profundidade pm, out T previousPoint)
        {
            previousPoint = default;

            var profundidades = _pmSortedPoints.GetKeyList();

            if (profundidades.Count < 2 || pm.Valor <= ((Profundidade)profundidades[0]).Valor)
                return false;

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > pm.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < pm.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int index;
            if (pm.Valor > ((Profundidade)profundidades[meio]).Valor)
            {
                index = meio;
            }
            else
            {
                index = meio - 1;
            }

            previousPoint = _pmSortedPoints[(Profundidade)profundidades[index]];
            return true;
        }

        /// <summary>
        /// Obtém o próximo ponto .
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="nextPoint"></param>
        /// <returns></returns>
        public bool TryGetNextPointEmPm(Profundidade pm, out T nextPoint)
        {
            nextPoint = default;

            var profundidades = _pmSortedPoints.GetKeyList();

            if (profundidades.Count < 2 || pm.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
                return false;

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > pm.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < pm.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int index;
            if (pm.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                index = meio;
            }
            else
            {
                index = meio + 1;
            }

            nextPoint = _pmSortedPoints[(Profundidade)profundidades[index]];
            return true;
        }

        #endregion

        #region Pv

        /// <summary>
        /// Tenta obter ponto a partir de uma profundidade PV passada.
        /// </summary>
        /// <param name="pv">Profundidade em PV.</param>
        /// <param name="ponto">Ponto obtido.</param>
        /// <returns></returns>
        public bool TryGetPontoEmPv(IConversorProfundidade conversorProfundidade, PerfilBase perfil, Profundidade pv, out T ponto)
        {
            try
            {
                if (!_pvPointsCache.Any())
                {
                    ponto = default;
                    return false;
                }

                if (pv < PvMínimo || pv > PvMáximo)
                {
                    ponto = default;
                    return false;
                }

                TryGetPointsAtSamePv(pv, out var pvPoints);

                if (pvPoints.Count == 1)
                {
                    ponto = (T)pvPoints[0];
                    return true;
                }
                else if (pvPoints.Count > 1)
                {
                    ponto = GetUniquePvPoint(pv);
                    return true;
                }

                if (!TryGetPreviousAndNextEmPv(pv, out Profundidade profundidadeAnterior,
                    out Profundidade profundidadePosterior))
                {
                    ponto = default;
                    return false;
                }

                TryGetPointsAtSamePv(profundidadeAnterior, out var pontosAnterioresMesmoPv);
                var pontoAnterior = (T)pontosAnterioresMesmoPv[pontosAnterioresMesmoPv.Count - 1];

                TryGetPointsAtSamePv(profundidadePosterior, out var pontosPosterioresMesmoPv);
                var pontoPosterior = (T)pontosPosterioresMesmoPv[0];

                if (!conversorProfundidade.TryGetMDFromTVD(pv.Valor, out double pm))
                {
                    ponto = default;
                    return false;
                }

                var valor = InterpolarEmPv(pv, pontoPosterior, pontoAnterior);

                if (double.IsNaN(valor) || double.IsInfinity(valor))
                {
                    ponto = default;
                    return false;
                }

                var pmProf = new Profundidade(pm);
                var result = perfil.Criar(conversorProfundidade, pmProf, pv, valor, TipoProfundidade.PV, OrigemPonto.Interpolado, null, out var temp);
                ponto = (T) (IPonto) temp;
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Pontos: TryGetPontoFromTvd: Razão: {e.Message}.", e);
            }
        }

        /// <summary>
        /// Tenta obter ponto a partir de uma profundidade PV passada.
        /// </summary>
        /// <param name="conversorProfundidade">Trajetória do poço.</param>
        /// <param name="litologia">Litologia do poço em questão.</param>
        /// <param name="pv">Profundidade em PV.</param>
        /// <param name="ponto">Ponto obtido.</param>
        /// <returns></returns>
        public bool TryGetPontoEmPv(IConversorProfundidade conversorProfundidade, Litologia litologia, Profundidade pv, bool montagemLitologia, out T ponto)
        {
            
            try
            {
                if (!_pvPointsCache.Any())
                {
                    ponto = default;
                    return false;
                }

                if (pv < PvMínimo || pv > PvMáximo)
                {
                    ponto = default;
                    return false;
                }

                TryGetPointsAtSamePv(pv, out var pvPoints);

                if (pvPoints.Count == 1)
                {
                    ponto = (T)pvPoints[0];
                    return true;
                }
                else if (pvPoints.Count > 1)
                {
                    ponto = GetUniquePvPoint(pv);
                    return true;
                }

                if (!TryGetPreviousAndNextEmPv(pv, out Profundidade profundidadeAnterior,
                    out Profundidade profundidadePosterior, montagemLitologia))
                {
                    ponto = default;
                    return false;
                }

                TryGetPointsAtSamePv(profundidadeAnterior, out var pontosAnterioresMesmoPv);
                var pontoAnterior = (IPonto)pontosAnterioresMesmoPv[pontosAnterioresMesmoPv.Count - 1];
                var tipoRocha = ((PontoLitologia) pontoAnterior).TipoRocha;

                var result = litologia.CriarEmPv(conversorProfundidade, pv, tipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Interpolado, out var temp);

                //TryGetPointsAtSamePv(profundidadePosterior, out var pontosPosterioresMesmoPv);
                //var pontoPosterior = (T)pontosPosterioresMesmoPv[0];

                //if (!conversorProfundidade.TryGetMDFromTVD(pv.Valor, out double pm))
                //{
                //    ponto = default;
                //    return false;
                //}

                //var valor = InterpolarEmPv(pv, pontoPosterior, pontoAnterior);

                //if (double.IsNaN(valor) || double.IsInfinity(valor))
                //{
                //    ponto = default;
                //    return false;
                //}

                //var pmProf = new Profundidade(pm);
                //var result = litologia.Criar(conversorProfundidade, pmProf, pv, valor, TipoProfundidade.PV, OrigemPonto.Interpolado, out var temp);
                ponto = (T)(IPonto)temp;
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Pontos: TryGetPontoFromTvd: Razão: {e.Message}.", e);
            }
        }

        /// <summary>
        /// // Interpolação linear entre os pontos anterior e posterior, realizada em pv.
        /// </summary>
        /// <param name="pvProf">Profundidade na qual se quer obter o valor.</param>
        /// <param name="pontoPosterior">Ponto anterior à profundidade passada.</param>
        /// <param name="pontoAnterior">Ponto posterior à profundidade passada.</param>
        /// <returns></returns>
        private double InterpolarEmPv(Profundidade pvProf, T pontoPosterior, T pontoAnterior)
        {
            double valor;
            if (Math.Abs(pontoPosterior.Pv.Valor - pontoAnterior.Pv.Valor) < 0.009)
            {
                valor = (pontoAnterior.Valor + pontoPosterior.Valor) / 2;
            }

            valor = pontoAnterior.Valor + (pvProf.Valor - pontoAnterior.Pv.Valor) *
                    (pontoPosterior.Valor - pontoAnterior.Valor) /
                    (pontoPosterior.Pv.Valor - pontoAnterior.Pv.Valor);
            return valor;
        }


        //TODO(Roberto Miyoshi): Dar suporte a trajetórias senoidais
        /// <summary>
        /// Obtém uma coleção de profundidades em pv, onde há trechos horizontais.
        /// </summary>
        /// <param name="pvTopo"></param>
        /// <param name="pvBase"></param>
        /// <param name="pontosNoTrecho"></param>
        /// <returns></returns>
        public bool TryGetTrechosHorizontaisEmPv(Profundidade pvTopo, Profundidade pvBase, out IList<T> pontosNoTrecho)
        {
            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            pontosNoTrecho = new List<T>();
            int topoIndex = -1;

            if (_pvPointsCache.ContainsKey(pvTopo))
            {
                topoIndex = _pvSortedProfs.IndexOfKey(pvTopo);
            }
            else
            {
                var achouNextPv = TryGetNextPv(pvTopo, out var nextPv);

                if (achouNextPv && nextPv != null)
                {
                    topoIndex = _pvSortedProfs.IndexOfKey(nextPv);
                }
            }

            if (topoIndex >= 0)
            {
                var pvs = _pvSortedProfs.GetKeyList();
                for (int index = topoIndex; index < _pvSortedProfs.Count; index++)
                {
                    var pv = (Profundidade) pvs[index];

                    if (pv >= pvTopo && pv < pvBase)
                    {
                        if (!TryGetPointsAtSamePv(pv, out var pontosMesmoTvd) || pontosMesmoTvd.Count <= 1) 
                            continue;

                        foreach (var ponto in pontosMesmoTvd) // TODO(Roberto Miyoshi): verificar se os pontos com mesmo pv estão em trechos horizontais diferentes?
                        {
                            if(ponto.EstáEmTrechoHorizontal)
                                pontosNoTrecho.Add(ponto);
                        }
                    }
                    else if (pv > pvBase)
                    {
                        break;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Obtém uma coleção de pontos em pv, podendo haver trechos horizontais.
        /// </summary>
        /// <param name="pvTopo"></param>
        /// <param name="pvBase"></param>
        /// <param name="pontosNoTrecho"></param>
        /// <returns></returns>
        public bool TryGetPontosEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase, out IList<T> pontosNoTrecho)
        {
            pontosNoTrecho = new List<T>();
            int topoIndex = -1;

            if (!ContémPontos())
            {
                return false;
            }

            if (pvTopo > pvBase)
                throw new ArgumentException($"As profundidades topo: {pvTopo} e base: {pvBase} são inválidas. " +
                                            "A profundidade topo precisa ser sempre menor que a profundidade base, ou elas podem ser iguais.");

            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            if (_pvPointsCache.ContainsKey(pvTopo))
            {
                topoIndex = _pvSortedProfs.IndexOfKey(pvTopo);
            }
            else
            {
                var achouNextPv = TryGetNextPv(pvTopo, out var nextPv);

                if (achouNextPv && nextPv != null)
                {
                    if (nextPv >= pvBase)
                        return false;

                    topoIndex = _pvSortedProfs.IndexOfKey(nextPv);
                }
            }

            if (topoIndex >= 0)
            {
                var pvs = _pvSortedProfs.GetKeyList();
                for (int index = topoIndex; index < _pvSortedProfs.Count; index++)
                {
                    var pv = (Profundidade)pvs[index];

                    if (pv >= pvTopo && pv < pvBase)
                    {
                        //var uniquePvPoint = GetUniquePvPoint(pv);
                        //if (uniquePvPoint != null)
                        //    pontosNoTrecho.Add(uniquePvPoint.Pv, uniquePvPoint);
                        if (TryGetUniquePvPoints(conversorProfundidade, pv, out IList<T> uniquePoints))
                        {
                            foreach (var ponto in uniquePoints)
                            {
                                pontosNoTrecho.Add(ponto);
                            }
                        }
                    }
                    else if (pv > pvBase)
                    {
                        break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Obtém os pvs no trecho.
        /// </summary>
        /// <param name="pvTopo"></param>
        /// <param name="pvBase"></param>
        /// <param name="pvsNoTrecho"></param>
        /// <returns></returns>
        public bool TryGetPvsNoTrecho(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase, out IList<T> pvsNoTrecho)
        {
            pvsNoTrecho = new List<T>();
            int topoIndex = -1;

            if (!ContémPontos())
            {
                return false;
            }

            if (pvTopo > pvBase)
                throw new ArgumentException($"As profundidades topo: {pvTopo} e base: {pvBase} são inválidas. " +
                                            "A profundidade topo precisa ser sempre menor que a profundidade base, ou elas podem ser iguais.");

            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            if (_pvPointsCache.ContainsKey(pvTopo))
            {
                topoIndex = _pvSortedProfs.IndexOfKey(pvTopo);
            }
            else
            {
                var achouNextPv = TryGetNextPv(pvTopo, out var nextPv);

                if (achouNextPv && nextPv != null)
                {
                    if (nextPv >= pvBase)
                        return false;

                    topoIndex = _pvSortedProfs.IndexOfKey(nextPv);
                }
            }

            if (topoIndex >= 0)
            {
                var pvs = _pvSortedProfs.GetKeyList();
                for (int index = topoIndex; index < _pvSortedProfs.Count; index++)
                {
                    var pv = (Profundidade)pvs[index];

                    if (pv >= pvTopo && pv < pvBase)
                    {
                        //var uniquePvPoint = GetUniquePvPoint(pv);

                        //if (uniquePvPoint != null)
                        //    pontosNoTrecho.Add(uniquePvPoint.Pv, uniquePvPoint);
                        if (TryGetUniquePvPoints(conversorProfundidade, pv, out IList<T> uniquePoints))
                        {
                            foreach (var ponto in uniquePoints)
                            {
                                pvsNoTrecho.Add(ponto);
                            }
                        }
                    }
                    else if (pv > pvBase)
                    {
                        if (index > 0)
                        {
                            var lastPv = _pvSortedProfs.GetByIndex(index - 1);

                            if (lastPv >= pvTopo && lastPv < pvBase)
                            {
                                var pt = GetUniquePvPoint(lastPv);

                                if(!pvsNoTrecho.Contains(pt))
                                    pvsNoTrecho.Add(pt);
                            }
                        }
                        break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Obtém Pontos com o pv passado, caso haja pontos nesse pv.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pointsAtSamePv"></param>
        /// <returns></returns>
        public bool TryGetPointsAtSamePv(Profundidade pv, out IList<T> pointsAtSamePv)
        {
            pointsAtSamePv = new List<T>();

            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            if (!_pvPointsCache.ContainsKey(pv))
                return false;

            for (int i = 0; i < _pvPointsCache[pv].Count; i++)
            {
                pointsAtSamePv.Add(_pvPointsCache[pv].GetByIndex(i));
            }

            return true;
        }

        /// <summary>
        /// Retorna apenas um Ponto por PV. Se estiver num trecho horizontal, retorna o último ponto do trecho.
        /// Para Trajetórias senoidais, usar TryGetUniquePvPoints
        /// </summary>
        /// <param name="pv"></param>
        /// <returns></returns>
        private T GetUniquePvPoint(Profundidade pv)
        {
            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            if (!_pvPointsCache.ContainsKey(pv))
                return default;

            if (_pvPointsCache[pv].Count == 1)
                return (T)_pvPointsCache[pv].GetValueList()[0];

            int index = 0;
            T uniquePonto = (T)_pvPointsCache[pv].GetValueList()[index];

            while (uniquePonto.EstáEmTrechoHorizontal && ++index < _pvPointsCache[pv].Count)
            {
                uniquePonto = (T)_pvPointsCache[pv].GetValueList()[index];
            }

            return uniquePonto;
            //return (T)_pvPointsCache[pv].GetValueList()[_pvPointsCache[pv].Count - 1]; //TODO(RCM): Tratar trajetória senoidal.
        }

        /// <summary>
        /// Retorna os pontos senoidais únicos acrescidos dos últimos pontos dos trechos horizontais.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="uniquePoints"></param>
        /// <returns></returns>
        private bool TryGetUniquePvPoints(IConversorProfundidade conversorProfundidade, Profundidade pv, out IList<T> uniquePoints)
        {
            uniquePoints = new List<T>();

            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            if (!_pvPointsCache.ContainsKey(pv))
                return false;

            if (_pvPointsCache[pv].Count == 1)
            {
                uniquePoints.Add((T)_pvPointsCache[pv].GetValueList()[0]);
                return true;
            }

            if (!conversorProfundidade.TryGetUniquePms(pv, out IList<Profundidade> uniquePms))
                return false; // não deveria acontecer

            foreach (var pm in uniquePms)
            {
                uniquePoints.Add(_pvPointsCache[pv][pm]);
            }

            return true;
        }

        public IList GetTodasProfundidadesEmPv()
        {
            return _pvSortedProfs.GetKeyList();
        }

        /// <summary>
        /// Obtém Pontos com a profundidade em pv passada, caso haja pontos nessa profundidade.
        /// Caso contrário, obtém os pontos do próximo pv. 
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pontosTvd"></param>
        /// <returns></returns>
        public bool TryGetPointsOrNextPointsEmPv(Profundidade pv, out IList<T> pontosTvd)
        {
            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            if (TryGetPointsAtSamePv(pv, out pontosTvd) && pontosTvd.Count > 0)
                return true;

            var achouNextPv = TryGetNextPv(pv, out var nextPv);

            if (achouNextPv && nextPv != null)
            {
                TryGetPointsAtSamePv(nextPv, out pontosTvd);
            }

            return achouNextPv;
        }

        /// <summary>
        /// Obtém Pontos com a profundidade em pv passada, caso haja pontos nessa profundidade.
        /// Caso contrário, obtém os pontos do pv anterior. 
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="pontosTvd"></param>
        /// <returns></returns>
        public bool TryGetPointsOrPreviousPointsEmPv(Profundidade pv, out IList<T> pontosTvd)
        {
            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            if (TryGetPointsAtSamePv(pv, out pontosTvd) && pontosTvd.Count > 0)
                return true;

            var achouPreviousPv = TryGetPreviousPv(pv, out var previousPv);

            if (achouPreviousPv && previousPv != null)
            {
                TryGetPointsAtSamePv(previousPv, out pontosTvd);
            }

            return achouPreviousPv;
        }

        /// <summary>
        /// Obtém o próximo pv.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="nextPv"></param>
        /// <returns></returns>
        public bool TryGetNextPv(Profundidade pv, out Profundidade nextPv)
        {
            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            var profundidades = _pvSortedProfs.GetKeyList();

            if (profundidades.Count < 2 || pv.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                nextPv = null;
                return false;
            }

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > pv.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < pv.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int index;
            if (pv.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                index = meio;
            }
            else
            {
                index = meio + 1;
            }

            nextPv = _pvSortedProfs[(Profundidade)profundidades[index]];
            return true;
        }

        /// <summary>
        /// Obtém o pv anterior.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="previousPv"></param>
        /// <returns></returns>
        public bool TryGetPreviousPv(Profundidade pv, out Profundidade previousPv)
        {
            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            var profundidades = _pvSortedProfs.GetKeyList();

            if (profundidades.Count < 2 || pv.Valor <= ((Profundidade)profundidades[0]).Valor)
            {
                previousPv = null;
                return false;
            }

            int menor = 0;
            int maior = profundidades.Count - 1;
            int meio = 0;

            while (menor <= maior)
            {
                meio = menor + (maior - menor >> 1);

                if (((Profundidade)profundidades[meio]).Valor > pv.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < pv.Valor)
                {
                    menor = meio + 1;
                }
                else
                {
                    break;
                }
            }

            int index;
            if (pv.Valor > ((Profundidade)profundidades[meio]).Valor)
            {
                index = meio;
            }
            else
            {
                index = meio - 1;
            }

            previousPv = _pvSortedProfs[(Profundidade)profundidades[index]];
            return true;
        }

        public bool TryGetPreviousAndNextEmPv(Profundidade pv,  out Profundidade previousPv, out Profundidade nextPv, bool montagemLitologia = false)
        {
            if (_pvPointsCache == null)
                throw new InvalidOperationException("PV precisam ser previamente atualizados!");

            var profundidades = _pvSortedProfs.GetKeyList();

            if (profundidades.Count < 2 || pv.Valor <= ((Profundidade)profundidades[0]).Valor ||
                pv.Valor >= ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
            {
                previousPv = default;
                nextPv = default;
                return false;
            }

            if (pv.Valor > ((Profundidade)profundidades[profundidades.Count - 2]).Valor &&
                pv.Valor < ((Profundidade)profundidades[profundidades.Count - 1]).Valor)
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

                if (((Profundidade)profundidades[meio]).Valor > pv.Valor)
                {
                    maior = meio - 1;
                }
                else if (((Profundidade)profundidades[meio]).Valor < pv.Valor)
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
            if (Math.Abs(pv.Valor - ((Profundidade)profundidades[meio]).Valor) < 0.09 && montagemLitologia == false)
            {
                indexAnterior = meio - 1;
                indexPosterior = meio + 1;
            }
            else if (pv.Valor > ((Profundidade)profundidades[meio]).Valor)
            {
                indexAnterior = meio;
                indexPosterior = meio + 1;
            }
            else if (pv.Valor < ((Profundidade)profundidades[meio]).Valor)
            {
                indexPosterior = meio;
                indexAnterior = meio - 1;

            }

            if (indexAnterior < 0 || indexPosterior > profundidades.Count - 1)
            {
                previousPv = default;
                nextPv = default;
                return false;
            }

            previousPv = (Profundidade)profundidades[indexAnterior];
            nextPv = (Profundidade)profundidades[indexPosterior];
            return true;
        }

        #endregion

        #region Pm e Pv

        public Profundidade GetProfundidadeMáxima()
        {
            if (TipoProfundidade == TipoProfundidade.PM)
                return GetPmMáximo();

            if (TipoProfundidade == TipoProfundidade.PV)
                return GetPvMáximo();

            throw new ArgumentException("Pontos: GetProfundidadeMáxima: Tipo profundidade não reconhecido.");
        }

        public Profundidade GetPvMáximo()
        {
            return _pvSortedProfs.Count == 0 ? null : (Profundidade)_pvSortedProfs.GetKeyList()[_pvSortedProfs.Count - 1];
        }

        public Profundidade GetPmMáximo()
        {
            return _pmSortedPoints.Count == 0 ? null : (Profundidade)_pmSortedPoints.GetKeyList()[_pmSortedPoints.Count - 1];
        }

        public Profundidade GetPvMínimo()
        {
            return _pvSortedProfs.Count == 0 ? null : (Profundidade)_pvSortedProfs.GetKeyList()[0];
        }

        public Profundidade GetPmMínimo()
        {
            return _pmSortedPoints.Count == 0 ? null : (Profundidade)_pmSortedPoints.GetKeyList()[0];
        }

        #endregion

        #endregion

        #region Remoção

        public bool RemovePontoEmPm(Profundidade pm)
        {
            if (!RemovePontoInternal(pm, out var ponto)) 
                return false;

            //_itemsNotificator.ExecutarOnItemRemovido(ponto);
            return true;
        }

        private bool RemovePontoInternal(Profundidade profundidade, out T ponto)
        {
            if (_pmPointsCache.ContainsKey(profundidade))
            {
                ponto = _pmPointsCache[profundidade];
                _pmPointsCache.Remove(profundidade);
                return true;
            }

            ponto = default;
            return false;
        }

        public bool RemovePonto(T ponto)
        {
            return RemovePontoEmPm(ponto.Pm);
        }

        public void RemovePontos(IList<T> pontos)
        {
            var count = pontos.Count;
            if (count == 0) return;

            for (var i = 0; i < count; i++)
            {
                RemovePontoInternal(pontos[i].Pm, out T p);
            }

            //_itemsNotificator.ExecutarOnItensRemovidos(pontos);
        }

        public void RemovePontosEmPm(IList<Profundidade> pms)
        {
            var count = pms.Count;
            if (count == 0) return;
            var pontosRemovidos = new List<T>();

            for (var i = 0; i < count; i++)
            {
                RemovePontoInternal(pms[i], out T p);
                pontosRemovidos.Add(p);
            }

            //_itemsNotificator.ExecutarOnItensRemovidos(pontosRemovidos);
        }

        public int RemovePontosEmPm(Profundidade pmTopo, Profundidade pmBase)
        {
            int pontosRemovidos = 0;

            if (!ContémPontos())
                return pontosRemovidos;

            if (pmTopo > pmBase)
                throw new ArgumentException($"As profundidades topo: {pmTopo} e base: {pmBase} são inválidas. " +
                                            "A profundidade topo precisa ser sempre menor que a profundidade base, ou elas podem ser iguais.");

            var pontosNoTrecho = new List<T>();

            var gotTopoTrecho = TryGetPontoOrNextEmPm(pmTopo, out var pontoTopo);

            if (gotTopoTrecho && pontoTopo != null)
            {
                int topoIndex = _pmSortedPoints.IndexOfKey(pontoTopo.Pm);

                var pontos = _pmSortedPoints.GetValueList();
                for (int index = topoIndex; index < _pmSortedPoints.Count; index++)
                {
                    var pontoNoTrecho = (T)pontos[index];
                    var profundidade = pontoNoTrecho.Pm;

                    if (profundidade >= pmTopo && profundidade <= pmBase)
                    {
                        pontosNoTrecho.Add(pontoNoTrecho);
                    }
                    else if (profundidade > pmBase)
                    {
                        break;
                    }
                }

                if (pontosNoTrecho.Any())
                    RemovePontos(pontosNoTrecho);
            }

            return pontosNoTrecho.Count;
        }

        public void RemovePontosEmPv(Profundidade pv)
        {
            if (!TryGetPointsAtSamePv(pv, out var pvPoints))
                return;

            RemovePontos(pvPoints);
        }

        public void RemovePontosEmPv(IList<Profundidade> pvs)
        {
            if (pvs == null || pvs.Count == 0)
                return;

            for (int index = 0; index < pvs.Count; index++)
            {
                RemovePontosEmPv(pvs[index]);
            }
        }

        public void RemovePontosEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvTopo, Profundidade pvBase)
        {
            if (!TryGetPontosEmPv(conversorProfundidade, pvTopo, pvBase, out var pontos))
                return;

            RemovePontos(pontos);
        }

        public void Clear()
        {
            _pmPointsCache.Clear();
            _pmPointsDict.Clear();
            _pmSortedPoints.Clear();
            _pvPointsCache.Clear();
            _pvSortedProfs.Clear();
            //_itemsNotificator.ExecutarOnItensApagados();
        }

        private void ClearPvs()
        {
            _pvPointsCache.Clear();
            _pvSortedProfs.Clear();
        }

        private void OnItemRemoved(KeyValuePair<Profundidade, T> itemAdded)
        {
            ItemRemovedHandle(itemAdded.Value);
        }

        private bool ItemRemovedHandle(T ponto)
        {
            var profPrimária = ponto.Pm;
            if (!_pmPointsDict.ContainsKey(profPrimária))
                return false;

            _pmPointsDict.Remove(profPrimária);
            _pmSortedPoints.Remove(profPrimária);
            RemovePvPointInternal(ponto);
            return true;
        }

        private void RemovePvPointInternal(T ponto)
        {
            if (ponto.Pv == null)
                return;

            var profundidadeAtualPv = ponto.Pv;
            if (!_pvPointsCache.ContainsKey(profundidadeAtualPv))
                return;

            var profundidadeAtualPm = ponto.Pm;
            if (!_pvPointsCache[profundidadeAtualPv].ContainsKey(profundidadeAtualPm))
                return;

            _pvPointsCache[profundidadeAtualPv].Remove(profundidadeAtualPm);

            if (_pvPointsCache[profundidadeAtualPv].Count == 0)
            {
                _pvPointsCache.Remove(profundidadeAtualPv);
                _pvSortedProfs.Remove(profundidadeAtualPv);
            }
        }

        public bool EstáEmTrechoHorizontal(T ponto)
        {
            if(!_pvPointsCache.ContainsKey(ponto.Pv))
                return false;

            if (_pvPointsCache[ponto.Pv].Count == 1)
                return false;

            if (_conversorProfundidade.EstáEmTrechoHorizontal(ponto.Pm, ponto.Pv))
                return true;

            return false;
        }

        #endregion

        #region Edição

        /// <summary>
        /// Remove o ponto do pm e adiciona novo ponto.
        /// </summary>
        /// <param name="newPoint"></param>
        /// <returns></returns>
        public bool ReplacePoint(T newPoint)
        {
            if (!_pmPointsDict.TryGetValue(newPoint.Pm, out T pontoAtual))
                return false;

            if (!RemovePontoInternal(newPoint.Pm, out T p))
                return false;

            if (!AddPontoInternal(newPoint))
            {
                AddPontoInternal(p);
                return false;
            }

            //_itemsNotificator.ExecutarOnItemAlterado(pontoAtual, novoPonto);

            return true;
        }

        public void ReplacePontos(IList<T> newPoints)
        {
            Parallel.ForEach(newPoints, newPoint => { ReplacePoint(newPoint); });
        }

        /// <summary>
        /// Edita o valor do ponto no pm.
        /// Não faz nada caso não exista ponto no pm.
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool EditPontoEmPm(Profundidade pm, double newValue)
        {
            if (!_pmPointsDict.TryGetValue(pm, out T ponto))
                return false;

            ponto.Edit(newValue);

            //_itemsNotificator.ExecutarOnItemAlterado(pontoAtual, novoPonto);

            return true;
        }

        public bool EditPontoEmPm(Profundidade pm, string newTipoRocha)
        {
            if (!_pmPointsDict.TryGetValue(pm, out T ponto))
                return false;

            ponto.Edit(newTipoRocha);

            //_itemsNotificator.ExecutarOnItemAlterado(pontoAtual, novoPonto);

            return true;
        }

        public void EditPonto(T editingPoint, double newValue)
        {
            EditPontoEmPm(editingPoint.Pm, newValue);
        }

        public void EditPonto(T editingPoint, string newTipoRocha)
        {
            EditPontoEmPm(editingPoint.Pm, newTipoRocha);
        }

        public void EditPontos(IList<T> editingPoints, IList<double> newValues)
        {
            if (editingPoints.Count != newValues.Count)
                throw new ArgumentException("Edição de pontos de perfis com parâmetros inválidos.");

            Parallel.For(0, editingPoints.Count, index =>
            {
                EditPonto(editingPoints[index], newValues[index]);
            });
        }

        public void EditPontosEmPm(IList<Profundidade> pms, IList<double> newValues)
        {
            if (pms.Count != newValues.Count)
                throw new ArgumentException("Edição de pontos de perfis com parâmetros inválidos.");

            Parallel.For(0, pms.Count, index =>
            {
                EditPontoEmPm(pms[index], newValues[index]);
            });
        }

        public void EditPontosEmPm(IList<Profundidade> pms, IList<string> newTiposRocha)
        {
            if (pms.Count != newTiposRocha.Count)
                throw new ArgumentException("Edição de pontos de perfis com parâmetros inválidos.");

            Parallel.For(0, pms.Count, index =>
            {
                EditPontoEmPm(pms[index], newTiposRocha[index]);
            });
        }

        public void EditPontosEmPv(Profundidade pv, double newValue)
        {
            if(!TryGetPointsAtSamePv(pv, out IList<T> pointsSamePv))
                return;

            foreach (var ponto in pointsSamePv)
            {
                EditPonto(ponto, newValue);
            }
        }

        public void EditPontosEmPv(Profundidade pv, string newTipoRocha)
        {
            if (!TryGetPointsAtSamePv(pv, out IList<T> pointsSamePv))
                return;

            foreach (var ponto in pointsSamePv)
            {
                EditPonto(ponto, newTipoRocha);
            }
        }

        /// <summary>
        /// Sobrescreve os pontos no trecho estabelecido pelos argumentos.
        /// Condição de entrada: coleção de pontos a serem inseridos esteja ordenada. 
        /// </summary>
        /// <param name="pmTopo"></param>
        /// <param name="pmBase"></param>
        /// <param name="pontos"></param>
        /// <returns></returns>
        public bool SobrescreverEmPm(Profundidade pmTopo, Profundidade pmBase, List<T> pontos) //TODO(Gabriel Pinheiro) : Verificar corretude dessa mudança
        {
            if (pontos.Count <= 0)
                return false;

            RemovePontosEmPm(pmTopo, pmBase);

            // Manipulação de pontos deve estar encapsulada na classe
            BuscaBinariaPorPmMenorIgual(pontos, pmTopo, out var índiceTopoPontosRecebidos, out var _2);
            BuscaBinariaPorPmMenorIgual(pontos, pmBase, out var índiceBasePontosRecebidos, out var _3);

            if (índiceTopoPontosRecebidos < 0)
            {
                // Pontos já começam em profundidade inferior ao PM do topo
                índiceTopoPontosRecebidos = 0;
            }
            else if (índiceBasePontosRecebidos < 0)
            {
                // Pontos terminam em profundidade inferior ao PM da base
                índiceBasePontosRecebidos = pontos.Count - 1;
            }

            for (int index = índiceTopoPontosRecebidos; index < índiceBasePontosRecebidos; index++)
            {
                if (pontos[index].Pm >= pmTopo && pontos[index].Pm < pmBase)
                    AddPonto(pontos[index]);

                if (pontos[index].Pm >= pmBase)
                    break;
            }

            return true;
        }

        public void Shift(List<Tuple<Profundidade, Profundidade>> shifts)
        {
            // TODO:(RCM) Verificar compatibilidade de uso em pv ou pm
            T[] pontosShiftados = new T[_pmSortedPoints.Count];
            for (int index = 0; index < shifts.Count; index++)
            {
                var profundidadeOriginal = shifts[index].Item1;
                var profundidadeFinal = shifts[index].Item2;
                RemovePontoInternal(profundidadeOriginal, out T ponto);
                var delta = profundidadeFinal.Valor - profundidadeOriginal.Valor;
                ponto.Shift(delta);
                AddPontoInternal(ponto);
                pontosShiftados[index] = ponto;
            }

            // Notifica items shifts
            //_itemsNotificator.ExecutarOnItensAdicionados(pontosShiftados);
        }

        public void Shift(double delta, bool isGradiente)
        {
            T[] pontos = new T[_pmSortedPoints.Count];
            _pmSortedPoints.GetValueList().CopyTo(pontos, 0);
            T[] pontosShiftados = new T[_pmSortedPoints.Count];
            for (int index = 0; index < pontos.Length; index++)
            {
                var ponto = pontos[index];
                ponto.Shift(delta, isGradiente);
                pontosShiftados[index] = ponto;
            }

            Clear();
            AddPontos(pontosShiftados);
        }

        #endregion

        #region Verificação

        /// <summary>
        /// Verifica se a coleção contém ponto e uma determinada profundidade de medida
        /// </summary>
        /// <param name="pmProf">Profundidade de Medida</param>
        /// <returns>True se a coleção contém um ponto na profundidade, False caso contrário</returns>
        public bool ContémPontoNoPm(Profundidade pmProf)
        {
            return _pmPointsDict.ContainsKey(pmProf);
        }

        /// <summary>
        /// Verifica se a coleção contém ponto e uma determinada profundidade de medida. Usará a profundidade de medida do ponto para fazer a verificação
        /// </summary>
        /// <param name="ponto">Ponto</param>
        /// <returns>True se a coleção contém um ponto na profundidade, False caso contrário</returns>
        public bool ContémPonto(T ponto)
        {
            return _pmPointsDict.ContainsKey(ponto.Pm);
        }

        public bool EstáEmBuraco(Profundidade profundidade)
        {
            if (_pmPointsDict.ContainsKey(profundidade))
            {
                return false;
            }

            var pontosObtidos = TryGetPreviousAndNextPointEmPm(profundidade, out T pontoAnterior, out T pontoPosterior);

            if (pontosObtidos && (profundidade.Valor - pontoAnterior.Pm.Valor > _stepMínimoIdentificadorBuraco || pontoPosterior.Pm.Valor - profundidade.Valor > _stepMínimoIdentificadorBuraco))
                return true;

            return false;
        }

        public bool ContémPontos()
        {
            return _pmPointsDict.Keys.Any();
        }

        #endregion

        #region Conversão PM/PV

        public void ConverterParaPv( )
        {
            if (TipoProfundidade == TipoProfundidade.PV)
                return;

            ClearPvs();
            var sorted = _pmSortedPoints.GetValueList();
            Parallel.For(0, _pmSortedPoints.Count, (index, state) =>
            {
                var ponto = (T)sorted[index];

                if (ponto.Pm.Valor > _conversorProfundidade.ÚltimoPonto.Pm.Valor)
                    state.Break();

                if (ponto.Pv == null)
                {
                    ponto.ConverterParaPv();
                    AddPontoEmPvInternal(ponto);
                }
            });

            TipoProfundidade = TipoProfundidade.PV;
        }

        public void ConverterParaPm()
        {
            if (TipoProfundidade == TipoProfundidade.PM)
                return;

            var sorted = _pmSortedPoints.GetValueList();
            Parallel.For(0, _pmSortedPoints.Count, index =>
            {
                var ponto = (T) sorted[index];
                ponto.ConverterParaPM();
            });

            TipoProfundidade = TipoProfundidade.PM;
        }

        public void AtualizarPvs(IConversorProfundidade trajetória)
        {
            _conversorProfundidade = trajetória;
            ClearPvs();
            var sorted = _pmSortedPoints.GetValueList();
            Parallel.For(0, _pmSortedPoints.Count, (index, state) =>
            {
                var ponto = (T)sorted[index];

                if (ponto.Pm.Valor > _conversorProfundidade.ÚltimoPonto.Pm.Valor)
                    state.Break();

                ponto.ConverterParaPv();
                AddPontoEmPvInternal(ponto);
            });

            TipoProfundidade = TipoProfundidade.PV;
        }

        #endregion

        #region Auxiliar

        public bool BuscaBinariaPorPmMenorIgual(List<T> pontos, Profundidade pm, out int index, out bool equal)
        {
            index = -1;

            if (!pontos.Any() || pm.Valor > pontos[pontos.Count - 1].Pm.Valor || pm.Valor < pontos[0].Pm.Valor)
            {
                equal = false;
                return false;
            }

            var low = 0;
            var high = pontos.Count - 1;
            var mid = 0;

            while (low <= high)
            {
                mid = low + ((high - low) / 2);

                var pontoCorrente = pontos[mid];

                if (Math.Abs(pontos[mid].Pm.Valor - pm.Valor) < 0.001)
                {
                    index = mid; // key found
                    equal = true;
                    return true;
                }

                if (pontos[mid].Pm.Valor < pm.Valor)
                {
                    low = mid + 1;
                }
                else if (pontoCorrente.Pm.Valor > pm.Valor)
                {
                    high = mid - 1;
                }
            }

            if (pm.Valor > pontos[mid].Pm.Valor)
            {
                index = mid;
            }
            else // less
            {
                index = mid - 1;
            }

            equal = false;
            return index != -1;
        }

        #endregion

        #region IEquatable<Pontos<T>> Members

        public bool Equals(Pontos<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_pmPointsCache, other._pmPointsCache);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pontos<T>)obj);
        }

        public override int GetHashCode()
        {
            return (_pmPointsCache != null ? _pmPointsCache.GetHashCode() : 0);
        }

        public static bool operator ==(Pontos<T> left, Pontos<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Pontos<T> left, Pontos<T> right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IEqualityComparer<Pontos<T>> Members

        private sealed class PmPointsCacheEqualityComparer : IEqualityComparer<Pontos<T>>
        {
            public bool Equals(Pontos<T> x, Pontos<T> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x._pmPointsCache, y._pmPointsCache);
            }

            public int GetHashCode(Pontos<T> obj)
            {
                return (obj._pmPointsCache != null ? obj._pmPointsCache.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<Pontos<T>> PointsComparer { get; } = new PmPointsCacheEqualityComparer();

        #endregion

        #region Map

        public static void Map()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Profundidade)))
            {
                Profundidade.Map();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(p =>
                {
                    p.AutoMap();
                });

            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Pontos<Ponto>)))
            {
                BsonSerializer.RegisterSerializer(
                    typeof(Pontos<Ponto>),
                    new PontosSerializer()
                );
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Pontos<PontoLitologia>)))
            {
                BsonSerializer.RegisterSerializer(
                    typeof(Pontos<PontoLitologia>),
                    new PontosLitologiaSerializer()
                );
            }
        }

        protected internal void SetConversor(IConversorProfundidade conversor)
        {
            _conversorProfundidade = conversor;
        }

        protected internal IConversorProfundidade GetCoversor()
        {
            return _conversorProfundidade;
        }

        #endregion

        #endregion
    }
}
