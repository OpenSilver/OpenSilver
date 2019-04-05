
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
using System.Windows;
#else
using Windows.Foundation;
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
        private static bool IsStarColumn(ColumnDefinition definition)
        {
            return ((GridLength)definition.GetValue(ColumnDefinition.WidthProperty)).IsStar;
        }

        private static bool IsStarRow(RowDefinition definition)
        {
            return ((GridLength)definition.GetValue(RowDefinition.HeightProperty)).IsStar;
        }

#if CSHTML5_NOT_SUPPORTED
        private void SetColumnWidth(ColumnDefinition columnDefinition, double horizontalChange, GridUnitType unitType)
#else
        private void SetColumnWidth(ColumnDefinition columnDefinition, double horizontalChange, GridUnitType unitType, Dictionary<ColumnDefinition, double> columnDefinitionToActualWidthDictionary) // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
        {
#if CSHTML5_NOT_SUPPORTED
            var newWidth = columnDefinition.ActualWidth + horizontalChange;
#else
            var newWidth = columnDefinitionToActualWidthDictionary[columnDefinition] + horizontalChange; // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
            if (newWidth > ActualWidth)
            {
                columnDefinition.Width = new GridLength(newWidth, unitType);
            }
        }

#if CSHTML5_NOT_SUPPORTED
        private bool IsValidColumnWidth(ColumnDefinition columnDefinition, double horizontalChange)
#else
        private bool IsValidColumnWidth(ColumnDefinition columnDefinition, double horizontalChange, Dictionary<ColumnDefinition, double> columnDefinitionToActualWidthDictionary) // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
        {
#if CSHTML5_NOT_SUPPORTED
            var newWidth = columnDefinition.ActualWidth + horizontalChange;
#else
            var newWidth = columnDefinitionToActualWidthDictionary[columnDefinition] + horizontalChange; // Read the comment in the method "GetColumnDefinitionToActualWidthDictionary".
#endif
            if (newWidth > ActualWidth)
            {
                return true;
            }

            return false;
        }

#if CSHTML5_NOT_SUPPORTED
        private void SetRowHeight(RowDefinition rowDefinition, double verticalChange, GridUnitType unitType)
#else
        private void SetRowHeight(RowDefinition rowDefinition, double verticalChange, GridUnitType unitType, Dictionary<RowDefinition, double> rowDefinitionToActualHeightDictionary) // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
        {
#if CSHTML5_NOT_SUPPORTED
            var newHeight = rowDefinition.ActualHeight + verticalChange;
#else
            var newHeight = rowDefinitionToActualHeightDictionary[rowDefinition] + verticalChange; // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
            if (newHeight > ActualHeight)
            {
                rowDefinition.Height = new GridLength(newHeight, unitType);
            }
        }

#if CSHTML5_NOT_SUPPORTED
        private bool IsValidRowHeight(RowDefinition rowDefinition, double verticalChange)
#else
        private bool IsValidRowHeight(RowDefinition rowDefinition, double verticalChange, Dictionary<RowDefinition, double> rowDefinitionToActualHeightDictionary) // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
        {
#if CSHTML5_NOT_SUPPORTED
            var newHeight = rowDefinition.ActualHeight + verticalChange;
#else
            var newHeight = rowDefinitionToActualHeightDictionary[rowDefinition] + verticalChange; // Read the comment in the method "GetRowDefinitionToActualHeightDictionary".
#endif
            if (newHeight > ActualHeight)
            {
                return true;
            }

            return false;
        }

        // Return the targeted Column based on the resize behavior
        private int GetTargetedColumn()
        {
            var currentIndex = Grid.GetColumn(TargetControl);
            return GetTargetIndex(currentIndex);
        }

        // Return the sibling Row based on the resize behavior
        private int GetTargetedRow()
        {
            var currentIndex = Grid.GetRow(TargetControl);
            return GetTargetIndex(currentIndex);
        }

        // Return the sibling Column based on the resize behavior
        private int GetSiblingColumn()
        {
            var currentIndex = Grid.GetColumn(TargetControl);
            return GetSiblingIndex(currentIndex);
        }

        // Return the sibling Row based on the resize behavior
        private int GetSiblingRow()
        {
            var currentIndex = Grid.GetRow(TargetControl);
            return GetSiblingIndex(currentIndex);
        }

        // Gets index based on resize behavior for first targeted row/column
        private int GetTargetIndex(int currentIndex)
        {
            switch (_resizeBehavior)
            {
                case GridResizeBehavior.CurrentAndNext:
                    return currentIndex;
                case GridResizeBehavior.PreviousAndNext:
                    return currentIndex - 1;
                case GridResizeBehavior.PreviousAndCurrent:
                    return currentIndex - 1;
                default:
                    return -1;
            }
        }

        // Gets index based on resize behavior for second targeted row/column
        private int GetSiblingIndex(int currentIndex)
        {
            switch (_resizeBehavior)
            {
                case GridResizeBehavior.CurrentAndNext:
                    return currentIndex + 1;
                case GridResizeBehavior.PreviousAndNext:
                    return currentIndex + 1;
                case GridResizeBehavior.PreviousAndCurrent:
                    return currentIndex;
                default:
                    return -1;
            }
        }

        // Checks the control alignment and Width/Height to detect the control resize direction columns/rows
        private GridResizeDirection GetResizeDirection()
        {
            GridResizeDirection direction = ResizeDirection;

            if (direction == GridResizeDirection.Auto)
            {
                // Check Width vs Height. If width and height cannot be determined, check if there is one of the sizes that is set explicitely, or check the horizontal and vertical alignments:
                Size actualSize = this.INTERNAL_GetActualWidthAndHeight();
                if (!double.IsNaN(actualSize.Width)
                    && !double.IsNaN(actualSize.Height)
                    && actualSize.Width > 0
                    && actualSize.Height > 0)
                {
                    if (actualSize.Width <= actualSize.Height)
                    {
                        direction = GridResizeDirection.Columns;
                    }
                    else
                    {
                        direction = GridResizeDirection.Rows;
                    }
                }
                else
                {
                    // When Width is set but not Height, resize Columns:
                    if (!double.IsNaN(actualSize.Width) && actualSize.Width > 0)
                    {
                        direction = GridResizeDirection.Columns;
                    }
                    // When Height is set but not Width, resize Rows:
                    else if (!double.IsNaN(actualSize.Height) && actualSize.Height > 0)
                    {
                        direction = GridResizeDirection.Rows;
                    }
                    // When HorizontalAlignment is Left, Right or Center, resize Columns:
                    else if (HorizontalAlignment != HorizontalAlignment.Stretch)
                    {
                        direction = GridResizeDirection.Columns;
                    }
                    // When VerticalAlignment is Top, Bottom or Center, resize Rows:
                    else if (VerticalAlignment != VerticalAlignment.Stretch)
                    {
                        direction = GridResizeDirection.Rows;
                    }
                }
            }

            return direction;
        }

        // Get the resize behavior (Which columns/rows should be resized) based on alignment and Direction
        private GridResizeBehavior GetResizeBehavior()
        {
            GridResizeBehavior resizeBehavior = ResizeBehavior;

            if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
            {
                if (_resizeDirection == GridResizeDirection.Columns)
                {
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case HorizontalAlignment.Right:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }

                // resize direction is vertical
                else
                {
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case VerticalAlignment.Bottom:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
            }

            return resizeBehavior;
        }
    }
}
