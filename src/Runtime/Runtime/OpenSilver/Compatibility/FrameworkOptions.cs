
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
using System.Windows;

namespace OpenSilver.Compatibility;

/// <summary>
/// Provides options for controlling compatibility with WPF and Silverlight.
/// </summary>
public static class FrameworkOptions
{
    /// <summary>
    /// Identifies the <b>FrameworkOptions.TemplateKind</b> attached property.
    /// </summary>
    public static readonly DependencyProperty TemplateKindProperty =
        DependencyProperty.RegisterAttached(
            "TemplateKind",
            typeof(TemplateKind),
            typeof(FrameworkOptions),
            new PropertyMetadata(TemplateKind.Silverlight),
            IsTemplateKindValid);

    /// <summary>
    /// Gets the value that determines which control template conventions are used.
    /// </summary>
    /// <param name="element">
    /// The <see cref="DependencyObject"/> from which to read the <see cref="TemplateKindProperty"/>
    /// attached property.
    /// </param>
    /// <returns>
    /// A <see cref="TemplateKind"/> value that determines which control template conventions
    /// are used. The default is <see cref="TemplateKind.Silverlight"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static TemplateKind GetTemplateKind(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (TemplateKind)element.GetValue(TemplateKindProperty);
    }

    /// <summary>
    /// Sets the value that determines which control template conventions are used.
    /// </summary>
    /// <param name="element">
    /// The <see cref="DependencyObject"/> on which to set the <see cref="TemplateKindProperty"/>
    /// attached property.
    /// </param>
    /// <param name="value">
    /// A <see cref="TemplateKind"/> value that determines which control template conventions
    /// are used.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> is not a valid <see cref="TemplateKind"/> value.
    /// </exception>
    public static void SetTemplateKind(DependencyObject element, TemplateKind value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(TemplateKindProperty, value);
    }

    private static bool IsTemplateKindValid(object value)
    {
        TemplateKind kind = (TemplateKind)value;
        return kind == TemplateKind.Auto || kind == TemplateKind.Wpf || kind == TemplateKind.Silverlight;
    }
}
