using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>Provides data for the <see cref="E:System.ComponentModel.IPagedCollectionView.PageChanging" /> event.</summary>
    public sealed class PageChangingEventArgs : CancelEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PageChangingEventArgs" /> class.</summary>
        /// <param name="newPageIndex">The index of the requested page.</param>
        public PageChangingEventArgs(int newPageIndex)
        {
            this.NewPageIndex = newPageIndex;
        }

        /// <summary>Gets the index of the requested page.</summary>
        /// <returns>The index of the requested page.</returns>
        public int NewPageIndex { get; private set; }
    }
}
