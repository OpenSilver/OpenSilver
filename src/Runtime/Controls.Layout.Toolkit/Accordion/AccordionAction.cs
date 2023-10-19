// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Determines the action the AccordionItem will perform.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    internal enum AccordionAction
    {
        /// <summary>
        /// No action will be performed.
        /// </summary>
        None,

        /// <summary>
        /// A collapse will be performed.
        /// </summary>
        Collapse,

        /// <summary>
        /// An expand will be performed.
        /// </summary>
        Expand,

        /// <summary>
        /// A resize will be performed.
        /// </summary>
        Resize
    }
}