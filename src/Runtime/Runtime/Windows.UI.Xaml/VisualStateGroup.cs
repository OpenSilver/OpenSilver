

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


using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Contains mutually exclusive VisualState objects and VisualTransition objects
    /// that are used to go from one state to another.
    /// </summary>
    [ContentProperty("States")]
    public sealed partial class VisualStateGroup : DependencyObject
    {
        private VisualStatesCollection _states;

        /// <summary>
        /// Gets the most recently set VisualState from a successful call to the GoToState
        /// method.
        /// </summary>
        public VisualState CurrentState
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the VisualStateGroup.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of mutually exclusive VisualState objects.
        /// </summary>
        public IList States
        {
            get 
            {
                if (this._states == null)
                {
                    this._states = new VisualStatesCollection(this);
                }
                return this._states; 
            } 
        } 

#if WORKINPROGRESS

        private IList<VisualTransition> _transitions;

        /// <summary>
        /// Gets the collection of VisualTransition objects.
        /// </summary>
        [OpenSilver.NotImplemented]
        public IList<VisualTransition> Transitions
        {
            get
            {
                if(this._transitions == null)
                {
                    this._transitions = new List<VisualTransition>();
                }
                return this._transitions;
            }
        }
#endif

        internal VisualState GetState(string stateName)
        {
            for (int stateIndex = 0; stateIndex < States.Count; ++stateIndex)
            {
                VisualState state = (VisualState)States[stateIndex];
                if (state.Name == stateName)
                {
                    return state;
                }
            }

            return null;
        }

        internal void RaiseCurrentStateChanging(FrameworkElement stateGroupsRoot, VisualState oldState, VisualState newState, Control control)
        {
            if (CurrentStateChanging != null)
            {
                CurrentStateChanging(stateGroupsRoot, new VisualStateChangedEventArgs(oldState, newState, control));
            }
        }

        internal void RaiseCurrentStateChanged(FrameworkElement stateGroupsRoot, VisualState oldState, VisualState newState, Control control)
        {
            if (CurrentStateChanged != null)
            {
                CurrentStateChanged(stateGroupsRoot, new VisualStateChangedEventArgs(oldState, newState, control));
            }
        }

        /// <summary>
        ///     Raised when transition begins
        /// </summary>
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanged;

        /// <summary>
        ///     Raised when transition ends and new state storyboard begins.
        /// </summary>
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanging;
    }

    internal class VisualStatesCollection : PresentationFrameworkCollection<VisualState>
    {
        private readonly VisualStateGroup group;

        internal VisualStatesCollection(VisualStateGroup group)
        {
            Debug.Assert(group != null, "group should not be null !"); 
            this.group = group;
        }

        internal override void AddOverride(VisualState value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            value.INTERNAL_Group = this.group;
            this.AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            foreach (VisualState state in this)
            {
                state.INTERNAL_Group = null;
            }
            this.ClearDependencyObjectInternal();
        }

        internal override VisualState GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void InsertOverride(int index, VisualState value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            value.INTERNAL_Group = this.group;
            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this.GetItemInternal(index).INTERNAL_Group = null;
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override bool RemoveOverride(VisualState value)
        {
            int index = this.IndexOf(value);
            if (index > -1)
            {
                value.INTERNAL_Group = null;
                this.RemoveAtDependencyObjectInternal(index);
                return true;
            }
            return false;
        }

        internal override void SetItemOverride(int index, VisualState value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (index < 0 || index >= this.CountInternal - 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this.GetItemInternal(index).INTERNAL_Group = null;
            this.SetItemDependencyObjectInternal(index, value);
        }
    }
}