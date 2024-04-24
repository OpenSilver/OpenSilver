
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

namespace System.Windows;

/// <summary>
/// Represents information about an application that is configured for in-browser support.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class InBrowserSettings : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InBrowserSettings" /> class.
    /// </summary>
    public InBrowserSettings()
    {
        SetValueInternal(SecuritySettingsPropertyKey, new SecuritySettings());
    }

    private static readonly DependencyPropertyKey SecuritySettingsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(SecuritySettings),
            typeof(SecuritySettings),
            typeof(InBrowserSettings),
            null);

    /// <summary>
    /// Identifies the <see cref="SecuritySettings"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SecuritySettingsProperty = SecuritySettingsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the security requirements of an in-browser application.
    /// </summary>
    /// <returns>
    /// The security requirements of an in-browser application.
    /// </returns>
    public SecuritySettings SecuritySettings => (SecuritySettings)GetValue(SecuritySettingsProperty);
}
