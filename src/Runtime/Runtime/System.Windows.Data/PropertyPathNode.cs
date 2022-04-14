
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
    internal abstract class PropertyPathNode
    {
        protected PropertyPathNode(PropertyPathWalker listener)
        {
            Listener = listener;
        }

        public PropertyPathWalker Listener { get; }

        public object Source { get; private set; }

        public object Value { get; private set; } = DependencyProperty.UnsetValue;

        public bool IsBroken { get; private set; }

        public PropertyPathNode Next { get; set; }

        public abstract Type Type { get; }

        internal void SetSource(object source)
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

        internal void UpdateValueAndIsBroken(object newValue, bool isBroken)
        {
            IsBroken = isBroken;
            Value = newValue;

            if (Next == null)
            {
                Listener.ValueChanged(this);
            }
        }

        internal abstract void OnSourceChanged(object oldSource, object newSource);
        
        internal abstract void UpdateValue();

        internal abstract void SetValue(object value);
    }
}
