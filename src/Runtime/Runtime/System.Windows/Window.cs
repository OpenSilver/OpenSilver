
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

using CSHTML5.Internal;
using OpenSilver;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal.Controls.Primitives;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows;

/// <summary>
/// Represents an application window.
/// </summary>
public class Window : ContentControl, IResizeObserverListener
{
    private bool _contentRenderedRaised;

    static Window()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        EventManager.RegisterClassHandler<Window>(GotFocusEvent, new RoutedEventHandler(OnGotFocus), true);
        EventManager.RegisterClassHandler<Window>(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
    }

    private IDisposable _resizeObserver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    public Window()
    {
        BypassLayoutPolicies = true;

        if (Application.Current is Application app)
        {
            app.Windows.Add(this);
        }
    }

    ~Window() => _resizeObserver?.Dispose();

    internal TextMeasurementService TextMeasurementService { get; private set; }

    /// <inheritdoc />
    protected internal override IEnumerator LogicalChildren => new SingleChildEnumerator(GetValue(ContentProperty));

    /// <summary>
    /// Gets the currently activated window for an application.
    /// </summary>
    public static Window Current { get; set; }

    internal HtmlElementReference RootDomElement { get; private set; }

    /// <summary>
    /// Set the DOM element that will host the window. This can be set only to new windows. The MainWindow looks for a DIV that has the ID "cshtml5-root" or "opensilver-root".
    /// </summary>
    /// <param name="rootDomElement">The DOM element that will host the window</param>
    public void AttachToDomElement(HtmlElementReference rootDomElement)
    {
        if (OuterDiv.IsConnected || RootDomElement.IsConnected)
        {
            throw new InvalidOperationException("The method 'Window.AttachToDomElement' can be called only once.");
        }

        ArgumentNullException.ThrowIfNull(rootDomElement);

        //Note: The "rootDomElement" will contain one DIV for the root of the window visual tree, and other DIVs to host the popups.
        RootDomElement = rootDomElement;

        ParentWindow = this;

        // In case of XAML view hosted inside an HTML app, we usually set the "position" of the window root to "relative" rather than "absolute" (via external JavaScript code) in order to display it inside a specific DIV. However, in this case, the layers that contain the Popups are placed under the window DIV instead of over it. To work around this issue, we set the root element display to "grid". See the sample app "IntegratingACshtml5AppInAnSPA".
        RootDomElement.SetCssStyleProperty(CssPropertyNames.Display, "grid");
        RootDomElement.SetCssStyleProperty(CssPropertyNames.Overflow, "clip");

        // Create the DIV that will correspond to the root of the window visual tree:
        OuterDiv = INTERNAL_HtmlDomManager.CreateWindowDomElementAndAppendIt(this);

        _resizeObserver = ResizeObserver.Observe(RootDomElement, this);

        InputManager.Current.RegisterRoot(RootDomElement);

        // Set the window as "loaded":
        IsLoadedCache = true;
        IsConnectedToLiveTree = true;
        UpdateIsRenderableCache();
        UpdateIsVisibleCache();

        TextMeasurementService = new TextMeasurementService(this);

        // Raise the "Loaded" event:
        RaiseLoadedEvent();

        SetLayoutSize();
        RaiseContentRendered();
    }

    private static void OnGotFocus(object sender, RoutedEventArgs e) => Current = (Window)sender;

    private static void OnMouseDown(object sender, MouseEventArgs e) => PopupService.HandleMouseButton();

    #region Bounds and SizeChanged event

    /// <summary>
    /// Occurs when the window has rendered or changed its rendering size.
    /// </summary>
    public new event SizeChangedEventHandler SizeChanged;

    private void OnWindowSizeChanged(Size size)
    {
        InvalidateMeasure();
        SizeChanged?.Invoke(this, new SizeChangedEventArgs(size));
    }

    /// <summary>
    /// Gets the height and width of the application window, as a Rect value.
    /// </summary>
    public Rect Bounds
    {
        get
        {
            if (OuterDiv.IsConnected)
            {
                double width = OpenSilver.Interop.ExecuteJavaScriptDouble($"osjs.getProp('{RootDomElement.Uid}', 'offsetWidth')");
                double height = OpenSilver.Interop.ExecuteJavaScriptDouble($"osjs.getProp('{RootDomElement.Uid}', 'offsetHeight')");
                return new Rect(0, 0, width, height);
            }

            return new Rect(0, 0, 0, 0);
        }
    }

    #endregion

    /// <summary>
    /// Occurs after the window content has been rendered.
    /// </summary>
    public event EventHandler ContentRendered;

    protected virtual void OnContentRendered(EventArgs e) => ContentRendered?.Invoke(this, e);

    private void RaiseContentRendered()
    {
        if (_contentRenderedRaised)
        {
            return;
        }

        _contentRenderedRaised = true;
        OnContentRendered(EventArgs.Empty);
    }

    /// <summary>
    /// Identifies the <see cref="Content"/> dependency property.
    /// </summary>
    public static readonly new DependencyProperty ContentProperty =
        ContentControl.ContentProperty.AddOwner(
            typeof(Window),
            new FrameworkPropertyMetadata(null, OnContentChanged));

    /// <summary>
    /// Gets or sets the content of the <see cref="Window"/>.
    /// </summary>
    public new FrameworkElement Content
    {
        get => GetValue(ContentProperty) as FrameworkElement;
        set => SetValueInternal(ContentProperty, value);
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not null and not FrameworkElement)
        {
            throw new InvalidOperationException(Strings.WindowContentMustBeFrameworkElement);
        }
    }

    private void SetLayoutSize()
    {
        Rect bounds = Bounds;
        InvalidateMeasure();
        Measure(bounds.Size);
        Arrange(bounds);
        UpdateLayout();
    }

    /// <summary>
    /// Attempts to activate the application window by bringing it to the foreground
    /// and setting the input focus to it.
    /// </summary>
    public void Activate()
    {
        // Not needed in HTML.
    }

    #region Closing event

    /// <summary>
    /// Occurs when the window is about to close.
    /// </summary>
    public event EventHandler<ClosingEventArgs> Closing;

    /// <summary>
    /// Raises the Closing event
    /// </summary>
    /// <param name="e">The arguments for the event.</param>
    protected void OnClosing(ClosingEventArgs e) => Closing?.Invoke(this, e);

    internal bool InvokeOnClosing(bool cancellable)
    {
        if (Closing is not null)
        {
            var e = new ClosingEventArgs(cancellable);
            OnClosing(e);
            return e.IsCancelable && e.Cancel;
        }
        return false;
    }

    #endregion

    /// <summary>
    /// Gets the window that contains the specified <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="dependencyObject">
    /// The object contained by the <see cref="Window"/> to get.
    /// </param>
    /// <returns>
    /// The <see cref="Window"/> to get.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// dependencyObject is not a valid <see cref="DependencyObject"/>.
    /// </exception>
    public static Window GetWindow(DependencyObject dependencyObject)
    {
        if (dependencyObject is not UIElement uie)
        {
            throw new InvalidOperationException(string.Format(Strings.UIElement_NotAnUIElement, nameof(dependencyObject)));
        }

        return GetWindow(uie);
    }

    internal static Window GetWindow(UIElement uie)
    {
        Debug.Assert(uie is not null);
        return uie.ParentWindow;
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        Size size = Bounds.Size;

        if (VisualChildrenCount > 0)
        {
            if (GetVisualChild(0) is UIElement child)
            {
                child.Measure(size);
                return child.DesiredSize;
            }
        }

        return size;
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize) => base.ArrangeOverride(Bounds.Size);

    /// <summary>
    /// Called when the parent of the window is changed.
    /// </summary>
    /// <param name="oldParent">
    /// The previous parent. Set to null if the <see cref="DependencyObject"/> did not have a previous parent.
    /// </param>
    protected internal override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);

        if (VisualTreeHelper.GetParent(this) is not null)
        {
            throw new InvalidOperationException(Strings.WindowMustBeRoot);
        }
    }

    void IResizeObserverListener.OnSizeChanged(Size size) => OnWindowSizeChanged(size);

    /// <summary>
    /// Identifies the <see cref="Left"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty LeftProperty =
        DependencyProperty.Register(
            nameof(Left),
            typeof(double),
            typeof(Window),
            new FrameworkPropertyMetadata(double.NaN));

    /// <summary>
    /// Gets or sets the position of the window's left edge, in relation to the desktop.
    /// </summary>
    /// <returns>
    /// The position of the window's left edge.
    /// </returns>
    [NotImplemented]
    public double Left
    {
        get => (double)GetValue(LeftProperty);
        set => SetValueInternal(LeftProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Top"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty TopProperty =
        DependencyProperty.Register(
            nameof(Top),
            typeof(double),
            typeof(Window),
            new FrameworkPropertyMetadata(double.NaN));

    /// <summary>
    /// Gets or sets the position of the window's top edge, in relation to the desktop.
    /// </summary>
    /// <returns>
    /// The position of the window's top.
    /// </returns>
    [NotImplemented]
    public double Top
    {
        get => (double)GetValue(TopProperty);
        set => SetValueInternal(TopProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Window),
            new FrameworkPropertyMetadata(string.Empty), ValidateTitle);

    /// <summary>
    /// Gets or sets a window's title.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that contains the window's title.
    /// </returns>
    [NotImplemented]
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValueInternal(TitleProperty, value);
    }

    private static bool ValidateTitle(object value) => value is not null;

    /// <summary>
    /// Identifies the <see cref="AllowsTransparency"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty AllowsTransparencyProperty =
        DependencyProperty.Register(
            nameof(AllowsTransparency),
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value that indicates whether a window's client area supports transparency.
    /// </summary>
    /// <returns>
    /// true if the window supports transparency; otherwise, false.
    /// </returns>
    [NotImplemented]
    public bool AllowsTransparency
    {
        get => (bool)GetValue(AllowsTransparencyProperty);
        set => SetValueInternal(AllowsTransparencyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(ImageSource),
            typeof(Window),
            new FrameworkPropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets a window's icon.
    /// </summary>
    /// <returns>
    /// An System.Windows.Media.ImageSource object that represents the icon.
    /// </returns>
    [NotImplemented]
    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValueInternal(IconProperty, value);
    }

    private static readonly DependencyPropertyKey IsActivePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(IsActive),
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether the window is active.
    /// </summary>
    /// <returns>
    /// true if the window is active; otherwise, false. The default is false.
    /// </returns>
    [NotImplemented]
    public bool IsActive => (bool)GetValue(IsActiveProperty);

    [NotImplemented]
    public new bool IsVisible { get; private set; }

    /// <summary>
    /// Identifies the <see cref="ShowActivated"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty ShowActivatedProperty =
        DependencyProperty.Register(
            nameof(ShowActivated),
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that indicates whether a window is activated when first shown.
    /// </summary>
    /// <returns>
    /// true if a window is activated when first shown; otherwise, false. The default is true.
    /// </returns>
    [NotImplemented]
    public bool ShowActivated
    {
        get => (bool)GetValue(ShowActivatedProperty);
        set => SetValueInternal(ShowActivatedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ShowInTaskbar"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty ShowInTaskbarProperty =
        DependencyProperty.Register(
            nameof(ShowInTaskbar),
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that indicates whether the window has a task bar button.
    /// </summary>
    /// <returns>
    /// true if the window has a task bar button; otherwise, false. Does not apply when the window
    /// is hosted in a browser.
    /// </returns>
    [NotImplemented]
    public bool ShowInTaskbar
    {
        get => (bool)GetValue(ShowInTaskbarProperty);
        set => SetValueInternal(ShowInTaskbarProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Topmost"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty TopmostProperty =
        DependencyProperty.Register(
            nameof(Topmost),
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value that indicates whether a window appears in the topmost z-order.
    /// </summary>
    /// <returns>
    /// true if the window is topmost; otherwise, false.
    /// </returns>
    [NotImplemented]
    public bool Topmost
    {
        get => (bool)GetValue(TopmostProperty);
        set => SetValueInternal(TopmostProperty, value);
    }

    /// <summary>
    /// Gets or sets the position of the window when first shown.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.WindowStartupLocation"/> value that specifies the top/left position
    /// of a window when first shown. The default is <see cref="WindowStartupLocation.Manual"/>.
    /// </returns>
    [NotImplemented]
    public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.Manual;

    /// <summary>
    /// Identifies the <see cref="WindowStyle"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty WindowStyleProperty =
        DependencyProperty.Register(
            nameof(WindowStyle),
            typeof(WindowStyle),
            typeof(Window),
            new FrameworkPropertyMetadata(WindowStyle.SingleBorderWindow), ValidateWindowStyle);

    /// <summary>
    /// Gets or sets a window's border style.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.WindowStyle"/> that specifies a window's border style. The default is
    /// <see cref="WindowStyle.SingleBorderWindow"/>.
    /// </returns>
    [NotImplemented]
    public WindowStyle WindowStyle
    {
        get => (WindowStyle)GetValue(WindowStyleProperty);
        set => SetValueInternal(WindowStyleProperty, value);
    }

    private static bool ValidateWindowStyle(object value)
    {
        var style = (WindowStyle)value;
        return style == WindowStyle.SingleBorderWindow ||
               style == WindowStyle.None ||
               style == WindowStyle.BorderlessRoundCornersWindow;
    }

    /// <summary>
    /// Identifies the <see cref="WindowState"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty WindowStateProperty =
        DependencyProperty.Register(
            nameof(WindowState),
            typeof(WindowState),
            typeof(Window),
            new FrameworkPropertyMetadata(WindowState.Normal), ValidateWindowState);

    /// <summary>
    /// Gets or sets a value that indicates whether a window is restored, minimized, or maximized.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.WindowState"/> that determines whether a window is restored, minimized, or maximized.
    /// The default is <see cref="WindowState.Normal"/> (restored).
    /// </returns>
    [NotImplemented]
    public WindowState WindowState
    {
        get => (WindowState)GetValue(WindowStateProperty);
        set => SetValueInternal(WindowStateProperty, value);
    }

    private static bool ValidateWindowState(object value)
    {
        var state = (WindowState)value;
        return state == WindowState.Maximized ||
               state == WindowState.Minimized ||
               state == WindowState.Normal;
    }

    /// <summary>
    /// Identifies the <see cref="ResizeMode"/> dependency property.
    /// </summary>
    [NotImplemented]
    public static readonly DependencyProperty ResizeModeProperty =
        DependencyProperty.Register(
            nameof(ResizeMode),
            typeof(ResizeMode),
            typeof(Window),
            new FrameworkPropertyMetadata(ResizeMode.CanResize),
            ValidateResizeMode);

    /// <summary>
    /// Gets or sets the resize mode.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.ResizeMode"/> value specifying the resize mode.
    /// </returns>
    [NotImplemented]
    public ResizeMode ResizeMode
    {
        get => (ResizeMode)GetValue(ResizeModeProperty);
        set => SetValueInternal(ResizeModeProperty, value);
    }

    private static bool ValidateResizeMode(object value)
    {
        var mode = (ResizeMode)value;
        return mode is ResizeMode.NoResize
                    or ResizeMode.CanMinimize
                    or ResizeMode.CanResize
                    or ResizeMode.CanResizeWithGrip;
    }

    [NotImplemented]
    public bool? DialogResult { get; set; }

    [NotImplemented]
    public void Show() { }

    [NotImplemented]
    public void ShowDialog() { }

    [NotImplemented]
    public void Close() { }

    [NotImplemented]
    public void DragMove() { }

    [NotImplemented]
    public void DragResize(WindowResizeEdge resizeEdge) { }

    [NotImplemented]
    protected virtual void OnActivated(EventArgs e) { }

    [NotImplemented]
    protected virtual void OnDeactivated(EventArgs e) { }

    [NotImplemented]
    protected virtual void OnClosing(CancelEventArgs e) { }

    [NotImplemented]
    protected virtual void OnClosed(EventArgs e) { }
}
