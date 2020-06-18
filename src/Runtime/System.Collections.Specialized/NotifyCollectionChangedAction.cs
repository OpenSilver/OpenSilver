
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



namespace System.Collections.Specialized
{
    /// <summary>
    /// Describes the action that caused a System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged event.
    /// </summary>
    public enum NotifyCollectionChangedAction
    {
        /// <summary>
        /// One or more items were added to the collection.
        /// </summary>
        Add = 0,
        /// <summary>
        /// One or more items were removed from the collection.
        /// </summary>
        Remove = 1,
        /// <summary>
        ///     One or more items were replaced in the collection.
        /// </summary>
        Replace = 2,
        /// <summary>
        ///     One or more items were moved within the collection.
        /// </summary>
        Move = 3,
        /// <summary>
        ///     The content of the collection changed dramatically.
        /// </summary>
        Reset = 4,
    }
}