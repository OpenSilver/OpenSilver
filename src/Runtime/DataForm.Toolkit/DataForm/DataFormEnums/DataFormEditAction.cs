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
    /// Enumeration denoting an action taken to end an edit.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum DataFormEditAction
    {
        /// <summary>
        /// Represents the case where an edit has been canceled.
        /// </summary>
        Cancel = 0,

        /// <summary>
        /// Represents the case where an edit has been committed.
        /// </summary>
        Commit = 1,
    }
}
