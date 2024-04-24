
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
/// Represents the security configuration of an out-of-browser application.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class SecuritySettings : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecuritySettings"/> class.
    /// </summary>
    public SecuritySettings() { }

    private static readonly DependencyPropertyKey ElevatedPermissionsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ElevatedPermissions),
            typeof(ElevatedPermissions),
            typeof(SecuritySettings),
            new PropertyMetadata(ElevatedPermissions.NotRequired));

    /// <summary>
    /// Identifies the <see cref="ElevatedPermissions"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ElevatedPermissionsProperty = ElevatedPermissionsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether the out-of-browser application requires elevated permissions.
    /// </summary>
    /// <returns>
    /// A value that indicates whether the out-of-browser application requires elevated permissions.
    /// </returns>
    public ElevatedPermissions ElevatedPermissions => (ElevatedPermissions)GetValue(ElevatedPermissionsProperty);
}
