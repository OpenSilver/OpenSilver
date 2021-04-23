//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

#if MIGRATION
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.Common
#else
namespace Windows.UI.Xaml.Controls.Common
#endif
{
    /// <summary>
    /// Reservoir of attached properties for use by extension methods that require non-static information about objects.
    /// </summary>
    internal class ExtensionProperties : DependencyObject
    {
        /// <summary>
        /// Tracks whether or not the event handlers of a particular object are currently suspended.
        /// Used by the SetValueNoCallback and AreHandlersSuspended extension methods.
        /// </summary>
        public static readonly DependencyProperty AreHandlersSuspended = DependencyProperty.RegisterAttached(
            "AreHandlersSuspended",
            typeof(Boolean),
            typeof(ExtensionProperties),
            new PropertyMetadata(false)
        );
        public static void SetAreHandlersSuspended(DependencyObject obj, Boolean value)
        {
            obj.SetValue(AreHandlersSuspended, value);
        }
        public static Boolean GetAreHandlersSuspended(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(AreHandlersSuspended);
        }
    }

    /// <summary>
    /// Utility class for operations.
    /// </summary>
    internal static class Extensions
    {
        #region Static Methods

        private static Dictionary<DependencyObject, Dictionary<DependencyProperty, bool>> _suspendedHandlers
            = new Dictionary<DependencyObject, Dictionary<DependencyProperty, bool>>();

        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value, bool suspendAllHandlers = true)
        {
            if (suspendAllHandlers)
                ExtensionProperties.SetAreHandlersSuspended(obj, true);
            else
                obj.SuspendHandler(property, true);

            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                if (suspendAllHandlers)
                    ExtensionProperties.SetAreHandlersSuspended(obj, false);
                else
                    obj.SuspendHandler(property, false);
            }
        }

        public static bool AreHandlersSuspended(this DependencyObject obj)
        {
            return ExtensionProperties.GetAreHandlersSuspended(obj);
        }

#if WORKINPROGRESS
        internal static bool ContainsFocusedElement(this FrameworkElement element)
        {
            if (element != null)
            {
                DependencyObject focusedDependencyObject = FocusManager.GetFocusedElement() as DependencyObject;
                while (focusedDependencyObject != null)
                {
                    if (focusedDependencyObject == element)
                    {
                        return true;
                    }

                    // Walk up the visual tree.  If we hit the root, try using the framework element's
                    // parent.  We do this because Popups behave differently with respect to the visual tree,
                    // and it could have a parent even if the VisualTreeHelper doesn't find it.
                    DependencyObject parent = VisualTreeHelper.GetParent(focusedDependencyObject);
                    if (parent == null)
                    {
                        FrameworkElement focusedElement = focusedDependencyObject as FrameworkElement;
                        if (focusedElement != null)
                        {
                            parent = focusedElement.Parent;
                        }
                    }
                    focusedDependencyObject = parent;
                }
            }
            return false;
        }
#endif

        public static void SetStyleWithType(this FrameworkElement element, Style style)
        {
            if (element.Style != style && (style == null || style.TargetType != null))
            {
                element.Style = style;
            }
        }

        public static bool IsHandlerSuspended(this DependencyObject obj, DependencyProperty property)
        {
            if (_suspendedHandlers.ContainsKey(obj))
                return _suspendedHandlers[obj].ContainsKey(property);

            return false;
        }

        private static void SuspendHandler(this DependencyObject obj, DependencyProperty property, bool suspend)
        {
            if (_suspendedHandlers.ContainsKey(obj))
            {
                var suspensions = _suspendedHandlers[obj];
                if (suspend)
                {
                    suspensions[property] = true;
                }
                else
                {
                    suspensions.Remove(property);
                    if (suspensions.Count == 0)
                        _suspendedHandlers.Remove(obj);
                }
            }
            else
            {
                _suspendedHandlers[obj] = new Dictionary<DependencyProperty, bool>();
                _suspendedHandlers[obj][property] = true;
            }
        }

        #endregion Static Methods
    }
}
