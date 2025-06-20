
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

using OpenSilver.Internal;
using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;
using System.Xaml.Markup;

namespace System.Windows;

/// <summary>
/// Contains mutually exclusive <see cref="VisualState"/> objects and <see cref="VisualTransition"/> 
/// objects that are used to go from one state to another.
/// </summary>
[ContentProperty(nameof(States))]
[RuntimeNameProperty(nameof(Name))]
public sealed class VisualStateGroup : DependencyObject
{
    private WeakReference<DependencyObject> _visualElement;
    private VisualStatesCollection _states;
    private VisualTransitionsCollection _transitions;

    /// <summary>
    /// Gets the most recently set <see cref="VisualState"/> from a successful call to the 
    /// <see cref="VisualStateManager.GoToState(FrameworkElement, string, bool)"/> method.
    /// </summary>
    /// <returns>
    /// The most recently set <see cref="VisualState"/> from a successful call to the
    /// <see cref="VisualStateManager.GoToState(FrameworkElement, string, bool)"/> method.
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
    public IList States => _states ??= new VisualStatesCollection(this);

    internal VisualStatesCollection InternalStates => _states;

    /// <summary>
    /// Gets the collection of <see cref="VisualTransition"/> objects.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="VisualTransition"/> objects.
    /// </returns>
    [OpenSilver.NotImplemented]
    public IList Transitions => _transitions ??= new VisualTransitionsCollection(this);

    internal FrameworkElement VisualElement
    {
        get
        {
            if (_visualElement is not null && _visualElement.TryGetTarget(out DependencyObject visualElement))
            {
                return visualElement as FrameworkElement;
            }

            return null;
        }
    }

    internal void SetVisualElement(WeakReference<DependencyObject> visualElementRef) => _visualElement = visualElementRef;

    internal void UpdateStateTriggers()
    {
        VisualState newState = GetActiveTrigger();

        if (newState == CurrentState)
        {
            return;
        }

        VisualStateManager.GoToElementState(VisualElement, this, newState);
    }

    private VisualState GetActiveTrigger()
    {
        if (_states is null)
        {
            return null;
        }

        // When using StateTriggers to control visual states, the trigger engine uses the following rules to 
        // score triggers and determine which trigger, and the corresponding VisualState, will be active:
        //
        // 1. Custom trigger that derives from StateTriggerBase
        // 2. AdaptiveTrigger activated due to MinWindowWidth
        // 3. AdaptiveTrigger activated due to MinWindowHeight
        //
        // If there are multiple active triggers at a time that have a conflict in scoring (i.e.two custom 
        // triggers), then the first one declared in the markup file takes precedence.

        VisualState visualState = null;
        double minWindowWidth = -1;
        double minWindowHeight = -1;
        bool checkHeight = true;

        foreach (VisualState state in _states)
        {
            if (state.InternalStateTriggers is null)
            {
                continue;
            }

            foreach (StateTriggerBase trigger in state.InternalStateTriggers)
            {
                if (trigger.IsActive)
                {
                    if (trigger is not AdaptiveTrigger adaptiveTrigger)
                    {
                        return state;
                    }

                    if (adaptiveTrigger.MinWindowWidth > minWindowWidth)
                    {
                        checkHeight = false;
                        visualState = state;
                        continue;
                    }

                    if (checkHeight && adaptiveTrigger.MinWindowHeight > minWindowHeight)
                    {
                        visualState = state;
                    }
                }
            }
        }

        return visualState;
    }

    internal VisualState GetState(string stateName)
    {
        if (_states is VisualStatesCollection states)
        {
            for (int stateIndex = 0; stateIndex < states.Count; ++stateIndex)
            {
                VisualState state = states[stateIndex];
                if (state.Name == stateName)
                {
                    return state;
                }
            }
        }

        return null;
    }

    internal void RaiseCurrentStateChanging(FrameworkElement stateGroupsRoot, VisualState oldState, VisualState newState, FrameworkElement control)
    {
        CurrentStateChanging?.Invoke(stateGroupsRoot, new VisualStateChangedEventArgs(oldState, newState, control, stateGroupsRoot));
    }

    internal void RaiseCurrentStateChanged(FrameworkElement stateGroupsRoot, VisualState oldState, VisualState newState, FrameworkElement control)
    {
        CurrentStateChanged?.Invoke(stateGroupsRoot, new VisualStateChangedEventArgs(oldState, newState, control, stateGroupsRoot));
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
