
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
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;

namespace System.Windows;

/// <summary>
/// Represents an application window.
/// </summary>
public class Window : ContentControl, IResizeObserverListener
{
    static Window()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        EventManager.RegisterClassHandler<Window>(Keyboard.GotKeyboardFocusEvent, new RoutedEventHandler(OnGotKeyboardFocus), true);
        EventManager.RegisterClassHandler<Window>(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), true);
    }

    private IDisposable _resizeObserver;
    private DispatcherOperation _contentRenderedCallback;
    private bool _postContentRenderedFromLoadedHandler;
    private bool _isShowingAsSecondary;
    private bool _isModal;
    private bool _isClosed;
    private HtmlElementReference _overlayDiv;
    private TaskCompletionSource<bool?> _dialogResultTcs;
    private OpenSilver.Controls.WindowHost _windowHost;

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

    /// <summary>
    /// Called when the window is about to be shown as a secondary window.
    /// Disables BypassLayoutPolicies so the layout system can apply inline dimensions.
    /// </summary>
    private void PrepareForSecondaryDisplay()
    {
        BypassLayoutPolicies = false;
    }

    ~Window() => _resizeObserver?.Dispose();

    /// <summary>
    /// Occurs after a window's content has been rendered.
    /// </summary>
    public event EventHandler ContentRendered;

    /// <inheritdoc />
    protected internal override IEnumerator LogicalChildren => new SingleChildEnumerator(GetValue(ContentProperty));

    /// <summary>
    /// Gets the currently activated window for an application.
    /// </summary>
    public static Window Current { get; set; }

    internal static Window ActiveWindow { get; private set; }

    internal HtmlElementReference RootDomElement { get; private set; }

    internal TextMeasurementService TextMeasurementService { get; private set; }

    /// <inheritdoc />
    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);

        if (IsLoaded)
        {
            PostContentRendered();
        }
        else
        {
            if (!_postContentRenderedFromLoadedHandler)
            {
                Loaded += new RoutedEventHandler(LoadedHandler);
                _postContentRenderedFromLoadedHandler = true;
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="ContentRendered"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> that contains the event data.
    /// </param>
    protected virtual void OnContentRendered(EventArgs e) => ContentRendered?.Invoke(this, e);

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
    }

    private static void OnGotKeyboardFocus(object sender, RoutedEventArgs e)
    {
        var activeWindow = (Window)sender;

        Current = activeWindow;
        ActiveWindow = activeWindow;
    }

    private static void OnPreviewMouseDown(object sender, MouseEventArgs e) => PopupService.HandleMouseButton();

    private void LoadedHandler(object sender, RoutedEventArgs e)
    {
        if (_postContentRenderedFromLoadedHandler)
        {
            PostContentRendered();
            _postContentRenderedFromLoadedHandler = false;
            Loaded -= new RoutedEventHandler(LoadedHandler);
        }
    }

    private void PostContentRendered()
    {
        _contentRenderedCallback?.Abort();
        _contentRenderedCallback = Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            new DispatcherOperationCallback(arg =>
            {
                Window thisRef = (Window)arg;
                thisRef._contentRenderedCallback = null;
                thisRef.OnContentRendered(EventArgs.Empty);
                return null;
            }),
            this);
    }

    #region Bounds and SizeChanged event

    /// <summary>
    /// Occurs when the window has rendered or changed its rendering size.
    /// </summary>
    public new event WindowSizeChangedEventHandler SizeChanged;

    private void OnWindowSizeChanged(Size size)
    {
        InvalidateMeasure();
        SizeChanged?.Invoke(this, new WindowSizeChangedEventArgs(size));
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
                HtmlElementReference sizeReference = _isShowingAsSecondary ? _overlayDiv : RootDomElement;
                if (!sizeReference.IsConnected)
                {
                    return new Rect(0, 0, 0, 0);
                }
                double width = OpenSilver.Interop.ExecuteJavaScriptDouble($"osjs.getProp('{sizeReference.Uid}', 'offsetWidth')");
                double height = OpenSilver.Interop.ExecuteJavaScriptDouble($"osjs.getProp('{sizeReference.Uid}', 'offsetHeight')");
                return new Rect(0, 0, width, height);
            }

            return new Rect(0, 0, 0, 0);
        }
    }

    #endregion

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
        if (_isShowingAsSecondary)
        {
            InvalidateMeasure();
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(new Point(), DesiredSize));
            UpdateLayout();
        }
        else
        {
            Rect bounds = Bounds;
            InvalidateMeasure();
            Measure(bounds.Size);
            Arrange(bounds);
            UpdateLayout();
        }
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
        if (_isShowingAsSecondary)
        {
            Size constraintSize = GetSecondaryWindowConstraintSize();

            if (VisualChildrenCount > 0)
            {
                if (GetVisualChild(0) is UIElement child)
                {
                    child.Measure(constraintSize);
                    return constraintSize;
                }
            }
            return constraintSize;
        }

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
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_isShowingAsSecondary)
        {
            return base.ArrangeOverride(finalSize);
        }
        return base.ArrangeOverride(Bounds.Size);
    }

    private Size GetSecondaryWindowConstraintSize()
    {
        if (WindowState == WindowState.Maximized)
        {
            Size overlaySize = Bounds.Size;
            if (overlaySize.Width > 0 && overlaySize.Height > 0)
            {
                return overlaySize;
            }
        }

        double w = Width;
        double h = Height;
        if (!double.IsNaN(w) && !double.IsNaN(h))
        {
            return new Size(w, h);
        }

        return Bounds.Size;
    }

    /// <summary>
    /// Called when the parent of the window is changed.
    /// </summary>
    /// <param name="oldParent">
    /// The previous parent. Set to null if the <see cref="DependencyObject"/> did not have a previous parent.
    /// </param>
    protected internal override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);

        var parent = VisualTreeHelper.GetParent(this);
        if (parent is not null && !_isShowingAsSecondary)
        {
            throw new InvalidOperationException(Strings.WindowMustBeRoot);
        }
    }

    void IResizeObserverListener.OnSizeChanged(Size size) => OnWindowSizeChanged(size);

    /// <summary>
    /// Identifies the <see cref="Left"/> dependency property.
    /// </summary>
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
    public double Left
    {
        get => (double)GetValue(LeftProperty);
        set => SetValueInternal(LeftProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Top"/> dependency property.
    /// </summary>
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
    public double Top
    {
        get => (double)GetValue(TopProperty);
        set => SetValueInternal(TopProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Window),
            new FrameworkPropertyMetadata(string.Empty),
            ValidateTitle);

    /// <summary>
    /// Gets or sets a window's title.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that contains the window's title.
    /// </returns>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValueInternal(TitleProperty, value);
    }

    private static bool ValidateTitle(object value) => value is not null;

    /// <summary>
    /// Identifies the <see cref="AllowsTransparency"/> dependency property.
    /// </summary>
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
    public bool AllowsTransparency
    {
        get => (bool)GetValue(AllowsTransparencyProperty);
        set => SetValueInternal(AllowsTransparencyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
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
    public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether the window is active.
    /// </summary>
    /// <returns>
    /// true if the window is active; otherwise, false. The default is false.
    /// </returns>
    public bool IsActive => (bool)GetValue(IsActiveProperty);

    /// <summary>
    /// Identifies the <see cref="ShowActivated"/> dependency property.
    /// </summary>
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
    public bool ShowActivated
    {
        get => (bool)GetValue(ShowActivatedProperty);
        set => SetValueInternal(ShowActivatedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ShowInTaskbar"/> dependency property.
    /// </summary>
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
    public bool ShowInTaskbar
    {
        get => (bool)GetValue(ShowInTaskbarProperty);
        set => SetValueInternal(ShowInTaskbarProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Topmost"/> dependency property.
    /// </summary>
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
    public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.Manual;

    /// <summary>
    /// Identifies the <see cref="WindowStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WindowStyleProperty =
        DependencyProperty.Register(
            nameof(WindowStyle),
            typeof(WindowStyle),
            typeof(Window),
            new FrameworkPropertyMetadata(WindowStyle.SingleBorderWindow, OnWindowStyleChanged),
            ValidateWindowStyle);

    /// <summary>
    /// Gets or sets a window's border style.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.WindowStyle"/> that specifies a window's border style. The default is
    /// <see cref="WindowStyle.SingleBorderWindow"/>.
    /// </returns>
    public WindowStyle WindowStyle
    {
        get => (WindowStyle)GetValue(WindowStyleProperty);
        set => SetValueInternal(WindowStyleProperty, value);
    }

    private static void OnWindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = (Window)d;
        if (window._windowHost is not null)
        {
            WindowChrome chrome = WindowChrome.GetWindowChrome(window);
            window._windowHost.UpdateTitleBarVisibility(
                chrome is not null && (WindowStyle)e.NewValue != WindowStyle.None);
        }
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
    public static readonly DependencyProperty WindowStateProperty =
        DependencyProperty.Register(
            nameof(WindowState),
            typeof(WindowState),
            typeof(Window),
            new FrameworkPropertyMetadata(WindowState.Normal, OnWindowStateChanged),
            ValidateWindowState);

    /// <summary>
    /// Gets or sets a value that indicates whether a window is restored, minimized, or maximized.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.WindowState"/> that determines whether a window is restored, minimized, or maximized.
    /// The default is <see cref="WindowState.Normal"/> (restored).
    /// </returns>
    public WindowState WindowState
    {
        get => (WindowState)GetValue(WindowStateProperty);
        set => SetValueInternal(WindowStateProperty, value);
    }

    private static void OnWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private static bool ValidateWindowState(object value)
    {
        var state = (WindowState)value;
        return state == WindowState.Maximized ||
               state == WindowState.Minimized ||
               state == WindowState.Normal;
    }

    /// <summary>
    /// Identifies the <see cref="SizeToContent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SizeToContentProperty =
        DependencyProperty.Register(
            nameof(SizeToContent),
            typeof(SizeToContent),
            typeof(Window),
            new FrameworkPropertyMetadata(SizeToContent.Manual),
            IsValidSizeToContent);

    /// <summary>
    /// Gets or sets a value that indicates whether a window will automatically size itself to fit the size 
    /// of its content.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.SizeToContent"/> value. The default is <see cref="SizeToContent.Manual"/>.
    /// </returns>
    public SizeToContent SizeToContent
    {
        get => (SizeToContent)GetValue(SizeToContentProperty);
        set => SetValueInternal(SizeToContentProperty, value);
    }

    private static bool IsValidSizeToContent(object o)
    {
        var value = (SizeToContent)o;
        return value == SizeToContent.Manual ||
               value == SizeToContent.Width ||
               value == SizeToContent.Height ||
               value == SizeToContent.WidthAndHeight;
    }

    /// <summary>
    /// Identifies the <see cref="ResizeMode"/> dependency property.
    /// </summary>
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
    public ResizeMode ResizeMode
    {
        get => (ResizeMode)GetValue(ResizeModeProperty);
        set => SetValueInternal(ResizeModeProperty, value);
    }

    private static bool ValidateResizeMode(object value)
    {
        var mode = (ResizeMode)value;
        return mode == ResizeMode.NoResize ||
               mode == ResizeMode.CanMinimize ||
               mode == ResizeMode.CanResize ||
               mode == ResizeMode.CanResizeWithGrip;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a window was accepted or canceled.
    /// </summary>
    public bool? DialogResult
    {
        get => _dialogResultTcs?.Task.IsCompleted == true ? _dialogResultTcs.Task.Result : null;
        set
        {
            if (_isModal)
            {
                _dialogResultTcs?.TrySetResult(value);
                Close();
            }
        }
    }

    /// <summary>
    /// Gets or sets the owner of this window.
    /// </summary>
    public Window Owner { get; set; }

    /// <summary>
    /// Gets a value indicating whether the window is visible.
    /// </summary>
    public new bool IsVisible => _isShowingAsSecondary && !_isClosed;

    /// <summary>
    /// Opens a window and returns without waiting for the newly opened window to close.
    /// </summary>
    public void Show()
    {
        if (_isClosed)
        {
            throw new InvalidOperationException("Cannot show a window that has been closed.");
        }

        if (_isShowingAsSecondary)
        {
            return;
        }

        _isModal = false;
        ShowSecondaryWindow();
    }

    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    /// <returns>
    /// A <see cref="Nullable{Boolean}"/> value that specifies whether the activity was accepted (true) or canceled (false).
    /// </returns>
    public bool? ShowDialog()
    {
        if (_isClosed)
        {
            throw new InvalidOperationException("Cannot show a window that has been closed.");
        }

        if (_isShowingAsSecondary)
        {
            return null;
        }

        _isModal = true;
        _dialogResultTcs = new TaskCompletionSource<bool?>();
        ShowSecondaryWindow();
        return null;
    }

    /// <summary>
    /// Manually closes a <see cref="Window"/>.
    /// </summary>
    public void Close()
    {
        if (_isClosed)
        {
            return;
        }

        var e = new CancelEventArgs();
        OnClosing(e);

        if (e.Cancel)
        {
            return;
        }

        if (InvokeOnClosing(true))
        {
            return;
        }

        _isClosed = true;

        if (_isShowingAsSecondary)
        {
            CloseSecondaryWindow();
        }

        OnClosed(EventArgs.Empty);
        Closed?.Invoke(this, EventArgs.Empty);

        _dialogResultTcs?.TrySetResult(null);
    }

    /// <summary>
    /// Makes a window invisible.
    /// </summary>
    public void Hide()
    {
        if (_isShowingAsSecondary && _overlayDiv.IsConnected)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.getElementById('{_overlayDiv.Uid}').style.display='none'");
        }
    }

    /// <summary>
    /// Allows a window to be dragged by a mouse with its left button down over an exposed area
    /// of the window's client area.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The left mouse button is not pressed.
    /// </exception>
    public void DragMove()
    {
        if (!_isShowingAsSecondary)
        {
            return;
        }

        if (Mouse.LeftButton != MouseButtonState.Pressed)
        {
            throw new InvalidOperationException("Can only call DragMove when the left mouse button is pressed.");
        }

        BeginDrag();
    }

    /// <summary>
    /// Starts a window drag-resize operation.
    /// </summary>
    /// <param name="resizeEdge">The edge to resize from.</param>
    public void DragResize(WindowResizeEdge resizeEdge)
    {
        // TODO: implement resize drag
    }

    /// <summary>
    /// Occurs when a window becomes the foreground window.
    /// </summary>
    public event EventHandler Activated;

    /// <summary>
    /// Occurs when a window becomes a background window.
    /// </summary>
    public event EventHandler Deactivated;

    /// <summary>
    /// Occurs when the window is about to close.
    /// </summary>
    public event EventHandler Closed;

    /// <summary>
    /// Raises the <see cref="Activated"/> event.
    /// </summary>
    protected virtual void OnActivated(EventArgs e) => Activated?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="Deactivated"/> event.
    /// </summary>
    protected virtual void OnDeactivated(EventArgs e) => Deactivated?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:Closing"/> event.
    /// </summary>
    protected virtual void OnClosing(CancelEventArgs e) { }

    /// <summary>
    /// Raises the <see cref="Closed"/> event.
    /// </summary>
    protected virtual void OnClosed(EventArgs e) { }

    /// <summary>
    /// Raises the SourceInitialized event.
    /// </summary>
    protected virtual void OnSourceInitialized(EventArgs e) { }

    #region Secondary Window DOM Management

    private void ShowSecondaryWindow()
    {
        _isShowingAsSecondary = true;
        PrepareForSecondaryDisplay();

        Window mainWindow = Application.Current?.MainWindow;
        if (mainWindow is null)
        {
            throw new InvalidOperationException("Cannot show a secondary window before the main window is created.");
        }

        _overlayDiv = INTERNAL_HtmlDomManager.CreateWindowOverlayDomElementAndAppendIt(
            this, mainWindow.RootDomElement, _isModal);

        _windowHost = new OpenSilver.Controls.WindowHost(this);

        WindowChrome chrome = WindowChrome.GetWindowChrome(this);
        if (chrome is not null)
        {
            _windowHost.UpdateTitleBarHeight(chrome.CaptionHeight);
        }
        _windowHost.UpdateTitleBarVisibility(WindowStyle != WindowStyle.None && chrome is not null);

        _windowHost.Show(_overlayDiv, mainWindow);

        Current = this;
        ActiveWindow = this;
        OnActivated(EventArgs.Empty);
    }

    private void CloseSecondaryWindow()
    {
        _isShowingAsSecondary = false;

        _windowHost?.Close();
        _windowHost = null;

        // Remove overlay
        if (_overlayDiv.IsConnected)
        {
            INTERNAL_HtmlDomManager.RemoveNodeNative(_overlayDiv);
        }

        OnDeactivated(EventArgs.Empty);

        // Restore active window
        Window mainWindow = Application.Current?.MainWindow;
        if (mainWindow is not null)
        {
            Current = mainWindow;
            ActiveWindow = mainWindow;
        }
    }

    #endregion

    #region WindowChrome and Drag Support

    private bool _isDragging;
    private Point _dragStartMousePosition;
    private double _dragStartLeft;
    private double _dragStartTop;
    private MouseEventHandler _dragMoveHandler;
    private MouseButtonEventHandler _dragUpHandler;

    internal void OnWindowChromeChanged(WindowChrome oldChrome, WindowChrome newChrome)
    {
        if (_windowHost is not null)
        {
            _windowHost.UpdateTitleBarVisibility(newChrome is not null && WindowStyle != WindowStyle.None);
            if (newChrome is not null)
            {
                _windowHost.UpdateTitleBarHeight(newChrome.CaptionHeight);
            }
        }
    }

    private void BeginDrag()
    {
        if (WindowState == WindowState.Maximized)
        {
            return;
        }

        if (_isDragging)
        {
            return;
        }

        _isDragging = true;
        _dragStartMousePosition = Mouse.GetPosition(ParentWindow);
        _dragStartLeft = double.IsNaN(Left) ? 0 : Left;
        _dragStartTop = double.IsNaN(Top) ? 0 : Top;

        _dragMoveHandler ??= new MouseEventHandler(Window_DragMouseMove);
        _dragUpHandler ??= new MouseButtonEventHandler(Window_DragMouseUp);

        CaptureMouse();
        AddHandler(Mouse.MouseMoveEvent, _dragMoveHandler, true);
        AddHandler(Mouse.MouseUpEvent, _dragUpHandler, true);
    }

    private void Window_DragMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging) return;

        Point currentPosition = e.GetPosition(ParentWindow);
        double deltaX = currentPosition.X - _dragStartMousePosition.X;
        double deltaY = currentPosition.Y - _dragStartMousePosition.Y;

        Left = _dragStartLeft + deltaX;
        Top = _dragStartTop + deltaY;

        UpdateWindowPosition();
    }

    private void Window_DragMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            EndDrag();
        }
    }

    private void EndDrag()
    {
        if (!_isDragging) return;

        _isDragging = false;
        ReleaseMouseCapture();
        RemoveHandler(Mouse.MouseMoveEvent, _dragMoveHandler);
        RemoveHandler(Mouse.MouseUpEvent, _dragUpHandler);
    }

    private void UpdateWindowPosition()
    {
        if (_windowHost is null || !_windowHost.OuterDiv.IsConnected) return;

        double left = double.IsNaN(Left) ? 0 : Left;
        double top = double.IsNaN(Top) ? 0 : Top;

        OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
            $"(function(){{ var el=document.getElementById('{_windowHost.OuterDiv.Uid}'); if(el){{ el.style.left='{left.ToInvariantString()}px'; el.style.top='{top.ToInvariantString()}px'; }} }})()");
    }

    #endregion

    #region Main Window Enforcement

    internal bool IsMainWindow => this == Application.Current?.MainWindow;

    internal void EnforceMainWindowProperties()
    {
        if (IsMainWindow)
        {
            SetValueInternal(WindowStyleProperty, WindowStyle.None);
            SetValueInternal(WindowStateProperty, WindowState.Maximized);
        }
    }

    #endregion
}
