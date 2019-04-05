
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#if MIGRATION
namespace System.Collections.ObjectModel
#else
namespace Windows.UI.Xaml.Controls
#endif
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