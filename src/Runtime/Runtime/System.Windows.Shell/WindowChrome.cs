
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

namespace System.Windows.Shell;

/// <summary>
/// Represents an object that describes the customizations to the non-client area of a window.
/// </summary>
public class WindowChrome : DependencyObject
{
    private Window _owner;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowChrome"/> class.
    /// </summary>
    public WindowChrome() { }

    /// <summary>
    /// Identifies the <see cref="CaptionHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CaptionHeightProperty =
        DependencyProperty.Register(
            nameof(CaptionHeight),
            typeof(double),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(0d),
            value => (double)value >= 0d);

    /// <summary>
    /// Gets or sets the height of the caption area at the top of a window.
    /// </summary>
    public double CaptionHeight
    {
        get => (double)GetValue(CaptionHeightProperty);
        set => SetValueInternal(CaptionHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ResizeBorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ResizeBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(ResizeBorderThickness),
            typeof(Thickness),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(new Thickness(), OnResizeBorderThicknessChanged));

    /// <summary>
    /// Gets or sets a value that indicates the width of the border that is used to resize a window.
    /// </summary>
    /// <returns>
    /// The width of the border that is used to resize a window.
    /// </returns>
    public Thickness ResizeBorderThickness
    {
        get => (Thickness)GetValue(ResizeBorderThicknessProperty);
        set => SetValueInternal(ResizeBorderThicknessProperty, value);
    }

    private static void OnResizeBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var chrome = (WindowChrome)d;
        chrome._owner?.OnWindowChromeChanged(chrome, chrome);
    }

    /// <summary>
    /// Identifies the <see cref="CornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(new CornerRadius()));

    /// <summary>
    /// Gets or sets a value that indicates the amount that the corners of a window are rounded.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValueInternal(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="GlassFrameThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GlassFrameThicknessProperty =
        DependencyProperty.Register(
            nameof(GlassFrameThickness),
            typeof(Thickness),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(new Thickness()));

    /// <summary>
    /// Gets or sets a value that indicates the width of the glass frame around a window.
    /// </summary>
    /// <remarks>
    /// In a browser context, this property has no visual effect but is provided for WPF API compatibility.
    /// </remarks>
    public Thickness GlassFrameThickness
    {
        get => (Thickness)GetValue(GlassFrameThicknessProperty);
        set => SetValueInternal(GlassFrameThicknessProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="UseAeroCaptionButtons"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UseAeroCaptionButtonsProperty =
        DependencyProperty.Register(
            nameof(UseAeroCaptionButtons),
            typeof(bool),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that indicates whether hit-testing is enabled on the Windows Aero caption buttons.
    /// </summary>
    /// <remarks>
    /// In a browser context, this property has no effect but is provided for WPF API compatibility.
    /// </remarks>
    public bool UseAeroCaptionButtons
    {
        get => (bool)GetValue(UseAeroCaptionButtonsProperty);
        set => SetValueInternal(UseAeroCaptionButtonsProperty, value);
    }

    /// <summary>
    /// Identifies the WindowChrome attached property.
    /// </summary>
    public static readonly DependencyProperty WindowChromeProperty =
        DependencyProperty.RegisterAttached(
            "WindowChrome",
            typeof(WindowChrome),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(null, OnWindowChromeChanged));

    /// <summary>
    /// Gets the value of the <see cref="WindowChromeProperty"/> attached property from a <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window from which to read the property value.</param>
    /// <returns>The value of the <see cref="WindowChromeProperty"/> attached property.</returns>
    public static WindowChrome GetWindowChrome(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        return (WindowChrome)window.GetValue(WindowChromeProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="WindowChromeProperty"/> attached property on a <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window on which to set the attached property.</param>
    /// <param name="chrome">The value to set.</param>
    public static void SetWindowChrome(Window window, WindowChrome chrome)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.SetValueInternal(WindowChromeProperty, chrome);
    }

    private static void OnWindowChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window window)
        {
            var oldChrome = (WindowChrome)e.OldValue;
            var newChrome = (WindowChrome)e.NewValue;

            oldChrome?._owner = null;
            newChrome?._owner = window;

            window.OnWindowChromeChanged(oldChrome, newChrome);
        }
    }

    /// <summary>
    /// Identifies the IsHitTestVisibleInChrome attached property.
    /// </summary>
    public static readonly DependencyProperty IsHitTestVisibleInChromeProperty =
        DependencyProperty.RegisterAttached(
            "IsHitTestVisibleInChrome",
            typeof(bool),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets the value of the <see cref="IsHitTestVisibleInChromeProperty"/> attached property from a <see cref="UIElement"/>.
    /// </summary>
    /// <param name="inputElement">
    /// The element from which to read the property value.
    /// </param>
    /// <returns>
    /// The value of the <see cref="IsHitTestVisibleInChromeProperty"/> attached property.
    /// </returns>
    public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
    {
        if (inputElement is not DependencyObject d)
        {
            throw new ArgumentException("element must be a DependencyObject.", nameof(inputElement));
        }

        return (bool)d.GetValue(IsHitTestVisibleInChromeProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="IsHitTestVisibleInChromeProperty"/> attached property on a <see cref="UIElement"/>.
    /// </summary>
    /// <param name="inputElement">
    /// The element on which to set the attached property.
    /// </param>
    /// <param name="hitTestVisible">
    /// The value to set.
    /// </param>
    public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
    {
        if (inputElement is not DependencyObject d)
        {
            throw new ArgumentException("element must be a DependencyObject.", nameof(inputElement));
        }

        d.SetValueInternal(IsHitTestVisibleInChromeProperty, hitTestVisible);
    }
}
