// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the necessary information to draw connecting lines in a
    /// TreeViewItem.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal class TreeViewItemConnectingLineInfo
    {
        /// <summary>
        /// Gets the TreeViewItem.
        /// </summary>
        public TreeViewItem Item { get; private set; }

        /// <summary>
        /// Gets or sets the vertical connecting line of the TreeViewItem.
        /// </summary>
        public Line VerticalConnectingLine { get; set; }

        /// <summary>
        /// Gets or sets the horizontal connecting line of the TreeViewItem.
        /// </summary>
        public Line HorizontalConnectingLine { get; set; }

        /// <summary>
        /// Gets or sets the expander button of the TreeViewItem.
        /// </summary>
        public ToggleButton ExpanderButton { get; set; }

        /// <summary>
        /// Gets or sets the header of the TreeViewItem.
        /// </summary>
        public FrameworkElement Header { get; set; }

        /// <summary>
        /// Initializes a new instance of the TreeViewItemConnectingLineInfo
        /// class.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        public TreeViewItemConnectingLineInfo(TreeViewItem item)
        {
            Debug.Assert(item != null, "item should not be null!");
            Item = item;

            // Position the connecting lines when the layout is updated
            item.LayoutUpdated += (s, e) => PositionConnectingLines();
            item.Expanded += (s, e) => PositionConnectingLines();
            item.Collapsed += (s, e) => PositionConnectingLines();
            item.ItemContainerGenerator.ItemsChanged += (s, e) => PositionConnectingLines();
        }

        /// <summary>
        /// Position the connecting lines in the TreeViewItem.
        /// </summary>
        private void PositionConnectingLines()
        {
            PositionVerticalConnectingLine();
        }

        /// <summary>
        /// Position the vertical connecting line in the TreeViewItem.
        /// </summary>
        private void PositionVerticalConnectingLine()
        {
            // Nothing to position if we don't have a connecting line
            if (VerticalConnectingLine == null)
            {
                return;
            }

            // Nothing to position if there are no nested items or if we're
            // collapsed
            if (!Item.IsExpanded || Item.Items.Count <= 0)
            {
                VerticalConnectingLine.Visibility = Visibility.Collapsed;
                return;
            }

            // Get the last nested item (which tells us how far down to draw the
            // connecting line)
            TreeViewItem lastItem = Item.ItemContainerGenerator.ContainerFromIndex(Item.Items.Count - 1) as TreeViewItem;
            if (lastItem == null)
            {
                VerticalConnectingLine.Visibility = Visibility.Collapsed;
                return;
            }

            // Get the element on the last nested item that the vertical line
            // should connect to
            TreeViewItemConnectingLineInfo info = TreeViewConnectingLines.GetConnectingLineInfo(lastItem);
            FrameworkElement connection = lastItem.HasItems ?
                (FrameworkElement)info.ExpanderButton :
                (FrameworkElement)info.HorizontalConnectingLine;
            if (connection != null)
            {
                Rect? bounds = connection.GetBoundsRelativeTo(VerticalConnectingLine);
                if (bounds == null)
                {
                    return;
                }

                double bottomY = bounds.Value.Y;
                ////double centerX = (bounds.Value.Left + bounds.Value.Right) / 2.0;

                // Augment the length with the Y offset of the horizontal
                // connecting line
                if (!lastItem.HasItems)
                {
                    bottomY += info.HorizontalConnectingLine.Y1;
                    ////centerX = bounds.Value.Left + info.HorizontalConnectingLine.X1;
                }

                ////VerticalConnectingLine.X1 = centerX;
                ////VerticalConnectingLine.X2 = centerX;
                VerticalConnectingLine.Y2 = bottomY;
            }

            ////// Connect the top of the line to just below the header
            ////if (Header == null)
            ////{
            ////    VerticalConnectingLine.Y1 = 0;
            ////}
            ////else
            ////{
            ////    Rect? bounds = Header.GetBoundsRelativeTo(VerticalConnectingLine);
            ////    VerticalConnectingLine.Y1 = (bounds != null) ?
            ////        bounds.Value.Y :
            ////        0;
            ////}

            VerticalConnectingLine.Visibility = Visibility.Visible;
        }
    }
}