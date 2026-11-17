
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

using System.Collections.Specialized;

namespace System.Windows.Controls.Primitives
{
	/// <summary>
	/// The ItemsChanged event is raised by an ItemContainerGenerator to inform
	/// layouts that the items collection has changed.
	/// </summary>
	public class ItemsChangedEventArgs : EventArgs
	{
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        internal ItemsChangedEventArgs(NotifyCollectionChangedAction action,
                                        GeneratorPosition position,
                                        GeneratorPosition oldPosition,
                                        int itemCount,
                                        int itemUICount)
        {
            _action = action;
            _position = position;
            _oldPosition = oldPosition;
            _itemCount = itemCount;
            _itemUICount = itemUICount;
        }

        internal ItemsChangedEventArgs(NotifyCollectionChangedAction action,
                                        GeneratorPosition position,
                                        int itemCount,
                                        int itemUICount) : this(action, position, new GeneratorPosition(-1, 0), itemCount, itemUICount)

        {
        }

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        /// <summary> What happened </summary>
        public NotifyCollectionChangedAction Action { get { return _action; } }

        /// <summary> Where it happened </summary>
        public GeneratorPosition Position { get { return _position; } }

        /// <summary> Where it happened </summary>
        public GeneratorPosition OldPosition { get { return _oldPosition; } }

        /// <summary> How many items were involved </summary>
        public int ItemCount { get { return _itemCount; } }

        /// <summary> How many UI elements were involved </summary>
        public int ItemUICount { get { return _itemUICount; } }

        // Index of the item in the items collection (after the change for Add and Move, before it for
        // Remove), or -1 if unknown.
        internal int ItemIndex { get; set; } = -1;

        // For Move, index of the item before the change, or -1 if unknown.
        internal int OldItemIndex { get; set; } = -1;


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        NotifyCollectionChangedAction _action;
        GeneratorPosition _position;
        GeneratorPosition _oldPosition;
        int _itemCount;
        int _itemUICount;
    }

    /// <summary>
    ///     The delegate to use for handlers that receive ItemsChangedEventArgs.
    /// </summary>
    public delegate void ItemsChangedEventHandler(object sender, ItemsChangedEventArgs e);
}
