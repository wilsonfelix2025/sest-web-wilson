using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public class RegistroEvento : IRegistroEvento, ISupportInitialize
    {

        public RegistroEvento(string nome, TipoRegistroEvento tipo, double valorPadrão)
        {
            Nome = nome;
            Tipo = tipo;
            ValorPadrão = valorPadrão;

            EstiloVisual = new EstiloVisual();
            _trechos = new List<TrechoRegistroEvento>();
            if (Id == ObjectId.Empty)
            {
                Id = ObjectId.GenerateNewId();
            }
        }

        public RegistroEvento(string nome, TipoRegistroEvento tipo)
        {
            Nome = nome;
            Tipo = tipo;

            EstiloVisual = new EstiloVisual();
            _trechos = new List<TrechoRegistroEvento>();
            if (Id == ObjectId.Empty)
            {
                Id = ObjectId.GenerateNewId();
            }
        }

        #region Properties
        public ObjectId Id { get; private set; }
        public string IdPoço { get; set; }
        public TipoRegistroEvento Tipo { get; set; }

        public string Nome { get; private protected set; }

        public GrupoUnidades GrupoDeUnidades { get; private protected set; }

        public string Unidade { get; private protected set; }
        public object ValorPadrão { get; private protected set; }

        #region Visual Properties

        public EstiloVisual EstiloVisual { get; private protected set; }

        #endregion

        #endregion
        private List<TrechoRegistroEvento> _trechos;

        public IReadOnlyList<TrechoRegistroEvento> Trechos => GetTrechos();

        public IReadOnlyList<TrechoRegistroEvento> GetTrechos()
        {
            return _trechos.ToImmutableList();
        }

        #region Methods

        public void ResetTrechos()
        {
            _trechos = new List<TrechoRegistroEvento>();
        }

        public void AddTrecho(double pmTopo, double pmBase, double pvTopo, double pvBase, string comentário, IConversorProfundidade conversorProfundidade)
        {
            if (pmBase == null && pmTopo != null)
            {
                pmBase = pmTopo;
            }
            if (pvBase == null && pvTopo != null)
            {
                pvBase = pvTopo;
            }
            PontoRegistroEvento pTopo = new PontoRegistroEvento(pmTopo, pvTopo, conversorProfundidade);
            PontoRegistroEvento pBase = new PontoRegistroEvento(pmBase, pvBase, conversorProfundidade);

            AddTrecho(pTopo, pBase, comentário, conversorProfundidade);
        }

        public void AddTrecho(double pm, double pv, double valor, string comentário, IConversorProfundidade conversorProfundidade)
        {
            PontoRegistroEvento p = new PontoRegistroEvento(pm, pv, conversorProfundidade);

            AddTrecho(p, valor, comentário, conversorProfundidade);
        }
        public void SetEstiloVisual(string marcador, string corDoMarcador, string contornoDoMarcador)
        {
            EstiloVisual = new EstiloVisual(marcador, corDoMarcador, contornoDoMarcador);
        }
        public void SetValorPadrão(double valorPadrão)
        {
            ValorPadrão = valorPadrão;
        }
        public void SetUnidade(string unidade)
        {
            Unidade = unidade;
        }

        public void AddTrecho(PontoRegistroEvento p, double valor, string comentário, IConversorProfundidade conversorProfundidade)
        {
            TrechoRegistroEvento trecho = new TrechoRegistroEvento(p, valor, comentário, conversorProfundidade);

            _trechos.Add(trecho);
        }

        public void AddTrecho(PontoRegistroEvento pTopo, PontoRegistroEvento pBase, string comentário, IConversorProfundidade conversorProfundidade)
        {
            if (pTopo.Pm > pBase.Pm)
            {
                throw new InvalidOperationException("Topo deve ser menor ou igual a base");
            }

            TrechoRegistroEvento trecho = new TrechoRegistroEvento(pTopo, pBase, comentário, conversorProfundidade);

            _trechos.Add(trecho);
        }

        #region Convertion Methods

        public void AtualizarPvs(IConversorProfundidade conversor)
        {
            Parallel.ForEach(_trechos, trecho => trecho.AtualizarPV(conversor));
        }

        public void ConverterParaPv()
        {
            Parallel.ForEach(_trechos, trecho => trecho.ConverterParaPv());
        }

        public void ConverterParaPm()
        {
            Parallel.ForEach(_trechos, trecho => trecho.ConverterParaPM());
        }

        public void Shift(double delta)
        {
            Parallel.ForEach(_trechos, trecho => trecho.Shift(delta));
        }

        #endregion

        public override string ToString()
        {
            return $"{Nome} - {_trechos.Count} trechos";
        }

        #endregion

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(RegistroEvento)))
                return;


            if (!BsonClassMap.IsClassMapRegistered(typeof(TrechoRegistroEvento)))
            {
                TrechoRegistroEvento.Map();
            }

            BsonClassMap.RegisterClassMap<RegistroEvento>(registroEvento =>
            {
                registroEvento.AutoMap();
                registroEvento.MapMember(el => el.Id);
                registroEvento.SetIdMember(registroEvento.GetMemberMap(el => el.Id));
                registroEvento.MapMember(el => el.IdPoço);
                registroEvento.MapMember(el => el.Nome);
                registroEvento.UnmapMember(el => el.GrupoDeUnidades);
                registroEvento.MapMember(el => el.Unidade);
                registroEvento.MapMember(el => el.EstiloVisual);
                registroEvento.MapMember(el => el.Tipo).SetSerializer(new EnumSerializer<TipoRegistroEvento>(BsonType.String));
                registroEvento.MapMember(el => el._trechos);

                registroEvento.SetIsRootClass(true);
                registroEvento.SetIgnoreExtraElements(true);
                registroEvento.SetDiscriminator(nameof(RegistroEvento));
            });
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

    }
}
