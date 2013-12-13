using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FluentObjects
{
    public partial class FluentObject
    {
        private bool IsGenericDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                   type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        private bool IsDictionary(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        private bool IsGenericList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>) ||
                   type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
        }

        private bool IsList(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        private bool IsGenericEnumerable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IEnumerable<>) ||
                   type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEnumerable<>));
        }

        private bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        private object ConvertToGenericDictionary(Type type)
        {
            Type[] generics = type.GetGenericArguments();

            if (generics.Count() != 2)
            {
                throw new NotSupportedException("Generic dictionary with <> 2 generic elements not supported");
            }

            Type keyType = generics[0];
            Type valueType = generics[1];

            // Handle IDictionary here
            if (type.IsInterface)
            {
                type = typeof(Dictionary<,>).MakeGenericType(new[] { keyType, valueType });
            }

            object result = Activator.CreateInstance(type);

            dynamic genericDictionary = result;

            Type keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(new[] { keyType, valueType });

            foreach (KeyValuePair<object, dynamic> element in _dictionaryElements)
            {
                if (keyType.IsInstanceOfType(element.Key) && valueType.IsInstanceOfType(element.Value))
                {
                    dynamic keyValuePair = Activator.CreateInstance(keyValuePairType, element.Key, element.Value);

                    genericDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return result;
        }

        private object ConvertToDictionary(Type type)
        {
            object result = Activator.CreateInstance(type);

            IDictionary dict = (IDictionary)result;

            foreach (KeyValuePair<object, dynamic> element in _dictionaryElements)
            {
                dict.Add(element.Key, element.Value);
            }

            return result;
        }

        private object ConvertToGenericList(Type type)
        {
            object result = Activator.CreateInstance(type);

            dynamic genericList = result;

            Type[] generics = type.GetGenericArguments();

            if (generics.Count() == 1)
            {
                Type valueType = generics[0];

                // foreach only where key is numeric?
                foreach (dynamic element in _dictionaryElements.Values)
                {
                    if (valueType.IsInstanceOfType(element))
                    {
                        genericList.Add(element);
                    }
                }
            }
            else
            {
                // ???
            }

            return result;
        }

        private object ConvertToList(Type type)
        {
            object result = Activator.CreateInstance(type);

            // foreach only where key is numeric?
            IList list = (IList)result;

            foreach (dynamic value in _dictionaryElements.Values)
            {
                list.Add(value);
            }

            return result;
        }

        private object ConvertToGenericEnumerable(Type type)
        {
            object result;

            Type[] generics = type.GetGenericArguments();

            if (generics.Count() == 1)
            {
                Type valueType = generics[0];

                result = _dictionaryElements.Values.Where(element => valueType.IsInstanceOfType(element));
            }
            else
            {
                // ???
                result = Activator.CreateInstance(type);
            }

            return result;
        }

        private object ConvertToEnumerable(Type type)
        {
            return _dictionaryElements.Values.GetEnumerator();
        }

        private object Convert(Type type)
        {
            object result;

            // Accessed, never set or not an enumerable type
            if (IsEmpty() || type.IsValueType)
            {
                result = type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            else
            {
                if (IsGenericDictionary(type))
                {
                    result = ConvertToGenericDictionary(type);
                }
                else if (IsDictionary(type))
                {
                    result = ConvertToDictionary(type);
                }
                else if (IsGenericList(type))
                {
                    result = ConvertToGenericList(type);
                }
                else if (IsList(type))
                {
                    result = ConvertToList(type);
                }
                else if (IsGenericEnumerable(type))
                {
                    result = ConvertToGenericEnumerable(type);
                }
                else if (IsEnumerable(type))
                {
                    result = ConvertToEnumerable(type);
                }
                else
                {
                    // TODO: could attempt to copy the members over to the desired type
                    result = Activator.CreateInstance(type);
                }
            }

            return result;
        }
    }
}
