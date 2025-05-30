using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;

namespace SestWeb.Domain.Entities.LitologiaDoPoco
{
    public class GrupoLitologico : IEquatable<GrupoLitologico>
    {
        public static GrupoLitologico Carbonatos = new GrupoLitologico(nameof(Carbonatos), 0);
        public static GrupoLitologico Detríticas = new GrupoLitologico(nameof(Detríticas), 1);
        public static GrupoLitologico Arenitos = new GrupoLitologico(nameof(Arenitos), 2);

        public static GrupoLitologico Argilosas = new GrupoLitologico(nameof(Argilosas), 3);
        public static GrupoLitologico Ígneas = new GrupoLitologico(nameof(Ígneas), 4);
        public static GrupoLitologico Metamórficas = new GrupoLitologico(nameof(Metamórficas), 5);

        public static GrupoLitologico Evaporitos = new GrupoLitologico(nameof(Evaporitos), 6);
        public static GrupoLitologico Outros = new GrupoLitologico(nameof(Outros), 7);
        public static GrupoLitologico NãoIdentificado = new GrupoLitologico(nameof(NãoIdentificado), 8);

        public string Nome { get; private set; }
        public int Valor { get; private set; }

        [BsonConstructor]
        private GrupoLitologico(string nome, int valor)
        {
            Nome = nome;
            Valor = valor;
        }

        public static List<string> GetNames()
        {
            return typeof(GrupoLitologico)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(GrupoLitologico))
                .Select(f=>f.Name)
                .ToList();
        }

        public static GrupoLitologico GetFromName(string name)
        {
            try
            {
                return typeof(GrupoLitologico)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(GrupoLitologico) && f.Name == name.Trim())
                    .First()
                    .GetValue(null) as GrupoLitologico;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override string ToString() => $"{Nome} ({Valor})";

        public override bool Equals(object obj)
        {
            return Equals(obj as GrupoLitologico);
        }

        public bool Equals(GrupoLitologico other)
        {
            return other != null &&
                   Nome == other.Nome &&
                   Valor == other.Valor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nome, Valor);
        }

        public static bool operator ==(GrupoLitologico litologico1, GrupoLitologico litologico2)
        {
            return EqualityComparer<GrupoLitologico>.Default.Equals(litologico1, litologico2);
        }

        public static bool operator !=(GrupoLitologico litologico1, GrupoLitologico litologico2)
        {
            return !(litologico1 == litologico2);
        }
    }
}