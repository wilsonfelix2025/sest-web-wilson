using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace SestWeb.Domain.Entities.ProfundidadeEntity
{
    public class Profundidade : IEquatable<Profundidade>, IComparable<Profundidade>, IComparable
    {
        public Profundidade(double prof)
        {
            if (!double.IsNaN(prof))
                //Valor = (double)Math.Truncate((decimal)prof * 100) / 100;
                Valor = (double)Math.Truncate((decimal)prof * 10000000) / 10000000;
        }

        #region Propriedades

        public double Valor { get; private set; }

        #endregion

        #region IEquatable<Profundidade>

        public bool Equals(Profundidade other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Valor.Equals(other.Valor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Profundidade) obj);
        }

        public override int GetHashCode()
        {
            return Valor.GetHashCode();
        }

        public static bool operator ==(Profundidade left, Profundidade right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Profundidade left, Profundidade right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IEqualityComparer<Profundidade>

        private sealed class EqualityComparer : IEqualityComparer<Profundidade>
        {
            public bool Equals(Profundidade x, Profundidade y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Valor.Equals(y.Valor);
            }

            public int GetHashCode(Profundidade obj)
            {
                return obj.Valor.GetHashCode();
            }
        }

        public static IEqualityComparer<Profundidade> ProfundidadeComparer { get; } = new EqualityComparer();

        #endregion

        #region IComparable

        public int CompareTo(Profundidade other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Valor.CompareTo(other.Valor);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is Profundidade other ? CompareTo(other) : throw new ArgumentException($"Objeto deve ser do tipo {nameof(Profundidade)}");
        }

        public static bool operator <(Profundidade left, Profundidade right)
        {
            return Comparer<Profundidade>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(Profundidade left, Profundidade right)
        {
            return Comparer<Profundidade>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(Profundidade left, Profundidade right)
        {
            return Comparer<Profundidade>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(Profundidade left, Profundidade right)
        {
            return Comparer<Profundidade>.Default.Compare(left, right) >= 0;
        }

        #endregion

        public override string ToString()
        {
            return $"{Valor}";
        }

        #region Map()

        public static void Map()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Profundidade)))
            {
                BsonClassMap.RegisterClassMap<Profundidade>(profundidade =>
                {
                    profundidade.AutoMap();
                    profundidade.MapMember(prof => prof.Valor);
                });
            }
        }

        #endregion
    }

}
