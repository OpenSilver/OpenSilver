
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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Represents a collection of <see cref="ColorKeyFrame" /> objects 
    /// that can be individually accessed by index. 
    /// </summary>
    public sealed class ColorKeyFrameCollection : PresentationFrameworkCollection<ColorKeyFrame>
    {
        public ColorKeyFrameCollection()
            : base(false)
        {
        }

        internal ColorKeyFrameCollection(ColorAnimationUsingKeyFrames owner)
            : base(false)
        {
            Debug.Assert(owner != null);
            owner.ProvideSelfAsInheritanceContext(this, null);
        }

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
