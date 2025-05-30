using System.Collections;
using System.Collections.Generic;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.PontosEntity.InternalCollections
{
    internal class ConcurrentSortedListWrapper<Tkey, T>
    {
        private readonly SortedList _inner;

        public ConcurrentSortedListWrapper()
        {
            var comparer = Comparer<Profundidade>.Create(
                (k1, k2) => k1.Valor.CompareTo(k2.Valor));
            _inner = new SortedList(comparer);


        }

        public T this[Tkey key] => (T)_inner[key];

        public ICollection Keys => _inner.Keys;

        public IList Values => (IList)_inner.Values;

        public int Count => _inner.Count;

        public void Add(Tkey key, T item)
        {
            lock (_inner)
            {
                _inner.Add(key, item);
            }
        }

        public T GetByIndex(int index) => (T)_inner.GetByIndex(index);

        public void RemoveAt(int index)
        {
            lock (_inner)
            {
                _inner.RemoveAt(index);
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

        public bool Contains(T item) => _inner.Contains(item);

        public bool ContainsKey(Tkey key) => _inner.ContainsKey(key);

        public bool ContainsValue(T value) => _inner.ContainsValue(value);

        public IDictionaryEnumerator GetEnumerator() => _inner.GetEnumerator();

        public override int GetHashCode() => _inner.GetHashCode();

        /// <summary>
        /// Retorna o index do elemento na coleção.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// Retorna o index caso ache o elemento.
        /// Retorna -1 caso não ache o elemento.
        /// </returns>
        public int IndexOfKey(Tkey key) => _inner.IndexOfKey(key);

        public int IndexOfValue(T value) => _inner.IndexOfValue(value);

        public IList GetValueList() => _inner.GetValueList();

        public IList GetKeyList() => _inner.GetKeyList();

        public void SetByIndex(int index, T value)
        {
            lock (_inner)
            {
                _inner.SetByIndex(index, value);
            }
        }
    }
}
