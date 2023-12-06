// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// This type is used to determine the state of the item selected and the
    /// previous items.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum RatingSelectionMode
    {
        /// <summary>
        /// All items before the selected ones are selected.
        /// </summary>
        Continuous,

        /// <summary>
        /// Only the item selected is visually distinguished.
        /// </summary>
        Individual
    }
}
