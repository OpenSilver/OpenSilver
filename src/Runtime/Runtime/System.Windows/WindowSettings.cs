
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
/// Represents information about an out-of-browser application window.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class WindowSettings : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSettings" /> class.
    /// </summary>
    public WindowSettings() { }

    private static readonly DependencyPropertyKey HeightPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Height),
            typeof(double),
            typeof(WindowSettings),
            new PropertyMetadata(600.0));

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty = HeightPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the initial window height of the application.
    /// </summary>
    /// <returns>
    /// The initial window height of the application.
    /// </returns>
    public double Height => (double)GetValue(HeightProperty);

    private static readonly DependencyPropertyKey LeftPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Left),
            typeof(double),
            typeof(WindowSettings),
            new PropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="Left"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftProperty = LeftPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the initial position of the left edge of the out-of-browser application
    /// window when <see cref="WindowStartupLocation"/> is <see cref="WindowStartupLocation.Manual" />.
    /// </summary>
    /// <returns>
    /// The initial position of the left edge of the application window.
    /// </returns>
    public double Left => (double)GetValue(LeftProperty);

    private static readonly DependencyPropertyKey TitlePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Title),
            typeof(string),
            typeof(WindowSettings),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = TitlePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the full title of the out-of-browser application for display in the title
    /// bar of the application window.
    /// </summary>
    /// <returns>
    /// The full title of the application.
    /// </returns>
    public string Title => (string)GetValue(TitleProperty);

    private static readonly DependencyPropertyKey TopPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Top),
            typeof(double),
            typeof(WindowSettings),
            new PropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="Top"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TopProperty = TopPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the initial position of the top edge of the out-of-browser application window
    /// when <see cref="WindowStartupLocation"/> is <see cref="WindowStartupLocation.Manual"/>.
    /// </summary>
    /// <returns>
    /// The initial position of the top edge of the application window.
    /// </returns>
    public double Top => (double)GetValue(TopProperty);

    private static readonly DependencyPropertyKey WidthPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Width),
            typeof(double),
            typeof(WindowSettings),
            new PropertyMetadata(800.0));

    /// <summary>
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty = WidthPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the initial window width of the application.
    /// </summary>
    /// <returns>
    /// The initial window width of the application.
    /// </returns>
    public double Width => (double)GetValue(WidthProperty);

    private static readonly DependencyPropertyKey WindowStartupLocationPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(WindowStartupLocation),
            typeof(WindowStartupLocation),
            typeof(WindowSettings),
            new PropertyMetadata(WindowStartupLocation.CenterScreen));

    /// <summary>
    /// Identifies the <see cref="WindowStartupLocation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WindowStartupLocationProperty = WindowStartupLocationPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates how the out-of-browser application window is positioned at startup.
    /// </summary>
    /// <returns>
    /// A value that indicates how the application window is positioned at startup.
    /// </returns>
    public WindowStartupLocation WindowStartupLocation => (WindowStartupLocation)GetValue(WindowStartupLocationProperty);

    private static readonly DependencyPropertyKey WindowStylePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(WindowStyle),
            typeof(WindowStyle),
            typeof(WindowSettings),
            new PropertyMetadata(WindowStyle.SingleBorderWindow));

    /// <summary>
    /// Identifies the <see cref="WindowStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WindowStyleProperty = WindowStylePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates the appearance of the title bar and border for the
    /// out-of-browser application window.
    /// </summary>
    /// <returns>
    /// A value that indicates how the title bar and border should appear.
    /// </returns>
    public WindowStyle WindowStyle => (WindowStyle)GetValue(WindowStyleProperty);
}
