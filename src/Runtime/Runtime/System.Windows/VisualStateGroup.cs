
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using OpenSilver.Internal;

namespace System.Windows
{
    /// <summary>
    /// Contains mutually exclusive VisualState objects and VisualTransition objects
    /// that are used to go from one state to another.
    /// </summary>
    [ContentProperty(nameof(States))]
    public sealed class VisualStateGroup : DependencyObject
    {
        private Collection<VisualState> _states;

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
            get => (string)GetValue(FrameworkElement.NameProperty);
            [EditorBrowsable(EditorBrowsableState.Never)]
            set => SetValue(FrameworkElement.NameProperty, value);
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
                    this._states = new Collection<VisualState>(new VisualStatesCollection(this));
                }
                return this._states; 
            } 
        } 

        private IList _transitions;

        /// <summary>
        /// Gets the collection of VisualTransition objects.
        /// </summary>
        [OpenSilver.NotImplemented]
        public IList Transitions
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
}