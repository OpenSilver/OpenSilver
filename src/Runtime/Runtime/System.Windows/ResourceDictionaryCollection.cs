
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

using System.Diagnostics;

namespace System.Windows
{
    internal sealed class ResourceDictionaryCollection : PresentationFrameworkCollection<ResourceDictionary>
    {
        private readonly ResourceDictionary _owner;

        internal ResourceDictionaryCollection(ResourceDictionary owner) : base(true)
        {
            Debug.Assert(owner != null, "ResourceDictionaryCollection's owner cannot be null");
            _owner = owner;
        }

        internal override void AddOverride(ResourceDictionary value)
        {
            CheckValue(value);
            AddInternal(value);
            value._parentDictionary = _owner;
        }

        internal override void ClearOverride()
        {
            foreach (ResourceDictionary dictionary in InternalItems)
            {
                dictionary._parentDictionary = null;
                _owner.RemoveParentOwners(dictionary);
            }

            ClearInternal();
        }

        internal override void InsertOverride(int index, ResourceDictionary value)
        {
            CheckValue(value);
            InsertInternal(index, value);
            value._parentDictionary = _owner;
        }

        internal override void RemoveAtOverride(int index)
        {
            ResourceDictionary removedItem = GetItemInternal(index);
            RemoveAtInternal(index);
            removedItem._parentDictionary = null;
        }

        internal override ResourceDictionary GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, ResourceDictionary value)
        {
            CheckValue(value);
            ResourceDictionary originalItem = GetItemInternal(index);
            SetItemInternal(index, value);
            originalItem._parentDictionary = null;
            value._parentDictionary = _owner;
        }

        private static void CheckValue(ResourceDictionary value)
        {
            if (value._parentDictionary != null)
            {
                throw new InvalidOperationException("Element is already the child of another element.");
            }
        }
    }
}
