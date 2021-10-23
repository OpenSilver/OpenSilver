

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


using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

#if MIGRATION
namespace System.Windows.Controls.Common
#else
namespace Windows.UI.Xaml.Controls.Common
#endif
{
    /// <summary>
    /// Reservoir of attached properties for use by extension methods that require non-static information about objects.
    /// </summary>
    internal class DependencyObjectExtensionProperties : DependencyObject
    {
        /// <summary>
        /// Tracks whether or not the event handlers of a particular object are currently suspended.
        /// Used by the SetValueNoCallback and AreHandlersSuspended extension methods.
        /// </summary>
        public static readonly DependencyProperty AreHandlersSuspended = DependencyProperty.RegisterAttached(
            "AreHandlersSuspended",
            typeof(Boolean),
            typeof(DependencyObjectExtensionProperties),
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
    /// Utility class for DependencyObject/DependencyProperty related operations
    /// </summary>
    internal static class DependencyObjectExtensions
    {
        #region Static Methods

        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value)
        {
            DependencyObjectExtensionProperties.SetAreHandlersSuspended(obj, true);
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                DependencyObjectExtensionProperties.SetAreHandlersSuspended(obj, false);
            }
        }

        public static bool AreHandlersSuspended(this DependencyObject obj)
        {
            return DependencyObjectExtensionProperties.GetAreHandlersSuspended(obj);
        }

        #endregion Static Methods
    }
}
