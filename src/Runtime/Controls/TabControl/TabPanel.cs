// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Windows.Media;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Handles the layout of <see cref="TabItem" /> objects on a <see cref="TabControl" />.
    /// </summary>
    /// <remarks>
    /// TabPanel is a Panel designed to handle the intricacies of laying out the
    /// tab buttons in a TabControl.  Specifically, it handles:
    ///   Serving as an ItemsHost for TabItems within a TabControl
    ///   Determining correct sizing and positioning for TabItems 
    ///   Handling the logic associated with MultiRow scenarios, namely: 
    ///     Calculating row breaks in a collection of TabItems 
    ///     Laying out TabItems in multiple rows based on those breaks 
    ///   Performing specific layout for a selected item to indicate selection,
    ///   namely: 
    ///     Bringing the selected tab to the front, or, in other words, making
    ///       the selected tab appear to be in front of other tabs. 
    ///     Increasing the size pre-layout size of a selected item (note that
    ///       this is not a transform, but rather an increase in the size
    ///       allotted to the element in which to perform layout). 
    ///     Bringing the selected tab to the front 
    ///   Exposing attached properties that allow TabItems to be styled based on
    ///   their placement within the TabPanel.
    /// </remarks>
    /// <QualityBand>Mature</QualityBand>
    public class TabPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabPanel" /> class.
        /// </summary>
        public TabPanel()
        {
            NumberOfRows = 1;
        }

        /// <summary>
        /// Called when re-measuring the control is required.
        /// </summary>
        /// <param name="availableSize">
        /// Constraint size as an upper limit. The return value should not
        /// exceed this size.
        /// </param>
        /// <returns>
        /// The measured size of the control.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size contentSize = new Size();
            Dock tabAlignment = TabAlignment;

            NumberOfRows = 1;
            RowHeight = 0;

            // For top and bottom placement the panel flow its children to
            // calculate the number of rows and desired vertical size
            if (tabAlignment == Dock.Top || tabAlignment == Dock.Bottom)
            {
                int numInCurrentRow = 0;
                double currentRowWidth = 0;
                double maxRowWidth = 0;
                foreach (UIElement child in Children)
                {
                    // Helper measures child, and deals with Min, Max, and base
                    // Width & Height properties.  Helper returns the size a
                    // child needs to take up (DesiredSize or property specified
                    // size).
                    child.Measure(availableSize);

                    if (child.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }

                    Size childSize = GetDesiredSizeWithoutMargin(child);

                    if (RowHeight < childSize.Height)
                    {
                        RowHeight = childSize.Height;
                    }
                    if (currentRowWidth + childSize.Width > availableSize.Width && numInCurrentRow > 0)
                    {
                        // If child does not fit in the current row - create a
                        // new row
                        if (maxRowWidth < currentRowWidth)
                        {
                            maxRowWidth = currentRowWidth;
                        }
                        currentRowWidth = childSize.Width;
                        numInCurrentRow = 1;
                        NumberOfRows++;
                    }
                    else
                    {
                        currentRowWidth += childSize.Width;
                        numInCurrentRow++;
                    }
                }

                if (maxRowWidth < currentRowWidth)
                {
                    maxRowWidth = currentRowWidth;
                }
                contentSize.Height = RowHeight * NumberOfRows;

                // If we don't have constraint or content wisth is smaller than
                // constraint width then size to content
                if (double.IsInfinity(contentSize.Width) || double.IsNaN(contentSize.Width) || maxRowWidth < availableSize.Width)
                {
                    contentSize.Width = maxRowWidth;
                }
                else
                {
                    contentSize.Width = availableSize.Width;
                }
            }
            else if (tabAlignment == Dock.Left || tabAlignment == Dock.Right)
            {
                foreach (UIElement child in Children)
                {
                    if (child.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }

                    // Helper measures child, and deals with Min, Max, and base
                    // Width & Height properties.  Helper returns the size a
                    // child needs to take up (DesiredSize or property specified
                    // size).
                    child.Measure(availableSize);

                    Size childSize = GetDesiredSizeWithoutMargin(child);

                    if (contentSize.Width < childSize.Width)
                    {
                        contentSize.Width = childSize.Width;
                    }

                    contentSize.Height += childSize.Height;
                }
            }

            // Returns our minimum size & sets DesiredSize.
            return contentSize;
        }

        /// <summary>
        /// Arranges and sizes the content of a <see cref="TabPanel" /> object.
        /// </summary>
        /// <param name="finalSize">
        /// The size that a tab panel uses to position child elements.
        /// </param>
        /// <returns>The size of the arranged control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Dock tabAlignment = TabAlignment;
            if (tabAlignment == Dock.Top || tabAlignment == Dock.Bottom)
            {
                ArrangeHorizontal(finalSize);
            }
            else if (tabAlignment == Dock.Left || tabAlignment == Dock.Right)
            {
                ArrangeVertical(finalSize);
            }
            return finalSize;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="arrangeSize">Inherited code: Requires comment 1.</param>
        private void ArrangeHorizontal(Size arrangeSize)
        {
            Dock tabAlignment = TabAlignment;
            bool isMultiRow = NumberOfRows > 1;
            int activeRow = 0;
            int[] solution = new int[0];
            Size childOffset = new Size(0, 0);
            double[] headerSize = GetHeadersSize();

            // If we have multirows, then calculate the best header distribution
            if (isMultiRow)
            {
                solution = CalculateHeaderDistribution(arrangeSize.Width, headerSize);
                activeRow = GetActiveRow(solution);

                // TabPanel starts to layout children depend on activeRow which
                // should be always on bottom (top).  The first row should start
                // from Y = (NumberOfRows - 1 - activeRow) * RowHeight
                if (tabAlignment == Dock.Top)
                {
                    childOffset.Height = (NumberOfRows - 1 - activeRow) * RowHeight;
                }
                if (tabAlignment == Dock.Bottom && activeRow != 0)
                {
                    childOffset.Height = (NumberOfRows - activeRow) * RowHeight;
                }
            }

            int childIndex = 0;
            int separatorIndex = 0;
            foreach (UIElement child in Children)
            {
                Thickness margin = (Thickness)child.GetValue(MarginProperty);
                double leftOffset = margin.Left;
                double rightOffset = margin.Right;
                double topOffset = margin.Top;
                double bottomOffset = margin.Bottom;

                bool lastHeaderInRow = isMultiRow && (separatorIndex < solution.Length && solution[separatorIndex] == childIndex || childIndex == Children.Count - 1);

                // Length left, top, right, bottom;
                Size cellSize = new Size(headerSize[childIndex], RowHeight);

                // Align the last header in the row; If headers are not aligned
                // directional nav would not work correctly
                if (lastHeaderInRow)
                {
                    cellSize.Width = arrangeSize.Width - childOffset.Width;
                }

                // Set ZIndex
                TabItem tabItem = child as TabItem;
                if (tabItem != null)
                {
                    if (tabItem.IsSelected)
                    {
                        tabItem.SetValue(Canvas.ZIndexProperty, 1);
                    }
                    else
                    {
                        tabItem.SetValue(Canvas.ZIndexProperty, 0);
                    }
                }

                child.Arrange(new Rect(childOffset.Width, childOffset.Height, cellSize.Width, cellSize.Height));

                Size childSize = cellSize;
                childSize.Height = Math.Max(0d, childSize.Height - topOffset - bottomOffset);
                childSize.Width = Math.Max(0d, childSize.Width - leftOffset - rightOffset);

                // Calculate the offset for the next child
                childOffset.Width += cellSize.Width;
                if (lastHeaderInRow)
                {
                    if ((separatorIndex == activeRow && tabAlignment == Dock.Top) ||
                        (separatorIndex == activeRow - 1 && tabAlignment == Dock.Bottom))
                    {
                        childOffset.Height = 0d;
                    }
                    else
                    {
                        childOffset.Height += RowHeight;
                    }

                    childOffset.Width = 0d;
                    separatorIndex++;
                }

                childIndex++;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="arrangeSize">Inherited code: Requires comment 1.</param>
        private void ArrangeVertical(Size arrangeSize)
        {
            double childOffsetY = 0d;
            foreach (UIElement child in Children)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    // Set ZIndex
                    TabItem tabItem = child as TabItem;
                    if (tabItem != null)
                    {
                        if (tabItem.IsSelected)
                        {
                            tabItem.SetValue(Canvas.ZIndexProperty, 1);
                        }
                        else
                        {
                            tabItem.SetValue(Canvas.ZIndexProperty, 0);
                        }
                    }

                    Size childSize = GetDesiredSizeWithoutMargin(child);
                    child.Arrange(new Rect(0, childOffsetY, arrangeSize.Width, childSize.Height));

                    // Calculate the offset for the next child
                    childOffsetY += childSize.Height;
                }
            }
        }

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        private Dock TabAlignment
        {
            get { return TabControlParent != null ? TabControlParent.TabStripPlacement : Dock.Top; }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="element">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        internal static Size GetDesiredSizeWithoutMargin(UIElement element)
        {
            // We have a hard coded margin value on each side for the selected
            // item. To account for this additional size, we add this size to
            // the calculations taking place.
            double selectedMarginSize = 0;
            TabItem tabItem = element as TabItem;
            if (tabItem != null && tabItem.IsSelected)
            {
                Panel panel = tabItem.GetTemplate(tabItem.IsSelected, tabItem.TabStripPlacement) as Panel;
                FrameworkElement fe = (panel != null && panel.Children.Count > 0) ? panel.Children[0] as FrameworkElement : null;
                if (fe != null)
                {
                    selectedMarginSize += (Math.Abs(fe.Margin.Left + fe.Margin.Right));
                }
            }

            Thickness margin = (Thickness)element.GetValue(MarginProperty);
            Size desiredSizeWithoutMargin = new Size();
            desiredSizeWithoutMargin.Height = Math.Max(0d, element.DesiredSize.Height - margin.Top - margin.Bottom);
            desiredSizeWithoutMargin.Width = Math.Max(0d, element.DesiredSize.Width - margin.Left - margin.Right + selectedMarginSize);
            return desiredSizeWithoutMargin;
        }

        /// <summary>
        /// Returns the row which contain the child with IsSelected==true.
        /// </summary>
        /// <param name="solution">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private int GetActiveRow(int[] solution)
        {
            int activeRow = 0;
            int childIndex = 0;
            if (solution.Length > 0)
            {
                foreach (UIElement child in Children)
                {
                    bool isActiveTab = (bool)child.GetValue(TabItem.IsSelectedProperty);

                    if (isActiveTab)
                    {
                        return activeRow;
                    }

                    if (activeRow < solution.Length && solution[activeRow] == childIndex)
                    {
                        activeRow++;
                    }

                    childIndex++;
                }
            }

            // If the is no selected element and aligment is Top  - then the
            // active row is the last row 
            if (TabAlignment == Dock.Top)
            {
                activeRow = NumberOfRows - 1;
            }

            return activeRow;
        }

        // TabPanel layout calculation:
        //  
        // After measure call we have:
        // rowWidthLimit: width of the TabPanel
        // Header[0..n-1]: headers
        // headerWidth[0..n-1]: header width
        //  
        // Calculated values:
        // numSeparators: number of separators between numSeparators+1 rows
        // rowWidth[0..numSeparators]: row width
        // rowHeaderCount[0..numSeparators]: Row Count = number of headers on
        //     that row
        // rowAverageGap[0..numSeparators]: Average Gap for the row i =
        //     (rowWidth - rowWidth[i])/rowHeaderCount[i]
        // currentSolution[0..numSeparators-1]: separator currentSolution[i]=x
        //     means Header[x] and h[x+1] are separated with new line
        // bestSolution[0..numSeparators-1]: keep the last Best Solution
        // bestSolutionRowAverageGap: keep the last Best Solution Average Gap
        // 
        // Between all separators distribution the best solution have minimum
        // Average Gap - this is the amount of pixels added to the header (to
        // justify) in the row.
        // 
        // How does it work:
        // First we flow the headers to calculate the number of necessary rows
        // (numSeparators+1).  That means we need to insert numSeparators
        // separators between n headers (numSeparators<n always).  For each
        // current state rowAverageGap[1..numSeparators+1] are calculated for
        // each row.  Current state
        // rowAverageGap = MAX (rowAverageGap[1..numSeparators+1]).  Our goal is
        // to find the solution with MIN (rowAverageGap).  On each iteration
        // step we move a header from a previous row to the row with maximum
        // rowAverageGap.  We countinue the itterations only if we move to
        // better solution, i.e. rowAverageGap is smaller.  Maximum iteration
        // steps are less the number of headers.

        /// <summary>
        /// Input: Row width and width of all headers.  Output: int array which
        /// size is the number of separators and contains each separator
        /// position.
        /// </summary>
        /// <param name="rowWidthLimit">
        /// Inherited code: Requires comment.
        /// </param>
        /// <param name="headerWidth">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private int[] CalculateHeaderDistribution(double rowWidthLimit, double[] headerWidth)
        {
            double bestSolutionMaxRowAverageGap = 0;
            int numHeaders = headerWidth.Length;

            int numSeparators = NumberOfRows - 1;
            double currentRowWidth = 0;
            int numberOfHeadersInCurrentRow = 0;
            double currentAverageGap = 0;
            int[] currentSolution = new int[numSeparators];
            int[] bestSolution = new int[numSeparators];
            int[] rowHeaderCount = new int[NumberOfRows];
            double[] rowWidth = new double[NumberOfRows];
            double[] rowAverageGap = new double[NumberOfRows];
            double[] bestSolutionRowAverageGap = new double[NumberOfRows];

            // Initialize the current state; Do the initial flow of the headers
            int currentRowIndex = 0;

            for (int index = 0; index < numHeaders; index++)
            {
                if (currentRowWidth + headerWidth[index] > rowWidthLimit && numberOfHeadersInCurrentRow > 0)
                {
                    // if we cannot add next header - flow to next row
                    // Store current row before we go to the next

                    // Store the current row width
                    rowWidth[currentRowIndex] = currentRowWidth;

                    // For each row we store the number os headers inside
                    rowHeaderCount[currentRowIndex] = numberOfHeadersInCurrentRow;

                    // The amout of width that should be added to justify the header
                    currentAverageGap = Math.Max(0d, (rowWidthLimit - currentRowWidth) / numberOfHeadersInCurrentRow);
                    rowAverageGap[currentRowIndex] = currentAverageGap;

                    // Separator points to the last header in the row
                    currentSolution[currentRowIndex] = index - 1;

                    // Remember the maximum of all currentAverageGap
                    if (bestSolutionMaxRowAverageGap < currentAverageGap)
                    {
                        bestSolutionMaxRowAverageGap = currentAverageGap;
                    }

                    // Iterate to next row
                    currentRowIndex++;

                    // Accumulate header widths on the same row
                    currentRowWidth = headerWidth[index];
                    numberOfHeadersInCurrentRow = 1;
                }
                else
                {
                    // Accumulate header widths on the same row
                    currentRowWidth += headerWidth[index];

                    // Increase the number of headers only if they are not
                    // collapsed (width=0)
                    if (headerWidth[index] != 0)
                    {
                        numberOfHeadersInCurrentRow++;
                    }
                }
            }

            // If everithing fit in 1 row then exit (no separators needed)
            if (currentRowIndex == 0)
            {
                return new int[0];
            }

            // Add the last row
            rowWidth[currentRowIndex] = currentRowWidth;
            rowHeaderCount[currentRowIndex] = numberOfHeadersInCurrentRow;
            currentAverageGap = (rowWidthLimit - currentRowWidth) / numberOfHeadersInCurrentRow;
            rowAverageGap[currentRowIndex] = currentAverageGap;
            if (bestSolutionMaxRowAverageGap < currentAverageGap)
            {
                bestSolutionMaxRowAverageGap = currentAverageGap;
            }

            // Remember the first solution as initial bestSolution
            currentSolution.CopyTo(bestSolution, 0);

            // bestSolutionRowAverageGap is used in ArrangeOverride to calculate header sizes
            rowAverageGap.CopyTo(bestSolutionRowAverageGap, 0);

            // Search for the best solution
            // The exit condition if when we cannot move header to the next row 
            while (true)
            {
                // Find the row with maximum AverageGap

                // Keep the row index with maximum AverageGap
                int worstRowIndex = 0;
                double maxAG = 0;

                // for all rows
                for (int i = 0; i < NumberOfRows; i++)
                {
                    if (maxAG < rowAverageGap[i])
                    {
                        maxAG = rowAverageGap[i];
                        worstRowIndex = i;
                    }
                }

                // If we are on the first row - cannot move from previous
                if (worstRowIndex == 0)
                {
                    break;
                }

                // From the row with maximum AverageGap we try to move a header
                // from previous row
                int moveToRow = worstRowIndex;
                int moveFromRow = moveToRow - 1;
                int moveHeader = currentSolution[moveFromRow];
                double movedHeaderWidth = headerWidth[moveHeader];

                rowWidth[moveToRow] += movedHeaderWidth;

                // If the moved header cannot fit - exit. We have the best
                // solution already.
                if (rowWidth[moveToRow] > rowWidthLimit)
                {
                    break;
                }

                // If header is moved successfully to the worst row we update
                // the arrays keeping the row state
                currentSolution[moveFromRow]--;
                rowHeaderCount[moveToRow]++;
                rowWidth[moveFromRow] -= movedHeaderWidth;
                rowHeaderCount[moveFromRow]--;
                rowAverageGap[moveFromRow] = (rowWidthLimit - rowWidth[moveFromRow]) / rowHeaderCount[moveFromRow];
                rowAverageGap[moveToRow] = (rowWidthLimit - rowWidth[moveToRow]) / rowHeaderCount[moveToRow];

                // EvaluateSolution:
                // If the current solution is better than bestSolution - keep it
                // in bestSolution
                maxAG = 0;

                // for all rows
                for (int i = 0; i < NumberOfRows; i++)
                {
                    if (maxAG < rowAverageGap[i])
                    {
                        maxAG = rowAverageGap[i];
                    }
                }

                if (maxAG < bestSolutionMaxRowAverageGap)
                {
                    bestSolutionMaxRowAverageGap = maxAG;
                    currentSolution.CopyTo(bestSolution, 0);
                    rowAverageGap.CopyTo(bestSolutionRowAverageGap, 0);
                }
            }

            // Each header size should be increased so headers in the row
            // stretch to fit the row
            currentRowIndex = 0;
            int childIndex = 0;
            foreach (UIElement child in Children)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    // Add gap only to the visible children
                    headerWidth[childIndex] += bestSolutionRowAverageGap[currentRowIndex];
                }
                if (currentRowIndex < numSeparators && bestSolution[currentRowIndex] == childIndex)
                {
                    currentRowIndex++;
                }
                childIndex++;
            }

            // Use the best solution bestSolution[0..numSeparators-1] to layout
            return bestSolution;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <returns>Inherited code: Requires comment 1.</returns>
        internal double[] GetHeadersSize()
        {
            double[] headerSize = new double[Children.Count];
            int childIndex = 0;
            foreach (UIElement child in Children)
            {
                Size childSize = GetDesiredSizeWithoutMargin(child);
                headerSize[childIndex] = child.Visibility == Visibility.Collapsed ? 0 : childSize.Width;
                childIndex++;
            }
            return headerSize;
        }

        /// <summary>
        /// Gets the TabControl that contains this TabPanel by walking up the
        /// visual tree.
        /// </summary>
        internal TabControl TabControlParent
        {
            get
            {
                FrameworkElement fe = this as FrameworkElement;
                while (fe != null)
                {
                    TabControl tc = fe as TabControl;
                    if (tc != null)
                    {
                        return tc;
                    }
                    fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the number of rows calculated in measure and used in
        /// arrange.
        /// </summary>
        internal int NumberOfRows { get; set; }

        /// <summary>
        /// Gets or sets the maximum of all headers height.
        /// </summary>
        internal double RowHeight { get; set; }

        protected sealed override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return base.CreateUIElementCollection(null);
        }

        internal override IEnumerator LogicalChildren
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
