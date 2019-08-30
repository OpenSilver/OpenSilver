using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>Defines methods and properties that a collection view implements to provide paging capabilities to a collection.</summary>
    public interface IPagedCollectionView
    {
        /// <summary>When implementing this interface, raise this event after the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex" /> has changed.</summary>
        event EventHandler<EventArgs> PageChanged;

        /// <summary>When implementing this interface, raise this event before changing the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex" />. The event handler can cancel this event.</summary>
        event EventHandler<PageChangingEventArgs> PageChanging;

        /// <summary>Gets a value that indicates whether the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex" /> value can change.</summary>
        /// <returns>true if the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex" /> value can change; otherwise, false.</returns>
        bool CanChangePage { get; }

        /// <summary>Gets a value that indicates whether the page index is changing.</summary>
        /// <returns>true if the page index is changing; otherwise, false.</returns>
        bool IsPageChanging { get; }

        /// <summary>Gets the number of known items in the view before paging is applied.</summary>
        /// <returns>The number of known items in the view before paging is applied.</returns>
        int ItemCount { get; }

        /// <summary>Gets the zero-based index of the current page.</summary>
        /// <returns>The zero-based index of the current page.</returns>
        int PageIndex { get; }

        /// <summary>Gets or sets the number of items to display on a page.</summary>
        /// <returns>The number of items to display on a page.</returns>
        int PageSize { get; set; }

        /// <summary>Gets the total number of items in the view before paging is applied.</summary>
        /// <returns>The total number of items in the view before paging is applied, or -1 if the total number is unknown.</returns>
        int TotalItemCount { get; }

        /// <summary>Sets the first page as the current page.</summary>
        /// <returns>true if the operation was successful; otherwise, false.</returns>
        bool MoveToFirstPage();

        /// <summary>Sets the last page as the current page.</summary>
        /// <returns>true if the operation was successful; otherwise, false.</returns>
        bool MoveToLastPage();

        /// <summary>Moves to the page after the current page.</summary>
        /// <returns>true if the operation was successful; otherwise, false.</returns>
        bool MoveToNextPage();

        /// <summary>Moves to the page before the current page.</summary>
        /// <returns>true if the operation was successful; otherwise, false.</returns>
        bool MoveToPreviousPage();

        /// <summary>Moves to the page at the specified index.</summary>
        /// <returns>true if the operation was successful; otherwise, false.</returns>
        /// <param name="pageIndex">The index of the page to move to.</param>
        bool MoveToPage(int pageIndex);
    }
}
