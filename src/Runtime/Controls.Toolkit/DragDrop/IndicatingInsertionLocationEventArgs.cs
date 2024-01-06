// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Media;
using SW = Microsoft.Windows;

namespace System.Windows.Controls
{
    /// <summary>
    /// Information about the IndicatingInsertionLocation event.
    /// </summary>
    /// <typeparam name="TItemsControlType">The type of the control that 
    /// contains the items that can be dragged.</typeparam>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class IndicatingInsertionLocationEventArgs<TItemsControlType> : SW.ExtendedRoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the SW.DragEventArgs related to the event.
        /// </summary>
        internal SW.DragEventArgs DragEventArgs { get; set; }

        /// <summary>
        /// Gets the drop target.
        /// </summary>
        public TItemsControlType DropTarget { get; internal set; }

        /// <summary>
        /// Gets the insertion index.
        /// </summary>
        public int? InsertionIndex { get; internal set; }

        /// <summary>
        /// Gets or sets the geometry to use to indicate the insertion point.
        /// </summary>
        public Geometry InsertionIndicatorGeometry { get; set; }

        /// <summary>
        /// Returns a drop point that is relative to a specified System.Windows.UIElement.
        /// </summary>
        /// <param name="relativeTo">A UIElement object for which to get a relative drop point.</param>
        /// <returns>A drop point that is relative to the element specified in relativeTo.</returns>
        public Point GetPosition(UIElement relativeTo)
        {
            if (DragEventArgs != null && (relativeTo == Application.Current.RootVisual || VisualTreeHelper.GetParent(relativeTo) != null))
            {
                return DragEventArgs.GetPosition(relativeTo);
            }
            return new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Initializes a new instance of the IndicationInsertionLocationEventArgs class.
        /// </summary>
        internal IndicatingInsertionLocationEventArgs()
        {
        }
    }
}