
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