// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using SW = Microsoft.Windows;

namespace System.Windows
{
    /// <summary>
    /// Information about the SW.DragDropCompleted event.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class DragDropCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the effect of the drag operation.
        /// </summary>
        public SW.DragDropEffects Effects { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the DragCompletedEventArgs class.
        /// </summary>
        internal DragDropCompletedEventArgs()
        {
        }
    }
}