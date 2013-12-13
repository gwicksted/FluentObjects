using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using FormContextProvider.Annotations;

namespace FluentObjects
{
    public partial class FluentObject : DynamicObject, INotifyPropertyChanged 
    {
        // For . access this.something
        private readonly IDictionary<string, dynamic> _attributes = new Dictionary<string, dynamic>();

        // For [] access this["test"] or this[0] or this("function", 123, "call")
        private readonly IDictionary<object, dynamic> _dictionaryElements = new Dictionary<object, dynamic>();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool IsEmpty()
        {
            return !_attributes.Any() && !_dictionaryElements.Any();
        }
        
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _attributes.Keys;
        }
        
        private dynamic RetrieveAttribute(string memberName)
        {
            if (!_attributes.ContainsKey(memberName))
            {
                _attributes[memberName] = new FluentObject();
            }

            return _attributes[memberName];
        }

        private void ReplaceAttribute(string memberName, object value)
        {
            if (_attributes.ContainsKey(memberName))
            {
                _attributes.Remove(memberName);
            }

            _attributes[memberName] = value;
        }

        private dynamic RetrieveElement(object key)
        {
            if (!_dictionaryElements.ContainsKey(key))
            {
                _dictionaryElements[key] = new FluentObject();
            }

            return _dictionaryElements[key];
        }

        private void ReplaceElement(object index, object value)
        {
            if (_dictionaryElements.ContainsKey(index))
            {
                _dictionaryElements.Remove(index);
            }

            _dictionaryElements[index] = value;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            // TODO: store this along with args in a special area so this.test("abc").Value = "abc"; won't overwrite this.test("def").Value = "def";
            result = RetrieveElement(binder.Name);

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            object index = indexes.FirstOrDefault();

            result = index == null ? _dictionaryElements : RetrieveElement(index);

            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            object index = indexes.FirstOrDefault();
            
            ReplaceElement(index, value);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = RetrieveAttribute(binder.Name);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string memberName = binder.Name;

            ReplaceAttribute(memberName, value);

            OnPropertyChanged(memberName);

            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out Object result)
        {
            result = Convert(binder.ReturnType);
            
            return true;
        }
    }
}
