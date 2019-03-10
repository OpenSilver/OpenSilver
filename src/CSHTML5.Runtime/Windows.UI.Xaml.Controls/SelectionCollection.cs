
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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