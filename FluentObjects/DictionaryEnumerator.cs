using System;
using System.Collections;
using System.Collections.Generic;

namespace FluentObjects
{
    /// <summary>
    /// Create an <see cref="IDictionaryEnumerator"/> from an <see cref="IDictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    public class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator
    {
        private readonly IEnumerator<TValue> _values;

        private readonly IEnumerator<TKey> _keys;

        public DictionaryEnumerator(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            _values = dictionary.Values.GetEnumerator();
            _keys = dictionary.Keys.GetEnumerator();
        }

        public bool MoveNext()
        {
            return _values.MoveNext() && _keys.MoveNext();
        }

        public void Reset()
        {
            _values.Reset();
            _keys.Reset();
        }

        public object Current { get { return _values.Current; } }

        public object Key { get { return _keys.Current; } }

        public object Value { get { return _values.Current; } }

        public DictionaryEntry Entry { get { return new DictionaryEntry(Key, Value); } }
    }
}
