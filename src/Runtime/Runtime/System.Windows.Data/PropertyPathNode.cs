
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
using System.Collections.Specialized;
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal abstract class PropertyPathNode
    {
        private INotifyCollectionChanged _originalSourcePagedCollection;
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
            if (_originalSourcePagedCollection != null && source == null)    //when detaching a previous binding we unsubscribe
            {
                _originalSourcePagedCollection.CollectionChanged -= _originalSourcePagedCollection_CollectionChanged;
            }

            object oldSource = Source;
            Source = source;

            if (oldSource != Source)
            {
                //We need to handle when the source is a paged collection and use the current item as the source instead of the PagedCollection itself
                if (source is IPagedCollectionView && source is ICollectionView && source is INotifyCollectionChanged)
                {
                    if (!(Listener._expr.ParentBinding.Path == null || string.IsNullOrEmpty(Listener._expr.ParentBinding.Path.Path)))  //we only handle this way when a binding path has been set
                    {
                        _originalSourcePagedCollection = source as INotifyCollectionChanged;
                        _originalSourcePagedCollection.CollectionChanged += _originalSourcePagedCollection_CollectionChanged;
                        Source = (source as ICollectionView).CurrentItem;
                    }
                }

                OnSourceChanged(oldSource, Source);
            }

            UpdateValue();

            if (Next != null)
            {
                Next.SetSource(Value == DependencyProperty.UnsetValue ? null : Value);
            }
        }

        private void _originalSourcePagedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetSource((_originalSourcePagedCollection as ICollectionView).CurrentItem);
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
