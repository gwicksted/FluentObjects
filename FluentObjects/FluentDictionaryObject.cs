using System;
using System.Collections;
using System.Collections.Generic;
using FormContextProvider;

namespace FluentObjects
{
    /// <summary>
    /// Temporary object used to fool implicit conversions in some circumstances.
    /// </summary>
    /// <typeparam name="TKey">Type of dictionary key.</typeparam>
    /// <typeparam name="TValue">Type of dictionary value.</typeparam>
    public class FluentDictionaryObject<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly FluentObject _original;

        public FluentDictionaryObject(FluentObject original)
        {
            _original = original;
        }

        public static implicit operator Dictionary<TKey, TValue>(FluentDictionaryObject<TKey, TValue> o)
        {
            return o._original.ToDictionary<TKey, TValue>();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICollection<TKey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }
    }

}
