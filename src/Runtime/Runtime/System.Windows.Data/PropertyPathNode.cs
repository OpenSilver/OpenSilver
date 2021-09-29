
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal abstract class PropertyPathNode : IPropertyPathNode
    {
        private IPropertyPathNodeListener _nodeListener;
        
        protected PropertyPathNode()
        {
            Value = DependencyProperty.UnsetValue;
        }

        public object Source { get; private set; }

        public object Value { get; private set; }

        public bool IsBroken { get; private set; }

        public IPropertyPathNode Next { get; set; }

        internal abstract Type TypeImpl { get; }

        internal void UpdateValueAndIsBroken(object newValue, bool isBroken)
        {
            bool emitBrokenChanged = IsBroken != isBroken;
            bool emitValueChanged = Value != newValue // reference equals works well for immutable types, like strings for example 
                || Value is System.Collections.ICollection;//Partial workaround: If the Value is a collection, then we cannot asume that the collection has not been changed only by doing a reference equals.

            IsBroken = isBroken;
            Value = newValue;

            if (emitValueChanged)
            {
                IPropertyPathNodeListener listener = _nodeListener;
                if (listener != null)
                {
                    listener.ValueChanged(this);
                }
            }
            else if (emitBrokenChanged)
            {
                IPropertyPathNodeListener listener = _nodeListener;
                if (listener != null)
                {
                    listener.IsBrokenChanged(this);
                }
            }
        }

        internal abstract void OnSourceChanged(object oldSource, object newSource);
        
        internal abstract void UpdateValue();

        internal abstract void SetValue(object value);

        Type IPropertyPathNode.Type => TypeImpl;

        void IPropertyPathNode.SetSource(object source)
        {
            if (source == null || source != Source)
            {
                object oldSource = Source;
                Source = source;

                OnSourceChanged(oldSource, Source);
                UpdateValue();

                if (Next != null)
                {
                    Next.SetSource(Value == DependencyProperty.UnsetValue ? null : Value);
                }
            }
        }

        void IPropertyPathNode.SetValue(object value)
        {
            SetValue(value);
        }

        void IPropertyPathNode.Listen(IPropertyPathNodeListener listener)
        {
            _nodeListener = listener;
        }

        void IPropertyPathNode.Unlisten(IPropertyPathNodeListener listener)
        {
            if (_nodeListener == listener)
            {
                _nodeListener = null;
            }
        }
    }
}
