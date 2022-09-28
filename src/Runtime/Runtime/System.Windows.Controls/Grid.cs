

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

#if BRIDGE
using Bridge;
#endif

using CSHTML5.Internal;
using OpenSilver.Internal.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;
using System.Diagnostics;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines a flexible grid area that consists of columns and rows. Child elements
    /// of the Grid are measured and arranged according to their row/column assignments
    /// and internal partial class logic.
    /// </summary>
    /// <example>
    /// You can add a Grid with two rows and columns to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <Grid Width="300"
    ///       Height="200"
    ///       Background="Blue"
    ///       HorizontalAlignment="Left">
    ///     <Grid.RowDefinitions>
    ///         <RowDefinition Height="40"/>
    ///         <RowDefinition Height="*"/>
    ///     </Grid.RowDefinitions>
    ///     <Grid.ColumnDefinitions>
    ///         <ColumnDefinition Width="70"/>
    ///         <ColumnDefinition Width="*"/>
    ///     </Grid.ColumnDefinitions>
    ///         <!--Children here.-->
    ///     </Grid>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// Grid myGrid = new Grid();
    /// myGrid.Width = 300;
    /// myGrid.Height = 200;
    /// myGrid.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
    /// myGrid.HorizontalAlignment = HorizontalAlignment.Left;
    /// 
    /// //We create and add the rows and columns:
    /// //First column:
    /// ColumnDefinition columnDefinition = new ColumnDefinition();
    /// columnDefinition.Width = new GridLength(70, GridUnitType.Pixel);
    /// myGrid.ColumnDefinitions.Add(columnDefinition);
    /// //Second column:
    /// ColumnDefinition columnDefinition2 = new ColumnDefinition();
    /// columnDefinition2.Width = new GridLength(1, GridUnitType.Star);
    /// myGrid.ColumnDefinitions.Add(columnDefinition2);
    /// 
    /// //First row:
    /// RowDefinition rowDefinition = new RowDefinition();
    /// rowDefinition.Height = new GridLength(40, GridUnitType.Pixel);
    /// myGrid.RowDefinitions.Add(rowDefinition);
    /// //Second Row:
    /// RowDefinition rowDefinition2 = new RowDefinition();
    /// rowDefinition2.Height = new GridLength(1, GridUnitType.Star);
    /// myGrid.RowDefinitions.Add(rowDefinition2);
    /// 
    /// //Do not forget to add the Grid to the visual tree.
    /// </code>
    /// </example>
    public partial class Grid : Panel
    {
        //todo: IMPORTANT fix the bug that makes the grid ignore the changes in ColumnDefinitions  when the grid is in the visual tree. (then remove the fix for this in Datagrid)
        //Note: todo above may not be relevant anymore?
        //todo: this is probably only a temporary solution used by the Datagrid:
        internal string INTERNAL_StringToSetVerticalGridLinesInCss = null;
        internal string INTERNAL_StringToSetHorizontalGridLinesInCss = null;

        internal List<List<INTERNAL_CellDefinition>> _currentCellsStructure;
        internal object _innerDiv;
        internal object _currentDomTable;
        internal ColumnDefinitionCollection _columnDefinitionsOrNull;
        internal RowDefinitionCollection _rowDefinitionsOrNull;

        /// <summary>
        /// Initializes a new instance of the Grid class.
        /// </summary>
        public Grid() { }

        /// <summary>
        /// Gets a list of ColumnDefinition objects defined on this instance of Grid.
        /// </summary>
        public ColumnDefinitionCollection ColumnDefinitions
        {
            get
            {
                if (_columnDefinitionsOrNull == null)
                {
                    _columnDefinitionsOrNull = new ColumnDefinitionCollection(this);
                    _columnDefinitionsOrNull.CollectionChanged += ColumnDefinitions_CollectionChanged;
                }
                return _columnDefinitionsOrNull;
            }
        }

        void ColumnDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                ColumnDefinitions_CollectionChanged_CSSVersion(sender, e);
            }
            else
            {
                ColumnDefinitions_CollectionChanged_NonCSSVersion(sender, e);
            }

            InvalidateDefinitions();
        }

        void ColumnDefinitions_CollectionChanged_NonCSSVersion(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._isLoaded)
            {
                this.RebuildDomStructure_NonCSSVersion(detachAndReattachTheChildrenIfNecessary: true); //todo: instead of calling this method, only make the actual change on the structure (adding/removing a column). Note: this can cause some chidren to move (when removing a column but when adding one as well if the children's Grid.Column attached property was bigger than the amount of columns).
                this.LocallyManageChildrenChanged();
            }
        }


        /// <summary>
        /// Gets a list of RowDefinition objects defined on this instance of Grid.
        /// </summary>
        public RowDefinitionCollection RowDefinitions
        {
            get
            {
                if (_rowDefinitionsOrNull == null)
                {
                    _rowDefinitionsOrNull = new RowDefinitionCollection(this);
                    _rowDefinitionsOrNull.CollectionChanged += RowDefinitions_CollectionChanged;
                }
                return _rowDefinitionsOrNull;
            }
        }

        void RowDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                RowDefinitions_CollectionChanged_CSSVersion(sender, e);
            }
            else
            {
                RowDefinitions_CollectionChanged_NonCSSVersion(sender, e);
            }
            
            InvalidateDefinitions();
        }

        void RowDefinitions_CollectionChanged_NonCSSVersion(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._isLoaded)
            {
                this.RebuildDomStructure_NonCSSVersion(detachAndReattachTheChildrenIfNecessary: true); //todo: instead of calling this method, only make the actual change on the structure (adding/removing a row). Note: this can cause some chidren to move (when removing a row but when adding one as well if the children's Grid.Row attached property was bigger than the amount of rows).
                this.LocallyManageChildrenChanged();
            }
        }

        private void UpdateStructureWhenAddingRow(RowDefinition rowDefinition)
        {
            //NOTE: for now, we can only add rows at the end of the grid. (or we could rebuild the whole structure when adding a row in the middle)

            //Add the row to the visual tree
            //(for now) for each child, check if it moved (its Grid.Row was bigger or equal than the amount of rows of the grid, also consider span)
            //(for later) keep a list of the chldren that have been restricted due to the lacking amount of rows and move those.
        }

        private void UpdateStructureWhenRemovingRow(RowDefinition rowDefinition)
        {
            //NOTE: for now, we can only remove rows at the end of the grid. (or we could rebuild the whole structure when adding a row in the middle)

            //Remove the row from the visual tree
            //go through all the children and check if they need to be restricted due to the row being removed
            //move those children/reduce their span.
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return CreateDomElement_CSSVersion(parentRef, out domElementWhereToPlaceChildren);
            }
            else
            {
#if !BRIDGE
                object outerDiv = base.CreateDomElement(parentRef, out _innerDiv);
#else
                object outerDiv = CreateDomElement_WorkaroundBridgeInheritanceBug(parentRef, out _innerDiv);
#endif
                domElementWhereToPlaceChildren = _innerDiv;
                return outerDiv;
            }
        }

        internal override void INTERNAL_UpdateDomStructureIfNecessary()
        {
            //Note: when arriving here, we have already checked that the Grid is in the Visual Tree.

            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                Grid_InternalHelpers.RefreshAllColumnsVisibility_CSSVersion(this);
            }
            else
            {
                RebuildDomStructure_NonCSSVersion(detachAndReattachTheChildrenIfNecessary: false);
            }
        }

        void RebuildDomStructure_NonCSSVersion(bool detachAndReattachTheChildrenIfNecessary)
        {
            // Calculate the new cells structure:    
            List<List<INTERNAL_CellDefinition>> newCellsStructure = Grid_InternalHelpers.CalculateNewCellsStructure(_columnDefinitionsOrNull, _rowDefinitionsOrNull);

            Dictionary<UIElement, INTERNAL_CellDefinition> childrenToUpdateIfStructureDifferent = Grid_InternalHelpers.HandleSpansInCellsStructureAndReturnRedirectedElements(newCellsStructure, Children);

            // (Re)create the DOM elements if the structure has changed:
            if (!AreCellsStructuresIdentical(_currentCellsStructure, newCellsStructure))
            {
                // Detach the children if necessary:
                if (detachAndReattachTheChildrenIfNecessary)
                {
                    foreach (UIElement child in Children)
                    {
                        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(child, this);
                    }
                }

                //instead of removing the table and putting another one, identify the changes and only apply them?
                //Note: the above comment was a todo but I think it's not as important anymore since this method is now only called when adding the Grid to the visual tree or when changing the amount of rows and columns (RowDefinitions and ColumnDefinition);

                // Remove the former table if any:
                if (_currentDomTable != null)
                {
                    INTERNAL_HtmlDomManager.RemoveFromDom(_currentDomTable);
                    _currentDomTable = null;
                }

                // Remember the cell structure:
                _currentCellsStructure = newCellsStructure;

                List<ColumnDefinition> normalizedColumnDefinitions;
                List<RowDefinition> normalizedRowDefinitions;
                _currentDomTable = Grid_InternalHelpers.GenerateDomElementsForGrid_NonCSSVersion(this, newCellsStructure, _columnDefinitionsOrNull, _rowDefinitionsOrNull, _innerDiv, out normalizedColumnDefinitions, out normalizedRowDefinitions);

                //Note: I moved this from the inside of the Grid_InternalHelpers.GenerateDomElementsForGrid_NonCSSVersion method since we need _currentDomTable to be set before calling the refreshes.
                // Refresh rows heights and columns widths:
                Grid_InternalHelpers.RefreshAllRowsHeight_NonCSSVersion(this, normalizedRowDefinitions);
                Grid_InternalHelpers.RefreshAllColumnsWidth_NonCSSVersion(this, normalizedColumnDefinitions);

                foreach (UIElement key in childrenToUpdateIfStructureDifferent.Keys)
                {
                    key.INTERNAL_SpanParentCell = childrenToUpdateIfStructureDifferent[key];
                }

                // Put the children back in the new cells structure if necessary
                if (detachAndReattachTheChildrenIfNecessary)
                {
                    //todo-perf: when adding the columns and rows of the Grid, we currently remove and add back all the children many times.
                    foreach (UIElement child in Children)
                    {
                        INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
                    }
                }
            }
        }

        private void UpdateStructureWhenAddingChild(UIElement child)
        {
            int columnSpan = Grid.GetColumnSpan(child);
            int rowSpan = Grid.GetRowSpan(child);

            int childColumn = Grid.GetColumn(child);
            int childRow = Grid.GetRow(child);
            int maxColumn = ColumnDefinitions.Count - 1;
            int maxRow = RowDefinitions.Count - 1;
            if (childRow > maxRow)
            {
                childRow = maxRow;
            }
            if (childRow < 0)
            {
                childRow = 0;
            }
            if (childColumn > maxColumn)
            {
                childColumn = maxColumn;
            }
            if (childColumn < 0)
            {
                childColumn = 0;
            }
            var cell = _currentCellsStructure[childRow][childColumn]; //Note: row and column should always have a correct value.

            if (cell.IsOverlapped)
            {
                //no changes since it will be "sucked in" the cell that overlaps this one.
                child.INTERNAL_SpanParentCell = cell.ParentCell;
                ++child.INTERNAL_SpanParentCell.numberOfChildren;
            }
            else
            {
                int childLastRow = childRow + Grid.GetRowSpan(child) - 1;
                int childLastColumn = childColumn + Grid.GetColumnSpan(child) - 1;

                if (childLastRow > maxRow)
                {
                    childLastRow = maxRow;
                }
                if (childLastRow < childRow)
                {
                    childLastRow = childRow;
                }
                if (childLastColumn > maxColumn)
                {
                    childLastColumn = maxColumn;
                }
                if (childLastColumn < childColumn)
                {
                    childLastColumn = childColumn;
                }
                INTERNAL_CellDefinition parentCell = null;
                bool isFirst = true;
                for (int row = childRow; row <= childLastRow; ++row)
                {
                    for (int column = childColumn; column <= childLastColumn; ++column)
                    {
                        var currentCell = _currentCellsStructure[row][column];
                        if (currentCell.IsOverlapped || (currentCell.IsOccupied && !isFirst)) //note: the test on isFirst is for example for when we have 2 elements in one cell but the second that was added is the one that causes a span.
                        {
                            //no changes since it will either be "sucked in" the cell that overlaps this one or it is already the same overlapping happening on this cell..
                            if (currentCell.IsOverlapped)
                            {
                                parentCell = currentCell.ParentCell;
                            }
                            else
                            {
                                parentCell = currentCell;
                            }
                            child.INTERNAL_SpanParentCell = parentCell;
                            ++parentCell.numberOfChildren;
                            break;
                        }
                        else
                        {
                            //the cell is not overlapped --> causes a change
                        }
                        isFirst = false;
                    }
                    if (parentCell != null)
                    {
                        break;
                    }
                }
                if (parentCell == null) //none of the cells of the child are overlapped so we need to update the cells that are newly overlapped.
                {
                    MakeCellSpan(childRow, childColumn, childLastRow - childRow + 1, childLastColumn - childColumn + 1);
                    ++cell.numberOfChildren;
                }
            }
        }


        private void MakeCellSpan(int cellRow, int cellColumn, int cellRowSpan, int cellColumnSpan)
        {
            bool isFirst = true;
            INTERNAL_CellDefinition parentCell = null;
            int cellLastRow = cellRow + cellRowSpan - 1;
            int cellLastColumn = cellColumn + cellColumnSpan - 1;
            for (int row = cellRow; row <= cellLastRow; ++row)
            {
                for (int column = cellColumn; column <= cellLastColumn; ++column)
                {
                    var cell2 = _currentCellsStructure[row][column];
                    if (isFirst)
                    {
                        parentCell = cell2;
                        cell2.IsOccupied = true;
                        cell2.RowSpan = cellRowSpan;
                        cell2.ColumnSpan = cellColumnSpan;
                        if (cell2.ColumnDomElement != null)
                        {
                            var td = cell2.ColumnDomElement;
                            INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "rowspan", cellRowSpan);
                            INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "colspan", cellColumnSpan);

                            string domElementStyleAppliedValue = null;

                            //We update the size of the cell if it was not already a star sized cell:
                            Grid_InternalHelpers.RefreshCellWithSpanHeight(this, cell2, ref domElementStyleAppliedValue);

                            if (domElementStyleAppliedValue != null)
                            {
                                var domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell2.DomElement);
                                domElementStyle.height = domElementStyleAppliedValue;
                            }

                        }
                        isFirst = false;
                    }
                    else
                    {
                        cell2.IsOverlapped = true;
                        cell2.ParentCell = parentCell;
                        if (cell2.ColumnDomElement != null)
                        {
                            var td = cell2.ColumnDomElement;
                            INTERNAL_HtmlDomManager.RemoveFromDom(td); //Note: no need to handle the other children because when adding, arriving here means that there are no other children.
                            cell2.ColumnDomElement = null;
                        }
                    }
                }
            }
        }

        private void UpdateStructureWhenRemovingChild(UIElement child)
        {
            INTERNAL_CellDefinition cell;
            if (child.INTERNAL_SpanParentCell != null)
            {
                cell = child.INTERNAL_SpanParentCell;
                child.INTERNAL_SpanParentCell = null;
            }
            else
            {
                int childColumn = Grid.GetColumn(child);
                int childRow = Grid.GetRow(child);
                int maxColumn = ColumnDefinitions.Count - 1;
                int maxRow = RowDefinitions.Count - 1;
                if (childRow > maxRow)
                {
                    childRow = maxRow;
                }
                if (childRow < 0)
                {
                    childRow = 0;
                }
                if (childColumn > maxColumn)
                {
                    childColumn = maxColumn;
                }
                if (childColumn < 0)
                {
                    childColumn = 0;
                }
                cell = _currentCellsStructure[childRow][childColumn];
            }
            --cell.numberOfChildren;
            if (cell.numberOfChildren == 0)
            {
                if (cell.RowSpan != 1 || cell.ColumnSpan != 1)
                {
                    RemoveCellSpan(cell);
                }
                cell.IsOccupied = false;
            } //else nothing to do.

        }

        private void RemoveCellSpan(INTERNAL_CellDefinition cell)
        {
            int cellRow = cell.Row;
            int cellColumn = cell.Column;
            int cellRowSpan = cell.RowSpan;
            int cellColumnSpan = cell.ColumnSpan;
            int maxRow = cellRow + cell.RowSpan - 1;
            int maxColumn = cellColumn + cell.ColumnSpan - 1;
            bool isFirst = true;

            List<ColumnDefinition> normalizedColumnDefinitions;
            List<RowDefinition> normalizedRowDefinitions;
            Grid_InternalHelpers.NormalizeWidthAndHeightPercentages(this, _columnDefinitionsOrNull, _rowDefinitionsOrNull, out normalizedColumnDefinitions, out normalizedRowDefinitions);
            bool isTheOnlyRow = _rowDefinitionsOrNull.Count <= 1;
            bool isTheOnlyColumn = _columnDefinitionsOrNull.Count <= 1;

            for (int row = cellRow; row <= maxRow; ++row)
            {
                int columnDomIndex = 0; //this is the index of the column as seen in the DOM (it can be different from the column variable defined in the line below if we have a span in the way).
                //Example: if we have a grid with 2 rows and 3 columns and 2 elements with rowspan = 2 in column 1 and 2, and we remove the second one,
                //it's column index (as seen in the DOM) in the first row will be 2, and in the second row will be 1 because there is no tr for the column 1 due to the span of the other element.
                // We therefore only count the non-overlapped columns.
                for (int i = 0; i < cellColumn; ++i)
                {
                    var currentCell = _currentCellsStructure[row][i];
                    if (!currentCell.IsOverlapped) //there is no td in the dom tree for each overlapped cell.
                    {
                        ++columnDomIndex;
                    }
                }
                for (int column = cellColumn; column <= maxColumn; ++column)
                {
                    var currentCell = _currentCellsStructure[row][column];
                    if (isFirst)
                    {
                        currentCell.RowSpan = 1;
                        currentCell.ColumnSpan = 1;
                        currentCell.IsOccupied = false; //I think this should be handled somewhere else but it's not wrong anyway so I keep it for now.
                        if (currentCell.ColumnDomElement != null)
                        {
                            var td = currentCell.ColumnDomElement;
                            INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "rowspan", 1);
                            INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "colspan", 1);
                        }
                        isFirst = false;
                    }
                    else
                    {
                        currentCell.IsOverlapped = false;
                        currentCell.ParentCell = null;
                        // Put the td back (at the right position)
                        // apply the correct sizing on it

                        // Create and append "<td>" element:
                        var td = columnDomIndex == 0 ?
                            INTERNAL_HtmlDomManager.CreateDomElementAndInsertIt("td", currentCell.RowDomElement, this, columnDomIndex, "beforeBegin") :
                            INTERNAL_HtmlDomManager.CreateDomElementAndInsertIt("td", currentCell.RowDomElement, this, columnDomIndex - 1, "afterEnd");
                       
                        var tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(td);
                        var columnDefinition = ColumnDefinitions[currentCell.Column];
                        tdStyle.display = columnDefinition.Visibility == Visibility.Visible ? "table-cell" : "none";

                        //no need to set colspan and rowspan since we know there are no children in the cell --> no span.

                        Grid_InternalHelpers.ApplyGridLinesValues(this, row, column, tdStyle);

                        //we create the div to contain the children:
                        var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", td, this);
                        var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
                        divStyle.position = "relative";

                        //we fill the cell's definition in the structure:
                        currentCell.DomElement = div;
                        currentCell.ColumnDomElement = td;

                        Grid_InternalHelpers.RefreshCellWidthAndHeight(this, currentCell, normalizedColumnDefinitions[column], isTheOnlyColumn, normalizedRowDefinitions[row], isTheOnlyRow);
                    }
                    if (!currentCell.IsOverlapped)
                    {
                        ++columnDomIndex;
                    }
                }
            }
        }

        private bool AreCellsStructuresIdentical(List<List<INTERNAL_CellDefinition>> cellsStructure1, List<List<INTERNAL_CellDefinition>> cellsStructure2)
        {
            if (cellsStructure1 == null && cellsStructure2 != null)
                return false;
            if (cellsStructure1 != null && cellsStructure2 == null)
                return false;
            if (cellsStructure1 == null && cellsStructure2 == null)
                return true;

            if (cellsStructure1.Count != cellsStructure2.Count)
                return false;

            int listIndex = 0;
            foreach (List<INTERNAL_CellDefinition> list1 in cellsStructure1)
            {
                List<INTERNAL_CellDefinition> list2 = cellsStructure2[listIndex];
                if (list1.Count != list2.Count)
                {
                    return false;
                }
                int elementIndex = 0;
                foreach (INTERNAL_CellDefinition cellDefinition1 in list1)
                {
                    if (!AreCellsDefinitionsIdentical(cellDefinition1, list2[elementIndex]))
                    {
                        return false;
                    }
                    ++elementIndex;
                }
                ++listIndex;
            }
            return true;
        }

        private bool AreCellsDefinitionsIdentical(INTERNAL_CellDefinition cellDefinition1, INTERNAL_CellDefinition cellDefinition2)
        {
            if (cellDefinition1.ColumnSpan != cellDefinition2.ColumnSpan)
                return false;
            if (cellDefinition1.IsOverlapped != cellDefinition2.IsOverlapped)
                return false;
            if (cellDefinition1.RowSpan != cellDefinition2.RowSpan)
                return false;
            return true;
        }

        internal protected override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            _currentCellsStructure = null;
            _innerDiv = null;
            _currentDomTable = null;
        }

        internal void LocallyManageChildrenChanged()
        {
            if (this.IsCustomLayoutRoot || this.IsUnderCustomLayout)
                return;

#if PERFSTAT
            var t0 = Performance.now();
#endif
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                LocallyManageChildrenChanged_CSSVersion();
            }
            else
            {
                bool thereAreRowDefinitions = (_rowDefinitionsOrNull != null && _rowDefinitionsOrNull.Count > 0);
                bool thereAreColumnDefinitions = (_columnDefinitionsOrNull != null && _columnDefinitionsOrNull.Count > 0);
                if (thereAreRowDefinitions || thereAreColumnDefinitions)
                {
                    int amountOfRows = 1;
                    if (thereAreRowDefinitions)
                        amountOfRows = _rowDefinitionsOrNull.Count;

                    int amountOfColumns = 1;
                    if (thereAreColumnDefinitions)
                        amountOfColumns = _columnDefinitionsOrNull.Count;

                    UIElement[,] lastElements = new UIElement[amountOfRows, amountOfColumns];
                    foreach (UIElement uiElement in Children)
                    {
                        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                        {
                            int elementRow;
                            int elementColumn;
                            if (uiElement.INTERNAL_SpanParentCell != null)
                            {
                                INTERNAL_CellDefinition cell = uiElement.INTERNAL_SpanParentCell;
                                if (cell.ParentCell != null)
                                {
                                    cell = cell.ParentCell;
                                }
                                elementRow = cell.Row;
                                elementColumn = cell.Column;
                            }
                            else
                            {
                                elementRow = GetRow(uiElement);
                                if (elementRow >= amountOfRows)
                                {
                                    elementRow = amountOfRows - 1;
                                }
                                elementColumn = GetColumn(uiElement);
                                if (elementColumn >= amountOfColumns)
                                {
                                    elementColumn = amountOfColumns - 1;
                                }
                            }
                            if (lastElements[elementRow, elementColumn] != null)
                            {
                                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(
                                    INTERNAL_VisualChildrenInformation[lastElements[elementRow, elementColumn]]
                                    .INTERNAL_OptionalChildWrapper_OuterDomElement);
                                style.position = "absolute";
                            }
                            var style2 = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(
                                INTERNAL_VisualChildrenInformation[uiElement]
                                .INTERNAL_OptionalChildWrapper_OuterDomElement);
                            style2.position = "relative";
                            //uiElement.INTERNAL_AdditionalOutsideDivForMargins.style.position = "relative";
                            lastElements[elementRow, elementColumn] = uiElement;
                        }
                    }
                }
                else
                {
                    int i = 0;
                    foreach (UIElement uiElement in Children)
                    {
                        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                        {
                            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(
                                INTERNAL_VisualChildrenInformation[uiElement]
                                .INTERNAL_OptionalChildWrapper_OuterDomElement);

                            if (i < Children.Count - 1)
                            {
                                style.position = "absolute";
                                //uiElement.INTERNAL_AdditionalOutsideDivForMargins.style.position = "absolute";
                            }
                            else
                            {
                                style.position = "relative";
                                //uiElement.INTERNAL_AdditionalOutsideDivForMargins.style.position = "relative";
                            }
                        }
                        ++i;
                    }
                }
            }
#if PERFSTAT
            Performance.Counter("Grid.LocallyManageChildrenChanged", t0);
#endif
        }

        internal override void OnChildrenAdded(UIElement newChild, int index)
        {
            if (!Grid_InternalHelpers.isCSSGridSupported())
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    this.UpdateStructureWhenAddingChild(newChild);
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, this, index);
                    this.LocallyManageChildrenChanged();
                }
            }
            else
            {
                base.OnChildrenAdded(newChild, index);
                this.LocallyManageChildrenChanged();
            }
        }

        internal override void OnChildrenRemoved(UIElement oldChild, int index)
        {
            if (!Grid_InternalHelpers.isCSSGridSupported())
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    this.UpdateStructureWhenRemovingChild(oldChild);
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, this);
                    this.LocallyManageChildrenChanged();
                }
            }
            else
            {
                base.OnChildrenRemoved(oldChild, index);
                this.LocallyManageChildrenChanged();
            }
        }

        internal override void OnChildrenReplaced(UIElement oldChild, UIElement newChild, int index)
        {
            if (oldChild == newChild)
            {
                return;
            }

            if (!Grid_InternalHelpers.isCSSGridSupported())
            {
                this.UpdateStructureWhenRemovingChild(oldChild);
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, this);

                this.UpdateStructureWhenAddingChild(newChild);
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, this, index);

                this.LocallyManageChildrenChanged();
            }
            else
            {
                base.OnChildrenReplaced(oldChild, newChild, index);
                this.LocallyManageChildrenChanged();
            }
        }

        internal override void OnChildrenReset()
        {
            if (!Grid_InternalHelpers.isCSSGridSupported())
            {
                if (this.INTERNAL_VisualChildrenInformation != null)
                {
                    foreach (var childInfo in this.INTERNAL_VisualChildrenInformation.Select(kp => kp.Value).ToArray())
                    {
                        UpdateStructureWhenRemovingChild(childInfo.INTERNAL_UIElement);
                        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(childInfo.INTERNAL_UIElement, this);
                    }
                }

                if (!this.HasChildren)
                {
                    return;
                }

                for (int i = 0; i < this.Children.Count; ++i)
                {
                    this.UpdateStructureWhenAddingChild(this.Children[i]);
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this.Children[i], this, i);
                }

                this.LocallyManageChildrenChanged();
            }
            else
            {
                base.OnChildrenReset();

                if (this.HasChildren)
                {
                    this.LocallyManageChildrenChanged();
                }
            }
        }

        public override object GetDomElementWhereToPlaceChild(UIElement child)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return base.GetDomElementWhereToPlaceChild(child); //it was not overriden when using the css version.
            }
            else
            {
                var spanParentCell = child.INTERNAL_SpanParentCell;
                if (spanParentCell != null)
                {
                    if (spanParentCell.IsOverlapped)
                    {
                        spanParentCell = spanParentCell.ParentCell;
                    }
                    if (spanParentCell.DomElement != null)
                    {
                        return spanParentCell.DomElement;
                    }
                }
                //Note: we only reach this line if at least one of the cells of the element (plural because of spans) is overlapped.
                int columnIndex = GetColumn(child);
                int rowIndex = GetRow(child);

                if (rowIndex >= _currentCellsStructure.Count)
                    rowIndex = _currentCellsStructure.Count - 1; // Note: this is apparently the behaviour of MS XAML.

                var row = _currentCellsStructure[rowIndex];

                if (columnIndex >= row.Count)
                    columnIndex = row.Count - 1; // Note: this is apparently the behavious of MS XAML.

                var cell = row[columnIndex];

                return cell.DomElement;
            }
        }


        public override object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild, int index = -1)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return base.CreateDomChildWrapper(parentRef, out domElementWhereToPlaceChild); //this was not overriden when using the css version of the grid.
            }
            else
            {
                // NOTE: The two following lines were commented and replaced by "CreateDomElementAppendItAndGetStyle" because of a bug of JSIL that resulted in bad JavaScript code:
                object outerDiv;
                var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
                outerDivStyle.position = "absolute";
                outerDivStyle.height = "100%";
                outerDivStyle.width = "100%";

                outerDivStyle.pointerEvents = "none";
                object innerDiv;
                var innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out innerDiv);
                innerDivStyle.position = "relative";
                innerDivStyle.height = "100%";
                innerDivStyle.width = "100%";

                domElementWhereToPlaceChild = innerDiv;
                return outerDiv;
            }
        }

        #region ****************** Attached Properties ******************

        /// <summary>
        /// Sets the value of the Grid.Row XAML attached property on the specified FrameworkElement.
        /// </summary>
        /// <param name="element">The target element on which to set the Grid.Row XAML attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetRow(UIElement element, int value)
        {
            element.SetValue(RowProperty, value);
        }
        /// <summary>
        /// Gets the value of the Grid.Row XAML attached property from the specified
        /// FrameworkElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Grid.Row XAML attached property on the target element.</returns>
        public static int GetRow(UIElement element)
        {
            return (int)element.GetValue(RowProperty);
        }
        /// <summary>
        /// Identifies the Grid.Row XAML attached property.
        /// </summary>
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(UIElement), new PropertyMetadata(0, Row_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void Row_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                Row_Changed_CSSVersion(d, e);
            }
            else
            {
                //todo


                //UIElement element = (UIElement)d;
                //////int oldValue = (int)e.OldValue;
                //int newValue = (int)e.NewValue;

                ////if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
                ////{
                ////    if (UseCssGridLayout)
                ////    {
                ////        INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(element).msGridRow = (newValue + 1).ToString();

                ////        //element.INTERNAL_OuterDomElement.dataMsGridRow = (newValue + 1).ToString();
                ////        //element.INTERNAL_OuterDomElement.setAttribute("data-ms-grid-row", (newValue + 1).ToString());
                ////    }
                ////}
            }
        }

        public static void SetRowSpan(UIElement element, int value)
        {
            element.SetValue(RowSpanProperty, value);
        }
        public static int GetRowSpan(UIElement element)
        {
            return (int)element.GetValue(RowSpanProperty);
        }

        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(UIElement), new PropertyMetadata(1, RowSpan_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void RowSpan_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                RowSpan_Changed_CSSVersion(d, e);
            }
            else
            {
                //todo


                //UIElement element = (UIElement)d;
                //////int oldValue = (int)e.OldValue;
                //int newValue = (int)e.NewValue;

                ////if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
                ////{
                ////    if (UseCssGridLayout)
                ////    {
                ////        INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(element).msGridRowSpan = newValue.ToString();

                ////        //element.INTERNAL_OuterDomElement.dataMsGridRowSpan = newValue.ToString();
                ////        //element.INTERNAL_OuterDomElement.setAttribute("data-ms-grid-row-span", newValue.ToString());
                ////    }
                ////}
            }
        }

        /// <summary>
        /// Sets the value of the Grid.Column XAML attached property on the specified FrameworkElement.
        /// </summary>
        /// <param name="element">The target element on which to set the Grid.Row XAML attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetColumn(UIElement element, int value)
        {
            element.SetValue(ColumnProperty, value);
        }
        /// <summary>
        /// Gets the value of the Grid.Column XAML attached property from the specified
        /// FrameworkElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Grid.Column XAML attached property on the target element.</returns>
        public static int GetColumn(UIElement element)
        {
            return (int)element.GetValue(ColumnProperty);
        }
        /// <summary>
        /// Identifies the Grid.Column XAML attached property
        /// </summary>
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Column", typeof(int), typeof(UIElement), new PropertyMetadata(0, Column_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void Column_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                Column_Changed_CSSVersion(d, e);
            }
            else
            {
                //todo

                //UIElement element = (UIElement)d;
                //////int oldValue = (int)e.OldValue;
                //int newValue = (int)e.NewValue;

                ////if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
                ////{
                ////    if (UseCssGridLayout)
                ////    {
                ////        INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(element).msGridColumn = (newValue + 1).ToString();

                ////        //element.INTERNAL_OuterDomElement.dataMsGridColumn = (newValue + 1).ToString();
                ////        //element.INTERNAL_OuterDomElement.setAttribute("data-ms-grid-column", (newValue + 1).ToString());
                ////    }
                ////}
            }
        }


        public static void SetColumnSpan(UIElement element, int value)
        {
            element.SetValue(ColumnSpanProperty, value);
        }
        public static int GetColumnSpan(UIElement element)
        {
            return (int)element.GetValue(ColumnSpanProperty);
        }

        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(UIElement), new PropertyMetadata(1, ColumnSpan_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void ColumnSpan_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                ColumnSpan_Changed_CSSVersion(d, e);
            }
            else
            {
                //todo


                //UIElement element = (UIElement)d;
                //////int oldValue = (int)e.OldValue;
                //int newValue = (int)e.NewValue;

                ////if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
                ////{
                ////    if (UseCssGridLayout)
                ////    {
                ////        INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(element).msGridColumnSpan = newValue.ToString();

                ////        //element.INTERNAL_OuterDomElement.dataMsGridColumnSpan = newValue.ToString();
                ////        //element.INTERNAL_OuterDomElement.setAttribute("data-ms-grid-column-span", newValue.ToString());
                ////    }
                ////}
            }
        }

        #endregion

        internal double GetColumnActualWidth(ColumnDefinition columnDefinition)
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                return 0;

            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return GetColumnActualWidth_CSSVersion(columnDefinition);
            }
            else
            {
                if (this.INTERNAL_OuterDomElement != null
                    && _columnDefinitionsOrNull != null)
                {
                    int columnIndex = _columnDefinitionsOrNull.IndexOf(columnDefinition);

                    //we find a cell in the column that is neither overlapped nor has a span.
                    INTERNAL_CellDefinition cell = _currentCellsStructure[0][columnIndex]; //in the case where there are no cells in the column that are limited to the column, we take the first one by default.
                    foreach (List<INTERNAL_CellDefinition> cells in _currentCellsStructure)
                    {
                        INTERNAL_CellDefinition currentCell = cells[columnIndex];
                        if (!currentCell.IsOverlapped && currentCell.ColumnSpan == 1)
                        {
                            cell = currentCell;
                        }
                    }
                    var columnDomElement = cell.ColumnDomElement;
                    if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
                    {
                        return ((dynamic)columnDomElement).offsetWidth;
                    }
                    else
                    {
                        return Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(columnDomElement, "offsetWidth"));
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        internal double GetRowActualHeight(RowDefinition rowDefinition)
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                return 0;

            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return GetRowActualHeight_CSSVersion(rowDefinition);
            }
            else
            {
                //Note: in regards to spans, this one should be "correct" since the <tr> is shared between all the cells of the row (at least it is the correct tr, we still need to see if the result is correct).
                if (this.INTERNAL_OuterDomElement != null
                    && _rowDefinitionsOrNull != null)
                {
                    int rowIndex = _rowDefinitionsOrNull.IndexOf(rowDefinition);
                    var rowDomElement = _currentCellsStructure[rowIndex][0].RowDomElement;
                    if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
                    {
                        return ((dynamic)rowDomElement).offsetHeight;
                    }
                    else
                    {
                        return Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(rowDomElement, "offsetHeight"));
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // private
        RowDefinitionCollection m_pRows = null;         // Effective row collection.
        ColumnDefinitionCollection m_pColumns = null;   // Effective column collection.

        // Grid classifies cells into four groups based on their column/row
        // type. The following diagram depicts all the possible combinations
        // and their corresponding cell group:
        // 
        //                  Px      Auto     Star
        //              +--------+--------+--------+
        //              |        |        |        |
        //           Px |    1   |    1   |    3   |
        //              |        |        |        |
        //              +--------+--------+--------+
        //              |        |        |        |
        //         Auto |    1   |    1   |    3   |
        //              |        |        |        |
        //              +--------+--------+--------+
        //              |        |        |        |
        //         Star |    4   |    2   |    4   |
        //              |        |        |        |
        //              +--------+--------+--------+
        //
        struct CellGroups
        {
            internal int group1;
            internal int group2;
            internal int group3;
            internal int group4;
        }

        private enum GridFlags : byte
        {
            None = 0x00,
            HasStarRows = 0x01,
            HasStarColumns = 0x02,
            HasAutoRowsAndStarColumn = 0x04,
            DefinitionsChanged = 0x08,
        }

        GridFlags m_gridFlags = GridFlags.None;

        struct CellCache
        {
            internal UIElement m_child;

            // Index of the next cell in the group.
            internal int m_next;

            // Union of the different height unit types across the row
            // definitions within the row span of this cell.
            internal GridUnitType m_rowHeightTypes;

            // Union of the different width unit types across the column
            // definitions within the column span of this cell.
            internal GridUnitType m_columnWidthTypes;

            internal static bool IsStar(GridUnitType unitTypes)
            {
                return (unitTypes & GridUnitType.Star) == GridUnitType.Star;
            }

            internal static bool IsAuto(GridUnitType unitTypes)
            {
                return (unitTypes & GridUnitType.Auto) == GridUnitType.Auto;
            }
        };

        struct SpanStoreEntry
        {
            internal SpanStoreEntry(int spanStart, int spanCount, double desiredSize, bool isColumnDefinition)
            {
                m_spanStart = spanStart;
                m_spanCount = spanCount;
                m_desiredSize = desiredSize;
                m_isColumnDefinition = isColumnDefinition;
            }

            // Starting index of the cell.
            internal int m_spanStart;

            // Span value of the cell.
            internal int m_spanCount;

            // DesiredSize of the element in the cell.
            internal double m_desiredSize;

            internal bool m_isColumnDefinition;

        }

        // This is a temporary storage that is released after arrange.
        // Note the ScopeExit in ArrangeOveride
        IDefinitionBase[] m_ppTempDefinitions = null; // Temporary definitions storage.
        int m_cTempDefinitions = 0; // Size in elements of temporary definitions storage

        void SetGridFlags(GridFlags mask)
        {
            m_gridFlags |= mask;
        }

        void ClearGridFlags(GridFlags mask)
        {
            m_gridFlags &= ~mask;
        }

        bool HasGridFlags(GridFlags mask)
        {
            return (m_gridFlags & mask) == mask;
        }

        bool IsWithoutRowAndColumnDefinitions()
        {
            return (RowDefinitions == null || RowDefinitions.Count == 0) &&
                   (ColumnDefinitions == null || ColumnDefinitions.Count == 0);
        }

        void InvalidateDefinitions()
        {
            SetGridFlags(GridFlags.DefinitionsChanged);
            InvalidateMeasure();
        }

        //------------------------------------------------------------------------
        //
        //  Method:   ValidateDefinitionStructure
        //
        //  Synopsis: Initializes m_pRows and  m_pColumns either to user supplied ColumnDefinitions collection
        //                 or to a default single element collection. This is the only method where user supplied
        //                 row or column definitions is directly used. All other must use m_pRows/m_pColumns
        //------------------------------------------------------------------------
        void InitializeDefinitionStructure()
        {
            RowDefinition emptyRow = null;
            ColumnDefinition emptyColumn = null;

            Debug.Assert(!IsWithoutRowAndColumnDefinitions());

            if (RowDefinitions == null || RowDefinitions.Count == 0)
            {
                //empty collection defaults to single row
                m_pRows = new RowDefinitionCollection();
                emptyRow = new RowDefinition();
                m_pRows.Add(emptyRow);
            }
            else
            {
                m_pRows = RowDefinitions;
            }

            if (ColumnDefinitions == null || ColumnDefinitions.Count == 0)
            {
                //empty collection defaults to single row
                m_pColumns = new ColumnDefinitionCollection();
                emptyColumn = new ColumnDefinition();
                m_pColumns.Add(emptyColumn);
            }
            else
            {
                m_pColumns = ColumnDefinitions;
            }
        }

        // Sets the initial, effective values of an IEnumerable<IDefinitionBase> .
        void ValidateDefinitions(
            IEnumerable<IDefinitionBase> definitions,
            bool treatStarAsAuto)
        {
            //for (auto & cdo : definitions)
            foreach (IDefinitionBase def in definitions)
            {
                var userSize = double.PositiveInfinity;
                var userMinSize = def.GetUserMinSize();
                var userMaxSize = def.GetUserMaxSize();

                switch (def.GetUserSizeType())
                {
                    case GridUnitType.Pixel:
                        userSize = def.GetUserSizeValue();
                        userMinSize = Math.Max(userMinSize, Math.Min(userSize, userMaxSize));
                        def.SetEffectiveUnitType(GridUnitType.Pixel);
                        break;
                    case GridUnitType.Auto:
                        def.SetEffectiveUnitType(GridUnitType.Auto);
                        break;
                    case GridUnitType.Star:
                        if (treatStarAsAuto)
                        {
                            def.SetEffectiveUnitType(GridUnitType.Auto);
                        }
                        else
                        {
                            def.SetEffectiveUnitType(GridUnitType.Star);
                        }

                        break;
                    default:
                        break;
                }

                def.SetEffectiveMinSize(userMinSize);
                def.SetMeasureArrangeSize(Math.Max(userMinSize, Math.Min(userSize, userMaxSize)));
            }
        }

        // Gets the union of the length types for a given range of definitions.
        GridUnitType GetLengthTypeForRange(
            IEnumerable<IDefinitionBase> definitions,
            int start,
            int count)
        {
            Debug.Assert((count > 0) && ((start + count) <= definitions.Count()));

            GridUnitType unitTypes = GridUnitType.Auto;
            int index = start + count - 1;

            do
            {
                var def = (IDefinitionBase)(definitions.ElementAt(index));
                switch (def.GetEffectiveUnitType())
                {
                    case GridUnitType.Auto:
                        unitTypes |= GridUnitType.Auto;
                        break;
                    case GridUnitType.Pixel:
                        unitTypes |= GridUnitType.Pixel;
                        break;
                    case GridUnitType.Star:
                        unitTypes |= GridUnitType.Star;
                        break;
                }
            } while (index > 0 && --index >= start);

            return unitTypes;
        }

        CellGroups ValidateCells(
            UIElement[] children,
            List<CellCache> cellCacheVector)
        {
            m_gridFlags = GridFlags.None;

            CellGroups cellGroups;
            cellGroups.group1 = int.MaxValue;
            cellGroups.group2 = int.MaxValue;
            cellGroups.group3 = int.MaxValue;
            cellGroups.group4 = int.MaxValue;

            var childrenCount = children.Count();

            var childIndex = childrenCount;
            while (childIndex-- > 0)
            {
                UIElement currentChild = children[childIndex];
                CellCache cell = cellCacheVector[childIndex];

                cell.m_child = currentChild;
                cell.m_rowHeightTypes = GetLengthTypeForRange(m_pRows, GetRowIndex(currentChild), GetRowSpanAdjusted(currentChild));
                cell.m_columnWidthTypes = GetLengthTypeForRange(m_pColumns, GetColumnIndex(currentChild), GetColumnSpanAdjusted(currentChild));

                // Grid classifies cells into four groups based on their column/row
                // type. The following diagram depicts all the possible combinations
                // and their corresponding cell group:
                //
                //                  Px      Auto     Star
                //              +--------+--------+--------+
                //              |        |        |        |
                //           Px |    1   |    1   |    3   |
                //              |        |        |        |
                //              +--------+--------+--------+
                //              |        |        |        |
                //         Auto |    1   |    1   |    3   |
                //              |        |        |        |
                //              +--------+--------+--------+
                //              |        |        |        |
                //         Star |    4   |    2   |    4   |
                //              |        |        |        |
                //              +--------+--------+--------+

                if (!CellCache.IsStar(cell.m_rowHeightTypes))
                {
                    if (!CellCache.IsStar(cell.m_columnWidthTypes))
                    {
                        cell.m_next = cellGroups.group1;
                        cellGroups.group1 = childIndex;
                    }
                    else
                    {
                        cell.m_next = cellGroups.group3;
                        cellGroups.group3 = childIndex;

                        if (CellCache.IsAuto(cell.m_rowHeightTypes))
                        {
                            // Remember that this Grid has at least one Auto row;
                            // useful for detecting cyclic dependency while measuring.
                            SetGridFlags(GridFlags.HasAutoRowsAndStarColumn);
                        }
                    }
                }
                else
                {
                    SetGridFlags(GridFlags.HasStarRows);

                    if (CellCache.IsAuto(cell.m_columnWidthTypes) && !CellCache.IsStar(cell.m_columnWidthTypes))
                    {
                        cell.m_next = cellGroups.group2;
                        cellGroups.group2 = childIndex;
                    }
                    else
                    {
                        cell.m_next = cellGroups.group4;
                        cellGroups.group4 = childIndex;
                    }
                }

                if (CellCache.IsStar(cell.m_columnWidthTypes))
                {
                    SetGridFlags(GridFlags.HasStarColumns);
                }

                cellCacheVector[childIndex] = cell;
            }

            return cellGroups;
        }

        // Get the row index of a child.
        int GetRowIndex(
            UIElement child)
        {
            return Math.Min(
                GetRow(child),
                m_pRows.Count - 1);
        }

        // Get the column index of a child.
        int GetColumnIndex(
            UIElement child)
        {
            return Math.Min(
                GetColumn(child),
                m_pColumns.Count - 1);
        }

        int GetRowSpanAdjusted(UIElement child)
        {
            return Math.Min(GetRowSpan(child), m_pRows.Count - GetRowIndex(child));
        }

        int GetColumnSpanAdjusted(UIElement child)
        {
            return Math.Min(GetColumnSpan(child), m_pColumns.Count - GetColumnIndex(child));
        }

        IDefinitionBase GetRowNoRef(UIElement pChild)
        {
            return m_pRows.ElementAtOrDefault(GetRowIndex(pChild));
        }

        IDefinitionBase GetColumnNoRef(UIElement pChild)
        {
            return m_pColumns.ElementAtOrDefault(GetColumnIndex(pChild));
        }

        // Adds a span entry to the list.
        void RegisterSpan(
            List<SpanStoreEntry> spanStore,
            int spanStart,
            int spanCount,
            double desiredSize,
            bool isColumnDefinition)
        {
            var spanStoreVector = spanStore;
            // If an entry already exists with the same row/column index and span, 
            // then update the desired size stored in the entry.

            for(int i = 0; i < spanStore.Count; i++)
            {
                SpanStoreEntry it = spanStore[i];

                if (it.m_isColumnDefinition == isColumnDefinition && it.m_spanStart == spanStart && it.m_spanCount == spanCount)
                {
                    if (it.m_desiredSize < desiredSize)
                    {
                        it.m_desiredSize = desiredSize;
                        spanStore[i] = it;
                    }
                    return;
                }
            }

            spanStore.Add(new SpanStoreEntry(spanStart, spanCount, desiredSize, isColumnDefinition));
        }

        void MeasureCellsGroup(
            int cellsHead, //cell group number
            int cellCount, //elements in the cell
            double rowSpacing,
            double columnSpacing,
            bool ignoreColumnDesiredSize,
            bool forceRowToInfinity,
            ref List<CellCache> cellCacheVector)
        {

            List<SpanStoreEntry> spanStore = new List<SpanStoreEntry>();

            if (cellsHead >= cellCount)
            {
                return;
            }

            do
            {
                CellCache cell = cellCacheVector[cellsHead];
                UIElement pChild = cell.m_child;

                MeasureCell(pChild, cell.m_rowHeightTypes, cell.m_columnWidthTypes, forceRowToInfinity, rowSpacing,
                    columnSpacing);
                //If a span exists, add to span store for delayed processing. processing is done when
                //all the desired sizes for a given definition index and span value are known.

                if (!ignoreColumnDesiredSize)
                {
                    int columnSpan = GetColumnSpanAdjusted(pChild);
                    //pChild.EnsureLayoutStorage();
                    if (columnSpan == 1)
                    {
                        IDefinitionBase pChildColumn = GetColumnNoRef(pChild);
                        pChildColumn.UpdateEffectiveMinSize(pChild.DesiredSize.Width);
                    }
                    else
                    {
                        RegisterSpan(
                            spanStore,
                            GetColumnIndex(pChild),
                            columnSpan,
                            pChild.DesiredSize.Width,
                            true /* isColumnDefinition */);
                    }
                }

                if (!forceRowToInfinity)
                {
                    int rowSpan = GetRowSpanAdjusted(pChild);
                    //pChild.EnsureLayoutStorage();
                    if (rowSpan == 1)
                    {
                        IDefinitionBase pChildRow = GetRowNoRef(pChild);
                        pChildRow.UpdateEffectiveMinSize(pChild.DesiredSize.Height);
                    }
                    else
                    {
                        RegisterSpan(
                            spanStore,
                            GetRowIndex(pChild),
                            rowSpan,
                            pChild.DesiredSize.Height,
                            false /* isColumnDefinition */);
                    }
                }

                cellsHead = cellCacheVector[cellsHead].m_next;

            } while (cellsHead < cellCount);

            //Go through the spanned rows/columns allocating sizes.
            foreach(var entry in spanStore)
            {
                if (entry.m_isColumnDefinition)
                {
                    EnsureMinSizeInDefinitionRange(
                        m_pColumns,
                        entry.m_spanStart,
                        entry.m_spanCount,
                        columnSpacing,
                        entry.m_desiredSize);
                }
                else
                {
                    EnsureMinSizeInDefinitionRange(
                        m_pRows,
                        entry.m_spanStart,
                        entry.m_spanCount,
                        rowSpacing,
                        entry.m_desiredSize);
                }
            }
        }

        //------------------------------------------------------------------------
        //
        //  Method:   EnsureTempDefinitionsStorage
        //
        //  Synopsis:  allocates memory for temporary definitions storage.
        //
        //------------------------------------------------------------------------
        void EnsureTempDefinitionsStorage(int minCount)
        {
            if (m_ppTempDefinitions == null || m_cTempDefinitions < minCount)
            {
                m_ppTempDefinitions = new IDefinitionBase[minCount];
                m_cTempDefinitions = minCount;
            }
        }

        //------------------------------------------------------------------------
        //
        //  Method:   SortDefinitionsForSpanPreferredDistribution
        //
        //  Synopsis: Sort definitions for span processing, for the case when the element
        //                  desired Size is greater than rangeMinSize but less than rangePreferredSize.
        //
        //------------------------------------------------------------------------
        void SortDefinitionsForSpanPreferredDistribution(
            IList<IDefinitionBase> ppDefinitions,
            int cDefinitions)
        {
            IDefinitionBase pTemp;

            for (int i = 1, j; i < cDefinitions; i++)
            {
                pTemp = ppDefinitions[i];
                for (j = i; j > 0; j--)
                {
                    if (pTemp.GetUserSizeType() == GridUnitType.Auto)
                    {
                        if (ppDefinitions[j - 1].GetUserSizeType() == GridUnitType.Auto)
                        {
                            if (pTemp.GetEffectiveMinSize() >= ppDefinitions[j - 1].GetEffectiveMinSize())
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (ppDefinitions[j - 1].GetUserSizeType() != GridUnitType.Auto)
                        {
                            if (pTemp.GetPreferredSize() >= ppDefinitions[j - 1].GetPreferredSize())
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    ppDefinitions[j] = ppDefinitions[j - 1];
                }

                ppDefinitions[j] = pTemp;
            }
        }

        //------------------------------------------------------------------------
        //
        //  Method:   SortDefinitionsForSpanMaxSizeDistribution
        //
        //  Synopsis: Sort definitions for span processing, for the case when the element
        //                  desired Size is greater than rangePreferredSize but less than rangeMaxSize.
        //
        //------------------------------------------------------------------------
        void SortDefinitionsForSpanMaxSizeDistribution(
            IList<IDefinitionBase> ppDefinitions,
            int cDefinitions)
        {
            IDefinitionBase pTemp;

            for (int i = 1, j; i < cDefinitions; i++)
            {
                pTemp = ppDefinitions[i];
                for (j = i; j > 0; j--)
                {
                    if (pTemp.GetUserSizeType() == GridUnitType.Auto)
                    {
                        if (ppDefinitions[j - 1].GetUserSizeType() == GridUnitType.Auto)
                        {
                            if (pTemp.GetSizeCache() >= ppDefinitions[j - 1].GetSizeCache())
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (ppDefinitions[j - 1].GetUserSizeType() != GridUnitType.Auto)
                        {
                            if (pTemp.GetSizeCache() >= ppDefinitions[j - 1].GetSizeCache())
                            {
                                break;
                            }
                        }
                    }

                    ppDefinitions[j] = ppDefinitions[j - 1];
                }

                ppDefinitions[j] = pTemp;
            }
        }


        //------------------------------------------------------------------------
        //
        //  Method: SortDefinitionsForOverflowSizeDistribution
        //
        //  Synopsis: Sort definitions for final size processing in ArrangeOverride, for the case
        //                 when the combined size of all definitions across a dimension exceeds the
        //                 finalSize in that dimension.
        //
        //------------------------------------------------------------------------
        void SortDefinitionsForOverflowSizeDistribution(
            IList<IDefinitionBase> ppDefinitions,
            int cDefinitions)
        {
            IDefinitionBase pTemp;

            // use insertion sort...it is stable...
            for (int i = 1, j; i < cDefinitions; i++)
            {
                pTemp = ppDefinitions[i];
                for (j = i; j > 0; j--)
                {
                    if ((pTemp.GetMeasureArrangeSize() - pTemp.GetEffectiveMinSize())
                        >= (ppDefinitions[j - 1].GetMeasureArrangeSize() - ppDefinitions[j - 1].GetEffectiveMinSize()))
                    {
                        break;
                    }

                    ppDefinitions[j] = ppDefinitions[j - 1];
                }

                ppDefinitions[j] = pTemp;
            }
        }


        //------------------------------------------------------------------------
        //
        //  Method: SortDefinitionsForStarSizeDistribution
        //
        //  Synopsis: Sort definitions for distributing star space.
        //
        //------------------------------------------------------------------------
        void SortDefinitionsForStarSizeDistribution(
                IList<IDefinitionBase> ppDefinitions,
                int cDefinitions
        )
        {
            IDefinitionBase pTemp;

            // use insertion sort...it is stable...
            for (int i = 1, j; i < cDefinitions; i++)
            {
                pTemp = ppDefinitions[i];
                for (j = i; j > 0; j--)
                {
                    // Use >= instead of > to keep sort stable. If > is used,
                    // sort will not be stable & size will distributed in a different
                    // order than WPF.
                    if (pTemp.GetSizeCache() >= ppDefinitions[j - 1].GetSizeCache())
                    {
                        break;
                    }

                    ppDefinitions[j] = ppDefinitions[j - 1];
                }

                ppDefinitions[j] = pTemp;
            }
        }

        //------------------------------------------------------------------------
        //
        //  Method:   EnsureMinSizeInDefinitionRange
        //
        //  Synopsis:  Distributes min size back to definition array's range.
        //
        //------------------------------------------------------------------------
        void EnsureMinSizeInDefinitionRange(
            IEnumerable<IDefinitionBase> definitions,
            int spanStart,
            int spanCount,
            double spacing,
            double childDesiredSize)
        {
            Debug.Assert((spanCount > 1) && (spanStart + spanCount) <= definitions.Count());
            // The spacing between definitions that this element spans through must not
            // be distributed.
            double requestedSize = Math.Max((childDesiredSize - spacing * (spanCount - 1)), 0.0f);

            //  No need to process if asked to distribute "zero".
            if (requestedSize <= double.Epsilon)
            {
                return;
            }

            int spanEnd = spanStart + spanCount;
            int autoDefinitionsCount = 0;
            double rangeMinSize = 0.0f;
            double rangeMaxSize = 0.0f;
            double rangePreferredSize = 0.0f;
            double maxMaxSize = 0.0f;

            EnsureTempDefinitionsStorage(spanCount);

            // First, we need to obtain the necessary information:
            // a) Sum up the sizes in the range.
            // b) Cache the maximum size into SizeCache.
            // c) Obtain max of MaxSizes.
            // d) Count the number of var definitions in the range.
            // e) Prepare indices.
            for (int i = spanStart; i < spanEnd; i++)
            {
                var def = definitions.ElementAt(i);
                double effectiveMinSize = def.GetEffectiveMinSize();
                double preferredSize = def.GetPreferredSize();
                double maxSize = Math.Max(def.GetUserMaxSize(), effectiveMinSize);
                rangeMinSize += effectiveMinSize;
                rangePreferredSize += preferredSize;
                rangeMaxSize += maxSize;

                // Sanity check: effectiveMinSize must always be the smallest value, maxSize
                // must be the largest one, and the preferredSize should fall in between.
                Debug.Assert(effectiveMinSize <= preferredSize
                       && preferredSize <= maxSize
                       && rangeMinSize <= rangePreferredSize
                       && rangePreferredSize <= rangeMaxSize);

                def.SetSizeCache(maxSize);
                maxMaxSize = Math.Max(maxMaxSize, maxSize);

                if (def.GetUserSizeType() == GridUnitType.Auto)
                {
                    autoDefinitionsCount++;
                }

                m_ppTempDefinitions[i - spanStart] = def;
            }

            if (requestedSize <= rangeMinSize)
            {
                // No need to process if the range is already big enough.
                return;
            }
            else if (requestedSize <= rangePreferredSize)
            {
                // If the requested size fits within the preferred size of the range,
                // we distribute the space following this logic:
                // - Do not distribute into Auto definitions; they should continue to
                //   stay "tight".
                // - For all non-Auto definitions, distribute to equi-size min sizes
                //   without exceeding the preferred size of the definition.
                //
                // In order to achieve this, the definitions are sorted in a way so
                // that all Auto definitions go first, then the other definitions
                // follow in ascending order of PreferredSize.
                double sizeToDistribute = requestedSize;
                SortDefinitionsForSpanPreferredDistribution(m_ppTempDefinitions, spanCount);

                // Process Auto definitions.
                for (int i = 0; i < autoDefinitionsCount; i++)
                {
                    var def = m_ppTempDefinitions[i];
                    Debug.Assert(def.GetUserSizeType() == GridUnitType.Auto);

                    sizeToDistribute -= def.GetEffectiveMinSize();
                }

                // Process the remaining, non-Auto definitions, distributing
                // the requested size among them.
                for (int i = autoDefinitionsCount; i < spanCount; i++)
                {
                    var def = m_ppTempDefinitions[i];
                    Debug.Assert(def.GetUserSizeType() != GridUnitType.Auto);

                    double newMinSize = Math.Min((sizeToDistribute / (spanCount - i)), def.GetPreferredSize());
                    def.UpdateEffectiveMinSize(newMinSize);
                    sizeToDistribute -= newMinSize;

                    // Stop if there's no more space to distribute.
                    if (sizeToDistribute < double.Epsilon)
                    {
                        break;
                    }
                }
            }
            else if (requestedSize <= rangeMaxSize)
            {
                // If the requested size is larger than the preferred size of the range
                // but still fits within the max size of the range, we distribute the
                // space following this logic:
                // - Do not distribute into Auto definitions if possible; they should
                //   continue to stay "tight".
                // - For all non-Auto definitions, distribute to equi-size min sizes
                //   without exceeding the max size.
                //
                // In order to achieve this, the definitions are sorted in a way so
                // that all non-Auto definitions go first, followed by the Auto
                // definitions, and all of them in ascending order of MaxSize, which
                // is currently stored in the size cache of each definition.
                double sizeToDistribute = requestedSize - rangePreferredSize;
                SortDefinitionsForSpanMaxSizeDistribution(m_ppTempDefinitions, spanCount);

                int nonAutoDefinitionsCount = spanCount - autoDefinitionsCount;
                for (int i = 0; i < spanCount; i++)
                {
                    var def = m_ppTempDefinitions[i];
                    double newMinSize = def.GetPreferredSize();

                    if (i < nonAutoDefinitionsCount)
                    {
                        // Processing non-Auto definitions.
                        Debug.Assert(def.GetUserSizeType() != GridUnitType.Auto);
                        newMinSize += sizeToDistribute / (nonAutoDefinitionsCount - i);
                    }
                    else
                    {
                        // Processing the remaining, Auto definitions.
                        Debug.Assert(def.GetUserSizeType() == GridUnitType.Auto);
                        newMinSize += sizeToDistribute / (spanCount - i);
                    }

                    // Cache PreferredSize and update MinSize.
                    double preferredSize = def.GetPreferredSize();
                    newMinSize = Math.Min(newMinSize, def.GetSizeCache());
                    def.UpdateEffectiveMinSize(newMinSize);

                    sizeToDistribute -= def.GetEffectiveMinSize() - preferredSize;

                    // Stop if there's no more space to distribute.
                    if (sizeToDistribute < double.Epsilon)
                    {
                        break;
                    }
                }
            }
            else
            {
                // If the requested size is larger than the max size of the range, we
                // distribute the space following this logic:
                // - For all definitions, distribute to equi-size min sizes.
                double equallyDistributedSize = requestedSize / spanCount;

                if ((equallyDistributedSize < maxMaxSize) && ((maxMaxSize - equallyDistributedSize) > double.Epsilon))
                {
                    // If equi-size is less than the maximum of max sizes, then
                    // we distribute space so that smaller definitions grow
                    // faster than larger ones.
                    double totalRemainingSize = maxMaxSize * spanCount - rangeMaxSize;
                    double sizeToDistribute = requestedSize - rangeMaxSize;

                    Debug.Assert(double.IsInfinity(totalRemainingSize)
                           && totalRemainingSize > 0
                           && double.IsInfinity(sizeToDistribute)
                           && sizeToDistribute > 0);

                    for (int i = 0; i < spanCount; i++)
                    {
                        var def = m_ppTempDefinitions[i];
                        double deltaSize = (maxMaxSize - def.GetSizeCache()) * sizeToDistribute / totalRemainingSize;
                        def.UpdateEffectiveMinSize(def.GetSizeCache() + deltaSize);
                    }
                }
                else
                {
                    // If equi-size is greater or equal to the maximum of max sizes,
                    // then all definitions receive equi-size as their min sizes.
                    for (int i = 0; i < spanCount; i++)
                    {
                        m_ppTempDefinitions[i].UpdateEffectiveMinSize(equallyDistributedSize);
                    }
                }
            }
        }
        void MeasureCell(
            UIElement child,
            GridUnitType rowHeightTypes,
            GridUnitType columnWidthTypes,
            bool forceRowToInfinity,
            double rowSpacing,
            double columnSpacing)
        {
            Size availableSize = new Size();

            if (CellCache.IsAuto(columnWidthTypes) && !CellCache.IsStar(columnWidthTypes))
            {
                // If this cell belongs to at least one Auto column and not a single
                // Star column, then it should be measured freely to fit its content.
                // In other words, we must give it an infinite available width.
                availableSize.Width = double.PositiveInfinity;
            }
            else
            {
                availableSize.Width = GetAvailableSizeForRange(
                    m_pColumns,
                    GetColumnIndex(child),
                    GetColumnSpanAdjusted(child),
                    columnSpacing);
            }

            if (forceRowToInfinity
                || (CellCache.IsAuto(rowHeightTypes) && !CellCache.IsStar(rowHeightTypes)))
            {
                // If this cell belongs to at least one Auto row and not a single Star
                // row, then it should be measured freely to git its content. In other
                // words, we must give it an infinite available height.
                availableSize.Height = double.PositiveInfinity;
            }
            else
            {
                availableSize.Height = GetAvailableSizeForRange(
                    m_pRows,
                    GetRowIndex(child),
                    GetRowSpanAdjusted(child),
                    rowSpacing);
            }

            child.Measure(availableSize);

            return;
        }

        // Accumulates available size information for a given range of definitions.
        double GetAvailableSizeForRange(
            IEnumerable<IDefinitionBase> definitions,
            int start,
            int count,
            double spacing)
        {
            Debug.Assert((count > 0) && ((start + count) <= definitions.Count()));

            double availableSize = 0.0f;
            int index = start + count - 1;

            do
            {
                var def = (IDefinitionBase)(definitions.ElementAt(index));
                availableSize += (def.GetEffectiveUnitType() == GridUnitType.Auto)
                    ? def.GetEffectiveMinSize()
                    : def.GetMeasureArrangeSize();
            } while (index > 0 && --index >= start);

            availableSize += spacing * (count - 1);

            return availableSize;
        }

        //------------------------------------------------------------------------
        //
        //  Method:   ResolveStar
        //
        //  Synopsis:  Resolves Star's for given array of definitions during measure pass
        //
        //------------------------------------------------------------------------
        void ResolveStar(
            IEnumerable<IDefinitionBase> definitions, //the definitions collection
            double availableSize //the total available size across this dimension
        )
        {
            int cStarDefinitions = 0;
            double takenSize = 0.0f;
            double effectiveAvailableSize = availableSize;

            EnsureTempDefinitionsStorage(definitions.Count());

            for (int i = 0; i < definitions.Count(); i++)
            {
                //if star definition, setup values for distribution calculation

                IDefinitionBase pDef = (IDefinitionBase)(definitions.ElementAt(i));

                if (pDef.GetEffectiveUnitType() == GridUnitType.Star)
                {
                    m_ppTempDefinitions[cStarDefinitions++] = pDef;

                    // Note that this user value is in star units and not pixel units,
                    // and thus, there is no need to layout-round.
                    double starValue = pDef.GetUserSizeValue();

                    if (starValue < double.Epsilon)
                    {
                        pDef.SetMeasureArrangeSize(0.0f);
                        pDef.SetSizeCache(0.0f);
                    }
                    else
                    {
                        //clipping by a max to avoid overflow when all the star values are added up.
                        starValue = Math.Min(starValue, int.MaxValue);

                        pDef.SetMeasureArrangeSize(starValue);

                        // Note that this user value is used for a computation that is cached
                        // and then used in the call to CGrid.DistributeStarSpace below for
                        // further calculations where the final result is layout-rounded as
                        // appropriate. In other words, it doesn't seem like we need to apply
                        // layout-rounding just yet.
                        double maxSize = Math.Min(int.MaxValue,
                            Math.Max(pDef.GetEffectiveMinSize(), pDef.GetUserMaxSize()));
                        pDef.SetSizeCache(maxSize / starValue);
                    }
                }
                else
                {
                    //if not star definition, reduce the size available to star definitions
                    if (pDef.GetEffectiveUnitType() == GridUnitType.Pixel)
                    {
                        takenSize += pDef.GetMeasureArrangeSize();
                    }
                    else if (pDef.GetEffectiveUnitType() == GridUnitType.Auto)
                    {
                        takenSize += pDef.GetEffectiveMinSize();
                    }
                }
            }

            DistributeStarSpace(m_ppTempDefinitions, cStarDefinitions, effectiveAvailableSize - takenSize, ref takenSize);
        }

        //------------------------------------------------------------------------
        //
        //  Method:   DistributeStarSpace
        //
        //  Synopsis:  Distributes available space between star definitions.
        //
        //------------------------------------------------------------------------
        void DistributeStarSpace(
            IDefinitionBase[] ppStarDefinitions,
            int cStarDefinitions,
            double availableSize,
            ref double pTotalResolvedSize)
        {
            double resolvedSize;
            double starValue;
            double totalStarResolvedSize = 0.0f;

            if (cStarDefinitions < 0)
            {
                return;
            }

            //sorting definitions for order of space allocation. definition with the lowest
            //maxSize to starValue ratio gets the size first.
            SortDefinitionsForStarSizeDistribution(ppStarDefinitions, cStarDefinitions);

            double allStarWeights = 0.0f;
            int i = cStarDefinitions;

            while (i > 0)
            {
                i--;
                allStarWeights += ppStarDefinitions[i].GetMeasureArrangeSize();
                //store partial sum of weights
                ppStarDefinitions[i].SetSizeCache(allStarWeights);
            }

            i = 0;
            while (i < cStarDefinitions)
            {
                resolvedSize = 0.0f;
                starValue = ppStarDefinitions[i].GetMeasureArrangeSize();

                if (starValue == 0.0f)
                {
                    resolvedSize = ppStarDefinitions[i].GetEffectiveMinSize();
                }
                else
                {
                    resolvedSize = Math.Max(availableSize - totalStarResolvedSize, 0.0f) *
                                   (starValue / ppStarDefinitions[i].GetSizeCache());
                    resolvedSize = Math.Max(ppStarDefinitions[i].GetEffectiveMinSize(),
                        Math.Min(resolvedSize, ppStarDefinitions[i].GetUserMaxSize()));
                }

                ppStarDefinitions[i].SetMeasureArrangeSize(resolvedSize);
                totalStarResolvedSize += resolvedSize;

                i++;
            }

            pTotalResolvedSize += totalStarResolvedSize;
        }

        // Calculates the desired size of the Grid minus its BorderThickness and
        // Padding assuming all the cells have already been measured.
        double GetDesiredInnerSize(IEnumerable<IDefinitionBase> definitions)
        {
            double desiredSize = 0.0f;

            for (int i = 0; i < definitions.Count(); ++i)
            {
                var def = (IDefinitionBase)(definitions.ElementAt(i));
                desiredSize += def.GetEffectiveMinSize();
            }

            return desiredSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double rowSpacing = 0;
            double columnSpacing = 0;

            Size desiredSize = new Size();

            Size innerAvailableSize = new Size(availableSize.Width, availableSize.Height);

            if (IsWithoutRowAndColumnDefinitions())
            {
                // If this Grid has no user-defined rows or columns, it is possible
                // to shortcut this MeasureOverride.
                UIElement[] childrens = Children.ToArray();
                foreach (UIElement child in childrens)
                {
                    child.Measure(availableSize);
                    desiredSize = desiredSize.Max(child.DesiredSize);
                }
            }
            else
            {
                if (HasGridFlags(GridFlags.DefinitionsChanged))
                {
                    ClearGridFlags(GridFlags.DefinitionsChanged);
                    InitializeDefinitionStructure();
                }

                ValidateDefinitions(m_pRows, availableSize.Height == double.PositiveInfinity /* treatStarAsAuto */);
                ValidateDefinitions(m_pColumns, availableSize.Width == double.PositiveInfinity /* treatStarAsAuto */);

                double combinedRowSpacing = rowSpacing * (m_pRows.Count - 1);
                double combinedColumnSpacing = columnSpacing * (m_pColumns.Count - 1);
                innerAvailableSize.Width -= combinedColumnSpacing;
                innerAvailableSize.Height -= combinedRowSpacing;

                UIElement[] childrens = Children.ToArray();
                int childrenCount = childrens.Count();

                List<CellCache> cellCacheVector = new List<CellCache>(new CellCache[childrenCount]);
                CellGroups cellGroups = ValidateCells(childrens, cellCacheVector);

                // Measure Group1. After Group1 is measured, only Group3 can have
                // cells belonging to var rows.
                MeasureCellsGroup((int)cellGroups.group1, childrenCount, rowSpacing, columnSpacing, false, false, ref cellCacheVector);

                // After Group1 is measured, only Group3 may have cells belonging to
                // Auto rows.
                if (!HasGridFlags(GridFlags.HasAutoRowsAndStarColumn))
                {
                    // We have no cyclic dependency; resolve star row/var column first.
                    if (HasGridFlags(GridFlags.HasStarRows))
                    {
                        ResolveStar(m_pRows, innerAvailableSize.Height);
                    }

                    // Measure Group2.
                    MeasureCellsGroup((int)cellGroups.group2, childrenCount, rowSpacing, columnSpacing, false, false, ref cellCacheVector);

                    if (HasGridFlags(GridFlags.HasStarColumns))
                    {
                        ResolveStar(m_pColumns, innerAvailableSize.Width);
                    }

                    // Measure Group3.
                    MeasureCellsGroup((int)cellGroups.group3, childrenCount, rowSpacing, columnSpacing, false, false, ref cellCacheVector);
                }
                else
                {
                    // If at least one cell exists in Group2, it must be measured
                    // before star columns can be resolved.
                    if (cellGroups.group2 > childrenCount)
                    {
                        if (HasGridFlags(GridFlags.HasStarColumns))
                        {
                            ResolveStar(m_pColumns, innerAvailableSize.Width);
                        }

                        // Measure Group3.
                        MeasureCellsGroup((int)cellGroups.group3, childrenCount, rowSpacing, columnSpacing, false, false, ref cellCacheVector);

                        if (HasGridFlags(GridFlags.HasStarRows))
                        {
                            ResolveStar(m_pRows, innerAvailableSize.Height);
                        }
                    }
                    else
                    {
                        // We have a cyclic dependency; measure Group2 for their
                        // widths, while setting the row heights to infinity.
                        MeasureCellsGroup((int)cellGroups.group2, childrenCount, rowSpacing, columnSpacing, false, true, ref cellCacheVector);

                        if (HasGridFlags(GridFlags.HasStarColumns))
                        {
                            ResolveStar(m_pColumns, innerAvailableSize.Width);
                        }

                        // Measure Group3.
                        MeasureCellsGroup(cellGroups.group3, childrenCount, rowSpacing, columnSpacing, false, false, ref cellCacheVector);

                        if (HasGridFlags(GridFlags.HasStarRows))
                        {
                            ResolveStar(m_pRows, innerAvailableSize.Height);
                        }

                        // Now, Measure Group2 again for their heights and ignore their widths.
                        MeasureCellsGroup((int)cellGroups.group2, childrenCount, rowSpacing, columnSpacing, true, false, ref cellCacheVector);
                    }
                }

                // Finally, measure Group4.
                MeasureCellsGroup((int)cellGroups.group4, childrenCount, rowSpacing, columnSpacing, false, false, ref cellCacheVector);

                desiredSize.Width = GetDesiredInnerSize(m_pColumns) + combinedColumnSpacing;
                desiredSize.Height = GetDesiredInnerSize(m_pRows) + combinedRowSpacing;
            }

            return desiredSize;
        }

        //------------------------------------------------------------------------
        //
        //  Method:   CGrid.SetFinalSize
        //
        //  Synopsis:
        //      Computes the offsets and sizes for each row and column
        //
        //------------------------------------------------------------------------
        void SetFinalSize(
            IEnumerable<IDefinitionBase> definitions,
            double finalSize
        )
        {
            double allPreferredArrangeSize = 0;
            IDefinitionBase currDefinition = null;
            IDefinitionBase nextDefinition = null;
            int cStarDefinitions = 0;
            int cNonStarDefinitions = definitions.Count();

            EnsureTempDefinitionsStorage(definitions.Count());

            for (int i = 0; i < definitions.Count(); i++)
            {
                IDefinitionBase pDef = (IDefinitionBase)(definitions.ElementAt(i));

                if (pDef.GetUserSizeType() == GridUnitType.Star)
                {
                    //if star definition, setup values for distribution calculation

                    m_ppTempDefinitions[cStarDefinitions++] = pDef;

                    // Note that this user value is in star units and not pixel units,
                    // and thus, there is no need to layout-round.
                    double starValue = pDef.GetUserSizeValue();

                    if (starValue < double.Epsilon)
                    {
                        //cache normalized star value temporary into MeasureSize
                        pDef.SetMeasureArrangeSize(0.0f);
                        pDef.SetSizeCache(0.0f);
                    }
                    else
                    {
                        //clipping by a max to avoid overflow when all the star values are added up.
                        starValue = Math.Min(starValue, int.MaxValue);

                        //cache normalized star value temporary into MeasureSize
                        pDef.SetMeasureArrangeSize(starValue);

                        // Note that this user value is used for a computation that is cached
                        // and then used in the call to CGrid.DistributeStarSpace below for
                        // further calculations where the final result is layout-rounded as
                        // appropriate. In other words, it doesn't seem like we need to apply
                        // layout-rounding just yet.
                        double maxSize = Math.Min(int.MaxValue,
                            Math.Max(pDef.GetEffectiveMinSize(), pDef.GetUserMaxSize()));
                        pDef.SetSizeCache(maxSize / starValue);
                    }
                }
                else
                {
                    //if not star definition, reduce the size available to star definitions
                    double userSize = 0.0f;
                    double userMaxSize = pDef.GetUserMaxSize();

                    m_ppTempDefinitions[--cNonStarDefinitions] = pDef;

                    switch (pDef.GetUserSizeType())
                    {
                        case GridUnitType.Pixel:
                            userSize = pDef.GetUserSizeValue();
                            break;
                        case GridUnitType.Auto:
                            userSize = pDef.GetEffectiveMinSize();
                            break;
                    }

                    pDef.SetMeasureArrangeSize(Math.Max(pDef.GetEffectiveMinSize(), Math.Min(userSize, userMaxSize)));
                    allPreferredArrangeSize += pDef.GetMeasureArrangeSize();
                }
            }

            //distribute available space among star definitions.
            DistributeStarSpace(m_ppTempDefinitions, cStarDefinitions, finalSize - allPreferredArrangeSize, ref allPreferredArrangeSize);

            //if the combined size of all definitions exceeds the finalSize, take the difference away.
            if ((allPreferredArrangeSize > finalSize) && Math.Abs(allPreferredArrangeSize - finalSize) > double.Epsilon)
            {
                //sort definitions to define an order for space distribution.
                SortDefinitionsForOverflowSizeDistribution(m_ppTempDefinitions, definitions.Count());
                double sizeToDistribute = finalSize - allPreferredArrangeSize;

                for (int i = 0; i < definitions.Count(); i++)
                {
                    double finalSize2 = m_ppTempDefinitions[i].GetMeasureArrangeSize() +
                                       (sizeToDistribute / (definitions.Count() - i));

                    finalSize2 = Math.Max(finalSize2, m_ppTempDefinitions[i].GetEffectiveMinSize());
                    finalSize2 = Math.Min(finalSize2, m_ppTempDefinitions[i].GetMeasureArrangeSize());
                    sizeToDistribute -= (finalSize2 - m_ppTempDefinitions[i].GetMeasureArrangeSize());
                    m_ppTempDefinitions[i].SetMeasureArrangeSize(finalSize2);
                }
            }

            //Process definitions in original order to calculate offsets
            currDefinition = (IDefinitionBase)(definitions.ElementAt(0));
            currDefinition.SetFinalOffset(0.0f);

            for (int i = 0; i < definitions.Count() - 1; i++)
            {
                nextDefinition = (IDefinitionBase)(definitions.ElementAt(i + 1));
                nextDefinition.SetFinalOffset(currDefinition.GetFinalOffset() + currDefinition.GetMeasureArrangeSize());
                currDefinition = nextDefinition;
                nextDefinition = null;
            }
        }

        // Accumulates final size information for a given range of definitions.
        double GetFinalSizeForRange(
            IEnumerable<IDefinitionBase> definitions,
            int start,
            int count,
            double spacing)
        {
            Debug.Assert((count > 0) && ((start + count) <= definitions.Count()));

            double finalSize = 0.0f;
            int index = start + count - 1;

            do
            {
                var def = (IDefinitionBase)(definitions.ElementAt(index));
                finalSize += def.GetMeasureArrangeSize();
            } while (index > 0 && --index >= start);

            finalSize += spacing * (count - 1);

            return finalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (HasGridFlags(GridFlags.DefinitionsChanged))
            {
                // A call to .Measure() is required before arranging children
                // When the DefinitionsChanged is set, the measure is already invalidated
                return default(Size);  // Returning (0, 0)
            }

            try
            {
                Rect innerRect = new Rect(0, 0,finalSize.Width, finalSize.Height);

                if (IsWithoutRowAndColumnDefinitions())
                {
                    // If this Grid has no user-defined rows or columns, it is possible
                    // to shortcut this ArrangeOverride.
                    UIElement[] childrens = Children.ToArray();
                    foreach (UIElement currentChild in childrens)
                    {
                        currentChild.Arrange(innerRect);
                    }
                }
                else
                {
                    double rowSpacing = 0;
                    double columnSpacing = 0;
                    double combinedRowSpacing = rowSpacing * (m_pRows.Count - 1);
                    double combinedColumnSpacing = columnSpacing * (m_pColumns.Count - 1);

                    // Given an effective final size, compute the offsets and sizes of each
                    // row and column, including the resdistribution of Star sizes based on
                    // the new width and height.
                    SetFinalSize(m_pRows, (double)innerRect.Height - combinedRowSpacing);
                    SetFinalSize(m_pColumns, (double)innerRect.Width - combinedColumnSpacing);

                    UIElement[] childrens = Children.ToArray();
                    int count = childrens.Count();

                    foreach (UIElement currentChild in childrens)
                    {
                        IDefinitionBase row = GetRowNoRef(currentChild);
                        IDefinitionBase column = GetColumnNoRef(currentChild);
                        int columnIndex = GetColumnIndex(currentChild);
                        int rowIndex = GetRowIndex(currentChild);

                        Rect arrangeRect = new Rect();
                        arrangeRect.X = column.GetFinalOffset() + innerRect.X + (columnSpacing * columnIndex);
                        arrangeRect.Y = row.GetFinalOffset() + innerRect.Y + (rowSpacing * rowIndex);
                        arrangeRect.Width = GetFinalSizeForRange(m_pColumns, columnIndex, GetColumnSpanAdjusted(currentChild), columnSpacing);
                        arrangeRect.Height = GetFinalSizeForRange(m_pRows, rowIndex, GetRowSpanAdjusted(currentChild), rowSpacing);

                        currentChild.Arrange(arrangeRect);
                    }
                }

                Size newFinalSize = finalSize;

                return newFinalSize;
            }
            finally
            {
                m_ppTempDefinitions = null;
                m_cTempDefinitions = 0;
            }
        }
    }

    internal sealed class GridNotLogical : Grid
    {
        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return base.CreateUIElementCollection(null);
        }

        /// <summary> 
        /// Returns enumerator to logical children.
        /// </summary>
        /*protected*/ internal override IEnumerator LogicalChildren
        {
            get
            {
                // Note: Since children are displayed in a grid in our implementation,
                // this panel's children are not logical children. There are the logical
                // children of the grid they are displayed in.
                return EmptyEnumerator.Instance;
            }
        }
    }
}
