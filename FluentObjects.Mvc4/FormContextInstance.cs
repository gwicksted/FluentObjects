using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace FluentObjects.Mvc4
{
    public class FormContextInstance : IDisposable
    {
        private readonly IContextElement _createdWith;

        internal FormContextInstance(IContextElement createdWith)
        {
            if (createdWith == null)
            {
                throw new ArgumentNullException("createdWith");
            }

            _createdWith = createdWith;
        }

        public string Path
        {
            get { return string.Join(".", from element in ContextToHere() select element.ToString()); }
        }

        public string PathFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return string.Format("{0}.{1}", Path, ExpressionHelper.GetExpressionText(expression));
        }

        private IEnumerable<IContextElement> ContextToHere()
        {
            foreach (IContextElement element in FluentFormContext.Context)
            {
                if (element == _createdWith)
                {
                    break;
                }

                yield return element;
            }

            yield return _createdWith;
        }

        public void Dispose()
        {
            if (FluentFormContext.Context.LastOrDefault() == _createdWith)
            {
                FluentFormContext.Last = _createdWith;

                FluentFormContext.Context.RemoveAt(FluentFormContext.Context.Count - 1);
            }
            else
            {
                // Allow forgetfulness to clean up back to this point (but don't wipe if not found)
                int index = FluentFormContext.Context.IndexOf(_createdWith);

                if (index >= 0)
                {
                    int total = FluentFormContext.Context.Count;

                    for (int i = 0; i < total - index; i++)
                    {
                        FluentFormContext.Context.RemoveAt(FluentFormContext.Context.Count - 1);
                    }

                    FluentFormContext.Last = _createdWith;
                }
            }
        }
    }
}
