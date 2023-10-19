// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Determines the order in which visual states are set.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum SelectionSequence
    {
        /// <summary>
        /// Collapses are set before expansions.
        /// </summary>
        CollapseBeforeExpand,

        /// <summary>
        /// No delays, all states are set immediately.
        /// </summary>
        Simultaneous
    }
}