
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



#if MIGRATION
namespace System.Collections.ObjectModel
#else
namespace Windows.UI.Xaml.Controls
#endif
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