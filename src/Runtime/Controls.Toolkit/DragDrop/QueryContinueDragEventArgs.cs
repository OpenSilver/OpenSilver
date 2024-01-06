// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows
{
    /// <summary>
    /// Contains arguments for the System.Windows.DragDrop.QueryContinueDrag event.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class QueryContinueDragEventArgs : ExtendedRoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the current status of the associated drag-and-drop operation.
        /// </summary>
        public DragAction Action { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ESC key has been pressed.
        /// </summary>
        public bool EscapePressed { get; internal set; }

        /// <summary>
        /// Gets a flag enumeration Indicating the current state of the SHIFT, CTRL,
        /// and ALT keys, as well as the state of the mouse buttons.
        /// </summary>
        public DragDropKeyStates KeyStates { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the QueryContinueDragEventArgs class.
        /// </summary>
        internal QueryContinueDragEventArgs()
        {
        }
    }
}