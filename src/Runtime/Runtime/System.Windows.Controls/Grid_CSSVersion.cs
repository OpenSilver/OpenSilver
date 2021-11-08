

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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class Grid
    {
        private void ColumnDefinitions_CollectionChanged_CSSVersion(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (e.NewItems != null)
                {
                    Grid_InternalHelpers.RefreshAllColumnsWidth_CSSVersion(this);
                }

                if (this._isLoaded)
                {
                    Grid_InternalHelpers.RefreshAllColumnsWidth_CSSVersion(this);

                    LocallyManageChildrenChanged();
                }
            }
        }

        private void RowDefinitions_CollectionChanged_CSSVersion(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._isLoaded)
            {
                Grid_InternalHelpers.RefreshAllRowsHeight_CSSVersion(this);

                LocallyManageChildrenChanged();
            }
        }

        internal object CreateDomElement_CSSVersion(object parentRef, out object domElementWhereToPlaceChildren)
        {
#if !BRIDGE
            object outerDiv = base.CreateDomElement(parentRef, out _innerDiv);
#else
            object outerDiv = CreateDomElement_WorkaroundBridgeInheritanceBug(parentRef, out _innerDiv);
#endif
            domElementWhereToPlaceChildren = _innerDiv;

            // Set the element on which the "MaxWidth" and "MaxHeight" properties should be applied (cf. zendesk ticket #1178 where scrollbars inside the ChildWindow did not function properly):
            this.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth = _innerDiv;

            // Set the "display" CSS property:
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv);
            style.display = !Grid_InternalHelpers.isMSGrid() ? style.display = "grid" : Grid_InternalHelpers.INTERNAL_CSSGRID_MS_PREFIX + "grid";

            // Normalize the sizes of the rows and columns:
            List<ColumnDefinition> normalizedColumnDefinitions = null;
            List<RowDefinition> normalizedRowDefinitions = null;
            Grid_InternalHelpers.NormalizeWidthAndHeightPercentages(this, _columnDefinitionsOrNull, _rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);

            // Refresh the rows heights and columns widths:
            Grid_InternalHelpers.RefreshAllRowsHeight_CSSVersion(this, normalizedColumnDefinitions, normalizedRowDefinitions);
            Grid_InternalHelpers.RefreshAllColumnsWidth_CSSVersion(this, normalizedColumnDefinitions, normalizedRowDefinitions);

            return outerDiv;
        }

        internal void LocallyManageChildrenChanged_CSSVersion()
        {
            int maxRow = 0;
            int maxColumn = 0;
            if (RowDefinitions != null && RowDefinitions.Count > 0)
            {
                maxRow = RowDefinitions.Count - 1;
            }
            if (ColumnDefinitions != null && ColumnDefinitions.Count > 0)
            {
                maxColumn = ColumnDefinitions.Count - 1;
            }

            //UIElement[,] lastElements = new UIElement[maxRow + 1, maxColumn + 1];


            foreach (UIElement uiElement in Children)
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                {
                    // Note: the code between here and the "until here" comment can be read as:
                    //  ApplyRowPosition(uiElement);
                    //  ApplyColumnPosition(uiElement);
                    // It has not been implemented that way because this is slightly more efficient and we need elementRow and elementColumn afterwards.
                    int elementRow = GetRow(uiElement);
                    int elementColumn = GetColumn(uiElement);
                    MakeGridPositionCorrect(ref elementRow, maxRow);
                    MakeGridPositionCorrect(ref elementColumn, maxColumn);

                    int rowSpan = GetRowSpan(uiElement);
                    if (rowSpan < 1)
                    {
                        rowSpan = 1;
                    }
                    int columnSpan = GetColumnSpan(uiElement);
                    if (columnSpan < 1)
                    {
                        columnSpan = 1;
                    }
                    int elementLastRow = rowSpan + elementRow - 1;
                    int elementLastColumn = columnSpan + elementColumn - 1;
                    MakeGridPositionCorrect(ref elementLastRow, maxRow);
                    MakeGridPositionCorrect(ref elementLastColumn, maxColumn);

                    var style = INTERNAL_HtmlDomManager.GetFrameworkElementBoxSizingStyleForModification(uiElement);

                    style.position = "relative";

                    bool isMsGrid = Grid_InternalHelpers.isMSGrid();
                    if (!isMsGrid)
                    {
                        style.gridRowStart = (elementRow + 1).ToString(); //Note: +1 because rows start from 1 instead of 0 in js.
                        style.gridColumnStart = (elementColumn + 1).ToString(); //Note: +1 because rows start from 1 instead of 0 in js.
                        style.gridRowEnd = (elementLastRow + 2).ToString(); //Note: +1 because rows start from 1 instead of 0 in js and another + 1 because the gridRowEnd seems to be the row BEFORE WHITCH the span ends.
                        style.gridColumnEnd = (elementLastColumn + 2).ToString(); //Note: +1 because columns start from 1 instead of 0 in js and another + 1 because the gridColumnEnd seems to be the column BEFORE WHITCH the span ends.
                    }
                    else
                    {
                        //probably doesn't work, it probably requires to use msGridRow and msGridColumn and msGridRowSpan and msGridColumnSpan
                        style.msGridRow = (elementRow + 1).ToString(); //Note: +1 because rows start from 1 instead of 0 in js.
                        style.msGridColumn = (elementColumn + 1).ToString(); //Note: +1 because rows start from 1 instead of 0 in js.
                        style.msGridRowSpan = (rowSpan).ToString(); //Note: +1 because rows start from 1 instead of 0 in js and another + 1 because the gridRowEnd seems to be the row BEFORE WHITCH the span ends.
                        style.msGridColumnSpan = (columnSpan).ToString(); //Note: +1 because columns start from 1 instead of 0 in js and another + 1 because the gridColumnEnd seems to be the column BEFORE WHITCH the span ends.
                    }
                    //-------------------------until here-------------------------

                    style.pointerEvents = "none";
                    //style.position = "absolute";
                    //lastElements[elementRow, elementColumn] = uiElement;


                    //---------------------------------------------------------------
                    // Handle the column visibility (mainly changed by the DataGrid)
                    //---------------------------------------------------------------
                    if (Grid_InternalHelpers.IsAllCollapsedColumns(this, elementColumn, elementLastColumn))
                    {
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.style.overflow = 'hidden'", uiElement.INTERNAL_OuterDomElement);
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.setAttribute('data-isCollapsedDueToHiddenColumn', true)", uiElement.INTERNAL_OuterDomElement);
                    }
                    else
                    {
                        // Note: we set to Visible only if it was previously Hidden due to the fact that a Grid column is hidden, to avoid conflicts such as replacing the "overflow" property set by the ScrollViewer or by the "ClipToBounds" property.
                        //setAttribute('data-isCollapsedDueToHiddenColumn', false)
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"document.setGridCollapsedDuetoHiddenColumn($0)", 
                            ((INTERNAL_HtmlDomElementReference)uiElement.INTERNAL_OuterDomElement).UniqueIdentifier);
                    }
                }
            }
        }

        /// <summary>
        /// Makes sure that the given index of Grid.Row or Grid.Column is between 0 and the maximum Row/Column index
        /// </summary>
        /// <param name="gridPosition">the row/column index to "coerce"</param>
        /// <param name="maxValue">the maximum value for a row/column index</param>
        private static void MakeGridPositionCorrect(ref int gridPosition, int maxValue)
        {
            if (gridPosition < 0)
            {
                gridPosition = 0;
            }
            if (gridPosition > maxValue)
            {
                gridPosition = maxValue;
            }
        }

        #region ****************** Attached Properties ******************

        //Note: in the attached properties, we get the boxSizing element's style to apply the grid because it needs to be the outermost of the element.

        //Note: the result of these properties will be defined in the html style as : grid-area: row-start column-start row-end column-end

        private static void Row_Changed_CSSVersion(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            ////int oldValue = (int)e.OldValue;
            ApplyRowPosition(element);
        }

        private static void ApplyRowPosition(UIElement element)
        {
            if (element.IsUnderCustomLayout)
                return;
            int maxRow = 0;
            if (element.INTERNAL_VisualParent != null && element.INTERNAL_VisualParent is Grid) //Note: this also checks if INTERNAL_VisualTreeManager.IsElementInVisualTree(element) is true because there is no point in setting it on Windows and Popups.
            {
                Grid parent = (Grid)element.INTERNAL_VisualParent;
                if (parent._rowDefinitionsOrNull != null && parent._rowDefinitionsOrNull.Count > 0)
                {
                    maxRow = parent._rowDefinitionsOrNull.Count - 1;
                }

                int elementRow = GetRow(element);
                MakeGridPositionCorrect(ref elementRow, maxRow);

                int rowSpan = GetRowSpan(element);
                if (rowSpan <= 1)
                    rowSpan = 1;

                var style = INTERNAL_HtmlDomManager.GetFrameworkElementBoxSizingStyleForModification(element);
                bool isMsGrid = Grid_InternalHelpers.isMSGrid();
                if (!isMsGrid)
                {
                    int lastRow = elementRow + rowSpan - 1; //note: there was a -1 here before but it seems to not give he result expected.
                    MakeGridPositionCorrect(ref lastRow, maxRow);

                    style.gridRowStart = (elementRow + 1).ToString(); //Note: +1 because rows start from 1 instead of 0 in js.
                    style.gridRowEnd = (lastRow + 2).ToString(); //Note: +1 because rows start from 1 instead of 0 in js and another + 1 because the gridRowEnd seems to be the row BEFORE WHITCH the span ends.
                }
                else
                {
                    //probably doesn't work, it probably requires to use msGridRow
                    style.msGridRow = (elementRow + 1).ToString(); //Note: +1 because rows start from 1 instead of 0 in js.
                    style.msGridRowSpan = (rowSpan).ToString();
                }
            }
        }

        private static void ApplyColumnPosition(UIElement element)
        {
            if (element.IsUnderCustomLayout)
                return;
            int maxColumn = 0;
            if (element.INTERNAL_VisualParent != null && element.INTERNAL_VisualParent is Grid) //Note: this also checks if INTERNAL_VisualTreeManager.IsElementInVisualTree(element) is true because there is no point in setting it on Windows and Popups.
            {
                Grid parent = (Grid)element.INTERNAL_VisualParent;
                if (parent._columnDefinitionsOrNull != null && parent._columnDefinitionsOrNull.Count > 0)
                {
                    maxColumn = parent._columnDefinitionsOrNull.Count - 1;

                    int elementColumn = GetColumn(element);
                    MakeGridPositionCorrect(ref elementColumn, maxColumn);

                    var style = INTERNAL_HtmlDomManager.GetFrameworkElementBoxSizingStyleForModification(element);
                    int columnSpan = GetColumnSpan(element);
                    if (columnSpan <= 1)
                        columnSpan = 1;
                    int lastColumn = elementColumn + columnSpan - 1; //note: there was a -1 here before but it seems to not give he result expected.
                    MakeGridPositionCorrect(ref lastColumn, maxColumn);

                    bool isMsGrid = Grid_InternalHelpers.isMSGrid();
                    if (!isMsGrid)
                    {
                        style.gridColumnStart = (elementColumn + 1).ToString(); //Note: +1 because columns start from 1 instead of 0 in js.
                        style.gridColumnEnd = (lastColumn + 2).ToString(); //Note: +1 because columns start from 1 instead of 0 in js and another + 1 because the gridColumnEnd seems to be the column BEFORE WHITCH the span ends.
                    }
                    else
                    {
                        style.msGridColumn = (elementColumn + 1).ToString(); //Note: +1 because columns start from 1 instead of 0 in js.
                        style.msGridColumnSpan = (columnSpan).ToString(); //Note: +1 because columns start from 1 instead of 0 in js and another + 1 because the gridColumnEnd seems to be the column BEFORE WHITCH the span ends.
                    }
                }
            }
        }

        private static void RowSpan_Changed_CSSVersion(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            ApplyRowPosition(element);
        }

        private static void Column_Changed_CSSVersion(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            ApplyColumnPosition(element);
        }

        private static void ColumnSpan_Changed_CSSVersion(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            ApplyColumnPosition(element);
        }

        #endregion

        private double GetColumnActualWidth_CSSVersion(ColumnDefinition columnDefinition)
        {
            double returnValue = 0;

            //make a new div that we add to the grid in the correct column, with width and height at 100%, (opacity at 0 ?), position: absolute
            int columnIndex = _columnDefinitionsOrNull.IndexOf(columnDefinition);

            var div1 = AddTemporaryDivForRowOrColumnDimensions(columnIndex, 0);

            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
            {
                returnValue = ((dynamic)div1).offsetWidth;
            }
            else
            {
                returnValue = Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(div1, "offsetWidth"));
            }

            INTERNAL_HtmlDomManager.RemoveFromDom(div1);

            return returnValue;
        }

        private double GetRowActualHeight_CSSVersion(RowDefinition rowDefinition)
        {
            double returnValue = 0;

            //make a new div that we add to the grid in the correct column, with width and height at 100%, (opacity at 0 ?), position: absolute
            int rowIndex = _rowDefinitionsOrNull.IndexOf(rowDefinition);

            var div1 = AddTemporaryDivForRowOrColumnDimensions(0, rowIndex);

            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
            {
                returnValue = ((dynamic)div1).offsetHeight;
            }
            else
            {
                returnValue = Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(div1, "offsetHeight"));
            }

            INTERNAL_HtmlDomManager.RemoveFromDom(div1);

            return returnValue;
        }

        private object AddTemporaryDivForRowOrColumnDimensions(int columnIndex, int rowIndex)
        {
            object div1;
            var div1style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", (object)_innerDiv, this, out div1);
            div1style.width = "100%";
            div1style.height = "100%";
            div1style.opacity = "0";
            div1style.position = "relative";

            bool isMsGrid = Grid_InternalHelpers.isMSGrid();
            if (!isMsGrid)
            {
                div1style.gridColumnStart = (columnIndex + 1).ToString(); //Note: +1 because columns start from 1 instead of 0 in js.
                div1style.gridRowStart = (rowIndex + 1).ToString(); //Note: +1 because columns start from 1 instead of 0 in js.
            }
            else
            {
                div1style.msGridColumn = (columnIndex + 1).ToString(); //Note: +1 because columns start from 1 instead of 0 in js.
                div1style.msGridRow = (rowIndex + 1).ToString(); //Note: +1 because columns start from 1 instead of 0 in js.
            }

            return div1;
        }
    }
}
