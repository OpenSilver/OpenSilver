// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows
{
    /// <summary>
    /// Represents a method that will handle drag-and-drop routed events, for example
    /// System.Windows.UIElement.DragEnter.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    /// <QualityBand>Experimental</QualityBand>
    public delegate void DragEventHandler(object sender, DragEventArgs e);
}