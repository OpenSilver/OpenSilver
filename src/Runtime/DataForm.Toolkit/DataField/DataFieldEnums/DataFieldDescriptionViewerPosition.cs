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
    /// Enumeration denoting a description viewer position.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum DataFieldDescriptionViewerPosition
    {
        /// <summary>
        /// Represents the case where a description position should be
        /// dependent on its parent (same behavior as BesideContent when there
        /// is no parent).
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Represents the case where the description should be placed to the
        /// right of the input control.
        /// </summary>
        BesideContent = 1,

        /// <summary>
        /// Represents the case where the description should be placed to the
        /// right of the label.
        /// </summary>
        BesideLabel = 2,
    }
}
