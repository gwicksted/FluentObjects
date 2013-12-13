using System.Collections.Generic;
using System.Linq;

namespace FluentObjects
{
    public partial class FluentObject
    {
        // Works for Dictionary cast but not IDictionary cast
        public static implicit operator FluentDictionaryObject<dynamic, dynamic>(FluentObject o)
        {
            return new FluentDictionaryObject<dynamic, dynamic>(o);
        }

        public Dictionary<K, V> ToDictionary<K, V>()
        {
            return _dictionaryElements.ToDictionary(element => (K)(element.Key), element => (V)(element.Value));
        }
    }
}
