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
using System.Collections.Generic;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    public sealed partial class DoubleKeyFrameCollection : PresentationFrameworkCollection<DoubleKeyFrame>
#else
    public sealed partial class DoubleKeyFrameCollection : List<DoubleKeyFrame>
#endif
    {
#if WORKINPROGRESS
        internal override void AddOverride(DoubleKeyFrame keyFrame)
        {
            this.AddDependencyObjectInternal(keyFrame);
        }

        internal override void ClearOverride()
        {
            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, DoubleKeyFrame keyFrame)
        {
            this.InsertDependencyObjectInternal(index, keyFrame);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override bool RemoveOverride(DoubleKeyFrame keyFrame)
        {
            return this.RemoveDependencyObjectInternal(keyFrame);
        }

        internal override DoubleKeyFrame GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, DoubleKeyFrame keyFrame)
        {
            this.SetItemDependencyObjectInternal(index, keyFrame);
        }
#endif
    }
}
