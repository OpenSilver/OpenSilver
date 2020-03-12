

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


// Source: https://github.com/Microsoft/UWPCommunityToolkit/tree/master/Microsoft.Toolkit.Uwp.UI.Controls/GridSplitter


// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System.Windows.Input;
#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows of a Grid control.
    /// </summary>
    public partial class GridSplitter
    {
        /// <summary>
        /// Identifies the <see cref="Element"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty
            = DependencyProperty.Register(
                "Element",
                typeof(UIElement),
                typeof(GridSplitter),
                new PropertyMetadata(null)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the <see cref="ResizeDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeDirectionProperty
            = DependencyProperty.Register(
                "ResizeDirection",
                typeof(GridResizeDirection),
                typeof(GridSplitter),
                new PropertyMetadata(GridResizeDirection.Auto)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the <see cref="ResizeBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeBehaviorProperty
            = DependencyProperty.Register(
                "ResizeBehavior",
                typeof(GridResizeBehavior),
                typeof(GridSplitter),
                new PropertyMetadata(GridResizeBehavior.BasedOnAlignment)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the <see cref="GripperForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperForegroundProperty
            = DependencyProperty.Register(
                "GripperForeground",
                typeof(Brush),
                typeof(GridSplitter),
                new PropertyMetadata(null, OnGripperForegroundPropertyChanged)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the <see cref="ParentLevel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentLevelProperty
            = DependencyProperty.Register(
                "ParentLevel",
                typeof(int),
                typeof(GridSplitter),
                new PropertyMetadata(0)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the <see cref="GripperCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperCursorProperty =
            DependencyProperty.RegisterAttached(
                "GripperCursor",
                typeof(GripperCursorType), //typeof(CoreCursorType?),
                typeof(GridSplitter),
                new PropertyMetadata(GripperCursorType.Default, OnGripperCursorPropertyChanged)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the <see cref="GripperCustomCursorResource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperCustomCursorResourceProperty =
            DependencyProperty.RegisterAttached(
                "GripperCustomCursorResource",
                typeof(int),
                typeof(GridSplitter),
                new PropertyMetadata(GripperCustomCursorDefaultResource, GripperCustomCursorResourcePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CursorBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CursorBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "CursorBehavior",
                typeof(SplitterCursorBehavior),
                typeof(GridSplitter),
                new PropertyMetadata(SplitterCursorBehavior.ChangeOnSplitterHover, CursorBehaviorPropertyChanged));

        /// <summary>
        /// Gets or sets the visual content of this Grid Splitter
        /// </summary>
        public UIElement Element
        {
            get { return (UIElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the Splitter resizes the Columns, Rows, or Both.
        /// </summary>
        public GridResizeDirection ResizeDirection
        {
            get { return (GridResizeDirection)GetValue(ResizeDirectionProperty); }

            set { SetValue(ResizeDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets which Columns or Rows the Splitter resizes.
        /// </summary>
        public GridResizeBehavior ResizeBehavior
        {
            get { return (GridResizeBehavior)GetValue(ResizeBehaviorProperty); }

            set { SetValue(ResizeBehaviorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground color of grid splitter grip
        /// </summary>
        public Brush GripperForeground
        {
            get { return (Brush)GetValue(GripperForegroundProperty); }

            set { SetValue(GripperForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the level of the parent grid to resize
        /// </summary>
        public int ParentLevel
        {
            get { return (int)GetValue(ParentLevelProperty); }

            set { SetValue(ParentLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the gripper Cursor type
        /// </summary>
        public GripperCursorType GripperCursor
        {
            get { return (GripperCursorType)GetValue(GripperCursorProperty); }
            set { SetValue(GripperCursorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the gripper Custom Cursor resource number
        /// </summary>
        public int GripperCustomCursorResource
        {
            get { return (int)GetValue(GripperCustomCursorResourceProperty); }
            set { SetValue(GripperCustomCursorResourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets splitter cursor on hover behavior
        /// </summary>
        public SplitterCursorBehavior CursorBehavior
        {
            get { return (SplitterCursorBehavior)GetValue(CursorBehaviorProperty); }
            set { SetValue(CursorBehaviorProperty, value); }
        }

        private static void OnGripperForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridSplitter = (GridSplitter)d;
            var grip = gridSplitter.Element as GridSplitterGripper;
            if (grip != null)
            {
                grip.GripperForeground = gridSplitter.GripperForeground;
            }
        }

        private static void OnGripperCursorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridSplitter = (GridSplitter)d;

            if (gridSplitter._hoverWrapper == null)
            {
                return;
            }

#if CSHTML5_NOT_SUPPORTED
            gridSplitter._hoverWrapper.GripperCursor = gridSplitter.GripperCursor;
#endif
        }

        private static void GripperCustomCursorResourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridSplitter = (GridSplitter)d;

            if (gridSplitter._hoverWrapper == null)
            {
                return;
            }

            gridSplitter._hoverWrapper.GripperCustomCursorResource = gridSplitter.GripperCustomCursorResource;
        }

        private static void CursorBehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridSplitter = (GridSplitter)d;

            if (gridSplitter._hoverWrapper != null)
            {
                gridSplitter._hoverWrapper.UpdateHoverElement(gridSplitter.CursorBehavior ==
                                                               SplitterCursorBehavior.ChangeOnSplitterHover
                    ? gridSplitter
                    : gridSplitter.Element);
            }
        }
    }
}
