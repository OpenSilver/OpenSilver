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
    /// Enumeration denoting a DataForm mode.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum DataFormMode
    {
        /// <summary>
        /// Represents the case where an object is not being edited.
        /// </summary>
        ReadOnly = 0,

        /// <summary>
        /// Represents the case where an object is being edited.
        /// </summary>
        Edit = 1,

        /// <summary>
        /// Represents the case where a new object is being added.
        /// </summary>
        AddNew = 2,
    }
}
