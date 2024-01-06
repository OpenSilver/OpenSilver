// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A collection of selected items.
    /// </summary>
    public sealed class SelectionCollection : Collection<Selection>
    {
        /// <summary>
        /// Returns data as a SelectionCollection.
        /// </summary>
        /// <param name="data">The data object.</param>
        /// <returns>A selection collection.</returns>
        internal static SelectionCollection ToSelectionCollection(object data)
        {
            if (data == null)
            {
                return new SelectionCollection();
            }

            SelectionCollection selectionCollection = data as SelectionCollection;
            if (selectionCollection == null)
            {
                selectionCollection = new SelectionCollection();
                Selection selection = data as Selection;
                if (selection != null)
                {
                    selection = new Selection(data);
                }
                selectionCollection.Add(selection);
            }

            return selectionCollection;
        }

        /// <summary>
        /// Initializes a new instance of a SelectionCollection.
        /// </summary>
        public SelectionCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of a SelectionCollection.
        /// </summary>
        /// <param name="items">The items to include in the selection 
        /// collection.</param>
        public SelectionCollection(IEnumerable<object> items)
        {
            foreach (object item in items)
            {
                this.Add(new Selection(item));
            }
        }

        /// <summary>
        /// Gets a sequence of the items in the selection collection.
        /// </summary>
        internal IEnumerable<object> SelectedItems { get { return this.Select(selection => selection.Item); } }
    }
}