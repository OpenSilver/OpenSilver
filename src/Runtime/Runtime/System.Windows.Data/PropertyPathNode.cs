
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
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal abstract class PropertyPathNode
    {
        private ICollectionView _icv;

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

        public abstract bool IsBound { get; }

        internal void SetSource(object source) => SetSource(source, false);

        private void SetSource(object source, bool sourceIsCurrentItem)
        {
            UpdateSource(source, sourceIsCurrentItem);

            UpdateValue();

            if (Next != null)
            {
                Next.SetSource(Value == DependencyProperty.UnsetValue ? null : Value, false);
            }
        }

        private void UpdateSource(object source, bool sourceIsCurrentItem)
        {
            object oldSource = Source;
            Source = source;

            if (oldSource != Source)
            {
                OnSourceChanged(oldSource, source, sourceIsCurrentItem);
            }
        }

        private void OnSourceChanged(object oldSource, object newSource, bool sourceIsCurrentItem)
        {
            if (!sourceIsCurrentItem && _icv != null)
            {
                _icv.CurrentChanged -= new EventHandler(OnCurrentChanged);
                _icv = null;
            }

            OnSourceChanged(oldSource, Source);

            if (!sourceIsCurrentItem && !IsBound && newSource is ICollectionView icv)
            {
                _icv = icv;
                icv.CurrentChanged += new EventHandler(OnCurrentChanged);
                UpdateSource(icv.CurrentItem, true);
            }
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            if (_icv == null || !Listener.ListenForChanges)
            {
                return;
            }

            SetSource(_icv.CurrentItem, true);
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
