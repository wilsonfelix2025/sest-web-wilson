using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Perfis
{
    public class GrupoPerfis : IEquatable<GrupoPerfis>
    {
        private static List<GrupoPerfis> All { get; set; } = new List<GrupoPerfis>();

        private static readonly Dictionary<string, GrupoPerfis> _gruposPorTipoPerfil = new Dictionary<string, GrupoPerfis>();

        public static GrupoPerfis Perfis { get; private set; } = new GrupoPerfis("Perfis", 0);
        public static GrupoPerfis PropriedadesMecanicas { get; private set; } = new GrupoPerfis("Propriedades Mecânicas", 1);
        public static GrupoPerfis Tensões { get; private set; } = new GrupoPerfis("Tensões/Pressões", 2);
        public static GrupoPerfis Gradientes { get; private set; } = new GrupoPerfis("Gradientes", 3);

        [BsonConstructor]
        private GrupoPerfis(string nome, int valor)
        {
            Nome = nome;
            Valor = valor;

            if (All.All(x => x != this))
            {
                All.Add(this);
            }
        }

        public IReadOnlyList<GrupoPerfis> List()
        {
            return All.AsReadOnly();
        }

        public string Nome { get; private set; }

        public int Valor { get; private set; }

        public static GrupoPerfis FromNome(string nome)
        {
            return All.Single(r => String.Equals(r.Nome, nome, StringComparison.OrdinalIgnoreCase));
        }

        public static GrupoPerfis FromValor(int valor)
        {
            return All.Single(r => r.Valor == valor);
        }

        public static void RegisterTipoPerfil(TipoPerfil tipoPerfil, GrupoPerfis grupoPerfis)
        {
            if (!_gruposPorTipoPerfil.ContainsKey(tipoPerfil.Mnemônico))
                _gruposPorTipoPerfil.Add(tipoPerfil.Mnemônico, grupoPerfis);
        }

        public static GrupoPerfis GetGrupoPerfis(string mnemônico)
        {
            return _gruposPorTipoPerfil[mnemônico];
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GrupoPerfis);
        }

        public bool Equals(GrupoPerfis other)
        {
            return other != null &&
                   Nome == other.Nome &&
                   Valor == other.Valor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nome, Valor);
        }

        public static bool operator ==(GrupoPerfis perfis1, GrupoPerfis perfis2)
        {
            return EqualityComparer<GrupoPerfis>.Default.Equals(perfis1, perfis2);
        }

        public static bool operator !=(GrupoPerfis perfis1, GrupoPerfis perfis2)
        {
            return !(perfis1 == perfis2);
        }
    }
}