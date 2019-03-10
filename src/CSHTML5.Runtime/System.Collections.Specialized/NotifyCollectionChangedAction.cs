
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