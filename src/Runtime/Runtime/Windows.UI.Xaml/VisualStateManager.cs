

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
using System.Collections.ObjectModel;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Manages states and the logic for transitioning between states for controls.
    /// </summary>
    public partial class VisualStateManager : DependencyObject
    {
        /// <summary>
        ///     Transitions a control's state.
        /// </summary>
        /// <param name="control">The control who's state is changing.</param>
        /// <param name="stateGroupsRoot">The element to get the VSG & customer VSM from.</param>
        /// <param name="stateName">The new state that the control is in.</param>
        /// <param name="useTransitions">Whether to use transition animations.</param>
        /// <returns>true if the state changed successfully, false otherwise.</returns>
        private static bool GoToStateCommon(Control control, FrameworkElement stateGroupsRoot, string stateName, bool useTransitions)
        {
            if (stateName == null)
            {
                throw new ArgumentNullException("stateName");
            }

            if (stateGroupsRoot == null)
            {
                return false; // Ignore state changes if a stateGroupsRoot doesn't exist yet
            }

            IList<VisualStateGroup> groups = GetVisualStateGroupsInternal(stateGroupsRoot);
            
            VisualState state = null;
            VisualStateGroup group = null;
            if (groups != null)
            {
                VisualStateManager.TryGetState(groups, stateName, out group, out state);
            }

            // Look for a custom VSM, and call it if it was found, regardless of whether the state was found or not.
            // This is because we don't know what the custom VSM will want to do. But for our default implementation,
            // we know that if we haven't found the state, we don't actually want to do anything.
            VisualStateManager customVsm = GetCustomVisualStateManager(stateGroupsRoot);
            if (customVsm != null)
            {
                return customVsm.GoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions);
            }
            else if (state != null)
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
        /// True to use a VisualTransition to transition between states; otherwise,
        /// false.
        /// </param>
        /// <returns>
        /// True if the control successfully transitioned to the new state; otherwise,
        /// false.
        /// </returns>
        public static bool GoToState(Control control, string stateName, bool useTransitions)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            FrameworkElement stateGroupsRoot = control.StateGroupsRoot;

            return GoToStateCommon(control, stateGroupsRoot, stateName, useTransitions);
        }

        /// <summary>
        ///     Allows subclasses to override the GoToState logic.
        /// </summary>
        protected virtual bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            return GoToStateInternal(control, templateRoot, group, state, useTransitions);
        }

        #region CustomVisualStateManager

        public static readonly DependencyProperty CustomVisualStateManagerProperty =
             DependencyProperty.RegisterAttached(
                 "CustomVisualStateManager",
                 typeof(VisualStateManager),
                 typeof(VisualStateManager),
                 null);

        public static VisualStateManager GetCustomVisualStateManager(FrameworkElement obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.GetValue(VisualStateManager.CustomVisualStateManagerProperty) as VisualStateManager;
        }

        public static void SetCustomVisualStateManager(FrameworkElement obj, VisualStateManager value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(VisualStateManager.CustomVisualStateManagerProperty, value);
        }

        #endregion CustomVisualStateManager

        #region VisualStateGroups

        /// <summary>
        /// Identifies the VisualStateManager.VisualStateGroup attached property
        /// </summary>
        /// <remarks>
        /// This field is not supposed to be public, but needs to be for now because
        /// of a limitation due to our XAML compiler. Using it to call 
        /// <see cref="DependencyObject.SetValue"/>, <see cref="BindingOperations.SetBinding"/>
        /// or any other method that manipulate dependency properties can lead to some 
        /// unexpected behavior.
        /// </remarks>
        public static readonly DependencyProperty VisualStateGroupsProperty =
            DependencyProperty.RegisterAttached(
                "VisualStateGroups",
                typeof(IList),
                typeof(VisualStateManager),
                new PropertyMetadata((object)null));

        internal static Collection<VisualStateGroup> GetVisualStateGroupsInternal(FrameworkElement obj)
        {
            return (Collection<VisualStateGroup>)obj.GetValue(VisualStateGroupsProperty);                
        }

        /// <summary>
        /// Gets the VisualStateManager.VisualStateGroups attached property.
        /// </summary>
        /// <param name="obj">
        /// The element to get the VisualStateManager.VisualStateGroups attached
        /// property from.
        /// </param>
        /// <returns>
        /// The collection of <see cref="VisualStateGroup"/> objects that is associated
        /// with the specified object.
        /// </returns>
        public static IList GetVisualStateGroups(FrameworkElement obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            IList value = (IList)obj.GetValue(VisualStateGroupsProperty);
            if (value == null)
            {
                // Note: In Silverlight, VisualStateGroups is not an ObservableCollection               
                value = new ObservableCollection<VisualStateGroup>();
                obj.SetValue(VisualStateGroupsProperty, value);
            }
            return value;
        }

        #endregion VisualStateGroups

        #region State Change

        internal static bool TryGetState(IList<VisualStateGroup> groups, string stateName, out VisualStateGroup group, out VisualState state)
        {
            for (int groupIndex = 0; groupIndex < groups.Count; ++groupIndex)
            {
                VisualStateGroup g = groups[groupIndex];
                VisualState s = g.GetState(stateName);
                if (s != null)
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

        private static bool GoToStateInternal(Control control, FrameworkElement stateGroupsRoot, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            if (stateGroupsRoot == null)
            {
                throw new ArgumentNullException("stateGroupsRoot");
            }

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            if (group == null)
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

                // todo: see if this should be possible or if there should be a default State.
                if (lastState != null)
                {
                    lastState.StopStoryBoard(control);
                }

                // Note: when a property is set for the first time through a Storyboard, we need
                // to replace the value with a copy so that the other elements with the same Style 
                // are not affected (example: PointerOver on one button must not change the 
                // Background of another with the same Style)

                // we apply the new state
                Dictionary<Tuple<string, string>, Timeline> newPropertiesChanged = state.ApplyStoryboard(control, useTransitions);

                // we remove the former state if any
                if (lastState != null)
                {
                    // if the former VisualState changed properties that are not set in the new 
                    // VisualState, we remove the value it put on it.
                    var formerPropertiesChanged = lastState.GetPropertiesChangedByStoryboard();
                    if (formerPropertiesChanged != null)
                    {
                        foreach (Tuple<string, string> formerProp in formerPropertiesChanged.Keys)
                        {
                            if (newPropertiesChanged == null || !newPropertiesChanged.ContainsKey(formerProp))
                            {
                                formerPropertiesChanged[formerProp].UnApply(control);
                            }
                        }
                        lastState.Storyboard.isUnApplied = true;
                    }
                }

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

        protected void RaiseCurrentStateChanging(VisualStateGroup stateGroup, VisualState oldState, VisualState newState, Control control)
        {
            if (stateGroup == null)
            {
                throw new ArgumentNullException("stateGroup");
            }

            if (newState == null)
            {
                throw new ArgumentNullException("newState");
            }

            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            FrameworkElement stateGroupsRoot = control.StateGroupsRoot;
            if (stateGroupsRoot == null)
            {
                return; // Ignore if a ControlTemplate hasn't been applied
            }

            stateGroup.RaiseCurrentStateChanging(stateGroupsRoot, oldState, newState, control);
        }

        protected void RaiseCurrentStateChanged(VisualStateGroup stateGroup, VisualState oldState, VisualState newState, Control control)
        {
            if (stateGroup == null)
            {
                throw new ArgumentNullException("stateGroup");
            }

            if (newState == null)
            {
                throw new ArgumentNullException("newState");
            }

            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            FrameworkElement stateGroupsRoot = control.StateGroupsRoot;
            if (stateGroupsRoot == null)
            {
                return; // Ignore if a ControlTemplate hasn't been applied
            }

            stateGroup.RaiseCurrentStateChanged(stateGroupsRoot, oldState, newState, control);
        }

        #endregion State Change
    }
}