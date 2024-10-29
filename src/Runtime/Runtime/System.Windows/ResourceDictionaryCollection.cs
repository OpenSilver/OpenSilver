
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
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows
{
    internal sealed class ResourceDictionaryCollection : PresentationFrameworkCollection<ResourceDictionary>
    {
        private readonly ResourceDictionary _owner;
        private readonly CollectionChangedHelper _collectionChanged;

        internal ResourceDictionaryCollection(ResourceDictionary owner)
        {
            Debug.Assert(owner != null, "ResourceDictionaryCollection's owner cannot be null");
            _owner = owner;
            _collectionChanged = new(this);
        }

        internal event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _collectionChanged.CollectionChanged += value;
            remove => _collectionChanged.CollectionChanged -= value;
        }

        internal override void AddOverride(ResourceDictionary value)
        {
            _collectionChanged.CheckReentrancy();

            CheckValue(value);
            AddInternal(value);
            value._parentDictionary = _owner;

            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, InternalCount - 1);
        }

        internal override void ClearOverride()
        {
            _collectionChanged.CheckReentrancy();

            foreach (ResourceDictionary dictionary in InternalItems)
            {
                dictionary._parentDictionary = null;
                _owner.RemoveParentOwners(dictionary);
            }

            ClearInternal();

            _collectionChanged.OnCollectionReset();
        }

        internal override void InsertOverride(int index, ResourceDictionary value)
        {
            _collectionChanged.CheckReentrancy();

            CheckValue(value);
            InsertInternal(index, value);
            value._parentDictionary = _owner;

            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index);
        }

        internal override void RemoveAtOverride(int index)
        {
            _collectionChanged.CheckReentrancy();

            ResourceDictionary removedItem = GetItemInternal(index);
            RemoveAtInternal(index);
            removedItem._parentDictionary = null;

            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        internal override ResourceDictionary GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, ResourceDictionary value)
        {
            _collectionChanged.CheckReentrancy();

            CheckValue(value);
            ResourceDictionary originalItem = GetItemInternal(index);
            SetItemInternal(index, value);
            originalItem._parentDictionary = null;
            value._parentDictionary = _owner;

            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, value, index);
        }

        private static void CheckValue(ResourceDictionary value)
        {
            if (value._parentDictionary is not null)
            {
                throw new InvalidOperationException("Element is already the child of another element.");
            }
        }
    }
}
