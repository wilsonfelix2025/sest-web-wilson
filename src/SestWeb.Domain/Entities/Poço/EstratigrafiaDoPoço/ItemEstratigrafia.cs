using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço
{
    public class ItemEstratigrafia : IEquatable<ItemEstratigrafia>, IComparable<ItemEstratigrafia>, IComparable
    {
        #region Constructor

        public ItemEstratigrafia(Profundidade pm, Profundidade pv, string sigla, string descrição, string idade)
        {
            Pm = pm;
            Pv = pv;
            Sigla = sigla;
            Descrição = descrição;
            Idade = idade;
        }

        #endregion

        #region Properties

        public Profundidade Pm { get; private set; }

        public Profundidade Pv { get; private set; }

        public string Sigla { get; private set; }

        public string Descrição { get; private set; }

        public string Idade { get; private set; }

        #endregion

        #region Methods

        public void Shift(double delta)
        {
            Pm = new Profundidade(Pm.Valor + delta);
        }

        public void AtualizarPv(Trajetória trajetoria)
        {
            trajetoria.TryGetTVDFromMD(Pm.Valor, out double pv);
            Pv = new Profundidade(pv);
        }

        #region IEquatable<ItemEstratigrafia>

        public bool Equals(ItemEstratigrafia other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Pm, other.Pm) && Equals(Pv, other.Pv) && Sigla == other.Sigla && Descrição == other.Descrição && Idade == other.Idade;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemEstratigrafia)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Pm != null ? Pm.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Pv != null ? Pv.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Sigla != null ? Sigla.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Descrição != null ? Descrição.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Idade != null ? Idade.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ItemEstratigrafia left, ItemEstratigrafia right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ItemEstratigrafia left, ItemEstratigrafia right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IComparable<ItemEstratigrafia>, IComparable

        public int CompareTo(ItemEstratigrafia other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var pmComparison = Comparer<Profundidade>.Default.Compare(Pm, other.Pm);
            if (pmComparison != 0) return pmComparison;
            var pvComparison = Comparer<Profundidade>.Default.Compare(Pv, other.Pv);
            if (pvComparison != 0) return pvComparison;
            var siglaComparison = string.Compare(Sigla, other.Sigla, StringComparison.Ordinal);
            if (siglaComparison != 0) return siglaComparison;
            var descriçãoComparison = string.Compare(Descrição, other.Descrição, StringComparison.Ordinal);
            if (descriçãoComparison != 0) return descriçãoComparison;
            return string.Compare(Idade, other.Idade, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is ItemEstratigrafia other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ItemEstratigrafia)}");
        }

        public static bool operator <(ItemEstratigrafia left, ItemEstratigrafia right)
        {
            return Comparer<ItemEstratigrafia>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(ItemEstratigrafia left, ItemEstratigrafia right)
        {
            return Comparer<ItemEstratigrafia>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(ItemEstratigrafia left, ItemEstratigrafia right)
        {
            return Comparer<ItemEstratigrafia>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(ItemEstratigrafia left, ItemEstratigrafia right)
        {
            return Comparer<ItemEstratigrafia>.Default.Compare(left, right) >= 0;
        }

        #endregion

        #endregion

        #region Map

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ItemEstratigrafia)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Profundidade)))
            {
                Profundidade.Map();
            }

            BsonClassMap.RegisterClassMap<ItemEstratigrafia>(item =>
            {
                item.AutoMap();
                item.MapMember(i => i.Pm);
                item.MapMember(i => i.Pv);
                item.MapMember(i => i.Sigla);
                item.MapMember(i => i.Descrição);
                item.MapMember(i => i.Idade);

                item.SetIgnoreExtraElements(true);
                item.SetDiscriminator(nameof(ItemEstratigrafia));
            });
        }

        #endregion
    }
}