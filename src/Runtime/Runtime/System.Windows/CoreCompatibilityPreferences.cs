
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

namespace System.Windows;

/// <summary>
/// Contains properties that specify how an application should behave relative to WPF features 
/// that are in the PresentationCore assembly.
/// </summary>
public static class CoreCompatibilityPreferences
{
    private static readonly object _lock = new();
    private static bool _isSealed;

    private static bool _isAltKeyRequiredInAccessKeyDefaultScope = false;
    private static bool? _enableMultiMonitorDisplayClipping = null;

    /// <summary>
    /// Gets or sets a value that indicates whether the user needs to use the ALT key to invoke a shortcut.
    /// </summary>
    /// <returns>
    /// true if the user needs to use the ALT key to invoke a shortcut; otherwise, false. The default is false.
    /// </returns>
    public static bool IsAltKeyRequiredInAccessKeyDefaultScope
    {
        get => _isAltKeyRequiredInAccessKeyDefaultScope;
        set
        {
            lock (_lock)
            {
                if (_isSealed)
                {
                    throw new InvalidOperationException(
                        string.Format(Strings.CompatibilityPreferencesSealed, nameof(IsAltKeyRequiredInAccessKeyDefaultScope), nameof(CoreCompatibilityPreferences)));
                }

                _isAltKeyRequiredInAccessKeyDefaultScope = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether to enable clipping in a multi-monitor display.
    /// </summary>
    /// <returns>
    /// true to enable clipping in a multi-monitor display; otherwise, false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static bool? EnableMultiMonitorDisplayClipping
    {
        get => GetEnableMultiMonitorDisplayClipping();
        set
        {
            lock (_lock)
            {
                if (_isSealed)
                {
                    throw new InvalidOperationException(
                        string.Format(Strings.CompatibilityPreferencesSealed, nameof(EnableMultiMonitorDisplayClipping), nameof(CoreCompatibilityPreferences)));
                }

                _enableMultiMonitorDisplayClipping = value;
            }
        }
    }

    internal static bool GetIsAltKeyRequiredInAccessKeyDefaultScope()
    {
        Seal();
        return IsAltKeyRequiredInAccessKeyDefaultScope;
    }

    internal static bool? GetEnableMultiMonitorDisplayClipping()
    {
        Seal();
        return _enableMultiMonitorDisplayClipping;
    }

    private static void Seal()
    {
        if (!_isSealed)
        {
            lock (_lock)
            {
                _isSealed = true;
            }
        }
    }
}
