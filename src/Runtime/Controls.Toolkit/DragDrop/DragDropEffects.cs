// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows
{
    /// <summary>
    /// Specifies the effects of a drag-and-drop operation.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [Flags]
    public enum DragDropEffects
    {
        /// <summary>
        /// Scrolling is about to start or is currently occurring in the drop target.
        /// </summary>
        Scroll = -2147483648,

        /// <summary>
        /// The data is copied, removed from the drag source, and scrolled in the drop
        /// target.
        /// </summary>
        All = -2147483645,

        /// <summary>
        /// The drop target does not accept the data.
        /// </summary>
        None = 0,

        /// <summary>
        /// The data is copied to the drop target.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// The data from the drag source is moved to the drop target.
        /// </summary>
        Move = 2,

        /// <summary>
        /// The data from the drag source is linked to the drop target.
        /// </summary>
        Link = 4,
    }
}