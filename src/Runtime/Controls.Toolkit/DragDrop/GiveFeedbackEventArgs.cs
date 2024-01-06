// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows
{
    /// <summary>
    /// Contains arguments for the System.Windows.DragDrop.GiveFeedback event.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class GiveFeedbackEventArgs : ExtendedRoutedEventArgs
    {       
        /// <summary>
        /// Gets a value that indicates the effects of drag-and-drop operation.
        /// </summary>
        public DragDropEffects Effects { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the GiveFeedbackEventArgs class.
        /// </summary>
        internal GiveFeedbackEventArgs()
        {
        }
    }
}