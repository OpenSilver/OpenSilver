
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

using System.Windows.Markup;

namespace System.Windows;

/// <summary>
/// Represents an icon that is used to identify an offline application.
/// </summary>
[ContentProperty(nameof(Source))]
[OpenSilver.NotImplemented]
public sealed class Icon : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Icon"/> class.
    /// </summary>
    public Icon() { }

    private static readonly DependencyPropertyKey SizePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Size),
            typeof(Size),
            typeof(Icon),
            new PropertyMetadata(new Size()));

    /// <summary>
    /// Identifies the <see cref="Size"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SizeProperty = SizePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the icon size.
    /// </summary>
    /// <returns>
    /// The icon size.
    /// </returns>
    public Size Size => (Size)GetValue(SizeProperty);

    private static readonly DependencyPropertyKey SourcePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Source),
            typeof(Uri),
            typeof(Icon),
            new PropertyMetadata(new Uri(string.Empty, UriKind.Relative)));

    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = SourcePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the path and file name to the PNG source file of the icon.
    /// </summary>
    /// <returns>
    /// The path to the PNG source file of the icon.
    /// </returns>
    public Uri Source => (Uri)GetValue(SourceProperty);
}
