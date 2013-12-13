using System;
using System.Collections;
using System.Linq;

namespace FluentObjects
{
    public partial class FluentObject : IDictionary
    {
        private readonly object _syncRoot = new object();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count { get { return _dictionaryElements.Count; } }
        
        public object SyncRoot { get { return _syncRoot; } }

        public bool IsSynchronized { get { return false; } }

        public bool Contains(object key)
        {
            return _dictionaryElements.ContainsKey(key);
        }

        public void Add(object key, object value)
        {
            _dictionaryElements.Add(key, value);
        }

        public void Clear()
        {
            _dictionaryElements.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<object, dynamic>(_dictionaryElements);
        }

        public void Remove(object key)
        {
            _dictionaryElements.Remove(key);
        }

        public object this[object key]
        {
            get { return RetrieveElement(key); }
            set { ReplaceElement(key, value); }
        }

        public ICollection Keys { get { return _dictionaryElements.Keys.ToArray(); } }

        public ICollection Values { get { return _dictionaryElements.Values.ToArray(); } }
        
        public bool IsReadOnly { get { return _dictionaryElements.IsReadOnly; } }
        
        public bool IsFixedSize { get { return false; } }

        public IEnumerator GetEnumerator()
        {
            return _dictionaryElements.Values.GetEnumerator();
        }
    }
}
