using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;

namespace SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço
{
    public class TipoEstratigrafia : IEquatable<TipoEstratigrafia>
    {
        private TipoEstratigrafia(int valor, string nome, string tipo)
        {
            Valor = valor;
            Nome = nome;
            Tipo = tipo;

            if (Tipos.All(x => x != this)) Tipos.Add(this);
        }

        private static List<TipoEstratigrafia> Tipos { get; } = new List<TipoEstratigrafia>();

        public static TipoEstratigrafia Cronoestratigráfica { get; } =
            new TipoEstratigrafia(0, "Cronoestratigráfica", "CR");
        public static TipoEstratigrafia Formação { get; } = new TipoEstratigrafia(1, "Formação", "FM");
        public static TipoEstratigrafia Membro { get; } = new TipoEstratigrafia(2, "Membro", "MB");
        public static TipoEstratigrafia Camada { get; } = new TipoEstratigrafia(3, "Camada", "CM");
        public static TipoEstratigrafia Grupo { get; } = new TipoEstratigrafia(4, "Grupo", "GR");
        public static TipoEstratigrafia Marco { get; } = new TipoEstratigrafia(5, "Marco", "MC");
        public static TipoEstratigrafia Indefinido { get; } = new TipoEstratigrafia(6, "Indefinido", "IN");
        public static TipoEstratigrafia Outros { get; } = new TipoEstratigrafia(7, "Outros", "OU");

        public int Valor { get; private set; }

        public string Nome { get; private set; }

        public string Tipo { get; private set; }

        public bool Equals(TipoEstratigrafia other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Nome == other.Nome && Tipo == other.Tipo && Valor == other.Valor;
        }

        public static IReadOnlyList<TipoEstratigrafia> ListarTipos()
        {
            return Tipos.AsReadOnly();
        }

        /// <summary>
        /// Obtém objeto do tipo TipoEstratigrafia.
        /// </summary>
        /// <param name="tipo">Tipo do item de estratigrafia.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="tipo" /> é nulo.</exception>
        /// <exception cref="InvalidOperationException">Lançado se não encontra TipoEstratigrafia a partir do tipo informado.</exception>
        public static TipoEstratigrafia ObterPeloTipo(string tipo)
        {
            if(tipo == null)
                throw new ArgumentNullException(nameof(tipo), "Tipo não pode ser nulo.");

            return Tipos.Find(t => string.Equals(t.Tipo, tipo, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException("TipoEstratigrafia - Tipo inválido.");
        }

        public static string ObterStringPeloTipo(string tipo)
        {
            switch (tipo)
            {
                case "Cronoestratigráfica":
                case "cr":
                case "CR":
                    return "CR";
                case "Formação":
                case "F":
                case "f":
                case "FM":
                case "fm":
                    return "FM";
                case "M":
                case "m":
                case "Membro":
                case "MB":
                case "mb":
                    return "MB";
                case "C":
                case "c":
                case "Camada":
                case "CM":
                case "cm":
                    return "CM";
                case "IN":
                case "in":
                case "Indefinido":
                case "ND":
                case "nd":
                    return "IN";
                case "G":
                case "g":
                case "Grupo":
                case "GR":
                case "gr":
                    return "GR";
                case "Marco":
                case "MC":
                case "mc":
                    return "MC";
                default:
                    return "OU";
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TipoEstratigrafia) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Nome != null ? Nome.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Tipo != null ? Tipo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Valor;
                return hashCode;
            }
        }

        public static bool operator ==(TipoEstratigrafia left, TipoEstratigrafia right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TipoEstratigrafia left, TipoEstratigrafia right)
        {
            return !Equals(left, right);
        }

        // Necessário para a serialização para o front
        public override string ToString()
        {
            return Tipo;
        }

        #region Map

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(TipoEstratigrafia)))
                return;

            BsonClassMap.RegisterClassMap<TipoEstratigrafia>(tipo =>
            {
                tipo.AutoMap();
                tipo.MapMember(t => t.Valor);
                tipo.MapMember(t => t.Nome);
                tipo.MapMember(t => t.Tipo);
                tipo.SetIgnoreExtraElements(true);
                tipo.SetDiscriminator(nameof(TipoEstratigrafia));
            });
        }

        #endregion
    }
}