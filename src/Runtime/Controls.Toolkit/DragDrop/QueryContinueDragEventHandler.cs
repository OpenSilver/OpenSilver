// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows
{
    /// <summary>
    /// Represents a method that will handle the routed events that enables a drag-and-drop
    /// operation to be canceled by the drag source, for example System.Windows.UIElement.QueryContinueDrag.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    /// <QualityBand>Experimental</QualityBand>
    public delegate void QueryContinueDragEventHandler(object sender, QueryContinueDragEventArgs e);
}