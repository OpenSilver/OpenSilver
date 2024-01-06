// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A class that holds a selected item.
    /// </summary>
    public sealed class Selection
    {
        /// <summary>
        /// Initializes an instance of the Selection class.
        /// </summary>
        /// <param name="index">The index of the selected item within the 
        /// source collection.</param>
        /// <param name="item">The selected item.</param>
        public Selection(int? index, object item)
        {
            this.Index = index;
            this.Item = item;
        }

        /// <summary>
        /// Initializes an instance of the Selection class.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public Selection(object item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the index of the selection within the source collection.
        /// </summary>
        public int? Index { get; internal set; }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public object Item { get; internal set; }
    }
}