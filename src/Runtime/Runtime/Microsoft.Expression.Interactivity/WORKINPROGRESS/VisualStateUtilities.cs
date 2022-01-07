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

using System.Collections;
using System.Collections.Generic;
using Microsoft.Expression.Interactivity.Core;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.Expression.Interactivity
{
	/// <summary>
	/// This class provides various platform agnostic standard operations for working with VisualStateManager.
	/// </summary>
	public static class VisualStateUtilities
	{
		/// <summary>
		/// Transitions the control between two states.
		/// </summary>
		/// <param name="element">The element to transition between states.</param>
		/// <param name="stateName">The state to transition to.</param>
		/// <param name="useTransitions">True to use a System.Windows.VisualTransition to transition between states; otherwise, false.</param>
		/// <returns>True if the control successfully transitioned to the new state; otherwise, false.</returns>
		/// <exception cref="T:System.ArgumentNullException">Control is null.</exception>
		/// <exception cref="T:System.ArgumentNullException">StateName is null.</exception>
		public static bool GoToState(FrameworkElement element, string stateName, bool useTransitions)
		{
			bool result = false;

			if (!string.IsNullOrEmpty(stateName))
			{
				if (element is Control control)
				{
					control.ApplyTemplate();
					result = VisualStateManager.GoToState(control, stateName, useTransitions);
				}
				else
				{
					result = ExtendedVisualStateManager.GoToElementState(element, stateName, useTransitions);
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the value of the VisualStateManager.VisualStateGroups attached property.
		/// </summary>
		/// <param name="targetObject">The element from which to get the VisualStateManager.VisualStateGroups.</param>
		/// <returns></returns>
		public static IList GetVisualStateGroups(FrameworkElement targetObject)
		{
			IList list = new List<VisualStateGroup>();

			if (targetObject != null)
			{
				list = VisualStateManager.GetVisualStateGroups(targetObject);

				if (list.Count == 0)
				{
					int childrenCount = VisualTreeHelper.GetChildrenCount(targetObject);

					if (childrenCount > 0)
					{
						FrameworkElement obj = VisualTreeHelper.GetChild(targetObject, 0) as FrameworkElement;
						list = VisualStateManager.GetVisualStateGroups(obj);
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Find the nearest parent which contains visual states.
		/// </summary>
		/// <param name="contextElement">The element from which to find the nearest stateful control.</param>
		/// <param name="resolvedControl">The nearest stateful control if True; else null.</param>
		/// <returns>True if a parent contains visual states; else False.</returns>
		public static bool TryFindNearestStatefulControl(FrameworkElement contextElement, out FrameworkElement resolvedControl)
		{
			FrameworkElement frameworkElement = contextElement;

			if (frameworkElement == null)
			{
				resolvedControl = null;
				return false;
			}

			FrameworkElement parentFrameworkElement = frameworkElement.Parent as FrameworkElement;
			
			bool result = true;
			
			while (!HasVisualStateGroupsDefined(frameworkElement) && ShouldContinueTreeWalk(parentFrameworkElement))
			{
				frameworkElement = parentFrameworkElement;
				parentFrameworkElement = parentFrameworkElement.Parent as FrameworkElement;
			}

			if (HasVisualStateGroupsDefined(frameworkElement))
			{
				if (VisualTreeHelper.GetParent(frameworkElement) is FrameworkElement parentFrameworkElementAsControl && parentFrameworkElementAsControl is Control)
				{
					frameworkElement = parentFrameworkElementAsControl;
				}
			}
			else
			{
				result = false;
			}

			resolvedControl = frameworkElement;
			
			return result;
		}

		private static bool HasVisualStateGroupsDefined(FrameworkElement frameworkElement)
		{
			if (frameworkElement != null)
			{
				return VisualStateManager.GetVisualStateGroups(frameworkElement).Count != 0;
			}

			return false;
		}

		internal static FrameworkElement FindNearestStatefulControl(FrameworkElement contextElement)
		{
			FrameworkElement resolvedControl = null;

			TryFindNearestStatefulControl(contextElement, out resolvedControl);
			
			return resolvedControl;
		}

		private static bool ShouldContinueTreeWalk(FrameworkElement element)
		{
			if (element == null)
			{
				return false;
			}

			if (element is UserControl)
			{
				return false;
			}

			if (element.Parent == null)
			{
				FrameworkElement frameworkElement = FindTemplatedParent(element);

				if (frameworkElement == null || (!(frameworkElement is Control) && !(frameworkElement is ContentPresenter)))
				{
					return false;
				}
			}

			return true;
		}

		private static FrameworkElement FindTemplatedParent(FrameworkElement parent)
		{
			return VisualTreeHelper.GetParent(parent) as FrameworkElement;
		}
	}
}
