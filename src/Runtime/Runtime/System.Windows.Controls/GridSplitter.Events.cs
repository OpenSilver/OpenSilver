

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

using System.Collections.Generic;
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
    public partial class GridSplitter
    {
        private void GridSplitter_Loaded(object sender, RoutedEventArgs e)
        {
            _resizeDirection = GetResizeDirection();
            _resizeBehavior = GetResizeBehavior();

            GridSplitterGripper gripper;

            // Adding Grip to Grid Splitter
            if (Element == default(UIElement))
            {
                gripper = new GridSplitterGripper(
                    _resizeDirection,
                    GripperForeground);
            }
            else
            {
                var content = Element;
                Element = null;
                gripper = new GridSplitterGripper(content, _resizeDirection);
            }

            Element = gripper;

#if CSHTML5_NOT_SUPPORTED
            gripper.KeyDown += Gripper_KeyDown;
#endif

#if CSHTML5_NOT_SUPPORTED
            var hoverWrapper = new GripperHoverWrapper(
                CursorBehavior == SplitterCursorBehavior.ChangeOnSplitterHover
                ? this
                : Element,
                _resizeDirection,
                GripperCursor,
                GripperCustomCursorResource);
            _hoverWrapper = hoverWrapper;
#else
            if (_resizeDirection == GridResizeDirection.Columns)
            {
                this.Cursor = ColumnsSplitterCursor;
            }
            else if (_resizeDirection == GridResizeDirection.Rows)
            {
                this.Cursor = RowSplitterCursor;
            }
#endif

#if CSHTML5_NOT_SUPPORTED
            ManipulationStarted += hoverWrapper.SplitterManipulationStarted;
            ManipulationCompleted += hoverWrapper.SplitterManipulationCompleted;
#endif
        }

#if CSHTML5_NOT_SUPPORTED
        private void Gripper_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var gripper = sender as GridSplitterGripper;

            if (gripper == null)
            {
                return;
            }

            var step = 1;
            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
            {
                step = 5;
            }

            if (gripper.ResizeDirection == GridResizeDirection.Columns)
            {
                if (e.Key == VirtualKey.Left)
                {
                    HorizontalMove(-step);
                }
                else if (e.Key == VirtualKey.Right)
                {
                    HorizontalMove(step);
                }
                else
                {
                    return;
                }

                e.Handled = true;
                return;
            }

            if (gripper.ResizeDirection == GridResizeDirection.Rows)
            {
                if (e.Key == VirtualKey.Up)
                {
                    VerticalMove(-step);
                }
                else if (e.Key == VirtualKey.Down)
                {
                    VerticalMove(step);
                }
                else
                {
                    return;
                }

                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            // saving the previous state
            PreviousCursor = Window.Current.CoreWindow.PointerCursor;
            _resizeDirection = GetResizeDirection();
            _resizeBehavior = GetResizeBehavior();

            if (_resizeDirection == GridResizeDirection.Columns)
            {
                Window.Current.CoreWindow.PointerCursor = ColumnsSplitterCursor;
            }
            else if (_resizeDirection == GridResizeDirection.Rows)
            {
                Window.Current.CoreWindow.PointerCursor = RowSplitterCursor;
            }

            base.OnManipulationStarted(e);
        }

        /// <inheritdoc />
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = PreviousCursor;

            base.OnManipulationCompleted(e);
        }

        /// <inheritdoc />
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            var horizontalChange = e.Delta.Translation.X;
            var verticalChange = e.Delta.Translation.Y;

            if (_resizeDirection == GridResizeDirection.Columns)
            {
                if (HorizontalMove(horizontalChange))
                {
                    return;
                }
            }
            else if (_resizeDirection == GridResizeDirection.Rows)
            {
                if (VerticalMove(verticalChange))
                {
                    return;
                }
            }

            base.OnManipulationDelta(e);
        }
#else
        void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            // saving the previous state
            PreviousCursor = Window.Current.Cursor;
            _resizeDirection = GetResizeDirection();
            _resizeBehavior = GetResizeBehavior();

            if (_resizeDirection == GridResizeDirection.Columns)
            {
                Window.Current.Cursor = ColumnsSplitterCursor;
            }
            else if (_resizeDirection == GridResizeDirection.Rows)
            {
                Window.Current.Cursor = RowSplitterCursor;
            }
        }

        void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Window.Current.Cursor = PreviousCursor;
        }

        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var horizontalChange = e.HorizontalChange;
            var verticalChange = e.VerticalChange;

            if (_resizeDirection == GridResizeDirection.Columns)
            {
                if (HorizontalMove(horizontalChange))
                {
                    return;
                }
            }
            else if (_resizeDirection == GridResizeDirection.Rows)
            {
                if (VerticalMove(verticalChange))
                {
                    return;
                }
            }
        }
#endif

        private bool VerticalMove(double verticalChange)
        {
            if (CurrentRow == null || SiblingRow == null)
            {
                return true;
            }

#if !CSHTML5_NOT_SUPPORTED // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
            Dictionary<RowDefinition, double> rowDefinitionToActualHeightDictionary = GetRowDefinitionToActualHeightDictionary(Resizable);
#endif

            // if current row has fixed height then resize it
            if (!IsStarRow(CurrentRow))
            {
                // No need to check for the row Min height because it is automatically respected
#if CSHTML5_NOT_SUPPORTED
                SetRowHeight(CurrentRow, verticalChange, GridUnitType.Pixel);
#else
                SetRowHeight(CurrentRow, verticalChange, GridUnitType.Pixel, rowDefinitionToActualHeightDictionary); // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
            }

            // if sibling row has fixed width then resize it
            else if (!IsStarRow(SiblingRow))
            {
#if CSHTML5_NOT_SUPPORTED
                SetRowHeight(SiblingRow, verticalChange * -1, GridUnitType.Pixel);
#else
                SetRowHeight(SiblingRow, verticalChange * -1, GridUnitType.Pixel, rowDefinitionToActualHeightDictionary); // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
            }

            // if both row haven't fixed height (auto *)
            else
            {
                // change current row height to the new height with respecting the auto
                // change sibling row height to the new height relative to current row
                // respect the other star row height by setting it's height to it's actual height with stars

                // We need to validate current and sibling height to not cause any un expected behavior
#if CSHTML5_NOT_SUPPORTED
                if (!IsValidRowHeight(CurrentRow, verticalChange) || !IsValidRowHeight(SiblingRow, verticalChange * -1))
#else
                if (!IsValidRowHeight(CurrentRow, verticalChange, rowDefinitionToActualHeightDictionary) || !IsValidRowHeight(SiblingRow, verticalChange * -1, rowDefinitionToActualHeightDictionary))  // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
                {
                    return true;
                }

                foreach (var rowDefinition in Resizable.RowDefinitions)
                {
                    if (rowDefinition == CurrentRow)
                    {
#if CSHTML5_NOT_SUPPORTED
                        SetRowHeight(CurrentRow, verticalChange, GridUnitType.Star);
#else
                        SetRowHeight(CurrentRow, verticalChange, GridUnitType.Star, rowDefinitionToActualHeightDictionary); // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
                    }
                    else if (rowDefinition == SiblingRow)
                    {
#if CSHTML5_NOT_SUPPORTED
                        SetRowHeight(SiblingRow, verticalChange * -1, GridUnitType.Star);
#else
                        SetRowHeight(SiblingRow, verticalChange * -1, GridUnitType.Star, rowDefinitionToActualHeightDictionary); // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
                    }
                    else if (IsStarRow(rowDefinition))
                    {
#if CSHTML5_NOT_SUPPORTED
                        rowDefinition.Height = new GridLength(rowDefinition.ActualHeight, GridUnitType.Star);
#else
                        rowDefinition.Height = new GridLength(rowDefinitionToActualHeightDictionary[rowDefinition], GridUnitType.Star); // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
                    }
                }
            }

            return false;
        }

        private bool HorizontalMove(double horizontalChange)
        {
            if (CurrentColumn == null || SiblingColumn == null)
            {
                return true;
            }

#if !CSHTML5_NOT_SUPPORTED // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
            Dictionary<ColumnDefinition, double> columnDefinitionToActualWidthDictionary = GetColumnDefinitionToActualWidthDictionary(Resizable);
#endif

            // if current column has fixed width then resize it
            if (!IsStarColumn(CurrentColumn))
            {
                // No need to check for the Column Min width because it is automatically respected
#if CSHTML5_NOT_SUPPORTED
                SetColumnWidth(CurrentColumn, horizontalChange, GridUnitType.Pixel);
#else
                SetColumnWidth(CurrentColumn, horizontalChange, GridUnitType.Pixel, columnDefinitionToActualWidthDictionary); // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
            }

            // if sibling column has fixed width then resize it
            else if (!IsStarColumn(SiblingColumn))
            {
#if CSHTML5_NOT_SUPPORTED
                SetColumnWidth(SiblingColumn, horizontalChange * -1, GridUnitType.Pixel);
#else
                SetColumnWidth(SiblingColumn, horizontalChange * -1, GridUnitType.Pixel, columnDefinitionToActualWidthDictionary); // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
            }

            // if both column haven't fixed width (auto *)
            else
            {
                // change current column width to the new width with respecting the auto
                // change sibling column width to the new width relative to current column
                // respect the other star column width by setting it's width to it's actual width with stars

                // We need to validate current and sibling width to not cause any un expected behavior
#if CSHTML5_NOT_SUPPORTED
                if (!IsValidColumnWidth(CurrentColumn, horizontalChange) ||
                    !IsValidColumnWidth(SiblingColumn, horizontalChange * -1))
#else
                if (!IsValidColumnWidth(CurrentColumn, horizontalChange, columnDefinitionToActualWidthDictionary) ||
                    !IsValidColumnWidth(SiblingColumn, horizontalChange * -1, columnDefinitionToActualWidthDictionary)) // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
                {
                    return true;
                }

                foreach (var columnDefinition in Resizable.ColumnDefinitions)
                {
                    if (columnDefinition == CurrentColumn)
                    {
#if CSHTML5_NOT_SUPPORTED
                        SetColumnWidth(CurrentColumn, horizontalChange, GridUnitType.Star);
#else
                        SetColumnWidth(CurrentColumn, horizontalChange, GridUnitType.Star, columnDefinitionToActualWidthDictionary); // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
                    }
                    else if (columnDefinition == SiblingColumn)
                    {
#if CSHTML5_NOT_SUPPORTED
                        SetColumnWidth(SiblingColumn, horizontalChange * -1, GridUnitType.Star);
#else
                        SetColumnWidth(SiblingColumn, horizontalChange * -1, GridUnitType.Star, columnDefinitionToActualWidthDictionary); // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
                    }
                    else if (IsStarColumn(columnDefinition))
                    {
#if CSHTML5_NOT_SUPPORTED
                        columnDefinition.Width = new GridLength(columnDefinition.ActualWidth, GridUnitType.Star);
#else
                        columnDefinition.Width = new GridLength(columnDefinitionToActualWidthDictionary[columnDefinition], GridUnitType.Star); // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
                    }
                }
            }

            return false;
        }

#if !CSHTML5_NOT_SUPPORTED
        static Dictionary<ColumnDefinition, double> GetColumnDefinitionToActualWidthDictionary(Grid grid)
        {
            // This method is required for best results in CSHTML5, because we need to make a "snapshot" of all the "ActualWidths" of the columns BEFORE starting to modify them. The reason is that, in CSHTML5, as you start changing columns, the ActualWidth is reflected immediately, whereas in Silverlight, ActualWidth is recalculated only after all the changes have been made (ie. when the UI thread is free).
            Dictionary<ColumnDefinition, double> output = new Dictionary<ColumnDefinition, double>();
            foreach (var columnDefinition in grid.ColumnDefinitions)
            {
                output.Add(columnDefinition, columnDefinition.ActualWidth);
            }
            return output;
        }

        static Dictionary<RowDefinition, double> GetRowDefinitionToActualHeightDictionary(Grid grid)
        {
            // This method is required for best results in CSHTML5, because we need to make a "snapshot" of all the "ActualHeights" of the rows BEFORE starting to modify them. The reason is that, in CSHTML5, as you start changing rows, the ActualHeight is reflected immediately, whereas in Silverlight, ActualHeight is recalculated only after all the changes have been made (ie. when the UI thread is free).
            Dictionary<RowDefinition, double> output = new Dictionary<RowDefinition, double>();
            foreach (var rowDefinition in grid.RowDefinitions)
            {
                output.Add(rowDefinition, rowDefinition.ActualHeight);
            }
            return output;
        }
#endif
    }
}
