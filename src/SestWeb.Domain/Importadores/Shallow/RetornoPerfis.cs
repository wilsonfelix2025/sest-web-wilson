using System;

namespace SestWeb.Domain.Importadores.Shallow
{
    public struct RetornoPerfis : IEquatable<RetornoPerfis>
    {
        public RetornoPerfis(string nome, string tipo, string unidade)
        {
            Nome = nome;
            Tipo = tipo;
            Unidade = unidade;
        }

        public string Nome { get; }
        public string Tipo { get; }
        public string Unidade { get; }

        public override bool Equals(object obj)
        {
            return obj is RetornoPerfis other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nome, Tipo, Unidade);
        }

        public bool Equals(RetornoPerfis other)
        {
            return Nome == other.Nome && Tipo == other.Tipo && Unidade == other.Unidade;
        }

        public static bool operator ==(RetornoPerfis left, RetornoPerfis right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RetornoPerfis left, RetornoPerfis right)
        {
            return !(left == right);
        }
    }
}