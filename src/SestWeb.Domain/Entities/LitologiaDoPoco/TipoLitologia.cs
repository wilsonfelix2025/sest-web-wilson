using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace SestWeb.Domain.Entities.LitologiaDoPoco
{
    public class TipoLitologia : IEquatable<TipoLitologia>
    {
        private static List<TipoLitologia> All { get; } = new List<TipoLitologia>();

        public static TipoLitologia Adaptada { get; } = new TipoLitologia("Adaptada", 0);
        public static TipoLitologia Prevista { get; } = new TipoLitologia("Prevista", 1);
        public static TipoLitologia Interpretada { get; } = new TipoLitologia("Interpretada", 2);

        [BsonConstructor]
        private TipoLitologia(string nome, int valor)
        {
            Nome = nome;
            Valor = valor;

            if (All.All(x => x != this))
            {
                All.Add(this);
            }
        }

        public static IReadOnlyList<TipoLitologia> List()
        {
            return All.AsReadOnly();
        }

        public string Nome { get; private set; }
        public int Valor { get; private set; }

        public static TipoLitologia FromNome(string nome)
        {
            return All.SingleOrDefault(r => string.Equals(r.Nome, nome, StringComparison.OrdinalIgnoreCase));
        }

        public static TipoLitologia FromValor(int valor)
        {
            return All.SingleOrDefault(r => r.Valor == valor);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TipoLitologia);
        }

        public bool Equals(TipoLitologia other)
        {
            return other != null &&
                   Nome == other.Nome &&
                   Valor == other.Valor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nome, Valor);
        }

        public static bool operator ==(TipoLitologia litologia1, TipoLitologia litologia2)
        {
            return EqualityComparer<TipoLitologia>.Default.Equals(litologia1, litologia2);
        }

        public static bool operator !=(TipoLitologia litologia1, TipoLitologia litologia2)
        {
            return !(litologia1 == litologia2);
        }
    }
}