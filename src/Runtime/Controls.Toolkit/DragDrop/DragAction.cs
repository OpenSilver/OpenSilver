// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows
{
    /// <summary>
    /// Specifies how and if a drag-and-drop operation should continue.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public enum DragAction
    {
        /// <summary>
        /// The operation will continue.
        /// </summary>
        Continue = 0,

        /// <summary>
        /// The operation will stop with a drop.
        /// </summary>
        Drop = 1,

        /// <summary>
        /// The operation is canceled with no drop message.
        /// </summary>
        Cancel = 2,
    }
}