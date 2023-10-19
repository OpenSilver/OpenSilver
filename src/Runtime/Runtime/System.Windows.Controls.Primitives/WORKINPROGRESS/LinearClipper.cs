
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

using System.Windows.Media;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Clips the content of the control in a given direction.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class LinearClipper : Clipper
    {
#region public ExpandDirection ExpandDirection
        /// <summary>
        /// Gets or sets the clipped edge.
        /// </summary>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpandDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                "ExpandDirection",
                typeof(ExpandDirection),
                typeof(LinearClipper),
                new PropertyMetadata(ExpandDirection.Right, OnExpandDirectionChanged));

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandDirectionView that changed its ExpandDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnExpandDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearClipper source = (LinearClipper)d;
            ExpandDirection oldValue = (ExpandDirection)e.OldValue;
            ExpandDirection newValue = (ExpandDirection)e.NewValue;
            source.OnExpandDirectionChanged(oldValue, newValue);
        }

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnExpandDirectionChanged(ExpandDirection oldValue, ExpandDirection newValue)
        {
            ClipContent();
        }
#endregion public ExpandDirection ExpandDirection

        /// <summary>
        /// Updates the clip geometry.
        /// </summary>
        protected override void ClipContent()
        {
            if (ExpandDirection == ExpandDirection.Right)
            {
                double width = RenderSize.Width * RatioVisible;
                Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, RenderSize.Height) };
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                double width = RenderSize.Width * RatioVisible;
                double rightSide = RenderSize.Width - width;
                Clip = new RectangleGeometry { Rect = new Rect(rightSide, 0, width, RenderSize.Height) };
            }
            else if (ExpandDirection == ExpandDirection.Up)
            {
                double height = RenderSize.Height * RatioVisible;
                double bottom = RenderSize.Height - height;
                Clip = new RectangleGeometry { Rect = new Rect(0, bottom, RenderSize.Width, height) };
            }
            else if (ExpandDirection == ExpandDirection.Down)
            {
                double height = RenderSize.Height * RatioVisible;
                Clip = new RectangleGeometry { Rect = new Rect(0, 0, RenderSize.Width, height) };
            }
        }
    }
}
