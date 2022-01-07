// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See THIRD-PARTY-NOTICES file in the project root for full license information.

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
using System.Globalization;
using System.Windows.Interactivity;

#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.Expression.Interactivity.Core
{
	/// <summary>
	/// Toggles between two states based on a conditional statement.
	/// </summary>
	public class DataStateBehavior : Behavior<FrameworkElement>
	{
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(object), typeof(DataStateBehavior), new PropertyMetadata(OnBindingChanged));

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataStateBehavior), new PropertyMetadata(OnValueChanged));

		public static readonly DependencyProperty TrueStateProperty = DependencyProperty.Register("TrueState", typeof(string), typeof(DataStateBehavior), new PropertyMetadata(OnTrueStateChanged));

		public static readonly DependencyProperty FalseStateProperty = DependencyProperty.Register("FalseState", typeof(string), typeof(DataStateBehavior), new PropertyMetadata(OnFalseStateChanged));

		/// <summary>
		/// Gets or sets the binding that produces the property value of the data object. This is a dependency property.
		/// </summary>
		public object Binding
		{
			get
			{
				return GetValue(BindingProperty);
			}
			set
			{
				SetValue(BindingProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the value to be compared with the property value of the data object. This is a dependency property.
		/// </summary>
		public object Value
		{
			get
			{
				return GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the name of the visual state to transition to when the condition is met. This is a dependency property.
		/// </summary>
		public string TrueState
		{
			get
			{
				return (string)GetValue(TrueStateProperty);
			}
			set
			{
				SetValue(TrueStateProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the name of the visual state to transition to when the condition is not met. This is a dependency property.
		/// </summary>
		public string FalseState
		{
			get
			{
				return (string)GetValue(FalseStateProperty);
			}
			set
			{
				SetValue(FalseStateProperty, value);
			}
		}

		private FrameworkElement TargetObject => VisualStateUtilities.FindNearestStatefulControl(base.AssociatedObject);

		private IEnumerable<VisualState> TargetedVisualStates
		{
			get
			{
				List<VisualState> list = new List<VisualState>();

				if (TargetObject != null)
				{
					IList visualStateGroups = VisualStateUtilities.GetVisualStateGroups(TargetObject);
					{
						foreach (VisualStateGroup item2 in visualStateGroups)
						{
							foreach (VisualState state in item2.States)
							{
								list.Add(state);
							}
						}

						return list;
					}
				}

				return list;
			}
		}

		/// <summary>
		/// Called after the behavior is attached to an AssociatedObject.
		/// </summary>
		/// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
		protected override void OnAttached()
		{
			base.OnAttached();
			ValidateStateNamesDeferred();
		}

		private void ValidateStateNamesDeferred()
		{
			if (base.AssociatedObject.Parent is FrameworkElement element && IsElementLoaded(element))
			{
				ValidateStateNames();
				return;
			}

			base.AssociatedObject.Loaded += delegate
			{
				ValidateStateNames();
			};
		}

		/// <summary>
		/// A helper function to take the place of FrameworkElement.IsLoaded, as this property isn't available in Silverlight.
		/// </summary>
		/// <param name="element">The element of interest.</param>
		/// <returns>Returns true if the element has been loaded; otherwise, returns false.</returns>
		internal static bool IsElementLoaded(FrameworkElement element)
		{
			UIElement rootVisual = Application.Current.RootVisual;
			DependencyObject parent = element.Parent;
			
			if (parent == null)
			{
				parent = VisualTreeHelper.GetParent(element);
			}

			if (parent == null)
			{
				if (rootVisual != null)
				{
					return element == rootVisual;
				}

				return false;
			}

			return true;
		}

		private void ValidateStateNames()
		{
			ValidateStateName(TrueState);
			ValidateStateName(FalseState);
		}

		private void ValidateStateName(string stateName)
		{
			if (base.AssociatedObject == null || string.IsNullOrEmpty(stateName))
			{
				return;
			}

			foreach (VisualState targetedVisualState in TargetedVisualStates)
			{
				if (stateName == targetedVisualState.Name)
				{
					return;
				}
			}

			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot find state named '{0}' on type '{1}'. Ensure that the state exists and that it can be accessed from this context.", stateName, (TargetObject != null) ? TargetObject.GetType().Name : "null"));
		}

		private static void OnBindingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DataStateBehavior dataStateBehavior = (DataStateBehavior)obj;
			dataStateBehavior.Evaluate();
		}

		private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DataStateBehavior dataStateBehavior = (DataStateBehavior)obj;
			dataStateBehavior.Evaluate();
		}

		private static void OnTrueStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DataStateBehavior dataStateBehavior = (DataStateBehavior)obj;
			dataStateBehavior.ValidateStateName(dataStateBehavior.TrueState);
			dataStateBehavior.Evaluate();
		}

		private static void OnFalseStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DataStateBehavior dataStateBehavior = (DataStateBehavior)obj;
			dataStateBehavior.ValidateStateName(dataStateBehavior.FalseState);
			dataStateBehavior.Evaluate();
		}

		private void Evaluate()
		{
			if (TargetObject != null)
			{
				VisualStateUtilities.GoToState(stateName: (!ComparisonLogic.EvaluateImpl(Binding, ComparisonConditionType.Equal, Value)) ? FalseState : TrueState, element: TargetObject, useTransitions: true);
			}
		}
	}
}
