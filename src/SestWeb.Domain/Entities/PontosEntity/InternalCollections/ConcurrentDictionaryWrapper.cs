using System.Collections.Generic;

namespace SestWeb.Domain.Entities.PontosEntity.InternalCollections
{
    internal class ConcurrentDictionaryWrapper<Tkey, T>
    {
        private readonly Dictionary<Tkey, T> _inner;

        public ConcurrentDictionaryWrapper()
        {
            _inner = new Dictionary<Tkey, T>();
        }

        public ConcurrentDictionaryWrapper(IDictionary<Tkey, T> dictionary)
        {
            _inner = (Dictionary<Tkey, T>)dictionary;
        }

        public T this[Tkey key] => _inner[key];

        public int Count => _inner.Count;

        public Dictionary<Tkey, T>.KeyCollection Keys => _inner.Keys;

        public Dictionary<Tkey, T>.ValueCollection Values => _inner.Values;

        public void Add(Tkey key, T item)
        {
            lock (_inner)
            {
                _inner.Add(key, item);
            }
        }

        public void Remove(Tkey key)
        {
            lock (_inner)
            {
                _inner.Remove(key);
            }
        }

        public void Clear()
        {
            lock (_inner)
            {
                _inner.Clear();
            }
        }

        public bool ContainsKey(Tkey key) => _inner.ContainsKey(key);

        public bool ContainsValue(T value) => _inner.ContainsValue(value);

        public bool TryGetValue(Tkey key, out T value) => _inner.TryGetValue(key, out value);

        public Dictionary<Tkey, T>.Enumerator GetEnumerator() => _inner.GetEnumerator();

        public override int GetHashCode() => _inner.GetHashCode();

        public Dictionary<Tkey, T> ToDictionary() => _inner;
    }
}
