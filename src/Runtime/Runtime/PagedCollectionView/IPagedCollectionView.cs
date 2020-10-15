//-----------------------------------------------------------------------
// <copyright file="IPagedCollectionView.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel
{
    /// <summary>
    /// Interface used to drive paging of collection views.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface IPagedCollectionView
    {
        /// <summary>
        /// Raised when a page index change completed.
        /// </summary>
        event EventHandler<EventArgs> PageChanged;

        /// <summary>
        /// Raised when a page index change is requested.
        /// </summary>
        event EventHandler<PageChangingEventArgs> PageChanging;

        /// <summary>
        /// Gets a value indicating whether the PageIndex value is allowed to change or not.
        /// </summary>
        bool CanChangePage
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether a page index change is in process or not.
        /// </summary>
        bool IsPageChanging
        {
            get;
        }

        /// <summary>
        /// Gets the number of known items in the view before paging is applied.
        /// </summary>
        int ItemCount
        {
            get;
        }

        /// <summary>
        /// Gets the current page we are on. (zero based)
        /// </summary>
        int PageIndex
        {
            get;
        }

        /// <summary>
        /// Gets or sets the number of items to display on a page.
        /// </summary>
        int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total number of items in the view before paging is applied,
        /// or -1 if that total number is unknown.
        /// </summary>
        int TotalItemCount
        {
            get;
        }

        /// <summary>
        /// Moves to the first page.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToFirstPage();

        /// <summary>
        /// Moves to the last page.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToLastPage();

        /// <summary>
        /// Moves to the page after the current page we are on.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToNextPage();

        /// <summary>
        /// Moves to the page before the current page we are on.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToPreviousPage();

        /// <summary>
        /// Moves to page <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">The index of the page to which to move.</param>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToPage(int pageIndex);
    }
}
