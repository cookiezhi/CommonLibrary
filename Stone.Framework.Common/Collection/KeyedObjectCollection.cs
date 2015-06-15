using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Stone.Framework.Common.Collection
{
    public class KeyedObjectCollection<TKey, TItem> : IKeyedObjectCollection<TKey, TItem> where TItem : IKeyedObject<TKey>
    {
        private readonly Dictionary<TKey, TItem> _entriesTable;
        private readonly List<TItem> _entries;
        private readonly IComparer _comparer;

        public KeyedObjectCollection(IEqualityComparer<TKey> equalityComaprer)
        {
            if (equalityComaprer == null)
            {
                if (typeof(TKey) == typeof(string))
                {
                    _entriesTable =
                        new Dictionary<TKey, TItem>(
                            new CaseInsensitiveStringEqualityComparer() as IEqualityComparer<TKey>);
                }
                else
                {
                    _entriesTable = new Dictionary<TKey, TItem>();
                }
            }
            else
            {
                _entriesTable = new Dictionary<TKey, TItem>(equalityComaprer);
            }

            _entries = new List<TItem>();
            _comparer = new CaseInsensitiveComparer(CultureInfo.InvariantCulture);
        }

        public TItem this[int index]
        {
            get { return (TItem)_entries[index]; }
        }

        public TItem this[TKey key]
        {
            get { return GetItemByKey(key); }
        }

        public TItem GetItemByKey(TKey key)
        {
            TItem val;
            _entriesTable.TryGetValue(key, out val);
            return val;
        }

        public bool Contains(TKey key)
        {
            return _entriesTable.ContainsKey(key);
        }

        #region ICollection<T> Members

        public void Add(TItem item)
        {
            _entriesTable.Add(item.Key, item);
            _entries.Add(item);
        }

        public void Clear()
        {
            _entriesTable.Clear();
            _entries.Clear();
        }

        public bool Contains(TItem item)
        {
            return _entriesTable.ContainsKey(item.Key);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _entriesTable.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _entriesTable.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(TItem item)
        {
            bool removed;
            try
            {
                _entriesTable.Remove(item.Key);

                for (var i = 0; i < _entries.Count - 1; i++)
                {
                    var entry = _entries[i];
                    if (_comparer.Compare(item.Key, entry.Key) == 0)
                    {
                        _entries.RemoveAt(i);
                    }
                }

                removed = true;
            }
            catch (Exception)
            {
                removed = false;
            }

            return removed;
        }

        #endregion ICollection<T> Members

        #region IEnumerable<T> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        #endregion IEnumerable<T> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}