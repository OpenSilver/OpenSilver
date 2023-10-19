//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls
{
    /// <summary>
    /// Enumeration denoting a label position.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum DataFieldLabelPosition
    {
        /// <summary>
        /// Represents the case where a field label position should be
        /// dependent on its parent (same behavior as Left when there
        /// is no parent).
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Represents the case where the label should be placed to the
        /// left of the input control.
        /// </summary>
        Left = 1,

        /// <summary>
        /// Represents the case where the label should be placed above
        /// the input control.
        /// </summary>
        Top = 2,
    }
}
