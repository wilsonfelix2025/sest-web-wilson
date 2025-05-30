using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SestWeb.Domain.Entities.Correlações.AutorCorrelação
{
    public class Autor : IAutor, IEquatable<Autor>
    {
        private Autor(string nome, string chave, DateTime dataCriação)
        {
            Nome = nome;
            Chave = chave;
            DataCriação = dataCriação;
        }

        #region Properties

        public string Nome { get; }

        public string Chave { get; }

        public DateTime DataCriação { get; }

        #endregion

        #region Methods

        #region Factory Members

        public static IAutorFactory GetFactory()
        {
            return new AutorFactory((nome, chave, dataCriação) => new Autor(nome, chave, dataCriação));
        }

        #endregion

        #region Map Members

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Autor)))
                return;

            BsonClassMap.RegisterClassMap<Autor>(autor =>
            {
                autor.AutoMap();
                autor.MapCreator(a => new Autor(a.Nome, a.Chave, a.DataCriação));
                autor.MapProperty(a => a.Nome);
                autor.MapProperty(a => a.Chave);
                autor.MapProperty(c => c.DataCriação).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                autor.SetIgnoreExtraElements(true);
                autor.SetDiscriminator("Autor");
            });
        }

        #endregion

        #region IEquatable Members

        public bool Equals(Autor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Nome == other.Nome && Chave == other.Chave;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Nome != null ? Nome.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Chave != null ? Chave.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Autor) obj);
        }

        public static bool operator ==(Autor left, Autor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Autor left, Autor right)
        {
            return !Equals(left, right);
        }

        private sealed class AutorEqualityComparer : IEqualityComparer<Autor>
        {
            public bool Equals(Autor x, Autor y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Nome == y.Nome && x.Chave == y.Chave;
            }

            public int GetHashCode(Autor obj)
            {
                unchecked
                {
                    var hashCode = (obj.Nome != null ? obj.Nome.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Chave != null ? obj.Chave.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<Autor> AutorComparer { get; } = new AutorEqualityComparer();

        #endregion

        #endregion
    }
}
