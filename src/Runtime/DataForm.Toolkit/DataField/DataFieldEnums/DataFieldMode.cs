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
    /// Enumeration denoting a DataField mode.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum DataFieldMode
    {
        /// <summary>
        /// Represents the case where the field should inherit its mode
        /// from the parent DataForm.  Behavior is the same as Edit if
        /// there is no parent DataForm.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Represents the case where the field is read-only.
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// Represents the case where the field is in edit mode.
        /// </summary>
        Edit = 2,

        /// <summary>
        /// Represents the case where the field is in add-new mode.
        /// </summary>
        AddNew = 3,
    }
}
