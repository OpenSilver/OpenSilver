
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
/// Represents information about an application that is configured for out-of-browser support.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class OutOfBrowserSettings : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfBrowserSettings"/> class.
    /// </summary>
    public OutOfBrowserSettings()
    {
        SetValueInternal(IconsPropertyKey, new IconCollection());
        SetValueInternal(SecuritySettingsPropertyKey, new SecuritySettings());
        SetValueInternal(WindowSettingsPropertyKey, new WindowSettings());
    }

    private static readonly DependencyPropertyKey BlurbPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Blurb),
            typeof(string),
            typeof(OutOfBrowserSettings),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="Blurb"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BlurbProperty = BlurbPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a short description of the application.
    /// </summary>
    /// <returns>
    /// A short description of the application.
    /// </returns>
    public string Blurb => (string)GetValue(BlurbProperty);

    private static readonly DependencyPropertyKey EnableGPUAccelerationPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(EnableGPUAcceleration),
            typeof(bool),
            typeof(OutOfBrowserSettings),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="EnableGPUAcceleration"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EnableGPUAccelerationProperty = EnableGPUAccelerationPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether to use graphics processor unit hardware acceleration
    /// for cached compositions, which potentially results in graphics optimization.
    /// </summary>
    /// <returns>
    /// true if GPU acceleration is enabled; otherwise, false. The default is false.
    /// </returns>
    public bool EnableGPUAcceleration => (bool)GetValue(EnableGPUAccelerationProperty);

    private static readonly DependencyPropertyKey IconsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Icons),
            typeof(IconCollection),
            typeof(OutOfBrowserSettings),
            null);

    /// <summary>
    /// Identifies the <see cref="Icons"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconsProperty = IconsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a collection of <see cref="Icon"/> instances associated with the application.
    /// </summary>
    /// <returns>
    /// The icons associated with the application.
    /// </returns>
    public IconCollection Icons => (IconCollection)GetValue(IconsProperty);

    private static readonly DependencyPropertyKey SecuritySettingsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(SecuritySettings),
            typeof(SecuritySettings),
            typeof(OutOfBrowserSettings),
            null);

    /// <summary>
    /// Identifies the <see cref="SecuritySettings"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SecuritySettingsProperty = SecuritySettingsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the security settings applied to the out-of-browser application.
    /// </summary>
    /// <returns>
    /// The security settings applied to the out-of-browser application.
    /// </returns>
    public SecuritySettings SecuritySettings => (SecuritySettings)GetValue(SecuritySettingsProperty);

    private static readonly DependencyPropertyKey ShortNamePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ShortName),
            typeof(string),
            typeof(OutOfBrowserSettings),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="ShortName"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShortNameProperty = ShortNamePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the short version of the application title.
    /// </summary>
    /// <returns>
    /// The short version of the application title.
    /// </returns>
    public string ShortName => (string)GetValue(ShortNameProperty);

    private static readonly DependencyPropertyKey ShowInstallMenuItemPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ShowInstallMenuItem),
            typeof(bool),
            typeof(OutOfBrowserSettings),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Identifies the <see cref="ShowInstallMenuItem"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowInstallMenuItemProperty = ShowInstallMenuItemPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether the application right-click menu includes
    /// an install option.
    /// </summary>
    /// <returns>
    /// true if the right-click menu includes an install option; otherwise, false. The
    /// default is true.
    /// </returns>
    public bool ShowInstallMenuItem => (bool)GetValue(ShowInstallMenuItemProperty);

    private static readonly DependencyPropertyKey WindowSettingsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(WindowSettings),
            typeof(WindowSettings),
            typeof(OutOfBrowserSettings),
            null);

    /// <summary>
    /// Identifies the <see cref="WindowSettings"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WindowSettingsProperty = WindowSettingsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the settings applied to the application window.
    /// </summary>
    /// <returns>
    /// The settings applied to the application window.
    /// </returns>
    public WindowSettings WindowSettings => (WindowSettings)GetValue(WindowSettingsProperty);
}
