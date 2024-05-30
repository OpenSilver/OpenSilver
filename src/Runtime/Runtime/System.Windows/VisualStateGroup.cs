
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

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml.Markup;
using OpenSilver.Internal;

namespace System.Windows
{
    /// <summary>
    /// Contains mutually exclusive <see cref="VisualState"/> objects and <see cref="VisualTransition"/> 
    /// objects that are used to go from one state to another.
    /// </summary>
    [ContentProperty(nameof(States))]
    [RuntimeNameProperty(nameof(Name))]
    public sealed class VisualStateGroup : DependencyObject
    {
        private Collection<VisualState> _states;
        private Collection<VisualTransition> _transitions;

        /// <summary>
        /// Gets the most recently set <see cref="VisualState"/> from a successful call to the 
        /// <see cref="VisualStateManager.GoToState(Control, string, bool)"/> method.
        /// </summary>
        /// <returns>
        /// The most recently set <see cref="VisualState"/> from a successful call to the
        /// <see cref="VisualStateManager.GoToState(Control, string, bool)"/> method.
        /// </returns>
        public VisualState CurrentState { get; internal set; }

        /// <summary>
        /// Gets the name of the <see cref="VisualStateGroup"/>.
        /// </summary>
        /// <returns>
        /// The name of the <see cref="VisualStateGroup"/>.
        /// </returns>
        public string Name
        {
            get => (string)GetValue(FrameworkElement.NameProperty);
            [EditorBrowsable(EditorBrowsableState.Never)]
            set => SetValueInternal(FrameworkElement.NameProperty, value);
        }

        /// <summary>
        /// Gets the collection of mutually exclusive <see cref="VisualState"/> objects.
        /// </summary>
        /// <returns>
        /// The collection of mutually exclusive <see cref="VisualState"/> objects.
        /// </returns>
        public IList States => _states ??= new Collection<VisualState>(new VisualStatesCollection(this));

        /// <summary>
        /// Gets the collection of <see cref="VisualTransition"/> objects.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="VisualTransition"/> objects.
        /// </returns>
        [OpenSilver.NotImplemented]
        public IList Transitions => _transitions ??= new Collection<VisualTransition>(new VisualTransitionsCollection(this));

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
            CurrentStateChanging?.Invoke(stateGroupsRoot, new VisualStateChangedEventArgs(oldState, newState, control));
        }

        internal void RaiseCurrentStateChanged(FrameworkElement stateGroupsRoot, VisualState oldState, VisualState newState, Control control)
        {
            CurrentStateChanged?.Invoke(stateGroupsRoot, new VisualStateChangedEventArgs(oldState, newState, control));
        }

        /// <summary>
        /// Occurs after a control transitions into a different state.
        /// </summary>
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanged;

        /// <summary>
        /// Occurs when a control begins transitioning into a different state.
        /// </summary>
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanging;
    }
}