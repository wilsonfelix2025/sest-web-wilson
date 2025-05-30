using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using MongoDB.Bson.Serialization;

namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public class TrechoRegistroEvento : IEquatable<TrechoRegistroEvento>, ITrechoRegistroEvento
    {
        private readonly IConversorProfundidade _conversorProfundidade;

        #region Properties
        public PontoRegistroEvento Ponto { get; private protected set; }
        public PontoRegistroEvento Topo { get; private protected set; }
        public PontoRegistroEvento Base { get; private protected set; }
        public object Valor { get; private protected set; }
        public string Comentário { get; private protected set; }
        #endregion

        public TrechoRegistroEvento(PontoRegistroEvento pTopo, PontoRegistroEvento pBase, string comentário, IConversorProfundidade conversorProfundidade)
        {
            _conversorProfundidade = conversorProfundidade;

            Topo = pTopo;
            Base = pBase;

            Comentário = comentário;
        }

        public TrechoRegistroEvento(PontoRegistroEvento p, double valor, string comentário, IConversorProfundidade conversorProfundidade)
        {
            _conversorProfundidade = conversorProfundidade;

            Ponto = p;

            Valor = valor;
            Comentário = comentário;
        }

        #region Methods

        public override string ToString() => $"{Topo} - {Base}";

        public override bool Equals(object obj)
        {
            return Equals(obj as TrechoRegistroEvento);
        }

        public bool Equals(TrechoRegistroEvento other)
        {
            return other != null &&
                   Topo == other.Topo &&
                   Base == other.Base;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Topo, Base);
        }

        public static bool operator ==(TrechoRegistroEvento trecho1, TrechoRegistroEvento trecho2)
        {
            return EqualityComparer<TrechoRegistroEvento>.Default.Equals(trecho1, trecho2);
        }

        public static bool operator !=(TrechoRegistroEvento trecho1, TrechoRegistroEvento trecho2)
        {
            return !(trecho1 == trecho2);
        }

        public void Edit(double novoValor)
        {
            Valor = novoValor;
        }

        public void AtualizarPV(IConversorProfundidade conversor)
        {
            Topo.AtualizarPV(conversor);
            Base.AtualizarPV(conversor);
        }

        public void ConverterParaPv()
        {
            Topo.ConverterParaPv();
            Base.ConverterParaPv();
        }

        public void ConverterParaPM()
        {
            Topo.ConverterParaPM();
            Base.ConverterParaPM();
        }

        public void Shift(double delta)
        {
            Topo.Shift(delta);
            Base.Shift(delta);
        }

        #region Map()

        public static void Map()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(PontoRegistroEvento)))
            {
                PontoRegistroEvento.Map();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TrechoRegistroEvento)))
            {
                BsonClassMap.RegisterClassMap<TrechoRegistroEvento>(trecho =>
                {
                    trecho.AutoMap();
                    trecho.MapMember(t => t.Ponto);
                    trecho.MapMember(t => t.Topo);
                    trecho.MapMember(t => t.Base);
                    trecho.MapMember(t => t.Valor);
                    trecho.MapMember(t => t.Comentário);
                    trecho.UnmapMember(t => t._conversorProfundidade);
                });
            }
        }

        #endregion

        #endregion
    }
}