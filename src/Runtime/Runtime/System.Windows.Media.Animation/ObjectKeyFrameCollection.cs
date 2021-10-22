﻿
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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Represents a collection of ObjectKeyFrame objects that can be individually
    /// accessed by index.
    /// </summary>
    public sealed partial class ObjectKeyFrameCollection : PresentationFrameworkCollection<ObjectKeyFrame>
    {
        public ObjectKeyFrameCollection() : base(false)
        {
        }

        internal override void AddOverride(ObjectKeyFrame keyFrame)
        {
            this.AddDependencyObjectInternal(keyFrame);
        }

        internal override void ClearOverride()
        {
            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, ObjectKeyFrame keyFrame)
        {
            this.InsertDependencyObjectInternal(index, keyFrame);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override ObjectKeyFrame GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, ObjectKeyFrame keyFrame)
        {
            this.SetItemDependencyObjectInternal(index, keyFrame);
        }
    }
}