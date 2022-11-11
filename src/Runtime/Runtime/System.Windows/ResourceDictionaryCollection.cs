
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
using System.Diagnostics;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal sealed class ResourceDictionaryCollection : PresentationFrameworkCollection<ResourceDictionary>
    {
        private readonly ResourceDictionary _owner;

        internal ResourceDictionaryCollection(ResourceDictionary owner) : base(true)
        {
            Debug.Assert(owner != null, "ResourceDictionaryCollection's owner cannot be null");
            this._owner = owner;
        }

        internal override void AddOverride(ResourceDictionary value)
        {
            CheckValue(value);
            this.AddInternal(value);
            value._parentDictionary = this._owner;
        }

        internal override void ClearOverride()
        {
            foreach (ResourceDictionary dictionary in this)
            {
                dictionary._parentDictionary = null;
                this._owner.RemoveParentOwners(dictionary);
            }

            this.ClearInternal();
        }

        internal override void InsertOverride(int index, ResourceDictionary value)
        {
            CheckValue(value);
            this.InsertInternal(index, value);
            value._parentDictionary = this._owner;
        }

        internal override void RemoveAtOverride(int index)
        {
            ResourceDictionary removedItem = this.GetItemInternal(index);
            this.RemoveAtInternal(index);
            removedItem._parentDictionary = null;
        }

        internal override ResourceDictionary GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, ResourceDictionary value)
        {
            CheckValue(value);
            ResourceDictionary originalItem = this.GetItemInternal(index);
            this.SetItemInternal(index, value);
            originalItem._parentDictionary = null;
            value._parentDictionary = this._owner;
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
