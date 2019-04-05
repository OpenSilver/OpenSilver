
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Specialized
{
    /// <summary>
    /// Provides data for the System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged
    /// event.
    /// </summary>
    public class NotifyCollectionChangedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class
        /// that describes a System.Collections.Specialized.NotifyCollectionChangedAction.Reset change.
        /// </summary>
        /// <param name="action">The action that caused the event. This must be set to System.Collections.Specialized.NotifyCollectionChangedAction.Reset.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class that describes a multi-item change.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to System.Collections.Specialized.NotifyCollectionChangedAction.Reset,
        /// System.Collections.Specialized.NotifyCollectionChangedAction.Add, or System.Collections.Specialized.NotifyCollectionChangedAction.Remove.
        /// </param>
        /// <param name="changedItems">The items that are affected by the change.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
        {
            Action = action;
            if (action == NotifyCollectionChangedAction.Reset || action == NotifyCollectionChangedAction.Remove)
            {
                OldItems = changedItems;
            }
            else if (action == NotifyCollectionChangedAction.Add)
            {
                NewItems = changedItems;
            }
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class that describes a one-item change.
        /// Throws an exception if action is not Reset, Add, or Remove, or if action is Reset and changedItem is not null.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to System.Collections.Specialized.NotifyCollectionChangedAction.Reset, System.Collections.Specialized.NotifyCollectionChangedAction.Add, or System.Collections.Specialized.NotifyCollectionChangedAction.Remove.</param>
        /// <param name="changedItem">The item that is affected by the change.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
        {
            if ((action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove && action != NotifyCollectionChangedAction.Reset) || (action == NotifyCollectionChangedAction.Reset && changedItem != null))
            {
                throw new ArgumentException();
            }
            if (action == NotifyCollectionChangedAction.Add)
            {
                NewItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
                NewItems.Add(changedItem);
            }
            if (action == NotifyCollectionChangedAction.Remove || action == NotifyCollectionChangedAction.Reset)
            {
                OldItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
                OldItems.Add(changedItem);
            }

            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class
        /// that describes a multi-item System.Collections.Specialized.NotifyCollectionChangedAction.Replace change.
        /// Exceptions:
        ///   System.ArgumentException:
        ///     If action is not Replace.
        /// 
        ///   System.ArgumentNullException:
        ///     If oldItems or newItems is null.
        /// </summary>
        /// <param name="action">The action that caused the event. This can only be set to System.Collections.Specialized.NotifyCollectionChangedAction.Replace.</param>
        /// <param name="newItems">The new items that are replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException();
            }
            if (oldItems == null || newItems == null)
            {
                throw new ArgumentNullException();
            }
            NewItems = newItems;
            OldItems = oldItems;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class
        /// that describes a multi-item change or a System.Collections.Specialized.NotifyCollectionChangedAction.Reset change.
        /// Exceptions:
        ///   System.ArgumentException:
        ///     If action is not Reset, Add, or Remove, if action is Reset and either changedItems
        ///     is not null or startingIndex is not -1, or if action is Add or Remove and
        ///     startingIndex is less than -1.
        /// 
        ///   System.ArgumentNullException:
        ///     If action is Add or Remove and changedItems is null.
        /// </summary>
        /// <param name="action">
        /// The action that caused the event. This can be set to System.Collections.Specialized.NotifyCollectionChangedAction.Reset,
        /// System.Collections.Specialized.NotifyCollectionChangedAction.Add, or System.Collections.Specialized.NotifyCollectionChangedAction.Remove.
        /// </param>
        /// <param name="changedItems">The items affected by the change.</param>
        /// <param name="startingIndex">The index where the change occurred.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            if ((action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove && action != NotifyCollectionChangedAction.Reset) 
                || (action == NotifyCollectionChangedAction.Reset && (changedItems != null || startingIndex != -1))
                || (action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Remove) && startingIndex < 0) //todo: check if <0 or <-1 (we put <0 because it seems more logical)
            {
                throw new ArgumentException();
            }
            if ((action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Remove) && changedItems == null)
            {
                throw new ArgumentNullException();
            }
            if (action == NotifyCollectionChangedAction.Add)
            {
                NewItems = changedItems;
                NewStartingIndex = startingIndex;
            }
            else //we know it is either remove or reset
            {
                OldItems = changedItems;
                OldStartingIndex = startingIndex;
            }
            
            Action = action;
        }
       
        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class that describes a one-item change.
        /// Exceptions:
        ///   System.ArgumentException:
        ///     If action is not Reset, Add, or Remove, or if action is Reset and either
        ///     changedItems is not null or index is not -1.
        /// </summary>
        /// <param name="action">
        /// The action that caused the event. This can be set to System.Collections.Specialized.NotifyCollectionChangedAction.Reset,
        /// System.Collections.Specialized.NotifyCollectionChangedAction.Add, or System.Collections.Specialized.NotifyCollectionChangedAction.Remove.
        /// </param>
        /// <param name="changedItem">The item that is affected by the change.</param>
        /// <param name="index">The index where the change occurred.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            if ((action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove && action != NotifyCollectionChangedAction.Reset)
                || (action == NotifyCollectionChangedAction.Reset && (changedItem != null || index != -1)))
            {
                throw new ArgumentException();
            }

            if (action == NotifyCollectionChangedAction.Add)
            {
                NewItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
                NewItems.Add(changedItem);
                NewStartingIndex = index;
            }
            if (action == NotifyCollectionChangedAction.Remove || action == NotifyCollectionChangedAction.Reset)
            {
                OldItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
                OldItems.Add(changedItem);
                OldStartingIndex = index;
            }

            Action = action;
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class
        /// that describes a one-item System.Collections.Specialized.NotifyCollectionChangedAction.Replace change.
        /// System.ArgumentException:
        ///   If action is not Replace.
        /// </summary>
        /// <param name="action">The action that caused the event. This can only be set to System.Collections.Specialized.NotifyCollectionChangedAction.Replace.</param>
        /// <param name="newItem">The new item that is replacing the original item.</param>
        /// <param name="oldItem">The original item that is replaced.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException();
            }
            NewItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
            NewItems.Add(newItem);

            OldItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
            OldItems.Add(oldItem);

            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs class
        /// that describes a multi-item System.Collections.Specialized.NotifyCollectionChangedAction.Replace change.
        /// Exceptions:
        ///   System.ArgumentException:
        ///     If action is not Replace.
        /// 
        ///   System.ArgumentNullException:
        ///     If oldItems or newItems is null.
        /// </summary>
        /// <param name="action">The action that caused the event. This can only be set to System.Collections.Specialized.NotifyCollectionChangedAction.Replace.</param>
        /// <param name="newItems">The new items that are replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        /// <param name="startingIndex">The index of the first item of the items that are being replaced.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException();
            }

            NewItems = newItems;
            OldItems = oldItems;
            OldStartingIndex = startingIndex;
            Action = action;
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs
        /// class that describes a multi-item System.Collections.Specialized.NotifyCollectionChangedAction.Move
        /// change.
        /// Exceptions:
        ///   System.ArgumentException:
        ///     If action is not Move or index is less than 0.
        /// </summary>
        /// <param name="action">The action that caused the event. This can only be set to System.Collections.Specialized.NotifyCollectionChangedAction.Move.</param>
        /// <param name="changedItems">The items affected by the change.</param>
        /// <param name="index">The new index for the changed items.</param>
        /// <param name="oldIndex">The old index for the changed items.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        {
            if (action != NotifyCollectionChangedAction.Move || index < 0)
            {
                throw new ArgumentException();
            }
            OldItems = changedItems; //todo: see if it is OldItems that should be filled or NewItems
            NewStartingIndex = index;
            OldStartingIndex = oldIndex;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs
        /// class that describes a one-item System.Collections.Specialized.NotifyCollectionChangedAction.Move
        /// change.
        /// Exceptions:
        ///   System.ArgumentException:
        ///     If action is not Move or index is less than 0.
        /// </summary>
        /// <param name="action">The action that caused the event. This can only be set to System.Collections.Specialized.NotifyCollectionChangedAction.Move.</param>
        /// <param name="changedItem">The item affected by the change.</param>
        /// <param name="index">The new index for the changed item.</param>
        /// <param name="oldIndex">The old index for the changed item.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
        {
            if (action != NotifyCollectionChangedAction.Move || index < 0)
            {
                throw new ArgumentException();
            }
            OldItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
            OldItems.Add(changedItem);//todo: see if it is OldItems that should be filled or NewItems
            NewStartingIndex = index;
            OldStartingIndex = oldIndex;
            Action = action;
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Collections.Specialized.NotifyCollectionChangedEventArgs
        /// class that describes a one-item System.Collections.Specialized.NotifyCollectionChangedAction.Replace
        /// change.
        /// Exceptions:
        ///   System.ArgumentException:
        ///     If action is not Replace.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to System.Collections.Specialized.NotifyCollectionChangedAction.Replace.</param>
        /// <param name="newItem">The new item that is replacing the original item.</param>
        /// <param name="oldItem">The original item that is replaced.</param>
        /// <param name="index">The index of the item being replaced.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException();
            }

            NewItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
            NewItems.Add(newItem);

            OldItems = new List<object>(); //todo: see if we actually get a List here or if it is another type of IList
            OldItems.Add(oldItem);

            OldStartingIndex = index;
            Action = action;
        }

        
        private NotifyCollectionChangedAction _action;
        /// <summary>
        /// Gets the action that caused the event. Returns a System.Collections.Specialized.NotifyCollectionChangedAction value that describes the action that caused the event.
        /// </summary>
        public NotifyCollectionChangedAction Action
        {
            get { return _action; }
            internal set
            {
                _action = value;
            }
        }



        private IList _newItems;
        /// <summary>
        /// Gets the list of new items involved in the change. Returns the list of new items involved in the change.
        /// </summary>
        public IList NewItems
        {
            get { return _newItems; }
            internal set
            {
                _newItems = value;
            }
        }



        private int _newStartingIndex;
        /// <summary>
        /// Gets the index at which the change occurred.
        /// Returns the zero-based index at which the change occurred.
        /// </summary>
        public int NewStartingIndex
        {
            get { return _newStartingIndex; }
            internal set
            {
                _newStartingIndex = value;
            }
        }


        private IList _oldItems;
        /// <summary>
        /// Gets the list of items affected by a System.Collections.Specialized.NotifyCollectionChangedAction.Replace, Remove, or Move action.
        /// Returns he list of items affected by a System.Collections.Specialized.NotifyCollectionChangedAction.Replace, Remove, or Move action.
        /// </summary>
        public IList OldItems
        {
            get { return _oldItems; }
            internal set
            {
                _oldItems = value;
            }
        }


        private int _oldStartingIndex;
        /// <summary>
        /// Gets the index at which a System.Collections.Specialized.NotifyCollectionChangedAction.Move, Remove, or Replace action occurred.
        /// Returns the zero-based index at which a System.Collections.Specialized.NotifyCollectionChangedAction.Move, Remove, or Replace action occurred.
        /// </summary>
        public int OldStartingIndex
        {
            get { return _oldStartingIndex; }
            internal set
            {
                _oldStartingIndex = value;
            }
        }


    }
}