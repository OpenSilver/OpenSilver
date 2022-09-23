
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
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal.Data
{
    internal abstract class PropertyPathNode : IPropertyPathNode
    {
        private ICollectionView _icv;
        private object _source;

        protected PropertyPathNode(PropertyPathWalker listener)
        {
            Listener = listener;
        }

        public PropertyPathWalker Listener { get; }

        public object Source
        {
            get => _source;
            set => SetSource(value, false);
        }

        public object Value { get; private set; } = DependencyProperty.UnsetValue;

        public bool IsBroken { get; private set; }

        public IPropertyPathNode Next { get; set; }

        public abstract Type Type { get; }

        public abstract string PropertyName { get; }

        public abstract bool IsBound { get; }

        void IPropertyPathNode.SetValue(object value) => SetValue(value);

        private void SetSource(object source, bool sourceIsCurrentItem)
        {
            UpdateSource(source, sourceIsCurrentItem);

            UpdateValue();

            if (Next != null)
            {
                Next.Source = Value == DependencyProperty.UnsetValue ? null : Value;
            }
        }

        private void UpdateSource(object source, bool sourceIsCurrentItem)
        {
            object oldSource = _source;
            _source = source;

            if (oldSource != _source)
            {
                OnSourceChanged(oldSource, source, sourceIsCurrentItem);
            }
        }

        private void OnSourceChanged(object oldSource, object newSource, bool sourceIsCurrentItem)
        {
            if (!sourceIsCurrentItem && _icv != null)
            {
                if (Listener.ListenForChanges)
                {
                    _icv.CurrentChanged -= new EventHandler(OnCurrentChanged);
                }
                _icv = null;
            }

            OnSourceChanged(oldSource, _source);

            if (!sourceIsCurrentItem && !IsBound && newSource is ICollectionView icv)
            {
                if (Listener.ListenForChanges)
                {
                    icv.CurrentChanged += new EventHandler(OnCurrentChanged);
                }
                _icv = icv;
                UpdateSource(icv.CurrentItem, true);
            }
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            if (_icv == null)
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
                Listener.ValueChanged();
            }
        }

        internal abstract void OnSourceChanged(object oldSource, object newSource);

        internal abstract void UpdateValue();

        internal abstract void SetValue(object value);
    }
}
