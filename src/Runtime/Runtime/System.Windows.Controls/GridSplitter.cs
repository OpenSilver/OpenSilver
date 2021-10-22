

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
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
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
    public partial class GridSplitter : Control
    {
        internal const int GripperCustomCursorDefaultResource = -1;
#if CSHTML5_NOT_SUPPORTED
        internal static readonly CoreCursor ColumnsSplitterCursor = new CoreCursor(CoreCursorType.SizeWestEast, 1);
        internal static readonly CoreCursor RowSplitterCursor = new CoreCursor(CoreCursorType.SizeNorthSouth, 1);
        internal CoreCursor PreviousCursor { get; set; }
#else
        internal static readonly Cursor ColumnsSplitterCursor = Cursors.SizeWE;
        internal static readonly Cursor RowSplitterCursor = Cursors.SizeNS;
        internal Cursor PreviousCursor { get; set; }
#endif

        private GridResizeDirection _resizeDirection;
        private GridResizeBehavior _resizeBehavior;
        private GripperHoverWrapper _hoverWrapper;


#if !CSHTML5_NOT_SUPPORTED
        private Thumb _thumb; // Note: the thumb control is here only in CSHTML5.
#endif

        /// <summary>
        /// Gets the target parent grid from level
        /// </summary>
        private FrameworkElement TargetControl
        {
            get
            {
                if (ParentLevel == 0)
                {
                    return this;
                }

                var parent = Parent;
                for (int i = 2; i < ParentLevel; i++)
                {
                    var frameworkElement = parent as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        parent = frameworkElement.Parent;
                    }
                }

                return parent as FrameworkElement;
            }
        }

        /// <summary>
        /// Gets GridSplitter Container Grid
        /// </summary>
        private Grid Resizable
        {
            get
            {
                if (TargetControl != null)
                    return TargetControl.Parent as Grid;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the current Column definition of the parent Grid
        /// </summary>
        private ColumnDefinition CurrentColumn
        {
            get
            {
                if (Resizable == null)
                {
                    return null;
                }

                var gridSplitterTargetedColumnIndex = GetTargetedColumn();

                if ((gridSplitterTargetedColumnIndex >= 0)
                    && (gridSplitterTargetedColumnIndex < Resizable.ColumnDefinitions.Count))
                {
                    return Resizable.ColumnDefinitions[gridSplitterTargetedColumnIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the Sibling Column definition of the parent Grid
        /// </summary>
        private ColumnDefinition SiblingColumn
        {
            get
            {
                if (Resizable == null)
                {
                    return null;
                }

                var gridSplitterSiblingColumnIndex = GetSiblingColumn();

                if ((gridSplitterSiblingColumnIndex >= 0)
                    && (gridSplitterSiblingColumnIndex < Resizable.ColumnDefinitions.Count))
                {
                    return Resizable.ColumnDefinitions[gridSplitterSiblingColumnIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current Row definition of the parent Grid
        /// </summary>
        private RowDefinition CurrentRow
        {
            get
            {
                if (Resizable == null)
                {
                    return null;
                }

                var gridSplitterTargetedRowIndex = GetTargetedRow();

                if ((gridSplitterTargetedRowIndex >= 0)
                    && (gridSplitterTargetedRowIndex < Resizable.RowDefinitions.Count))
                {
                    return Resizable.RowDefinitions[gridSplitterTargetedRowIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the Sibling Row definition of the parent Grid
        /// </summary>
        private RowDefinition SiblingRow
        {
            get
            {
                if (Resizable == null)
                {
                    return null;
                }

                var gridSplitterSiblingRowIndex = GetSiblingRow();

                if ((gridSplitterSiblingRowIndex >= 0)
                    && (gridSplitterSiblingRowIndex < Resizable.RowDefinitions.Count))
                {
                    return Resizable.RowDefinitions[gridSplitterSiblingRowIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridSplitter"/> class.
        /// </summary>
        public GridSplitter()
        {
#if CSHTML5_NOT_SUPPORTED
            DefaultStyleKey = typeof(GridSplitter);
#else
            this.DefaultStyleKey = typeof(GridSplitter);
            this.Loaded += GridSplitter_Loaded;
#endif
        }

        /// <inheritdoc />
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // Note: the following block is not supported as in Silverlight,
            // we are not sure that the Loaded event will be raised after the
            // OnApplyTemplate method is called. This is moved to the constructor.
#if CSHTML5_NOT_SUPPORTED
            // Unhook registered events
            Loaded -= GridSplitter_Loaded;

            // Register Events
            Loaded += GridSplitter_Loaded;
#endif

            if (_hoverWrapper != null)
            {
                _hoverWrapper.UnhookEvents();
            }

#if CSHTML5_NOT_SUPPORTED
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
#else
            if (_thumb != null)
            {
                _thumb.DragDelta -= Thumb_DragDelta;
                _thumb.DragStarted -= Thumb_DragStarted;
                _thumb.DragCompleted -= Thumb_DragCompleted;
            }

            _thumb = this.GetTemplateChild("PART_Thumb") as Thumb;
            // Note: the thumb control is here only in CSHTML5. We use it to compensate for the lack of the "Manipulation" events.
            
            if (_thumb != null)
            {
                _thumb.DragDelta += Thumb_DragDelta;
                _thumb.DragStarted += Thumb_DragStarted;
                _thumb.DragCompleted += Thumb_DragCompleted;
            }
#endif
        }
#region Not supported yet

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ShowsPreviewProperty = DependencyProperty.Register("ShowsPreview", typeof(bool), typeof(GridSplitter), null);

        [OpenSilver.NotImplemented]
        public bool ShowsPreview
        {
            get { return (bool)this.GetValue(GridSplitter.ShowsPreviewProperty); }
            set { this.SetValue(GridSplitter.ShowsPreviewProperty, value); }
        }

#endregion
    }
}
