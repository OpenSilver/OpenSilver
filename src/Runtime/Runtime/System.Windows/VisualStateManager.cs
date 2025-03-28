
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
using System.Diagnostics;
using System.Windows.Controls;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Manages states and the logic for transitioning between states for controls.
/// </summary>
public class VisualStateManager : DependencyObject
{
    /// <summary>
    /// Transitions a control's state.
    /// </summary>
    /// <param name="control">The control who's state is changing.</param>
    /// <param name="stateGroupsRoot">The element to get the VSG &amp; customer VSM from.</param>
    /// <param name="stateName">The new state that the control is in.</param>
    /// <param name="useTransitions">Whether to use transition animations.</param>
    /// <returns>true if the state changed successfully, false otherwise.</returns>
    private static bool GoToStateCommon(Control control, FrameworkElement stateGroupsRoot, string stateName, bool useTransitions)
    {
        if (stateName is null)
        {
            throw new ArgumentNullException(nameof(stateName));
        }

        if (stateGroupsRoot is null)
        {
            return false; // Ignore state changes if a stateGroupsRoot doesn't exist yet
        }

        VisualState state = null;
        VisualStateGroup group = null;
        if (GetVisualStateGroupsInternal(stateGroupsRoot) is Collection<VisualStateGroup> groups)
        {
            TryGetState(groups, stateName, out group, out state);
        }

        // Look for a custom VSM, and call it if it was found, regardless of whether the state was found or not.
        // This is because we don't know what the custom VSM will want to do. But for our default implementation,
        // we know that if we haven't found the state, we don't actually want to do anything.
        if (GetCustomVisualStateManager(stateGroupsRoot) is VisualStateManager customVsm)
        {
            return customVsm.GoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions);
        }
        else if (state is not null)
        {
            return GoToStateInternal(control, stateGroupsRoot, group, state, useTransitions);
        }

        return false;
    }

    /// <summary>
    /// Transitions the control between two states.
    /// </summary>
    /// <param name="control">
    /// The control to transition between states.
    /// </param>
    /// <param name="stateName">
    /// The state to transition to.
    /// </param>
    /// <param name="useTransitions">
    /// True to use a <see cref="VisualTransition"/> to transition between states;
    /// otherwise, false.
    /// </param>
    /// <returns>
    /// true if the control successfully transitioned to the new state; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// control is null.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// stateName is null.
    /// </exception>
    public static bool GoToState(Control control, string stateName, bool useTransitions)
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        FrameworkElement stateGroupsRoot = control.StateGroupsRoot;

        return GoToStateCommon(control, stateGroupsRoot, stateName, useTransitions);
    }

    /// <summary>
    /// Transitions a control's state.
    /// </summary>
    /// <param name="stateGroupsRoot">The root element that contains the VisualStateManager.</param>
    /// <param name="stateName">The new state that the control is in.</param>
    /// <param name="useTransitions">Whether to use transition animations.</param>
    /// <returns>true if the state changed successfully, false otherwise.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static bool GoToElementState(FrameworkElement stateGroupsRoot, string stateName, bool useTransitions)
    {
        if (stateGroupsRoot is null)
        {
            throw new ArgumentNullException(nameof(stateGroupsRoot));
        }

        return GoToStateCommon(null, stateGroupsRoot, stateName, useTransitions);
    }

    /// <summary>
    /// Transitions a control between states.
    /// </summary>
    /// <param name="control">
    /// The control to transition between states.
    /// </param>
    /// <param name="templateRoot">
    /// The root element of the control's <see cref="ControlTemplate"/>.
    /// </param>
    /// <param name="stateName">
    /// The name of the state to transition to.
    /// </param>
    /// <param name="group">
    /// The <see cref="VisualStateGroup"/> that the state belongs to.
    /// </param>
    /// <param name="state">
    /// The representation of the state to transition to.
    /// </param>
    /// <param name="useTransitions">
    /// true to use a <see cref="VisualTransition"/> to transition between states; otherwise, false.
    /// </param>
    /// <returns>
    /// true if the control successfully transitioned to the new state; otherwise, false.
    /// </returns>
    protected virtual bool GoToStateCore(Control control,
        FrameworkElement templateRoot,
        string stateName,
        VisualStateGroup group,
        VisualState state,
        bool useTransitions)
    {
        return GoToStateInternal(control, templateRoot, group, state, useTransitions);
    }

    #region CustomVisualStateManager

    /// <summary>
    /// Identifies the VisualStateManager.CustomVisualStateManager dependency property.
    /// </summary>
    public static readonly DependencyProperty CustomVisualStateManagerProperty =
         DependencyProperty.RegisterAttached(
             "CustomVisualStateManager",
             typeof(VisualStateManager),
             typeof(VisualStateManager),
             null);

    /// <summary>
    /// Gets the value of the VisualStateManager.CustomVisualStateManager attached property.
    /// </summary>
    /// <param name="obj">
    /// The element from which to get the VisualStateManager.CustomVisualStateManager.
    /// </param>
    /// <returns>
    /// The <see cref="VisualStateManager"/> that transitions between the states of a control.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// obj is null.
    /// </exception>
    public static VisualStateManager GetCustomVisualStateManager(FrameworkElement obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (VisualStateManager)obj.GetValue(CustomVisualStateManagerProperty);
    }

    /// <summary>
    /// Sets the value of the VisualStateManager.CustomVisualStateManager attached property.
    /// </summary>
    /// <param name="obj">
    /// The object on which to set the property.
    /// </param>
    /// <param name="value">
    /// The <see cref="VisualStateManager"/> that transitions between the states of a control.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// obj is null.
    /// </exception>
    public static void SetCustomVisualStateManager(FrameworkElement obj, VisualStateManager value)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValueInternal(CustomVisualStateManagerProperty, value);
    }

    #endregion CustomVisualStateManager

    #region VisualStateGroups

    private static readonly DependencyPropertyKey VisualStateGroupsPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
        "VisualStateGroups",
        typeof(IList),
        typeof(VisualStateManager),
        new PropertyMetadata((object)null));

    /// <summary>
    /// Identifies the VisualStateManager.VisualStateGroup attached property.
    /// </summary>
    public static readonly DependencyProperty VisualStateGroupsProperty = VisualStateGroupsPropertyKey.DependencyProperty;

    internal static Collection<VisualStateGroup> GetVisualStateGroupsInternal(FrameworkElement obj)
    {
        Debug.Assert(obj is not null);
        return (Collection<VisualStateGroup>)obj.GetValue(VisualStateGroupsProperty);
    }

    /// <summary>
    /// Gets the value of the VisualStateManager.VisualStateGroups attached property.
    /// </summary>
    /// <param name="obj">
    /// The element from which to get the VisualStateManager.VisualStateGroups.
    /// </param>
    /// <returns>
    /// The collection of <see cref="VisualStateGroup"/> objects that is associated
    /// with the specified object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// obj is null.
    /// </exception>
    public static IList GetVisualStateGroups(FrameworkElement obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (obj.GetValue(VisualStateGroupsProperty) is not Collection<VisualStateGroup> value)
        {
            value = new(new VisualStateGroupCollection(obj));
            obj.SetValueInternal(VisualStateGroupsPropertyKey, value);
        }

        return value;
    }

    #endregion VisualStateGroups

    #region State Change

    internal static bool TryGetState(Collection<VisualStateGroup> groups, string stateName, out VisualStateGroup group, out VisualState state)
    {
        for (int groupIndex = 0; groupIndex < groups.Count; ++groupIndex)
        {
            VisualStateGroup g = groups[groupIndex];
            if (g.GetState(stateName) is VisualState s)
            {
                group = g;
                state = s;
                return true;
            }
        }

        group = null;
        state = null;
        return false;
    }

    private static bool GoToStateInternal(Control control,
        FrameworkElement stateGroupsRoot,
        VisualStateGroup group,
        VisualState state,
        bool useTransitions)
    {
        if (stateGroupsRoot is null)
        {
            throw new ArgumentNullException(nameof(stateGroupsRoot));
        }

        if (state is null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (group is null)
        {
            throw new InvalidOperationException();
        }

        try
        {
            VisualState lastState = group.CurrentState;
            if (lastState == state)
            {
                return true;
            }

            state.Storyboard?.BeginVSM(stateGroupsRoot);
            lastState?.Storyboard?.RemoveVSM(stateGroupsRoot);

            // remember the current state
            group.CurrentState = state;

            // Fire both CurrentStateChanging and CurrentStateChanged events
            group.RaiseCurrentStateChanging(stateGroupsRoot, lastState, state, control);
            group.RaiseCurrentStateChanged(stateGroupsRoot, lastState, state, control);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Raises the <see cref="VisualStateGroup.CurrentStateChanging"/> event on the specified 
    /// <see cref="VisualStateGroup"/>.
    /// </summary>
    /// <param name="stateGroup">
    /// The object on which the <see cref="VisualStateGroup.CurrentStateChanging"/> event.
    /// </param>
    /// <param name="oldState">
    /// The state that the control is transitioning from.
    /// </param>
    /// <param name="newState">
    /// The state that the control is transitioning to.
    /// </param>
    /// <param name="control">
    /// The control that is transitioning states.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// stateGroup is null or newState is null or control is null.
    /// </exception>
    protected void RaiseCurrentStateChanging(VisualStateGroup stateGroup, VisualState oldState, VisualState newState, Control control)
    {
        if (stateGroup is null)
        {
            throw new ArgumentNullException(nameof(stateGroup));
        }

        if (newState is null)
        {
            throw new ArgumentNullException(nameof(newState));
        }

        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        FrameworkElement stateGroupsRoot = control.StateGroupsRoot;
        if (stateGroupsRoot is null)
        {
            return; // Ignore if a ControlTemplate hasn't been applied
        }

        stateGroup.RaiseCurrentStateChanging(stateGroupsRoot, oldState, newState, control);
    }

    /// <summary>
    /// Raises the <see cref="VisualStateGroup.CurrentStateChanged"/> event on the specified
    /// <see cref="VisualStateGroup"/>.
    /// </summary>
    /// <param name="stateGroup">
    /// The object on which the <see cref="VisualStateGroup.CurrentStateChanging"/> event.
    /// </param>
    /// <param name="oldState">
    /// The state that the control transitioned from.
    /// </param>
    /// <param name="newState">
    /// The state that the control transitioned to.
    /// </param>
    /// <param name="control">
    /// The control that transitioned states.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// stateGroup is null or newState is null or control is null.
    /// </exception>
    protected void RaiseCurrentStateChanged(VisualStateGroup stateGroup, VisualState oldState, VisualState newState, Control control)
    {
        if (stateGroup is null)
        {
            throw new ArgumentNullException(nameof(stateGroup));
        }

        if (newState is null)
        {
            throw new ArgumentNullException(nameof(newState));
        }

        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        FrameworkElement stateGroupsRoot = control.StateGroupsRoot;
        if (stateGroupsRoot is null)
        {
            return; // Ignore if a ControlTemplate hasn't been applied
        }

        stateGroup.RaiseCurrentStateChanged(stateGroupsRoot, oldState, newState, control);
    }

    #endregion State Change
}