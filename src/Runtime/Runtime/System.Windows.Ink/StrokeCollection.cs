
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

using System.Collections.Specialized;
using OpenSilver.Internal;

namespace System.Windows.Ink
{
    /// <summary>
    /// Represents a collection of <see cref="Stroke"/> objects.
    /// </summary>
    public sealed class StrokeCollection : PresentationFrameworkCollection<Stroke>
    {
        private readonly CollectionChangedHelper _collectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrokeCollection"/> class.
        /// </summary>
        public StrokeCollection()
        {
            _collectionChanged = new(this);
        }

        internal event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _collectionChanged.CollectionChanged += value;
            remove => _collectionChanged.CollectionChanged -= value;
        }

        internal override void AddOverride(Stroke point)
        {
            _collectionChanged.CheckReentrancy();
            AddInternal(point);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Add, point, InternalCount - 1);
        }

        internal override void ClearOverride()
        {
            _collectionChanged.CheckReentrancy();
            ClearInternal();
            _collectionChanged.OnCollectionReset();
        }

        internal override void RemoveAtOverride(int index)
        {
            _collectionChanged.CheckReentrancy();
            Stroke removedItem = GetItemInternal(index);
            RemoveAtInternal(index);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        internal override void InsertOverride(int index, Stroke point)
        {
            _collectionChanged.CheckReentrancy();
            InsertInternal(index, point);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Add, point, index);
        }

        internal override Stroke GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Stroke point)
        {
            _collectionChanged.CheckReentrancy();
            Stroke originalItem = GetItemInternal(index);
            SetItemInternal(index, point);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, point, index);
        }
    }
}
