//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls.Common
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

        public static void SetStyleWithType(this FrameworkElement element, Style style)
        {
            if (element.Style != style && (style == null || style.TargetType != null))
            {
                element.Style = style;
            }
        }

        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value)
        {
            ExtensionProperties.SetAreHandlersSuspended(obj, true);
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                ExtensionProperties.SetAreHandlersSuspended(obj, false);
            }
        }
                
        public static bool AreHandlersSuspended(this DependencyObject obj)
        {
            return ExtensionProperties.GetAreHandlersSuspended(obj);
        }

#endregion Static Methods
    }
}
