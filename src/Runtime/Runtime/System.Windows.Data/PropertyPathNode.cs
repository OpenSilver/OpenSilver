
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
            IsBroken = isBroken;
            Value = newValue;

            IPropertyPathNodeListener listener = _nodeListener;
            if (listener != null)
            {
                listener.ValueChanged(this);
            }
        }

        internal abstract void OnSourceChanged(object oldSource, object newSource);
        
        internal abstract void UpdateValue();

        internal abstract void SetValue(object value);

        Type IPropertyPathNode.Type => TypeImpl;

        void IPropertyPathNode.SetSource(object source)
        {
            object oldSource = Source;
            Source = source;

            if (oldSource != Source)
            {
                OnSourceChanged(oldSource, Source);
            }

            UpdateValue();

            if (Next != null)
            {
                Next.SetSource(Value == DependencyProperty.UnsetValue ? null : Value);
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
