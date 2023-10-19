
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

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a collection of GradientStop objects that can be individually
    /// accessed by index.
    /// </summary>
    public sealed class GradientStopCollection : PresentationFrameworkCollection<GradientStop>
    {
        private Brush _parentBrush;

        public GradientStopCollection() : base(false)
        {
        }

        internal void SetParentBrush(Brush brush)
        {
            if (this._parentBrush != brush)
            {
                this._parentBrush = brush;
                foreach (GradientStop gs in this)
                {
                    gs.INTERNAL_ParentBrush = brush;
                }
            }
        }

        internal override void AddOverride(GradientStop gradientStop)
        {
            this.AddDependencyObjectInternal(gradientStop);
            gradientStop.INTERNAL_ParentBrush = this._parentBrush;
        }

        internal override void ClearOverride()
        {
            if (this._parentBrush != null)
            {
                foreach (GradientStop gs in this)
                {
                    gs.INTERNAL_ParentBrush = null;
                }
            }

            this.ClearDependencyObjectInternal();
        }

        internal override void RemoveAtOverride(int index)
        {
            this.GetItemInternal(index).INTERNAL_ParentBrush = null;
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override void InsertOverride(int index, GradientStop gradientStop)
        {
            gradientStop.INTERNAL_ParentBrush = this._parentBrush;
            this.InsertDependencyObjectInternal(index, gradientStop);
        }

        internal override GradientStop GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, GradientStop gradientStop)
        {
            GradientStop oldItem = this.GetItemInternal(index);
            oldItem.INTERNAL_ParentBrush = null;
            gradientStop.INTERNAL_ParentBrush = this._parentBrush;
            this.SetItemDependencyObjectInternal(index, gradientStop);
        }
    }
}