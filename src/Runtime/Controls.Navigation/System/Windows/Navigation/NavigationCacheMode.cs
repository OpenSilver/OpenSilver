//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Used to describe how a <see cref="Page"/> should be cached when
    /// used by a <see cref="Frame"/>
    /// </summary>
    public enum NavigationCacheMode
    {
        /// <summary>
        /// The <see cref="Page"/> should never be cached, and a new
        /// instance should be created on each navigation.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// The <see cref="Page"/> should always be cached, and kept
        /// around forever, reused in all subsequent navigations
        /// to the same Uri.
        /// </summary>
        Required = 1,

        /// <summary>
        /// The <see cref="Page"/> should be cached only within
        /// the size of the cache on the <see cref="Frame"/>,
        /// and thrown away if it would exceed that.
        /// </summary>
        Enabled = 2
    }
}
