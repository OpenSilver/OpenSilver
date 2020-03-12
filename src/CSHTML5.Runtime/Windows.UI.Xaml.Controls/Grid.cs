

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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if BRIDGE
using Bridge;
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
                    _columnDefinitionsOrNull = new ColumnDefinitionCollection();
                    _columnDefinitionsOrNull.CollectionChanged += ColumnDefinitions_CollectionChanged;
                }
                return _columnDefinitionsOrNull;
            }
        }

        void ColumnDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ColumnDefinition columnDefinition in e.NewItems)
                    columnDefinition.Parent = this;
            }

            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                ColumnDefinitions_CollectionChanged_CSSVersion(sender, e);
            }
            else
            {
                ColumnDefinitions_CollectionChanged_NonCSSVersion(sender, e);
            }
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
                    _rowDefinitionsOrNull = new RowDefinitionCollection();
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
        }

        void RowDefinitions_CollectionChanged_NonCSSVersion(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (RowDefinition rowDefinition in e.NewItems)
                    rowDefinition.Parent = this;
            }

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
#if REWORKLOADED
                        this.AddVisualChild(child);
#else
                        INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
#endif
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
            INTERNAL_CellDefinition cell = _currentCellsStructure[childRow][childColumn]; //Note: row and column should always have a correct value.

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
                        INTERNAL_CellDefinition currentCell = _currentCellsStructure[row][column];
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
                    INTERNAL_CellDefinition cell2 = _currentCellsStructure[row][column];
                    if (isFirst)
                    {
                        parentCell = cell2;
                        cell2.IsOccupied = true;
                        cell2.RowSpan = cellRowSpan;
                        cell2.ColumnSpan = cellColumnSpan;
                        if (cell2.ColumnDomElement != null)
                        {
                            dynamic td = cell2.ColumnDomElement;
                            INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "rowspan", cellRowSpan);
                            INTERNAL_HtmlDomManager.SetDomElementAttribute(td, "colspan", cellColumnSpan);

                            string domElementStyleAppliedValue = null;


                            //We update the size of the cell if it was not already a star sized cell:
                            Grid_InternalHelpers.RefreshCellWithSpanHeight(this, cell2, ref domElementStyleAppliedValue);

                            if (domElementStyleAppliedValue != null)
                            {
                                dynamic domElementStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(cell2.DomElement);
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
                            dynamic td = cell2.ColumnDomElement;
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
                    INTERNAL_CellDefinition currentCell = _currentCellsStructure[row][i];
                    if (!currentCell.IsOverlapped) //there is no td in the dom tree for each overlapped cell.
                    {
                        ++columnDomIndex;
                    }
                }
                for (int column = cellColumn; column <= maxColumn; ++column)
                {
                    INTERNAL_CellDefinition currentCell = _currentCellsStructure[row][column];
                    if (isFirst)
                    {
                        currentCell.RowSpan = 1;
                        currentCell.ColumnSpan = 1;
                        currentCell.IsOccupied = false; //I think this should be handled somewhere else but it's not wrong anyway so I keep it for now.
                        if (currentCell.ColumnDomElement != null)
                        {
                            dynamic td = currentCell.ColumnDomElement;
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

                        dynamic td;
                        // Create and append "<td>" element:
                        if (columnDomIndex == 0)
                        {
                            td = INTERNAL_HtmlDomManager.CreateDomElementAndInsertIt("td", currentCell.RowDomElement, this, columnDomIndex, "beforeBegin");
                        }
                        else
                        {

                            td = INTERNAL_HtmlDomManager.CreateDomElementAndInsertIt("td", currentCell.RowDomElement, this, columnDomIndex - 1, "afterEnd");
                        }
                        dynamic tdStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(td);
                        ColumnDefinition columnDefinition = ColumnDefinitions[currentCell.Column];
                        if (columnDefinition.Visibility == Visibility.Visible)
                        {
                            tdStyle.display = "table-cell";
                        }
                        else
                        {
                            tdStyle.display = "none";
                        }

                        //no need to set colspan and rowspan since we know there are no children in the cell --> no span.

                        Grid_InternalHelpers.ApplyGridLinesValues(this, row, column, tdStyle);

                        //we create the div to contain the children:
                        dynamic div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", td, this);
                        dynamic divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
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
            _currentCellsStructure = null;
            _innerDiv = null;
            _currentDomTable = null;
        }

        internal void LocallyManageChildrenChanged()
        {
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

        internal override void ManageChildrenChanged(UIElementCollection oldChildren, UIElementCollection newChildren)
        {
            //todo: remove this method? I'm not sure that it's called anymore
            if (!Grid_InternalHelpers.isCSSGridSupported())
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
#if PERFSTAT
                    var t1 = Performance.now();
#endif
                    if (oldChildren != null)
                    {
                        // Detach old children only if they are not in the "newChildren" collection:
                        foreach (UIElement child in oldChildren) //note: there is no setter for Children so the user cannot change the order of the elements in one step --> we cannot have the same children in another order (which would keep the former order with the way it is handled now) --> no problem here
                        {
#if PERFSTAT
                            var t2 = Performance.now();
#endif
                            if (newChildren == null || !newChildren.Contains(child))
                            {
#if PERFSTAT
                                Performance.Counter("Grid.ManageChildrenChanged 'Contains'", t2);
#endif
                                UpdateStructureWhenRemovingChild(child);
                                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(child, this);
                            }
                            else
                            {
#if PERFSTAT
                                Performance.Counter("Grid.ManageChildrenChanged 'Contains'", t2);
#endif
                            }
                        }
                    }
                    if (newChildren != null)
                    {
                        foreach (UIElement child in newChildren)
                        {
                            // Note: we do this for all items (regardless of whether they are in the oldChildren collection or not) to make it work when the item is first added to the Visual Tree (at that moment, all the properties are refreshed by calling their "Changed" method).
                            UpdateStructureWhenAddingChild(child);
#if REWORKLOADED
                            this.AddVisualChild(child);
#else
                            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
#endif
                        }
                    }
                    this.LocallyManageChildrenChanged();
                }
            }
            else
            {
                base.ManageChildrenChanged(oldChildren, newChildren);
                this.LocallyManageChildrenChanged();
            }
        }

        public override dynamic GetDomElementWhereToPlaceChild(UIElement child)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return base.GetDomElementWhereToPlaceChild(child); //it was not overriden when using the css version.
            }
            else
            {
                //if (!UseCssGridLayout)
                //{
                INTERNAL_CellDefinition spanParentCell = child.INTERNAL_SpanParentCell;
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
                //}
                //else
                //    return null;
            }
        }


        public override object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return base.CreateDomChildWrapper(parentRef, out domElementWhereToPlaceChild); //this was not overriden when using the css version of the grid.
            }
            else
            {

                // NOTE: The two following lines were commented and replaced by "CreateDomElementAppendItAndGetStyle" because of a bug of JSIL that resulted in bad JavaScript code:
                //dynamic outerDiv = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef);
                //dynamic outerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDiv);
                object outerDiv;
                dynamic outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
                outerDivStyle.position = "absolute";
                outerDivStyle.height = "100%";
                outerDivStyle.width = "100%";

                //if (!INTERNAL_HtmlDomManager.IsInternetExplorer()) // Note: we don't do this under IE10 because it causes the Showcase to not display the content of the pages. //todo: see why on other browsers we do this (cf. changeset 1701)
                //    outerDivStyle.overflow = "hidden";

                outerDivStyle.pointerEvents = "none";
                //outerDivStyle.display = "table";
                //dynamic innerDiv = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", outerDiv);
                //dynamic innerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(innerDiv);
                object innerDiv;
                dynamic innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out innerDiv);
                innerDivStyle.position = "relative";
                innerDivStyle.height = "100%";
                innerDivStyle.width = "100%";

                //if (!INTERNAL_HtmlDomManager.IsInternetExplorer()) // Note: we don't do this under IE10 because it causes the Showcase to not display the content of the pages. //todo: see why on other browsers we do this (cf. changeset 1701)
                //    innerDivStyle.overflow = "hidden";

                domElementWhereToPlaceChild = innerDiv;
                return outerDiv;
            }
        }

        //internal override dynamic ShowChild(UIElement child)
        //{
        //    dynamic elementToReturn = base.ShowChild(child); //we need to return this so that a class that inherits from this but doesn't create a wrapper (or a different one) is correctly handled 

        //    dynamic domChildWrapper = INTERNAL_VisualChildrenInformation[child].INTERNAL_OptionalChildWrapper_OuterDomElement;
        //    var domChildWrapperStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domChildWrapper);
        //    //domChildWrapperStyle.visibility = "visible";
        //    domChildWrapperStyle.display = "block"; //todo: verify that it is not necessary to revert to the previous value instead.
        //    domChildWrapperStyle.width = "100%";
        //    domChildWrapperStyle.height = "100%";

        //    return elementToReturn;
        //}


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
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return GetColumnActualWidth_CSSVersion(columnDefinition);
            }
            else
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this)
                    && (object)this.INTERNAL_OuterDomElement != null
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
                        return columnDomElement.offsetWidth;
                    }
                    else
                    {
                        return Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(columnDomElement, "offsetWidth"));
                    }
                }
                else
                    return double.NaN;
            }
        }

        //internal double GetColumnActualHeight(ColumnDefinition columnDefinition)
        //{
        //    bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
        //    if (isCSSGrid)
        //    {
        //        return GetColumnActualHeight_CSSVersion(columnDefinition);
        //    }
        //    else
        //    {
        //      //Note: this doesn't do what would be expected, it should do like the cssversion.
        //        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && this.INTERNAL_OuterDomElement != null)
        //        {
        //            var columnDomElement = _currentCellsStructure[0][0].ColumnDomElement; // Note: The height of the column should not depend on which column we chose.
        //            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
        //            {
        //                return columnDomElement.offsetHeight;
        //            }
        //            else
        //            {
        //                INTERNAL_SimulatorExecuteJavaScript.ForceExecutionOfAllPendingCode(); // Explanation: we usually optimize performance in the Simulator by postponing the JS code that sets the CSS properties. This reduces the number of interop calls between C# and the browser. However, in the current case here we need to have all the properties already applied in order to be able to calculate the size of the DOM element. Therefore we need to call the "ForceExecution" method.
        //                return (double)INTERNAL_HtmlDomManager.CastToJsValue_SimulatorOnly(INTERNAL_HtmlDomManager.GetDomElementAttribute(columnDomElement, "offsetHeight"));
        //            }
        //        }
        //        else
        //            return double.NaN;
        //    }
        //}



        internal double GetRowActualHeight(RowDefinition rowDefinition)
        {
            bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
            if (isCSSGrid)
            {
                return GetRowActualHeight_CSSVersion(rowDefinition);
            }
            else
            {
                //Note: in regards to spans, this one should be "correct" since the <tr> is shared between all the cells of the row (at least it is the correct tr, we still need to see if the result is correct).
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this)
                    && (object)this.INTERNAL_OuterDomElement != null
                    && _rowDefinitionsOrNull != null)
                {
                    int rowIndex = _rowDefinitionsOrNull.IndexOf(rowDefinition);
                    var rowDomElement = _currentCellsStructure[rowIndex][0].RowDomElement;
            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
                    {
                        return rowDomElement.offsetHeight;
                    }
#if !BRIDGE
                    else
                    {
                        return Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(rowDomElement, "offsetHeight"));
                    }
#else
            return 0; //NOTE : this code is usde for compilation and only for BRIDGE. We are NOT supposed to get there.
#endif
                }
                else
                    return double.NaN;
            }
        }

        //internal double GetRowActualWidth(RowDefinition rowDefinition)
        //{
        //    bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
        //    if (isCSSGrid)
        //    {
        //        return GetRowActualWidth_CSSVersion(rowDefinition);
        //    }
        //    else
        //    {
        //      //Note: this doesn't do what would be expected, it should do like the cssversion.
        //        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && this.INTERNAL_OuterDomElement != null)
        //        {
        //            var columnDomElement = _currentCellsStructure[0][0].RowDomElement; // Note: The width of the row should not depend on which row we chose.
        //            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
        //            {
        //                return columnDomElement.offsetWidth;
        //            }
        //            else
        //            {
        //                INTERNAL_SimulatorExecuteJavaScript.ForceExecutionOfAllPendingCode(); // Explanation: we usually optimize performance in the Simulator by postponing the JS code that sets the CSS properties. This reduces the number of interop calls between C# and the browser. However, in the current case here we need to have all the properties already applied in order to be able to calculate the size of the DOM element. Therefore we need to call the "ForceExecution" method.
        //                return (double)INTERNAL_HtmlDomManager.CastToJsValue_SimulatorOnly(INTERNAL_HtmlDomManager.GetDomElementAttribute(columnDomElement, "offsetWidth"));
        //            }
        //        }
        //        else
        //            return double.NaN;
        //    }
        //}
    }
}
