

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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>Represents a collection of <see cref="T:System.Windows.Media.Animation.ColorKeyFrame" /> objects that can be individually accessed by index. </summary>
    [OpenSilver.NotImplemented]
    public sealed partial class ColorKeyFrameCollection : PresentationFrameworkCollection<ColorKeyFrame>
    {
        internal override void AddOverride(ColorKeyFrame keyFrame)
        {
            this.AddDependencyObjectInternal(keyFrame);
        }

        internal override void ClearOverride()
        {
            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, ColorKeyFrame keyFrame)
        {
            this.InsertDependencyObjectInternal(index, keyFrame);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override bool RemoveOverride(ColorKeyFrame keyFrame)
        {
            return this.RemoveDependencyObjectInternal(keyFrame);
        }

        internal override ColorKeyFrame GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, ColorKeyFrame keyFrame)
        {
            this.SetItemDependencyObjectInternal(index, keyFrame);
        }
    }
}
