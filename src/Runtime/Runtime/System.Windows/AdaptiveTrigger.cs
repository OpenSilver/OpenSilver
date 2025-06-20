
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
/// Represents a declarative rule that applies visual states based on window properties.
/// </summary>
public sealed class AdaptiveTrigger : StateTriggerBase
{
    private Window _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdaptiveTrigger"/> class.
    /// </summary>
    public AdaptiveTrigger() { }

    /// <summary>
    /// Identifies the <see cref="MinWindowWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinWindowWidthProperty =
        DependencyProperty.Register(
            nameof(MinWindowWidth),
            typeof(double),
            typeof(AdaptiveTrigger),
            new PropertyMetadata(-1.0, OnMinWindowDimensionChanged));

    /// <summary>
    /// Gets or sets the minimum window width at which the <see cref="VisualState"/> should be applied.
    /// </summary>
    /// <returns>
    /// The minimum window width (in effective pixels) at which the <see cref="VisualState"/> should be applied.
    /// </returns>
    public double MinWindowWidth
    {
        get => (double)GetValue(MinWindowWidthProperty);
        set => SetValueInternal(MinWindowWidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MinWindowHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinWindowHeightProperty =
        DependencyProperty.Register(
            nameof(MinWindowHeight),
            typeof(double),
            typeof(AdaptiveTrigger),
            new PropertyMetadata(-1.0, OnMinWindowDimensionChanged));

    /// <summary>
    /// Gets or sets the minimum window height at which the <see cref="VisualState"/> should be applied.
    /// </summary>
    /// <returns>
    /// The minimum window height (in effective pixels) at which the <see cref="VisualState"/> should be applied.
    /// </returns>
    public double MinWindowHeight
    {
        get => (double)GetValue(MinWindowHeightProperty);
        set => SetValueInternal(MinWindowHeightProperty, value);
    }

    private static void OnMinWindowDimensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((AdaptiveTrigger)d).UpdateState();
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        AttachEvents();

        UpdateState(true);
    }

    /// <inheritdoc />
    protected override void OnDetached()
    {
        base.OnDetached();

        DetachEvents();
    }

    private void AttachEvents()
    {
        DetachEvents();

        FrameworkElement visualElement = VisualState?.VisualStateGroup?.VisualElement;

        _window = visualElement?.ParentWindow;
        if (_window is not null)
        {
            ((FrameworkElement)_window).SizeChanged += OnWindowSizeChanged;
        }
    }

    private void DetachEvents()
    {
        if (_window is not null)
        {
            ((FrameworkElement)_window).SizeChanged -= OnWindowSizeChanged;
        }
        _window = null;
    }

    private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e) => UpdateState();

    private void UpdateState(bool knownAttached = false)
    {
        if (!knownAttached && !IsAttached)
        {
            return;
        }

        if (_window is null)
        {
            return;
        }

        double w = _window.RenderSize.Width;
        double h = _window.RenderSize.Height;

        double mw = MinWindowWidth;
        double mh = MinWindowHeight;

        SetActive(w >= mw && h >= mh);
    }
}
