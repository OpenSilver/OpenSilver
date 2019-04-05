
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal abstract class PropertyPathNode : IPropertyPathNode
    {
        public IPropertyPathNode _next;
        bool _isBroken;
        object _source;
        object _value;
        internal DependencyProperty DependencyProperty;
        //todo: remove all the casts to Object when we check the (in)equality of PropertyInfo with something (simply find the References of the PropertyInfo property and remove the casts, it should be enough)
        internal PropertyInfo PropertyInfo; //this serves to find the property when using a INotifyPropertyChanged source (and not a DependencyProperty)
        internal FieldInfo FieldInfo; //this serves to find the field when binding to a simple field (this is a new feature that is supported in CSHTML5 but not in Silverlight)
        private IPropertyPathNodeListener _nodeListener;
        //IType ValueType;

        public bool IsBroken
        {
            get { return _isBroken; }
        }
        public object Source
        {
            get { return _source; }
        }
        public object Value
        {
            get { return _value; }
        }

        internal void Listen(IPropertyPathNodeListener listener) { _nodeListener = listener; }
        internal void Unlisten(IPropertyPathNodeListener listener)
        {
            if (_nodeListener == listener)
                _nodeListener = null;
        }

        internal virtual void OnSourceChanged(object oldSource, object newSource) { }
        internal virtual void OnSourcePropertyChanged(object o, PropertyChangedEventArgs e) { }

        abstract internal void UpdateValue();
        abstract internal void SetValue(object value);

        internal void SetSource(object value)
        {
            if (value == null || value != _source)
            {
                //we unsubscribe from the former source's changes:
                var oldSource = this._source;
                var oldSourceAsINotifyPropertyChanged = oldSource as INotifyPropertyChanged;
                if (oldSourceAsINotifyPropertyChanged != null)
                    oldSourceAsINotifyPropertyChanged.PropertyChanged -= OnSourcePropertyChanged;

                //we subscribe to the new source's changes:
                _source = value;
                var newSourceAsINotifyPropertyChanged = _source as INotifyPropertyChanged;
                if (newSourceAsINotifyPropertyChanged != null)
                    newSourceAsINotifyPropertyChanged.PropertyChanged += OnSourcePropertyChanged;

                OnSourceChanged(oldSource, _source);
                UpdateValue();
                if (Next != null)
                    Next.SetSource(_value);
            }
        }

        internal void UpdateValueAndIsBroken(object newValue, bool isBroken)
        {
            var emitBrokenChanged = _isBroken != isBroken;
            var emitValueChanged = Value != newValue;

            this._isBroken = isBroken;
            this._value = newValue;

            if (emitValueChanged)
            {
                var listener = this._nodeListener;
                if (listener != null)
                {
                    listener.ValueChanged(this);
                }
            }
            else if (emitBrokenChanged)
            {
                var listener = this._nodeListener;
                if (listener != null)
                {
                    listener.IsBrokenChanged(this);
                }
            }
        }
        internal bool CheckIsBroken(bool bindsDirectlyToSource = false)
        {
            return (Source == null || (!bindsDirectlyToSource && (PropertyInfo == null && FieldInfo == null && DependencyProperty == null)));
        }

        public IPropertyPathNode Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = value;
            }
        }

        void IPropertyPathNode.SetSource(object source)
        {
            SetSource(source);
        }

        void IPropertyPathNode.SetValue(object value)
        {
            SetValue(value);
        }

        void IPropertyPathNode.Listen(IPropertyPathNodeListener listener)
        {
            Listen(listener);
        }

        void IPropertyPathNode.Unlisten(IPropertyPathNodeListener listener)
        {
            Unlisten(listener);
        }

    }
}
