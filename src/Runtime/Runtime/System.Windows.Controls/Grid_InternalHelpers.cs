

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
        internal const string INTERNAL_CSSGRID_MS_PREFIX = "-ms-";

        private static bool? _isGridSupported;
        private static bool? _isMSGrid;

#if BRIDGE
        [Bridge.Template("document.isGridSupported")]
#endif
        internal static bool isCSSGridSupported()
        {
            if (!_isGridSupported.HasValue)
            {
                _isGridSupported = (bool)((CSHTML5.Types.INTERNAL_JSObjectReference)(CSHTML5.Interop.ExecuteJavaScript("document.isGridSupported"))).Value;
            }
            return _isGridSupported.Value;
        }

#if BRIDGE
        [Bridge.Template("document.isMSGrid")]
#endif
        internal static bool isMSGrid()
        {
            if (!_isMSGrid.HasValue)
            {
                _isMSGrid = (bool)((CSHTML5.Types.INTERNAL_JSObjectReference)(CSHTML5.Interop.ExecuteJavaScript("document.isMSGrid"))).Value;
            }
            return _isMSGrid.Value;
        }

#if BRIDGE
        [Bridge.Template("document.gridSupport")]
#endif
        private static string GetGridSupportAsString()
        {
            return CSHTML5.Interop.ExecuteJavaScript("document.gridSupport").ToString();
        }

        public static List<List<INTERNAL_CellDefinition>> CalculateNewCellsStructure(ColumnDefinitionCollection columnDefinitionsOrNull, RowDefinitionCollection rowDefinitionsOrNull)
        {
            var output = new List<List<INTERNAL_CellDefinition>>();
            int rowIndex = 0;
            foreach (RowDefinition row in IterateThroughCollectionOrCreateNewElementIfCollectionIsNullOrEmpty(rowDefinitionsOrNull))
            {
                int columnIndex = 0;
                var currentRow = new List<INTERNAL_CellDefinition>();
                output.Add(currentRow);
                foreach (ColumnDefinition column in IterateThroughCollectionOrCreateNewElementIfCollectionIsNullOrEmpty(columnDefinitionsOrNull))
                {
                    currentRow.Add(new INTERNAL_CellDefinition() { Row = rowIndex, Column = columnIndex });
                    ++columnIndex;
                }
                ++rowIndex;
            }

            return output;
        }

        private static IEnumerable<RowDefinition> IterateThroughCollectionOrCreateNewElementIfCollectionIsNullOrEmpty(RowDefinitionCollection rowDefinitionsOrNull)
        {
            if (rowDefinitionsOrNull != null && rowDefinitionsOrNull.Count > 0)
            {
                foreach (RowDefinition row in rowDefinitionsOrNull)
                {
                    yield return row;
                }
            }
            else
            {
                yield return new RowDefinition();
            }
        }

        private static IEnumerable<ColumnDefinition> IterateThroughCollectionOrCreateNewElementIfCollectionIsNullOrEmpty(ColumnDefinitionCollection columnDefinitionsOrNull)
        {
            if (columnDefinitionsOrNull != null && columnDefinitionsOrNull.Count > 0)
            {
                foreach (ColumnDefinition column in columnDefinitionsOrNull)
                {
                    yield return column;
                }
            }
            else
            {
                yield return new ColumnDefinition();
            }
        }

        public static string ConvertGridLengthToCssString(GridLength gridLength, double minSize, string signUsedForPercentage = "%")
        {
            if (gridLength.IsAuto)
            {
                return "auto";
            }
            else if (gridLength.IsStar)
            {
                bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
                if (isCSSGrid)
                {
                    string minWidthString = (double.IsNaN(minSize) || double.IsInfinity(minSize) ? 
                        "0px" : minSize.ToInvariantString() + "px");
                    return "minmax(" + minWidthString + ", " + gridLength.Value.ToInvariantString() + signUsedForPercentage + ")";
                }
                else
                {
                    //todo: implement MinWidth / MinHeight (minSize) for legacy non-CSS-Grid compatible browsers
                    return gridLength.Value.ToInvariantString() + signUsedForPercentage;
                }
            }
            else
            {
                return gridLength.Value.ToInvariantString() + "px";
            }
        }

        public static object GenerateDomElementsForGrid_NonCSSVersion(Grid grid, List<List<INTERNAL_CellDefinition>> cellsStructure, ColumnDefinitionCollection columnDefinitionsOrNull, RowDefinitionCollection rowDefinitionsOrNull, object parentRef, out List<ColumnDefinition> normalizedColumnDefinitions, out List<RowDefinition> normalizedRowDefinitions)
        {
            //Note: the "cellsStructure" parameter will be modified because we will store the DOM elements that correspond to each cell.

            // Create "<table>" element:
            var table = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("table", parentRef, grid);
            var tableStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(table);
            tableStyle.borderCollapse = "collapse";
            tableStyle.position = "relative";
            tableStyle.height = "100%";
            tableStyle.width = "100%";
            //the line below doesn't allow for Row.Height = Auto: We use another solution: make as if any fixed-size row/column is Auto with a div the size of what is expected.
            //table.style.tableLayout = "fixed";

            NormalizeWidthAndHeightPercentages(grid, columnDefinitionsOrNull, rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);

            //we create the elements:
            int currentRowIndex = 0;
            foreach (RowDefinition rowDefinition in normalizedRowDefinitions)
            {
                var currentRow = cellsStructure[currentRowIndex];

                // Create and append "<tr>" element:
                var tr = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("tr", table, grid);

                int currentColumnIndex = 0;
                foreach (ColumnDefinition columnDefinition in normalizedColumnDefinitions)
                {
                    var currentCell = currentRow[currentColumnIndex];

                    if (!currentCell.IsOverlapped) // "Overlapped" means that another cell is overlapping this cell due to the "ColumnSpan" or "RowSpan".
                    {
                        // Create and append "<td>" element:
                        var td = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("td", tr, grid);
                        var tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(td);
                        tdStyle.display = (columnDefinition.Visibility == Visibility.Visible) ? "table-cell" : "none";
                        
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "rowspan", currentCell.RowSpan);
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "colspan", currentCell.ColumnSpan);

                        ApplyGridLinesValues(grid, currentRowIndex, currentColumnIndex, tdStyle);

                        //we create the div to contain the children:
                        var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", td, grid);
                        var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
                        divStyle.position = "relative";

                        //we fill the cell's definition in the structure:
                        currentCell.DomElement = div;
                        currentCell.ColumnDomElement = td;
                    }
                    currentCell.RowDomElement = tr;
                    currentColumnIndex++;
                }
                currentRowIndex++;
            }

            return table;
        }

        internal static void RefreshGridLines(Grid grid, List<List<INTERNAL_CellDefinition>> cellsStructure, ColumnDefinitionCollection columnDefinitions, RowDefinitionCollection rowDefinitions)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (!isCSSGrid)
            {
                int currentRowIndex = 0;
                foreach (RowDefinition rowDefinition in rowDefinitions) //todo: see if removing the normalization from here didn't break anything (case where there are no rows or no columns defined, Normalization adds one)
                {
                    var currentRow = cellsStructure[currentRowIndex];
                    int currentColumnIndex = 0;
                    foreach (ColumnDefinition columnDefinition in columnDefinitions)
                    {
                        var currentCell = currentRow[currentColumnIndex];

                        if (!currentCell.IsOverlapped) // "Overlapped" means that another cell is overlapping this cell due to the "ColumnSpan" or "RowSpan".
                        {
                            var td = currentCell.ColumnDomElement;
                            var tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(td);
                            ApplyGridLinesValues(grid, currentRowIndex, currentColumnIndex, tdStyle);
                        }
                        currentColumnIndex++;
                    }
                    currentRowIndex++;
                }
            }
        }

#if BRIDGE
        internal static void ApplyGridLinesValues(Grid grid, int rowIndex, int columnIndex, dynamic tdStyle)
#else
        internal static void ApplyGridLinesValues(Grid grid, int rowIndex, int columnIndex, INTERNAL_HtmlDomStyleReference tdStyle)
#endif
        {
            if (grid.INTERNAL_StringToSetVerticalGridLinesInCss != null)
            {
                tdStyle.borderRight = grid.INTERNAL_StringToSetVerticalGridLinesInCss;
                if (columnIndex == 0)
                {
                    tdStyle.borderLeft = grid.INTERNAL_StringToSetVerticalGridLinesInCss;
                }
            }
            if (grid.INTERNAL_StringToSetHorizontalGridLinesInCss != null)
            {
                tdStyle.borderBottom = grid.INTERNAL_StringToSetHorizontalGridLinesInCss;
                if (rowIndex == 0)
                {
                    tdStyle.borderTop = grid.INTERNAL_StringToSetHorizontalGridLinesInCss;
                }
            }
        }

        internal static void NormalizeWidthAndHeightPercentages(Grid grid, ColumnDefinitionCollection columnDefinitionsOrNull, RowDefinitionCollection rowDefinitionsOrNull, out List<ColumnDefinition> normalizedColumnDefinitions, out List<RowDefinition> normalizedRowDefinitions)
        {
            if (isCSSGridSupported())
            {
                NormalizeWidthAndHeightPercentages_CSSVersion(grid, columnDefinitionsOrNull, rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);
            }
            else
            {
                NormalizeWidthAndHeightPercentages_NonCSSVersion(grid, columnDefinitionsOrNull, rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);
            }
        }

        private static void NormalizeWidthAndHeightPercentages_NonCSSVersion(Grid grid, ColumnDefinitionCollection columnDefinitionsOrNull, RowDefinitionCollection rowDefinitionsOrNull, out List<ColumnDefinition> normalizedColumnDefinitions, out List<RowDefinition> normalizedRowDefinitions)
        {
            normalizedColumnDefinitions = new List<ColumnDefinition>();
            normalizedRowDefinitions = new List<RowDefinition>();
            double sumOfWidthPercentages = 0;
            double sumOfHeightPercentages = 0;
            if (columnDefinitionsOrNull != null)
            {
                foreach (ColumnDefinition columnDefinition in columnDefinitionsOrNull)
                {
                    if (columnDefinition.Width.IsStar)
                        sumOfWidthPercentages += columnDefinition.Width.Value;
                }
            }
            if (rowDefinitionsOrNull != null)
            {
                foreach (RowDefinition rowDefinition in rowDefinitionsOrNull)
                {
                    if (rowDefinition.Height.IsStar)
                        sumOfHeightPercentages += rowDefinition.Height.Value;
                }
            }
            if (columnDefinitionsOrNull != null)
            {
                foreach (ColumnDefinition columnDefinition in columnDefinitionsOrNull)
                {
                    var normalizedColumnDefinition = columnDefinition;
                    if (columnDefinition.Width.IsStar)
                    {
                        normalizedColumnDefinition = columnDefinition.Clone();
                        normalizedColumnDefinition.Width = new GridLength(normalizedColumnDefinition.Width.Value * 100d / sumOfWidthPercentages, GridUnitType.Star);
                    }
                    normalizedColumnDefinitions.Add(normalizedColumnDefinition);
                }
            }
            if (rowDefinitionsOrNull != null)
            {
                foreach (RowDefinition rowDefinition in rowDefinitionsOrNull)
                {
                    var normalizedRowDefinition = rowDefinition;
                    if (rowDefinition.Height.IsStar)
                    {
                        normalizedRowDefinition = rowDefinition.Clone();
                        normalizedRowDefinition.Height = new GridLength(normalizedRowDefinition.Height.Value * 100d / sumOfHeightPercentages, GridUnitType.Star);
                    }
                    normalizedRowDefinitions.Add(normalizedRowDefinition);
                }
            }

            // If there is no row, add one. If there is no column, add one.
            if (normalizedColumnDefinitions.Count == 0)
                normalizedColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100d, GridUnitType.Star), Parent = grid });
            if (normalizedRowDefinitions.Count == 0)
                normalizedRowDefinitions.Add(new RowDefinition() { Height = new GridLength(100d, GridUnitType.Star), Parent = grid });
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
                    if (col.Width.IsStar && col.Width.Value < smallestColumnStarValue)
                    {
                        smallestColumnStarValue = col.Width.Value;
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
                    if (row.Height.IsStar && row.Height.Value < smallestRowStarValue)
                    {
                        smallestRowStarValue = row.Height.Value;
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

        /// <summary>
        /// Refreshes the height of all the rows (and their columns) of the given grid
        /// </summary>
        /// <param name="grid">the grid of which rows' heights we want to refresh</param>
        /// <param name="normalizedRowDefinitions">the normalized(aka. their stars add up to 100%) version of the rows' definitions, null value is allowed, it will be calculated in this method.</param>
        internal static void RefreshAllRowsHeight_NonCSSVersion(Grid grid, List<RowDefinition> normalizedRowDefinitions = null)
        {
            List<ColumnDefinition> normalizedColumnDefinitions;
            if (normalizedRowDefinitions == null) //it hasn't been calculated yet
            {
                NormalizeWidthAndHeightPercentages(grid, grid._columnDefinitionsOrNull, grid._rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);
            }
            bool hasOnlyOneRow = normalizedRowDefinitions.Count == 1;
            bool hasStar = false;
            for (int i = 0; i < normalizedRowDefinitions.Count; ++i)
            {
                RefreshRowHeight_NonCSSVersion(grid, i, hasOnlyOneRow, normalizedRowDefinitions);
                if (normalizedRowDefinitions[i].Height.IsStar)
                {
                    hasStar = true;
                }
            }
            if (!hasStar)
            {
                AddFillingStarRow_NonCSSVersion(grid);
            }
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
            bool isMsGrid = Grid_InternalHelpers.isMSGrid();
            if (!isMsGrid)
            {
                domElementStyle.gridTemplateRows = rowsAsString;
            }
            else
            {
                domElementStyle.msGridRows = rowsAsString;
            }
        }

        internal static void RefreshRowHeight_NonCSSVersion(Grid grid, int rowIndex, bool isTheOnlyRow, List<RowDefinition> normalizedRowsDefinitionsIfNeeded = null)
        {
            bool clipToBounds = grid.ClipToBounds;
            var row = grid._currentCellsStructure[rowIndex];
            RowDefinition rowDefinition;
            if (normalizedRowsDefinitionsIfNeeded != null)
            {
                rowDefinition = normalizedRowsDefinitionsIfNeeded[rowIndex];
            }
            else
            {
                rowDefinition = grid._rowDefinitionsOrNull[rowIndex];
            }
            string rowHeight = ConvertGridLengthToCssString(rowDefinition.Height, rowDefinition.MinHeight);
            string internalElementForRowHeight = "100%";
            if (rowHeight.EndsWith("px"))
            {
                internalElementForRowHeight = rowHeight;
                rowHeight = "auto";
            }
            else if (isTheOnlyRow && !double.IsNaN(grid.Height))
            {
                internalElementForRowHeight = grid.Height.ToInvariantString() + "px";
                rowHeight = "auto";
            }
            //we set the height of the row:
            var rowDomElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(row[0].RowDomElement);
            rowDomElementStyle.height = rowHeight;

            //we set the height of the columns inside the row:
            foreach (INTERNAL_CellDefinition cell in row)
            {
                if (!cell.IsOverlapped)
                {
                    var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.DomElement);

                    string domElementStyleAppliedValue = internalElementForRowHeight;

                    //domElementStyle.height = internalElementForRowHeight;
                    if (internalElementForRowHeight.EndsWith("px")) //todo: check if this should be if(rowHeight != "auto") in certain cases
                    {
                        //if (!INTERNAL_HtmlDomManager.IsInternetExplorer()) // Note: we don't do this under IE10 because it causes the Showcase to not display the content of the pages. //todo: see why on other browsers we do this (cf. changeset 1701)
                        //    domElementStyle.overflowY = "hidden";
                        // The code above was commented on 2017.04.07 due to the fact that it caused Grids to crop the content in excess of their grid cell, which was not wanted in case of negative margins or in case of Canvas control inside the grid cell. (cf. ZenDesk #553).
                        // We replaced it with the code below, which crops only if "ClipToBounds" is "True" (note: in XAML it works differently because ClipToBounds does not apply to grid cells, but "ClipToBounds" was the existing property that we found had the closest meaning for our users to use).
                        if (clipToBounds)
                            domElementStyle.overflowX = "hidden";

                        //domElementStyle.height = internalElementForRowHeight; //was 100% but I don't see how this would be good
                    }

                    var columnDomElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.ColumnDomElement);
                    columnDomElementStyle.height = "inherit"; //did this because apparently, height = 100% makes it take 100% of the size of the Grid instead of 100% of the size of the row... except in IE which simply doesn't care about the size we set to the td.
                    if (rowHeight == "auto")
                    {
                        //columnDomElementStyle.height = "auto";
                        if (!internalElementForRowHeight.EndsWith("px"))
                        {
                            domElementStyleAppliedValue = "auto";
                            //domElementStyle.height = "auto";
                        }
                    }
                    else
                    {
                        //columnDomElementStyle.height = "100%";
                        domElementStyleAppliedValue = "100%";
                        //domElementStyle.height = "100%";
                    }

                    if (cell.RowSpan > 1 && (domElementStyleAppliedValue.EndsWith("px") || domElementStyleAppliedValue == "auto")) //todo: check if this should be if(rowHeight != "auto") in certain cases
                    {
                        //we need to add the size of the other rows:
                        RefreshCellWithSpanHeight(grid, cell, ref domElementStyleAppliedValue);
                    }

                    domElementStyle.height = domElementStyleAppliedValue;
                }
            }
        }

        /// <summary>
        /// Sets the size of the dom element of the cell ONLY IN THE CASE WHERE THE CELL HAS A ROW SPAN.
        /// If any involved row has a star value, it will set it to 100%,
        /// otherwise, if they all have an absolute size, it will set it to the sum of those sizes,
        /// otherwise (if there is any auto sized row), it won't do anything.
        /// </summary>
        internal static void RefreshCellWithSpanHeight(Grid grid, INTERNAL_CellDefinition cell, ref string valueToApplyToDomElementStyle)
        {
            if (cell.RowSpan > 1)
            {
                //dynamic domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.DomElement);

                int rowIndex = cell.Row;
                double heightInPixels = 0;
                bool isAbsoluteHeight = true;
                for (int i = 0; i < cell.RowSpan; ++i)
                {
                    var currentRowDefinition = grid._rowDefinitionsOrNull[rowIndex];

                    if (currentRowDefinition.Height.IsAbsolute)
                    {
                        heightInPixels += currentRowDefinition.Height.Value;
                    }
                    else
                    {
                        isAbsoluteHeight = false;
                        break;
                    }
                }
                if (isAbsoluteHeight)
                {
                    //internalElementForRowHeight = heightInPixels + "px";
                    valueToApplyToDomElementStyle = heightInPixels + "px";
                }
                else //todo: we can have a problem here when a row is completely overlapped when either said row or the overlapping one have a hard coded height but not both)
                {
                    valueToApplyToDomElementStyle = "100%"; //was internalElementForRowHeight with the comment: "was 100% but I don't see how this would be good"
                }
            }
        }

        internal static void RefreshCellWidthAndHeight(Grid grid, INTERNAL_CellDefinition cell, ColumnDefinition normalizedColumnDefinition, bool isTheOnlyColumn, RowDefinition normalizedRowDefinition, bool isTheOnlyRow)
        {
            //We set the height of the div according to the rowdefinition method:
            //Note: this first part is very similar to the RefreshRowHeight_NonCSSVersion.
            //todo: factorize the common parts ([getting rowHeight and internalElementForRowHeight] and [setting the cell's dom element's style]).
            bool clipToBounds = grid.ClipToBounds;

            string rowHeight = ConvertGridLengthToCssString(normalizedRowDefinition.Height, normalizedRowDefinition.MinHeight);
            string internalElementForRowHeight = "100%";
            if (rowHeight.EndsWith("px"))
            {
                internalElementForRowHeight = rowHeight;
                rowHeight = "auto";
            }
            else if (isTheOnlyRow && !double.IsNaN(grid.Height))
            {
                internalElementForRowHeight = grid.Height.ToInvariantString() + "px";
                rowHeight = "auto";
            }

            //we set the height of the cell based on the row's height
            if (!cell.IsOverlapped)
            {
                var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.DomElement);

                string domElementStyleAppliedValue = internalElementForRowHeight;

                //domElementStyle.height = internalElementForRowHeight;
                if (internalElementForRowHeight.EndsWith("px"))
                {
                    //if (!INTERNAL_HtmlDomManager.IsInternetExplorer()) // Note: we don't do this under IE10 because it causes the Showcase to not display the content of the pages. //todo: see why on other browsers we do this (cf. changeset 1701)
                    //    domElementStyle.overflowY = "hidden";
                    // The code above was commented on 2017.04.07 due to the fact that it caused Grids to crop the content in excess of their grid cell, which was not wanted in case of negative margins or in case of Canvas control inside the grid cell. (cf. ZenDesk #553).
                    // We replaced it with the code below, which crops only if "ClipToBounds" is "True" (note: in XAML it works differently because ClipToBounds does not apply to grid cells, but "ClipToBounds" was the existing property that we found had the closest meaning for our users to use).
                    if (clipToBounds)
                        domElementStyle.overflowX = "hidden";

                    //domElementStyle.height = internalElementForRowHeight; //was 100% but I don't see how this would be good
                }

                var columnDomElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.ColumnDomElement);
                columnDomElementStyle.height = "inherit"; //did this because apparently, height = 100% makes it take 100% of the size of the Grid instead of 100% of the size of the row... except in IE which simply doesn't care about the size we set to the td.
                if (rowHeight == "auto")
                {
                    //columnDomElementStyle.height = "auto";
                    if (!internalElementForRowHeight.EndsWith("px"))
                    {
                        domElementStyleAppliedValue = "auto";
                        //domElementStyle.height = "auto";
                    }
                }
                else
                {
                    //columnDomElementStyle.height = "100%";
                    domElementStyleAppliedValue = "100%";
                    //domElementStyle.height = "100%";
                }

                if (cell.RowSpan > 1 && (internalElementForRowHeight.EndsWith("px") || internalElementForRowHeight == "auto")) //todo: check if this should be if(rowHeight != "auto") in certain cases
                {
                    //we need to add the size of the other rows:
                    RefreshCellWithSpanHeight(grid, cell, ref domElementStyleAppliedValue);
                    domElementStyle.height = domElementStyleAppliedValue;
                }
            }

            //We set the width:
            //Note: this part is very similar to the RefreshColumnWidth_NonCSSVersion method.
            //todo: factorize the common parts ([getting columnWidth and internalElementForColumnWidth] and [setting the cell's dom element's style]).

            if (!cell.IsOverlapped)
            {
                var tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.ColumnDomElement);
                string columnWidth = ConvertGridLengthToCssString(normalizedColumnDefinition.Width, normalizedColumnDefinition.MinWidth);
                string internalElementForColumnWidth = "100%";
                if (columnWidth.EndsWith("px"))
                {
                    internalElementForColumnWidth = columnWidth;
                    columnWidth = "auto";
                }
                else if (isTheOnlyColumn && !double.IsNaN(grid.Width))
                {
                    internalElementForColumnWidth = grid.Width.ToInvariantString() + "px";
                    columnWidth = "auto";
                }

                tdStyle.width = columnWidth;
                tdStyle.position = "relative";
                tdStyle.padding = "0px";

                var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.DomElement);
                domElementStyle.width = internalElementForColumnWidth;

                if (internalElementForColumnWidth.EndsWith("px"))//todo: check if this should be if(columnwidth != "auto") in certain cases
                {
                    //if (!INTERNAL_HtmlDomManager.IsInternetExplorer()) // Note: we don't do this under IE10 because it causes the Showcase to not display the content of the pages. //todo: see why on other browsers we do this (cf. changeset 1701)
                    //    domElementStyle.overflowX = "hidden";
                    // The code above was commented on 2017.04.07 due to the fact that it caused Grids to crop the content in excess of their grid cell, which was not wanted in case of negative margins or in case of Canvas control inside the grid cell. (cf. ZenDesk #553).
                    // We replaced it with the code below, which crops only if "ClipToBounds" is "True" (note: in XAML it works differently because ClipToBounds does not apply to grid cells, but "ClipToBounds" was the existing property that we found had the closest meaning for our users to use).
                    if (clipToBounds)
                        domElementStyle.overflowX = "hidden";
                }
            }
        }

        internal static void RefreshAllColumnsWidth_NonCSSVersion(Grid grid, List<ColumnDefinition> normalizedColumnDefinitions = null)
        {
            List<RowDefinition> normalizedRowDefinitions;
            if (normalizedColumnDefinitions == null) //it hasn't been calculated yet
            {
                NormalizeWidthAndHeightPercentages(grid, grid._columnDefinitionsOrNull, grid._rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);
            }
            bool isTheOnlyColumn = normalizedColumnDefinitions.Count == 1;
            bool hasStar = false;
            for (int i = 0; i < normalizedColumnDefinitions.Count; ++i)
            {
                RefreshColumnWidth_NonCSSVersion(grid, i, isTheOnlyColumn, normalizedColumnDefinitions);
                if (normalizedColumnDefinitions[i].Width.IsStar)
                {
                    hasStar = true;
                }
            }
            if (!hasStar)
            {
                AddFillingStarColumn_NonCSSVersion(grid);
            }
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
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.style.overflow = 'hidden'", child.INTERNAL_OuterDomElement);
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.setAttribute('data-isCollapsedDueToHiddenColumn', true)", child.INTERNAL_OuterDomElement);
                            }
                            else
                            {
                                // Note: we set to Visible only if it was previously Hidden due to the fact that a Grid column is hidden, to avoid conflicts such as replacing the "overflow" property set by the ScrollViewer or by the "ClipToBounds" property.
                                // setAttribute('data-isCollapsedDueToHiddenColumn', false)
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"document.setGridCollapsedDuetoHiddenColumn($0)",
                                    ((INTERNAL_HtmlDomElementReference)child.INTERNAL_OuterDomElement).UniqueIdentifier);
                            }
                        }
                    }
                }
            }

            // todo: if the grid is not limited by its parent elements in width and height and has no height/width defined, put "auto" for all corresponding star type gridLength

            if (isFirstColumn)
                columnsAsString = "minmax(0px, 1fr)"; //this means that there is no columnDefinition or it is empty --> we want to put minmax(0px, 1fr) so that the grid actually limits its content size.

            var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(grid._innerDiv);
            bool isMsGrid = Grid_InternalHelpers.isMSGrid();
            if (!isMsGrid)
            {
                domElementStyle.gridTemplateColumns = columnsAsString;
            }
            else
            {
                domElementStyle.msGridColumns = columnsAsString;
            }
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

        internal static void RefreshColumnWidth_NonCSSVersion(Grid grid, int ColumnIndex, bool isTheOnlyColumn, List<ColumnDefinition> normalizedColumnsDefinitionsIfNeeded = null)
        {
            bool clipToBounds = grid.ClipToBounds;
            var column = new List<INTERNAL_CellDefinition>();

            foreach (var row in grid._currentCellsStructure)
            {
                column.Add(row[ColumnIndex]);
            }

            ColumnDefinition columnDefinition;
            if (normalizedColumnsDefinitionsIfNeeded != null)
            {
                columnDefinition = normalizedColumnsDefinitionsIfNeeded[ColumnIndex];
            }
            else
            {
                columnDefinition = grid._columnDefinitionsOrNull[ColumnIndex];
            }

            foreach (INTERNAL_CellDefinition cell in column)
            {
                if (!cell.IsOverlapped)
                {
                    var tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.ColumnDomElement);
                    string columnWidth = ConvertGridLengthToCssString(columnDefinition.Width, columnDefinition.MinWidth);
                    string internalElementForColumnWidth = "100%";
                    if (columnWidth.EndsWith("px"))
                    {
                        internalElementForColumnWidth = columnWidth;
                        columnWidth = "auto";
                    }
                    else if (isTheOnlyColumn && !double.IsNaN(grid.Width))
                    {
                        internalElementForColumnWidth = grid.Width.ToInvariantString() + "px";
                        columnWidth = "auto";
                    }

                    tdStyle.width = columnWidth;
                    tdStyle.position = "relative";
                    tdStyle.padding = "0px";

                    var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.DomElement);
                    domElementStyle.width = internalElementForColumnWidth;

                    if (internalElementForColumnWidth.EndsWith("px"))//todo: check if this should be if(columnwidth != "auto") in certain cases
                    {
                        //if (!INTERNAL_HtmlDomManager.IsInternetExplorer()) // Note: we don't do this under IE10 because it causes the Showcase to not display the content of the pages. //todo: see why on other browsers we do this (cf. changeset 1701)
                        //    domElementStyle.overflowX = "hidden";
                        // The code above was commented on 2017.04.07 due to the fact that it caused Grids to crop the content in excess of their grid cell, which was not wanted in case of negative margins or in case of Canvas control inside the grid cell. (cf. ZenDesk #553).
                        // We replaced it with the code below, which crops only if "ClipToBounds" is "True" (note: in XAML it works differently because ClipToBounds does not apply to grid cells, but "ClipToBounds" was the existing property that we found had the closest meaning for our users to use).
                        if (clipToBounds)
                            domElementStyle.overflowX = "hidden";

                        //todo: (?) the same as in RefreshRowHeight_NonCSSVersion with the RowSpan
                    }
                }
            }
        }

        internal static void AddFillingStarColumn_NonCSSVersion(Grid grid)
        {
            if (grid._currentCellsStructure != null)
            {
                foreach (var row in grid._currentCellsStructure)
                {
                    if (row != null && row[0] != null)
                    {
                        INTERNAL_CellDefinition cell = row[0];
                        if (cell.RowDomElement != null)
                        {
                            var td = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("td", cell.RowDomElement, grid);
                            var tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(td);

                            tdStyle.display = "table-cell";
                            tdStyle.width = "100%";
                        }
                    }
                }
            }
        }

        internal static void AddFillingStarRow_NonCSSVersion(Grid grid)
        {
            if (grid._currentDomTable != null)
            {
                var tr = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("tr", grid._currentDomTable, grid);
                var trStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(tr);
                trStyle.height = "100%";
            }
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
            if (isCSSGridSupported())
            {
                RefreshColumnVisibility_CSSVersion(grid, columnDefinition, newVisibility);
            }
            else
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(grid))
                {
                    var column = new List<INTERNAL_CellDefinition>();
                    int columnIndex = grid._columnDefinitionsOrNull.IndexOf(columnDefinition);

                    foreach (var row in grid._currentCellsStructure)
                    {
                        column.Add(row[columnIndex]);
                    }

                    foreach (var cell in column)
                    {
                        if (!cell.IsOverlapped)
                        {
                            var tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell.ColumnDomElement);
                            if (newVisibility == Visibility.Collapsed)
                            {
                                if (((string)tdStyle.display != "none"))
                                {
                                    cell.INTERNAL_previousValueOfDisplayCssProperty = (string)tdStyle.display;
                                    tdStyle.display = "none";
                                }
                            }
                            else
                            {
                                tdStyle.display = cell.INTERNAL_previousValueOfDisplayCssProperty;
                            }
                        }
                    }
                }
            }
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

        internal static Dictionary<UIElement, INTERNAL_CellDefinition> HandleSpansInCellsStructureAndReturnRedirectedElements(List<List<INTERNAL_CellDefinition>> newCellsStructure, UIElementCollection Children)
        {
            var redirectedElements = new Dictionary<UIElement, INTERNAL_CellDefinition>(); //this is to know which children to update (in regards to their INTERNAL_SpanParentCell field) if the structure actually needs to be updated

            // Determine the MIN/MAX cell coordinates by looking at the grid structure:
            int maxRowIndex = newCellsStructure.Count - 1;
            int maxColumnIndex = newCellsStructure[0].Count - 1;

            // Iterate throw all the children of the Grid:
            foreach (UIElement uielement in Children)
            {
                //todo: (?) child.INTERNAL_SpanParentCell = null; I think we should do this when OnDetached is called

                // Get the ROW/COLUMN coordinates of the element:
                int rowIndex = Grid.GetRow(uielement);
                int columnIndex = Grid.GetColumn(uielement);

                // Make sure that the those coordinates are within the [0,max] range:
                rowIndex = EnsureValueIsBetweenMinAndMax(value: rowIndex, min: 0, max: maxRowIndex);
                columnIndex = EnsureValueIsBetweenMinAndMax(value: columnIndex, min: 0, max: maxColumnIndex);

                // Get the "cell definition" of the top-left cell occupied by the current element. The "cell definition" is the structure that stores information for each cell of the table:
                var cellDefinition = newCellsStructure[rowIndex][columnIndex]; //Note: childRow and childColumn should always have a correct value.

                if (cellDefinition.IsOverlapped) //Note: "IsOverlapped" means that another cell is overlapping this cell due to the "ColumnSpan" or "RowSpan".
                {
                    //----------------------
                    // The cell is "covered" by another cell
                    //----------------------

                    //child.INTERNAL_SpanParentCell = cell; //todo: see if it would be possible to set it directly to cell.ParentCell and if yes, make sure we do not make useless tests.
                    redirectedElements.Add(uielement, cellDefinition); //todo: see if it would be possible to set it directly to cell.ParentCell and if yes, make sure we do not make useless tests.
                }
                else
                {
                    //----------------------
                    // The cell is NOT covered by another cell
                    //----------------------

                    // Get the coordinates of the last row/column of the spanned cell:
                    int spanLastRowIndex = rowIndex + Grid.GetRowSpan(uielement) - 1;
                    int spanLastColumnIndex = columnIndex + Grid.GetColumnSpan(uielement) - 1;

                    // Make sure that those coordinates are within the [ROWINDEX,max] and [COLUMNINDEX,max] range:
                    spanLastRowIndex = EnsureValueIsBetweenMinAndMax(value: spanLastRowIndex, min: rowIndex, max: maxRowIndex);
                    spanLastColumnIndex = EnsureValueIsBetweenMinAndMax(value: spanLastColumnIndex, min: columnIndex, max: maxColumnIndex);

                    INTERNAL_CellDefinition parentCell = null;
                    bool isTopLeftCellOfSpannedArea = true;
                    bool exitForLoop = false;

                    // Iterate throw all the cells that are spanned by the current element:
                    for (int row = rowIndex; row <= spanLastRowIndex; ++row)
                    {
                        for (int column = columnIndex; column <= spanLastColumnIndex; ++column)
                        {
                            // Get the "cell structure":
                            var cellStructure2 = newCellsStructure[row][column];

                            // If one of the cells spanned by the current element is already occupied by another element, we change the current element so that it goes into the cells of that other element. The reason is that with the HTML <Table>, it is not possible to have both spanned elements and non-spanned elements in a same cell.
                            if (cellStructure2.IsOverlapped == true || (cellStructure2.IsOccupied && !isTopLeftCellOfSpannedArea))
                            {
                                if (cellStructure2.ParentCell != null)
                                {
                                    parentCell = cellStructure2.ParentCell;
                                }
                                else
                                {
                                    parentCell = cellStructure2;
                                }
                                redirectedElements.Add(uielement, parentCell);
                                //child.INTERNAL_SpanParentCell = parentCell;

                                exitForLoop = true; // This variable is used to "break" from the two nested loops. A single "break" would not be enough.

                                if (exitForLoop)
                                    break;
                            }
                            isTopLeftCellOfSpannedArea = false;
                        }
                        if (exitForLoop)
                            break;
                    }

                    // If the element has not been "redirected" to another place,
                    // we change the "cell definition" of all the cells that it
                    // spans so that they remember that they are occupied by the
                    // current element:
                    if (parentCell == null)
                    {
                        bool isTopLeftCellOfSpannedArea2 = true;
                        for (int row = rowIndex; row <= spanLastRowIndex; ++row)
                        {
                            for (int column = columnIndex; column <= spanLastColumnIndex; ++column)
                            {
                                var cellDefinition2 = newCellsStructure[row][column];
                                if (isTopLeftCellOfSpannedArea2)
                                {
                                    //------------------------------
                                    // The current cell is the top-left cell of the spanned area
                                    //------------------------------

                                    parentCell = cellDefinition2;
                                    cellDefinition2.IsOccupied = true;
                                    cellDefinition2.RowSpan = spanLastRowIndex - rowIndex + 1;
                                    cellDefinition2.ColumnSpan = spanLastColumnIndex - columnIndex + 1;
                                    isTopLeftCellOfSpannedArea2 = false;
                                }
                                else
                                {
                                    //------------------------------
                                    // The current cell is NOT the top-left cell of the spanned area
                                    //------------------------------

                                    cellDefinition2.IsOverlapped = true;
                                    cellDefinition2.ParentCell = parentCell;
                                }
                            }
                        }
                    }
                }
            }
            return redirectedElements;
        }

        /// <summary>
        /// Constraints the value to be between the specified min and max/
        /// </summary>
        /// <param name="value">The value to constraint.</param>
        /// <param name="min">The minimum allowed.</param>
        /// <param name="max">The maximum allowed.</param>
        /// <returns>The value constrained between min and max.</returns>
        private static int EnsureValueIsBetweenMinAndMax(int value, int min, int max)
        {
            if (value > max)
                value = max;

            if (value < 0)
                value = 0;

            return value;
        }
    }
}
