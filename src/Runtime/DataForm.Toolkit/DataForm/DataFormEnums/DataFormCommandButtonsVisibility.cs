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
    /// Enumeration denoting which of the command buttons should be visible.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [Flags]
    public enum DataFormCommandButtonsVisibility
    {
        /// <summary>
        /// Represents the case where no buttons are visible.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents the case where the add button is visible.
        /// </summary>
        Add = 1,

        /// <summary>
        /// Represents the case where the delete button is visible.
        /// </summary>
        Delete = 2,

        /// <summary>
        /// Represents the case where the edit button is visible.
        /// </summary>
        Edit = 4,

        /// <summary>
        /// Represents the case where the navigation buttons are visible.
        /// </summary>
        Navigation = 8,

        /// <summary>
        /// Represents the case where the commit button is visible.
        /// </summary>
        Commit = 16,

        /// <summary>
        /// Represents the case where the cancel button is visible.
        /// </summary>
        Cancel = 32,

        /// <summary>
        /// Represents the case where all buttons is visible.
        /// </summary>
        All = Add | Delete | Edit | Navigation | Commit | Cancel,
    }
}
