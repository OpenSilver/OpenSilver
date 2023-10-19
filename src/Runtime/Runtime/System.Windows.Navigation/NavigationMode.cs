//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Navigation
{
    /// <summary>
    /// Specifies the type of navigation that is occurring.
    /// </summary>
    public enum NavigationMode : byte
    {
        /// <summary>
        /// New navigation.
        /// </summary>
        New = 0,

        /// <summary>
        /// Navigating back in history.
        /// </summary>
        Back = 1,

        /// <summary>
        /// Navigating forward in history.
        /// </summary>
        Forward = 2,

        /// <summary>
        /// Reloading the current content.
        /// </summary>
        Refresh = 3
    }
}