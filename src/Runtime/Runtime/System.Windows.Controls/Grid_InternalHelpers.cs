

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


using CSHTML5.Internal;
using OpenSilver.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal static class Grid_InternalHelpers
    {
        public static string ConvertGridLengthToCssString(GridLength gridLength, double minSize, string signUsedForPercentage = "%")
        {
            if (gridLength.IsAuto && !double.IsNaN(minSize) && !double.IsInfinity(minSize) && minSize > 0)
            {
                string minWidthString = minSize.ToInvariantString() + "px";
                return "minmax(" + minWidthString + ", auto)";
            }
            else if(gridLength.IsAuto)
            {
                return "auto";
            }
            else if (gridLength.IsStar)
            {
                string minWidthString = (double.IsNaN(minSize) || double.IsInfinity(minSize) ? 
                    "0px" : minSize.ToInvariantString() + "px");
                return "minmax(" + minWidthString + ", " + gridLength.Value.ToInvariantString() + signUsedForPercentage + ")";
            }
            else
            {
                return gridLength.Value.ToInvariantString() + "px";
            }
        }

        internal static void NormalizeWidthAndHeightPercentages(Grid grid, ColumnDefinitionCollection columnDefinitionsOrNull, RowDefinitionCollection rowDefinitionsOrNull, out List<ColumnDefinition> normalizedColumnDefinitions, out List<RowDefinition> normalizedRowDefinitions)
        {
            NormalizeWidthAndHeightPercentages_CSSVersion(grid, columnDefinitionsOrNull, rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);
        }

        private static void NormalizeWidthAndHeightPercentages_CSSVersion(Grid grid, ColumnDefinitionCollection columnDefinitionsOrNull, RowDefinitionCollection rowDefinitionsOrNull, out List<ColumnDefinition> normalizedColumnDefinitions, out List<RowDefinition> normalizedRowDefinitions)
        {
            //Note: The objective of this method is to make sure the "star" rows/columns occupy height/width proportionnaly relative to their star value.
            //      When using the CSS version of the grid, we use minmax(0px, myValuefr). (myValuefr is something like 2fr where 2 is myValue and fr is defined below)
            //      fr means "that amount (myValue) of the remaining space (after we counted the auto and absolute sizes that are defined in the rows/columns)"
            //      for example, 2fr will take twice the size of 1fr.
            //      This would not require normalization if this was all but Chrome and Firefox cut all the elements that have something lower than 1.
            //      for example, a row with minmax(0px, 0.5fr) containing only a border 20px high will be 10px high (particularly bad in StackPanels that do not give the grid a size to take).
            //      because of this, we need to make sure that smallest is 1fr and the proportions are kept.


            //We go through the rows and columns to determine the smallest star value s
            //We ge through them again and set the new list with the star values divided by s.


            double smallestRowStarValue = double.MaxValue;
            double smallestColumnStarValue = double.MaxValue;
            normalizedColumnDefinitions = new List<ColumnDefinition>();
            normalizedRowDefinitions = new List<RowDefinition>();

            if (columnDefinitionsOrNull != null)
            {
                //we get the smallest star value in the columns:
                foreach (ColumnDefinition col in columnDefinitionsOrNull)
                {
                    var value = col.Width.Value;
                    if (col.Width.IsStar && value < smallestColumnStarValue && value > 0)
                    {
                        smallestColumnStarValue = value;
                    }
                }

                //we "normalize" the star values (aka multiply them all so that the smallest is at 1):
                foreach (ColumnDefinition col in columnDefinitionsOrNull)
                {
                    if (col.Width.IsStar)
                    {
                        normalizedColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(col.Width.Value / smallestColumnStarValue, GridUnitType.Star), MinWidth = col.MinWidth, MaxWidth = col.MaxWidth });
                    }
                    else
                    {
                        normalizedColumnDefinitions.Add(col);
                    }
                }
            }

            //we get the smallest star value in the rows:
            if (rowDefinitionsOrNull != null)
            {
                foreach (RowDefinition row in rowDefinitionsOrNull)
                {
                    var value = row.Height.Value;
                    if (row.Height.IsStar && value < smallestRowStarValue && value > 0)
                    {
                        smallestRowStarValue = value;
                    }
                }

                //we "normalize" the star values (aka multiply them all so that the smallest is at 1):
                foreach (RowDefinition row in rowDefinitionsOrNull)
                {
                    if (row.Height.IsStar)
                    {
                        normalizedRowDefinitions.Add(new RowDefinition() { Height = new GridLength(row.Height.Value / smallestRowStarValue, GridUnitType.Star), MinHeight = row.MinHeight, MaxHeight = row.MaxHeight });
                    }
                    else
                    {
                        normalizedRowDefinitions.Add(row);
                    }
                }
            }

            // If there is no row, add one. If there is no column, add one.
            if (normalizedColumnDefinitions.Count == 0)
                normalizedColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star), Parent = grid });
            if (normalizedRowDefinitions.Count == 0)
                normalizedRowDefinitions.Add(new RowDefinition() { Height = new GridLength(1d, GridUnitType.Star), Parent = grid });
        }
        
        internal static void RefreshAllRowsHeight_CSSVersion(Grid grid, List<ColumnDefinition> normalizedColumnDefinitions = null, List<RowDefinition> normalizedRowDefinitions = null)
        {
            if (grid.IsUnderCustomLayout)
                return;

            //Note: when arriving here, we have already checked that the Grid is in the Visual Tree.

            // Create a string that defines the columns:
            string rowsAsString = "";
            bool isFirstRow = true;

            if (grid._rowDefinitionsOrNull != null)
            {
                // If not already done, normalize the sizes of the rows and columns:
                if (normalizedColumnDefinitions == null || normalizedRowDefinitions == null) //it hasn't been calculated yet
                {
                    Grid_InternalHelpers.NormalizeWidthAndHeightPercentages(grid, null, grid._rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);
                }

                bool hadStarRow = false;

                // Concatenate the string that defines the CSS "gridTemplateRows" property:
                foreach (RowDefinition rowDefinition in normalizedRowDefinitions)
                {
                    if (!hadStarRow && rowDefinition.Height.IsStar)
                        hadStarRow = true;

                    rowsAsString = rowsAsString + (!isFirstRow ? " " : "") + Grid_InternalHelpers.ConvertGridLengthToCssString(rowDefinition.Height, rowDefinition.MinHeight, signUsedForPercentage: "fr");
                    isFirstRow = false;
                }
                if (!hadStarRow) //We add a "star" row if there was none explicitely defined, since absolutely sized rows and columns are exactly their size.
                {
                    rowsAsString = rowsAsString + (!isFirstRow ? " minmax(0px, 1fr)" : "minmax(0px, 1fr)");
                }
            }

            if (isFirstRow)
                rowsAsString = "minmax(0px, 1fr)"; //this means that there is no rowDefinition or it is empty --> we want to put minmax(0px, 1fr) so that the grid actually limits its content size.

            var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(grid._innerDiv);
            domElementStyle.gridTemplateRows = rowsAsString;
        }

        internal static void RefreshAllColumnsWidth_CSSVersion(Grid grid, List<ColumnDefinition> normalizedColumnDefinitions = null, List<RowDefinition> normalizedRowDefinitions = null)
        {
            if (grid.IsUnderCustomLayout)
                return;

            //Note: when arriving here, we have already checked that the Grid is in the Visual Tree.

            // Create a string that defines the columns:
            string columnsAsString = "";
            bool isFirstColumn = true;

            if (grid._columnDefinitionsOrNull != null)
            {
                // If not already done, normalize the sizes of the rows and columns:
                if (normalizedColumnDefinitions == null || normalizedRowDefinitions == null) //it hasn't been calculated yet
                {
                    Grid_InternalHelpers.NormalizeWidthAndHeightPercentages(grid, grid._columnDefinitionsOrNull, null, out normalizedColumnDefinitions, out normalizedRowDefinitions);
                }

                bool hasStarColumn = false;
                //int indexToLastColumn = normalizedColumnDefinitions.Count;
                var collapsedColumns = new List<int>();
                int currentIndex = 0;
                // Concatenate the string that defines the CSS "gridTemplateColumns" property:
                foreach (ColumnDefinition columnDefinition in normalizedColumnDefinitions)
                {
                    if (!hasStarColumn && columnDefinition.Width.IsStar)
                        hasStarColumn = true;

                    if (columnDefinition.Visibility == Visibility.Visible)
                    {
                        columnsAsString = columnsAsString + (!isFirstColumn ? " " : "") + Grid_InternalHelpers.ConvertGridLengthToCssString(columnDefinition.Width, columnDefinition.MinWidth, signUsedForPercentage: "fr");
                    }
                    else
                    {
                        collapsedColumns.Add(currentIndex);
                        columnsAsString += (!isFirstColumn ? " 0px" : "0px");
                    }
                    ++currentIndex;

                    isFirstColumn = false;
                }
                if (!hasStarColumn) //We add a "star" column if there was none explicitely defined, since absolutely sized rows and columns are exactly their size.
                {
                    columnsAsString += (!isFirstColumn ? " minmax(0px, 1fr)" : "minmax(0px, 1fr)");
                }

                //---------------------------------------------------------------
                // Handle the column visibility (mainly changed by the DataGrid)
                //---------------------------------------------------------------
                //go through the children and set their overflow to hidden for those that are only in Collapsed columns, and to visible for the other ones:
                if (grid.HasChildren)
                {
                    foreach (UIElement child in grid.Children)
                    {
                        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(child))
                        {
                            int startColumn = Grid.GetColumn(child);
                            int endColumn = startColumn + Grid.GetColumnSpan(child) - 1; //Note: the element does not reach the index of endColumn (example: span = 1, the element is only in one column but endColumn is still biger than starColumn.
                                                                                         //Note: in the line above, "-1" because span includes the first column.
                            if (IsAllCollapsedColumns(grid, startColumn, endColumn))
                            {
                                string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(child.INTERNAL_OuterDomElement);
                                CSHTML5.Interop.ExecuteJavaScriptFastAsync($"{sElement}.style.overflow = 'hidden'");
                                CSHTML5.Interop.ExecuteJavaScriptFastAsync($"{sElement}.setAttribute('data-isCollapsedDueToHiddenColumn', true)");
                            }
                            else
                            {
                                // Note: we set to Visible only if it was previously Hidden due to the fact that a Grid column is hidden, to avoid conflicts such as replacing the "overflow" property set by the ScrollViewer or by the "ClipToBounds" property.
                                // setAttribute('data-isCollapsedDueToHiddenColumn', false)
                                CSHTML5.Interop.ExecuteJavaScriptFastAsync(
                                    $@"document.setGridCollapsedDuetoHiddenColumn(""{((INTERNAL_HtmlDomElementReference)child.INTERNAL_OuterDomElement).UniqueIdentifier}"")");
                            }
                        }
                    }
                }
            }

            // todo: if the grid is not limited by its parent elements in width and height and has no height/width defined, put "auto" for all corresponding star type gridLength

            if (isFirstColumn)
                columnsAsString = "minmax(0px, 1fr)"; //this means that there is no columnDefinition or it is empty --> we want to put minmax(0px, 1fr) so that the grid actually limits its content size.

            var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(grid._innerDiv);
            domElementStyle.gridTemplateColumns = columnsAsString;
        }

        internal static bool IsAllCollapsedColumns(Grid grid, int startColumn, int endColumn)
        {
            int maxColumnIndex = grid.ColumnDefinitions.Count - 1;
            if (maxColumnIndex > -1)
            {
                if (startColumn >= maxColumnIndex)
                {
                    startColumn = maxColumnIndex; //we make sure we are not after the last column
                }
                if (endColumn > maxColumnIndex)
                {
                    endColumn = maxColumnIndex; //we make sure we are not after the last column
                }
                for (int i = startColumn; i <= endColumn; ++i)
                {
                    if (grid.ColumnDefinitions[i].Visibility == Visibility.Visible)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        internal static void RefreshAllColumnsVisibility_CSSVersion(Grid grid)
        {
            if (grid != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(grid))
            {
                if (grid.ColumnDefinitions != null)
                {
                    Grid_InternalHelpers.RefreshAllColumnsWidth_CSSVersion(grid);
                }
            }
        }

        internal static void RefreshColumnVisibility(Grid grid, ColumnDefinition columnDefinition, Visibility newVisibility)
        {
            RefreshColumnVisibility_CSSVersion(grid, columnDefinition, newVisibility);
        }

        internal static void RefreshColumnVisibility_CSSVersion(Grid grid, ColumnDefinition columnDefinition, Visibility newVisibility)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(grid))
            {
                if (grid.INTERNAL_InnerDomElement != null)
                {
                    //refresh the grid columns:
                    Grid_InternalHelpers.RefreshAllColumnsWidth_CSSVersion(grid);
                }
            }
        }
    }
}
