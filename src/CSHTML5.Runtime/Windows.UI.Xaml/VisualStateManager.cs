

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Controls;
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
        //private HashSet2<Tuple<string, string>> _propertiesSetAtLeastOnce = new HashSet2<Tuple<string,string>>();

        ///// <summary>
        ///// Initializes a new instance of the VisualStateManager class.
        ///// </summary>
        //public VisualStateManager();

        ///// <summary>
        ///// Identifies the VisualStateManager.CustomVisualStateManager dependency property.
        ///// </summary>
        //public static DependencyProperty CustomVisualStateManagerProperty { get; }

        ///// <summary>
        ///// Gets the value of the VisualStateManager.CustomVisualStateManager attached
        ///// property.
        ///// </summary>
        ///// <param name="obj">The object to get the value from.</param>
        ///// <returns>The VisualStateManager that transitions between the states of a control.</returns>
        //public static VisualStateManager GetCustomVisualStateManager(FrameworkElement obj);


        /// <summary>
        /// Gets the value of the VisualStateManager.VisualStateGroups attached property.
        /// </summary>
        /// <param name="obj">The object to get the value from.</param>
        /// <returns>
        /// The collection of VisualStateGroup objects that is associated with the specified
        /// object.
        /// </returns>
        public static IList GetVisualStateGroups(DependencyObject obj) //Note: we keep this method to allow the compiler to reach the special handling code for this Attached property
        {
            throw new NotSupportedException("Method not supported.");

            //var list = (IList<VisualStateGroup>)obj.GetValue(VisualStateGroupsProperty);

            //// Create the list if it does not exist:
            //if (list == null)
            //{
            //    list = new List<VisualStateGroup>();
            //    obj.SetValue(VisualStateGroupsProperty, list);
            //}
            //return list;
        }

        #region We keep this stuff only for the XAML Code Editor to not complain about missing properties. Otherwise it is not really used, because the compiler processed the code so that it does not use the dependency property.

        /// <summary>
        /// Identifies the VisualStateManager.VisualStateGroup XAML attached property
        /// </summary>
        public static readonly DependencyProperty VisualStateGroupsProperty =
            DependencyProperty.RegisterAttached("VisualStateGroups", typeof(IList), typeof(UIElement), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        //note: there is no set accessible to the user in WinRT
        /// <summary>
        /// Sets the value of the Grid.Column XAML attached property on the specified FrameworkElement.
        /// </summary>
        /// <param name="obj">The target element on which to set the Grid.Row XAML attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetVisualStateGroups(DependencyObject obj, IList value)
        {
            obj.SetValue(VisualStateGroupsProperty, value);
        }
        #endregion

        /// <summary>
        /// Transitions the control between two states.
        /// </summary>
        /// <param name="control">The control to transition between states.</param>
        /// <param name="stateName">The state to transition to.</param>
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
            try
            {
                VisualState state = control.INTERNAL_GetVisualStateGroups().GetVisualState(stateName);
                if (state != null)
                {
                    VisualState formerState = state.INTERNAL_Group.CurrentState;
                    if (formerState != null) //todo: see if this should be possible or if there should be a default State.
                    {
                        formerState.StopStoryBoard(control);
                    }

                    if (state != formerState)
                    {
                        //Note: when a property is set for the first time through a Storyboard, we need to replace the value with a copy so that the other elements with the same Style are not affected (example: PointerOver on one button must not change the Background of another with the same Style)

                        //we apply the new state:
                        Dictionary<Tuple<string, string>, Timeline> newPropertiesChanged = state.ApplyStoryboard(control, useTransitions);//, _propertiesSetAtLeastOnce);

                        ////we add the properties that have been changed to the list of the properties already set:
                        //foreach (Tuple<string, string> propertyChanged in newPropertiesChanged.Keys)
                        //{
                        //    if (!_propertiesSetAtLeastOnce.Contains(propertyChanged))
                        //    {
                        //        _propertiesSetAtLeastOnce.Add(propertyChanged);
                        //    }
                        //}

                        //we remove the former state if any:
                        if (formerState != null)
                        {
                            //if the former VisualState changed properties that are not set in the new VisualState, we remove the value it put on it.
                            var formerPropertiesChanged = formerState.GetPropertiesChangedByStoryboard();
                            if (formerPropertiesChanged != null)
                            {
                                foreach (Tuple<string, string> formerProp in formerPropertiesChanged.Keys)
                                {
                                    if (newPropertiesChanged == null || !newPropertiesChanged.ContainsKey(formerProp))
                                    {
                                        formerPropertiesChanged[formerProp].UnApply(control);

                                    }
                                }
                                formerState.Storyboard.isUnApplied = true;
                            }
                        }

                        //remember the current state:
                        state.INTERNAL_Group.CurrentState = state;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}