
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
using System;
using System.Windows;
using System.Windows.Threading;

namespace OpenSilver;

/// <summary>
/// Contains properties that specify how an application should behave relative to features that are in the <i>OpenSilver</i>
/// assembly.
/// </summary>
public static class OpenSilverCompatibilityPreferences
{
    private static readonly object _lock = new();
    private static bool _isSealed;

    private static bool _handleDispatcherExceptions = true;
    private static bool _handleDispatcherTimerExceptions = true;
    private static bool _handleJavaScriptCallbackExceptions = true;

    /// <summary>
    /// Gets or sets a value that indicates whether the errors fired by <see cref="Dispatcher"/> should be handled by
    /// <see cref="Application.UnhandledException"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Cannot set a <see cref="OpenSilverCompatibilityPreferences"/> property after the <see cref="Application"/> startup.
    /// </exception>
    public static bool HandleDispatcherExceptions
    {
        get => _handleDispatcherExceptions;
        set
        {
            lock (_lock)
            {
                if (_isSealed)
                {
                    throw new InvalidOperationException(
                        string.Format(Strings.CompatibilityPreferencesSealed, nameof(HandleDispatcherExceptions), nameof(OpenSilverCompatibilityPreferences)));
                }

                _handleDispatcherExceptions = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the errors fired by <see cref="DispatcherTimer.Tick"/> should be handled
    /// by <see cref="Application.UnhandledException"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Cannot set a <see cref="OpenSilverCompatibilityPreferences"/> property after the <see cref="Application"/> startup.
    /// </exception>
    public static bool HandleDispatcherTimerExceptions
    {
        get => _handleDispatcherTimerExceptions;
        set
        {
            lock (_lock)
            {
                if (_isSealed)
                {
                    throw new InvalidOperationException(
                        string.Format(Strings.CompatibilityPreferencesSealed, nameof(HandleDispatcherTimerExceptions), nameof(OpenSilverCompatibilityPreferences)));
                }

                _handleDispatcherTimerExceptions = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the errors fired during JavaScript callbacks should be handled by 
    /// <see cref="Application.UnhandledException"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Cannot set a <see cref="OpenSilverCompatibilityPreferences"/> property after the <see cref="Application"/> startup.
    /// </exception>
    public static bool HandleJavaScriptCallbackExceptions
    {
        get => _handleJavaScriptCallbackExceptions;
        set
        {
            lock (_lock)
            {
                if (_isSealed)
                {
                    throw new InvalidOperationException(
                        string.Format(Strings.CompatibilityPreferencesSealed, nameof(HandleJavaScriptCallbackExceptions), nameof(OpenSilverCompatibilityPreferences)));
                }

                _handleJavaScriptCallbackExceptions = value;
            }
        }
    }

    internal static void Seal()
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
