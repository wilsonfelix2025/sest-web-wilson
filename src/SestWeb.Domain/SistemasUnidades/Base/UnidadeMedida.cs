using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;

namespace SestWeb.Domain.SistemasUnidades.Base
{
    public abstract class UnidadeMedida : IEquatable<UnidadeMedida>
    {
        private static readonly List<UnidadeMedida> TodasUnidades;

        static UnidadeMedida()
        {
            var pai = typeof(UnidadeMedida);
            var assembly = Assembly.GetExecutingAssembly();
            var tipos = assembly.GetTypes();

            var tiposSubclasses = tipos.Where(t => t.IsSubclassOf(pai));

            var classMap = new BsonClassMap(typeof(UnidadeMedida));
            classMap.AutoMap();

            TodasUnidades = new List<UnidadeMedida>();
            foreach (var item in tiposSubclasses)
            {
                var grupo = (UnidadeMedida)Activator.CreateInstance(item);
                TodasUnidades.Add(grupo);
                classMap.AddKnownType(item);
            }

            BsonClassMap.RegisterClassMap(classMap);
        }

        protected UnidadeMedida(string nome, string símbolo)
        {
            Nome = nome;
            Símbolo = símbolo;
        }

        public string Nome { get; private set; }
        public string Símbolo { get; private set; }

        public static UnidadeMedida ObterUnidade<T>() where T : UnidadeMedida
        {
            return TodasUnidades.Single(x => x.GetType() == typeof(T));
        }

        public static UnidadeMedida ObterPorSímbolo(string símbolo)
        {
            if (string.IsNullOrEmpty(símbolo))
                return null;

            return TodasUnidades.SingleOrDefault(x => x.Símbolo.ToLowerInvariant() == símbolo.ToLowerInvariant());
        }

        public static bool operator !=(UnidadeMedida unidadeMedida1, UnidadeMedida unidadeMedida2)
        {
            return !(unidadeMedida1 == unidadeMedida2);
        }

        public static bool operator ==(UnidadeMedida unidadeMedida1, UnidadeMedida unidadeMedida2)
        {
            return EqualityComparer<UnidadeMedida>.Default.Equals(unidadeMedida1, unidadeMedida2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnidadeMedida);
        }

        public bool Equals(UnidadeMedida other)
        {
            return other != null &&
                   Nome == other.Nome &&
                   Símbolo == other.Símbolo;
        }

        public override int GetHashCode()
        {
            var hashCode = 881997498;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Símbolo);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Nome} ({Símbolo})";
        }
    }
}