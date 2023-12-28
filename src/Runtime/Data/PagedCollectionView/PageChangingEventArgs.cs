//-----------------------------------------------------------------------
// <copyright file="PageChangingEventArgs.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel
{
    /// <summary>
    /// Event argument used for page index change notifications. The requested page move
    /// can be canceled by setting e.Cancel to True.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class PageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructor that takes the target page index
        /// </summary>
        /// <param name="newPageIndex">Index of the requested page</param>
        public PageChangingEventArgs(int newPageIndex)
        {
            this.NewPageIndex = newPageIndex;
        }

        /// <summary>
        /// Gets the index of the requested page
        /// </summary>
        public int NewPageIndex
        {
            get;
            private set;
        }
    }
}
