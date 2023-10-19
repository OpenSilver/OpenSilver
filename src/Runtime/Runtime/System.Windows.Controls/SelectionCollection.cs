
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

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
        /// Returns data as a SelectionCollection.
        /// </summary>
        /// <param name="data">The data object.</param>
        /// <returns>A selection collection.</returns>
        internal static SelectionCollection ToSelectionCollection(object data)
        {
            //------------------------
            // The purpose of this method is to wrap data into a "Selection" class
            // (unless it is already a Selection), and then put the Selection
            // into a SelectionCollection (unless it is already a SelectionCollection).
            //------------------------

            if (data == null)
            {
                return new SelectionCollection();
            }

            SelectionCollection selectionCollection = data as SelectionCollection;
            if (selectionCollection == null)
            {
                selectionCollection = new SelectionCollection();
                Selection selection = data as Selection;
                if (selection == null)
                {
                    selection = new Selection(data);
                }
                selectionCollection.Add(selection);
            }

            return selectionCollection;
        }

        /// <summary>
        /// Gets a sequence of the items in the selection collection.
        /// </summary>
        internal IEnumerable<object> SelectedItems { get { return this.Select(selection => selection.Item); } }
    }
}