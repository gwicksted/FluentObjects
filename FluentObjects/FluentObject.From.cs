using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace FluentObjects
{
    public partial class FluentObject
    {
        private static FluentObject FromDictionary(IDictionary dictionary)
        {
            if (dictionary != null)
            {
                FluentObject fluent = new FluentObject();

                foreach (object key in dictionary.Keys)
                {
                    fluent._dictionaryElements.Add(key, From(dictionary[key]));
                }

                return fluent;
            }

            return null;
        }

        private static FluentObject FromList(IList list)
        {
            if (list != null)
            {
                FluentObject fluent = new FluentObject();

                for (int i = 0; i < list.Count; i++)
                {
                    fluent._dictionaryElements.Add(i, From(list[i]));
                }

                return fluent;
            }

            return null;
        }

        public static dynamic From(object obj)
        {
            Type type = obj.GetType();

            if (!(obj is FluentObject))
            {
                if (obj is IDictionary)
                {
                    return FromDictionary(obj as IDictionary);
                }

                if (obj is IList)
                {
                    return FromList(obj as IList);
                }
            }

            if (type.IsValueType || type == typeof(string) || type == typeof(FluentObject) || obj is IEnumerable)
            {
                return obj;
            }

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            FluentObject fluent = new FluentObject();

            //foreach (MethodInfo methodInfo in type.GetMethods(flags))
            //{
            //fluent._attributes.Add(methodInfo.Name, From(methodInfo.ReturnType));
            //}

            foreach (PropertyInfo property in type.GetProperties(flags))
            {
                if (!property.GetIndexParameters().Any())
                {
                    fluent._attributes.Add(property.Name, From(property.GetValue(obj, new object[] { })));
                }
                else
                {
                    // ???
                }
            }

            foreach (FieldInfo field in type.GetFields(flags))
            {
                fluent._attributes.Add(field.Name, From(field.GetValue(obj)));
            }

            return fluent;
        }
    }
}
