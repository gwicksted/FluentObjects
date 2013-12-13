using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FluentObjects.Mvc4
{
    public static class FluentFormContext
    {
        [ThreadStatic]
        private static IList<IContextElement> _context;

        [ThreadStatic]
        private static IContextElement _last;

        internal static IList<IContextElement> Context
        {
            get
            {
                _context = _context ?? new List<IContextElement>();

                return _context;
            }
        }

        internal static IContextElement Last
        {
            get { return _last; }
            set { _last = value; }
        }

        public static string Path
        {
            get { return string.Join(".", from element in Context select element.ToString()); }
        }
        
        public static string PathFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return string.Format("{0}.{1}", Path, ExpressionHelper.GetExpressionText(expression));
        }

        public static string IdFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return HttpUtility.HtmlAttributeEncode(string.Format("{0}.{1}", Path, ExpressionHelper.GetExpressionText(expression)));
        }
        
        /*public static FluentObject GetFluentObject()
        {
            foreach (IContextElement contextElement in Context)
            {
                FluentObject inner;

                if (contextElement is ArrayContextElement)
                {
                    int index = ((ArrayContextElement) contextElement).Index;
                    string name = contextElement.Name;

                    inner = AddDictionary(current, contextElement.Name, );
                }
                else
                {
                    string name = contextElement.ToString();

                    inner = AddObject(current, );
                }

                current = inner;
            }
        }*/

        // Maybe return ElasticObject so it can be modified by assignment then used
        public static dynamic GetTree()
        {
            dynamic result = new ExpandoObject();

            dynamic current = result;

            foreach (IContextElement contextElement in Context)
            {
                ArrayContextElement element = contextElement as ArrayContextElement;

                current = element != null ? 
                    AddDictionary(current, contextElement.Name, element.Index) : 
                    AddObject(current, contextElement.ToString());
            }

            return result;
        }

        private static dynamic AddObject(dynamic tree, string name)
        {
            dynamic inner = new ExpandoObject();

            ((IDictionary<string, object>)tree).Add(name, inner);

            return inner;
        }

        // Possibly support splitting name on "." -- also detect [1] cases for arrays
        /*private static dynamic AddObjects(dynamic tree, IEnumerable<string> names)
        {
            dynamic current = tree;

            foreach (string name in names)
            {
                dynamic inner = new ExpandoObject();

                ((IDictionary<string, object>)current).Add(name, inner);

                current = inner;
            }

            return current;
        }*/

        private static dynamic AddDictionary(dynamic tree, string name, int index)
        {
            dynamic inner = new ExpandoObject();

            Dictionary<int, object> dictionary = new Dictionary<int, object>
                        {
                            {index, inner}
                        };

            ((IDictionary<string, object>)tree).Add(name, dictionary);

            return inner;
        }

        // Evaluate if these should be kept.

        public static MvcHtmlString EditorFor<TModel, TValue>(this HtmlHelper<TModel> html)
        {
            return html.Editor(Path, new {});
        }

        public static MvcHtmlString EditorFor<TModel, TValue>(this HtmlHelper<TModel> html, object additionalViewData)
        {
            return html.Editor(Path, additionalViewData);
        }

        public static FormContextInstance SpecifyObject(string path)
        {
            IContextElement element = new ObjectContextElement {Name = path};

            Context.Add(element);

            return new FormContextInstance(element);
        }
        
        public static FormContextInstance SpecifyArray(string path, object obj, bool forceNewArray)
        {
            IContextElement element;

            if (forceNewArray || _last == null || !IncrementPreviousArray(path, obj))
            {
                element = new ArrayContextElement {Name = path, Last = obj};

                Context.Add(element);
            }
            else
            {
                element = _last;
            }

            return new FormContextInstance(element);
        }

        public static FormContextInstance SpecifyArray(string path, object obj)
        {
            return SpecifyArray(path, obj, false);
        }

        private static bool IncrementPreviousArray(string path, object obj)
        {
            if (IncrementIfMatches(path, obj, Context.LastOrDefault() as ArrayContextElement))
            {
                return true;
            }
            else
            {
                ArrayContextElement last = _last as ArrayContextElement;

                if (IncrementIfMatches(path, obj, last))
                {
                    Context.Add(last);

                    return true;
                }
            }

            return false;
        }

        private static bool IncrementIfMatches(string path, object obj, ArrayContextElement last)
        {
            if (last != null && last.Name == path)
            {
                if (last.Last != obj)
                {
                    last.Index++;
                }

                return true;
            }

            return false;
        }

        public static void Clear()
        {
            Context.Clear();
            _last = null;
        }
    }
}
