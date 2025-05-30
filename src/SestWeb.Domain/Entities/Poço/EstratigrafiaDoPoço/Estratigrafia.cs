using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço
{
    public class Estratigrafia : ISupportInitialize
    {
        [JsonProperty("Itens")] 
        private Dictionary<string, List<ItemEstratigrafia>> _itens;

        public Estratigrafia()
        {
            _itens =
                new Dictionary<string, List<ItemEstratigrafia>>();
        }

        /// <summary>
        /// Tenta obter um item de estratigrafia pela profundidade.
        /// </summary>
        /// <param name="tipo">Tipo de estratigrafia.</param>
        /// <param name="prof">Profundidade medida.</param>
        /// <param name="itemEstratigrafia">Item de estratigrafia.</param>
        /// <returns><c>True</c> se encontrou item com a profundidade informada; <c>false</c> caso contrário.</returns>
        public bool TryObterItemEstratigrafia(TipoEstratigrafia tipo, Profundidade prof, out ItemEstratigrafia itemEstratigrafia)
        {
            itemEstratigrafia = _itens[tipo.Tipo].Find(item => item.Pm == prof);
            return itemEstratigrafia != null;
        }

        /// <summary>
        /// Obtém todos os itens de estratigrafia.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, List<ItemEstratigrafia>> ObterItensEstratigrafia()
        {
            return _itens;
        }

        /// <summary>
        ///     Cria um item de estratigrafia.
        /// </summary>
        /// <param name="pmProf"></param>
        /// <param name="sigla">Sigla</param>
        /// <param name="descrição">Descrição</param>
        /// <param name="trajetória"></param>
        /// <param name="tipo">Tipo</param>
        /// <param name="idade">Idade</param>
        /// <returns><c>true</c> se o item foi criado com sucesso; caso contrário, <c>false</c>.</returns>
        public bool CriarItemEstratigrafia(Trajetória trajetória, string tipo, Profundidade pmProf, string sigla, string descrição, string idade)
        {
            //var tipoEstratigrafia = TipoEstratigrafia.ObterPeloTipo(tipo);

            trajetória.TryGetTVDFromMD(pmProf.Valor, out double pv);

            //Profundidade pvProf = new Profundidade((double)Math.Truncate((decimal)pv * 100) / 100);
            Profundidade pvProf = new Profundidade((double)Math.Truncate((decimal)pv * 10000000) / 10000000);

            var item = new ItemEstratigrafia(pmProf, pvProf, sigla, descrição, idade);

            if (!_itens.ContainsKey(tipo))
            {
                var lista = new List<ItemEstratigrafia> {item};
                _itens.Add(tipo, lista);
                return true;
            }

            if (_itens[tipo].Contains(item))
                return false;

            _itens[tipo].Add(item);
            
            return true;
        }

        /// <summary>
        /// Ordena, por tipo a lista de pontos da estratigrafia
        /// </summary>
        public void Reset()
        {      
            _itens = _itens
                    .ToDictionary(
                        d => d.Key,
                        d => d.Value.OrderBy(v => v.Pm).ToList()
                    );
        }

        /// <summary>
        ///     Remove um item de estratigrafia.
        /// </summary>
        /// <param name="tipo">Tipo de estratigrafia</param>
        /// <param name="pm">Profundidade</param>
        /// <returns>
        ///     <c>true</c> se o item foi removido com sucesso; caso contrário, <c>false</c>.
        ///     Este método também retorna <c>false</c> se a profundidade não foi encontrada.
        /// </returns>
        public bool RemoverItemEstratigrafia(string tipo, Profundidade pm)
        {
            if (tipo == null || pm == null || !_itens.ContainsKey(tipo))
                return false;

            var remove = _itens[tipo].RemoveAll(item => item.Pm == pm);
            if (_itens[tipo].Count == 0)
            {
                _itens.Remove(tipo);
            }

            return remove != 0;
        }

        /// <summary>
        ///     Remove todos os itens de estratigrafia.
        /// </summary>
        public void ApagarEstratigrafia()
        {
            _itens.Clear();
        }

        public void TratarOverLappingDoMesmoTipo()
        {
            var itensToDelete = new Dictionary<string, List<ItemEstratigrafia>>();

            foreach (var tipo in _itens)
            {
                var pontoAnterior = 0.0;
                var primeiroPonto = true;

                foreach (var ponto in tipo.Value)
                {
                    if (primeiroPonto == false && ponto.Pm.Valor <= pontoAnterior)
                    {
                        if (!itensToDelete.ContainsKey(tipo.Key))
                        {
                            var lista = new List<ItemEstratigrafia> { };
                            itensToDelete.Add(tipo.Key, lista);
                        }

                        itensToDelete[tipo.Key].Add(ponto);
                    }

                    primeiroPonto = false;
                    pontoAnterior = ponto.Pm.Valor;
                }
            }

            foreach (var tipo in itensToDelete)
            {
                foreach (var ponto in tipo.Value)
                {
                    RemoverItemEstratigrafia(tipo.Key, ponto.Pm);
                }
            }
        }

        public void AtualizarEstratigrafiaComBaseSedimentos(Trajetória trajetória, double baseSedimentos)
        {
            var itensToDelete = new Dictionary<string, List<ItemEstratigrafia>>();

            foreach (var tipo in _itens)
            {

                var pontosNaLamina = tipo.Value.Where(l => l.Pm.Valor < baseSedimentos).ToList();

                if (pontosNaLamina.Count() > 0)
                {
                    var ponto = pontosNaLamina.Last();

                    CriarItemEstratigrafia(trajetória, tipo.Key, new Profundidade(baseSedimentos), ponto.Sigla,ponto.Descrição,ponto.Idade);

                    foreach (var pontoNaLda in pontosNaLamina)
                    {
                        if (!itensToDelete.ContainsKey(tipo.Key))
                        {
                            var lista = new List<ItemEstratigrafia> { };
                            itensToDelete.Add(tipo.Key, lista);
                        }

                        itensToDelete[tipo.Key].Add(pontoNaLda);

                    }
                }

            }

            Reset();

            foreach (var tipo in itensToDelete)
            {
                foreach (var ponto in tipo.Value)
                {
                    RemoverItemEstratigrafia(tipo.Key, ponto.Pm);
                }
            }
        }

        public void AtualizarPvs(Trajetória trajetória)
        {
            Parallel.ForEach(_itens, estratigrafia =>
            {
                foreach (var itemEstratigrafia in estratigrafia.Value)
                {
                    itemEstratigrafia.AtualizarPv(trajetória);
                }
            });
        }

        public void Shift(double delta)
        {
            Parallel.ForEach(_itens, estratigrafia =>
            {
                foreach (var itemEstratigrafia in estratigrafia.Value)
                {
                    itemEstratigrafia.Shift(delta);
                }
            });
        }

        #region Map

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Estratigrafia)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(ItemEstratigrafia)))
            {
                ItemEstratigrafia.Map();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TipoEstratigrafia)))
            {
                TipoEstratigrafia.Map();
            }

            BsonClassMap.RegisterClassMap<Estratigrafia>(estratigrafia =>
            {
                estratigrafia.AutoMap();
                estratigrafia.MapCreator(p => new Estratigrafia());
                estratigrafia.MapMember(p => p._itens).SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<string, List<ItemEstratigrafia>>>(DictionaryRepresentation.ArrayOfDocuments));
                estratigrafia.SetIgnoreExtraElements(true);
                estratigrafia.SetDiscriminator(nameof(Estratigrafia));
            });
        }

        public void BeginInit()
        {
            int a = 1;
        }

        public void EndInit()
        {
            int b = 2;
        }

        #endregion
    }
}