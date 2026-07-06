
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
using OpenSilver.Controls;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;
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

    private readonly List<Window> _ownedWindows = [];
    private IDisposable _resizeObserver;
    private DispatcherOperation _contentRenderedCallback;
    private bool _postContentRenderedFromLoadedHandler;
    private bool _isModal;
    internal bool _isClosed;
    private bool _isFullScreen;
    private bool _hasExplicitWindowProps;
    private HtmlElementReference _overlayDiv;
    private TaskCompletionSource<bool?> _dialogResultTcs;
    private WindowHost _windowHost;
    private Window _owner;

    /// <summary>
    /// True if this window was shown via Show()/ShowDialog() and has overlay infrastructure.
    /// </summary>
    internal bool HasOverlayInfrastructure => _windowHost is not null;

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
        if (_windowHost is not null && WindowState == WindowState.Maximized)
        {
            _windowHost.InvalidateMeasure();
            _windowHost.SetLayoutSize();
        }
        else
        {
            InvalidateMeasure();
        }

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
                HtmlElementReference sizeReference = _overlayDiv.IsConnected ? _overlayDiv : OuterDiv;
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

    /// <summary>
    /// Attempts to activate the application window by bringing it to the foreground
    /// and setting the input focus to it.
    /// </summary>
    public void Activate()
    {
        if (ActiveWindow != this)
        {
            BringToFront();

            Window previous = ActiveWindow;
            ActiveWindow = this;
            Current = this;
            previous?.SetValueInternal(IsActivePropertyKey, false);
            previous?.OnDeactivated(EventArgs.Empty);
            SetValueInternal(IsActivePropertyKey, true);
            OnActivated(EventArgs.Empty);

            WindowTaskbar.OnWindowActivated(this);
        }
    }

    /// <summary>
    /// Moves this window's overlay to the front of the z-order.
    /// </summary>
    internal void BringToFront()
    {
        if (!_overlayDiv.IsConnected) return;

        Application app = Application.Current;
        if (app is null) return;

        string rootId = app.GetRootDiv().Uid;
        OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
            $"(function(){{ var overlay=document.getElementById('{_overlayDiv.Uid}');" +
            $"var root=document.getElementById('{rootId}');" +
            $"if(overlay && root) root.appendChild(overlay); }})()");

        // Owned windows always stay in front of their owner
        foreach (var owned in _ownedWindows)
        {
            if (!owned._isClosed && owned._overlayDiv.IsConnected)
            {
                owned.BringToFront();
            }
        }
    }

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

    /// <summary>
    /// Occurs when the window is about to close.
    /// </summary>
    public event EventHandler Closed;

    /// <summary>
    /// Raises the <see cref="Closed"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> that contains the event data.
    /// </param>
    protected virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);

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
        if (_windowHost is not null)
        {
            Size constraintSize = GetSecondaryWindowConstraintSize(availableSize);

            if (VisualChildrenCount > 0)
            {
                if (GetVisualChild(0) is UIElement child)
                {
                    child.Measure(constraintSize);

                    // If constrained (explicit size or maximized), use that size.
                    // If unconstrained (no explicit size), use the child's desired size.
                    double resultWidth = double.IsPositiveInfinity(constraintSize.Width)
                        ? child.DesiredSize.Width : constraintSize.Width;
                    double resultHeight = double.IsPositiveInfinity(constraintSize.Height)
                        ? child.DesiredSize.Height : constraintSize.Height;
                    return new Size(resultWidth, resultHeight);
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
        if (_windowHost is not null)
        {
            return base.ArrangeOverride(finalSize);
        }
        return base.ArrangeOverride(Bounds.Size);
    }

    private Size GetSecondaryWindowConstraintSize(Size availableSize)
    {
        if (WindowState == WindowState.Maximized)
        {
            // Use whatever space the parent allocated (fills the WindowHost)
            return availableSize;
        }

        double w = Width;
        double h = Height;
        if (!double.IsNaN(w) && !double.IsNaN(h))
        {
            return new Size(w, h);
        }

        // No explicit size: let the content determine the window size
        return new Size(double.PositiveInfinity, double.PositiveInfinity);
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
        if (parent is not null && _windowHost is null)
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
    [OpenSilver.NotImplemented]
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
    [OpenSilver.NotImplemented]
    public bool AllowsTransparency
    {
        get => (bool)GetValue(AllowsTransparencyProperty);
        set => SetValueInternal(AllowsTransparencyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
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
    [OpenSilver.NotImplemented]
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
    [OpenSilver.NotImplemented]
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
    /// true if the window has a task bar button; otherwise, false.
    /// </returns>
    public bool ShowInTaskbar
    {
        get => (bool)GetValue(ShowInTaskbarProperty);
        set => SetValueInternal(ShowInTaskbarProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Topmost"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
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
    [OpenSilver.NotImplemented]
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
    public WindowStartupLocation WindowStartupLocation
    {
        get;
        set
        {
            if (!IsValidWindowStartupLocation(value))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(WindowStartupLocation));
            }

            field = value;
        }
    }

    private static bool IsValidWindowStartupLocation(WindowStartupLocation value)
    {
        return value == WindowStartupLocation.CenterScreen ||
               value == WindowStartupLocation.Manual ||
               value == WindowStartupLocation.CenterOwner;
    }

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
        var window = (Window)d;
        if (window._windowHost is null) return;

        var oldState = (WindowState)e.OldValue;
        var newState = (WindowState)e.NewValue;

        if (newState == WindowState.Minimized)
        {
            // Remember what state we were in before minimizing
            window._stateBeforeMinimize = oldState;
            window.MinimizeSecondaryWindow();
        }
        else if (oldState == WindowState.Minimized)
        {
            window.RestoreFromMinimized();

            // After restoring from minimized, apply the target state
            if (newState == WindowState.Maximized)
            {
                window.MaximizeSecondaryWindow(WindowState.Minimized);
            }
            else if (newState == WindowState.Normal && window._stateBeforeMinimize == WindowState.Maximized)
            {
                // Was maximized before minimize, now going to Normal → restore size
                window.RestoreFromMaximized();
            }
        }
        else if (newState == WindowState.Maximized)
        {
            window.MaximizeSecondaryWindow(oldState);
        }
        else if (newState == WindowState.Normal && oldState == WindowState.Maximized)
        {
            window.RestoreFromMaximized();
        }
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
    [OpenSilver.NotImplemented]
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
    [OpenSilver.NotImplemented]
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
            new FrameworkPropertyMetadata(ResizeMode.CanResize, OnResizeModeChanged),
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

    private static void OnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = (Window)d;
        window._windowHost?.UpdateResizeMode((ResizeMode)e.NewValue);
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
    public Window Owner
    {
        get => _owner;
        set
        {
            if (_owner == value)
            {
                return;
            }

            if (!_isClosed && _overlayDiv.IsConnected)
            {
                throw new InvalidOperationException("Owner cannot be set after the window has been shown.");
            }

            if (value == this)
            {
                throw new ArgumentException(Strings.CannotSetOwnerToItself);
            }

            if (value is not null && IsOwnerOf(value))
            {
                throw new ArgumentException(string.Format(Strings.CircularOwnerChild, value, this));
            }

            _owner?.RemoveOwnedWindow(this);
            _owner = value;
            _owner?.AddOwnedWindow(this);
        }
    }

    private bool IsOwnerOf(Window window)
    {
        for (Window w = window; w is not null; w = w._owner)
        {
            if (w == this) return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the collection of windows that are owned by this window.
    /// </summary>
    public IReadOnlyList<Window> OwnedWindows => _ownedWindows;

    private void AddOwnedWindow(Window window) => _ownedWindows.Add(window);

    private void RemoveOwnedWindow(Window window) => _ownedWindows.Remove(window);

    /// <summary>
    /// Gets a value indicating whether the window is visible.
    /// </summary>
    public new bool IsVisible => (_overlayDiv.IsConnected || IsMainWindow) && !_isClosed;

    /// <summary>
    /// Opens a window and returns without waiting for the newly opened window to close.
    /// </summary>
    public void Show()
    {
        if (_isClosed)
        {
            throw new InvalidOperationException(Strings.ReshowNotAllowed);
        }

        if (_overlayDiv.IsConnected)
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
    public Task<bool?> ShowDialog()
    {
        if (_isClosed)
        {
            throw new InvalidOperationException(Strings.ReshowNotAllowed);
        }

        if (_overlayDiv.IsConnected)
        {
            return Task.FromResult<bool?>(null);
        }

        Owner ??= ActiveWindow;

        _isModal = true;
        _dialogResultTcs = new TaskCompletionSource<bool?>();
        ShowSecondaryWindow();
        return _dialogResultTcs.Task;
    }

    /// <summary>
    /// Manually closes a <see cref="Window"/>.
    /// </summary>
    public void Close() => InternalClose(false);

    internal void InternalClose(bool ignoreCancel)
    {
        if (_isClosed)
        {
            return;
        }

        if (InvokeOnClosing(!ignoreCancel))
        {
            return;
        }

        _isClosed = true;

        // Detach from owner
        Owner = null;

        Application.Current?.Windows.Remove(this);

        // Close owned windows first
        foreach (var owned in _ownedWindows.ToArray())
        {
            owned.InternalClose(true);
        }

        if (IsMainWindow)
        {
            PromoteNextMainWindow();
        }

        if (_overlayDiv.IsConnected)
        {
            CloseSecondaryWindow();
        }

        OnClosed(EventArgs.Empty);

        _dialogResultTcs?.TrySetResult(null);
    }

    private void PromoteNextMainWindow()
    {
        if (Application.Current is not Application app)
        {
            return;
        }

        // Find the most recently shown window that has an overlay and isn't closed
        Window candidate = null;
        foreach (Window w in app.Windows)
        {
            if (!w._isClosed && w._overlayDiv.IsConnected)
            {
                candidate = w;
            }
        }

        if (candidate is not null)
        {
            app.MainWindow = candidate;
        }
    }

    /// <summary>
    /// Makes a window invisible.
    /// </summary>
    public void Hide()
    {
        if (_overlayDiv.IsConnected)
        {
            _overlayDiv.SetCssStyleProperty(CssPropertyNames.Display, "none");
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
        if (_windowHost is null)
        {
            return;
        }

        if (Mouse.LeftButton != MouseButtonState.Pressed)
        {
            throw new InvalidOperationException(Strings.DragMoveFail);
        }

        BeginDrag();
    }

    /// <summary>
    /// Starts a window drag-resize operation.
    /// </summary>
    /// <param name="resizeEdge">The edge to resize from.</param>
    public void DragResize(WindowResizeEdge resizeEdge)
    {
        if (_windowHost is null) return;
        if (WindowState == WindowState.Maximized) return;
        if (ResizeMode < ResizeMode.CanResize) return;
        if (Mouse.LeftButton != MouseButtonState.Pressed) return;

        BeginResize(resizeEdge);
    }

    /// <summary>
    /// Occurs when a window becomes the foreground window.
    /// </summary>
    public event EventHandler Activated;

    /// <summary>
    /// Raises the <see cref="Activated"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> that contains the event data.
    /// </param>
    protected virtual void OnActivated(EventArgs e) => Activated?.Invoke(this, e);

    /// <summary>
    /// Occurs when a window becomes a background window.
    /// </summary>
    public event EventHandler Deactivated;

    /// <summary>
    /// Raises the <see cref="Deactivated"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> that contains the event data.
    /// </param>
    protected virtual void OnDeactivated(EventArgs e) => Deactivated?.Invoke(this, e);

    /// <summary>
    /// Raises the SourceInitialized event.
    /// </summary>
    protected virtual void OnSourceInitialized(EventArgs e) { }

    #region Secondary Window DOM Management

    private void ShowSecondaryWindow()
    {
        PrepareForSecondaryDisplay();

        Application app = Application.Current;

        // Always append the overlay to the app's rootDiv (not the main window's RootDomElement,
        // which may be an overlay itself after promotion).
        HtmlElementReference rootDiv = app.GetRootDiv();
        _overlayDiv = INTERNAL_HtmlDomManager.CreateWindowOverlayDomElementAndAppendIt(
            this, rootDiv, _isModal);

        _windowHost = new WindowHost(this);

        WindowChrome chrome = WindowChrome.GetWindowChrome(this);
        if (chrome is not null)
        {
            _windowHost.UpdateTitleBarHeight(chrome.CaptionHeight);
        }
        _windowHost.UpdateTitleBarVisibility(WindowStyle != WindowStyle.None && chrome is not null);

        RootDomElement = _overlayDiv;

        _windowHost.Show(_overlayDiv);

        // Determine display mode before positioning (centering sets Left/Top which would
        // make HasExplicitWindowProps return true).
        UpdateFullScreenMode();

        if (!_isFullScreen)
        {
            ApplyStartupLocation();
        }

        // If another window exists and is in full-screen mode, it should exit full-screen.
        if (app?.MainWindow is Window mainWindow && mainWindow != this)
        {
            mainWindow.UpdateFullScreenMode();
        }

        WindowTaskbar.AddWindow(this);

        if (ShowActivated) Activate();
    }

    private void ApplyStartupLocation()
    {
        switch (WindowStartupLocation)
        {
            case WindowStartupLocation.CenterScreen:
                CenterInViewport();
                break;
            case WindowStartupLocation.CenterOwner:
                CenterOverOwner();
                break;
            case WindowStartupLocation.Manual:
            default:
                UpdateWindowPosition();
                break;
        }
    }

    private void CenterOverOwner()
    {
        if (_windowHost is null || _owner is null || _owner._windowHost is null)
        {
            CenterInViewport();
            return;
        }

        double ownerLeft = double.IsNaN(_owner.Left) ? 0 : _owner.Left;
        double ownerTop = double.IsNaN(_owner.Top) ? 0 : _owner.Top;
        double ownerWidth = _owner._windowHost.DesiredSize.Width;
        double ownerHeight = _owner._windowHost.DesiredSize.Height;
        double hostWidth = _windowHost.DesiredSize.Width;
        double hostHeight = _windowHost.DesiredSize.Height;

        Left = Math.Max(0, ownerLeft + (ownerWidth - hostWidth) / 2);
        Top = Math.Max(0, ownerTop + (ownerHeight - hostHeight) / 2);

        UpdateWindowPosition();
    }

    private void CenterInViewport()
    {
        if (_windowHost is null) return;

        Rect bounds = Bounds;
        double hostWidth = _windowHost.DesiredSize.Width;
        double hostHeight = _windowHost.DesiredSize.Height;

        if (bounds.Width > 0 && bounds.Height > 0 && hostWidth > 0 && hostHeight > 0)
        {
            Left = Math.Max(0, (bounds.Width - hostWidth) / 2);
            Top = Math.Max(0, (bounds.Height - hostHeight) / 2);
        }
        else
        {
            Left = 0;
            Top = 0;
        }

        UpdateWindowPosition();
    }

    private void CloseSecondaryWindow()
    {
        WindowTaskbar.RemoveWindow(this);

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

    internal void RestoreFromTaskbar()
    {
        WindowState = _stateBeforeMinimize == WindowState.Maximized
            ? WindowState.Maximized
            : WindowState.Normal;
    }

    private void MinimizeSecondaryWindow()
    {
        // Hide the overlay (and the WindowHost within it)
        Hide();

        // Minimize owned windows
        foreach (var owned in _ownedWindows)
        {
            if (!owned._isClosed && owned.WindowState != WindowState.Minimized)
            {
                owned.WindowState = WindowState.Minimized;
            }
        }

        WindowTaskbar.UpdateVisibility();

        // Activate the next available window
        if (ActiveWindow == this)
        {
            ActivateNextWindow();
        }
    }

    private void ActivateNextWindow()
    {
        if (Application.Current is not Application app) return;

        // The DOM order of overlays reflects z-order (appendChild moves to top).
        // Find the topmost non-minimized, non-closed window by checking DOM order.
        string rootId = app.GetRootDiv().Uid;
        string currentOverlayId = _overlayDiv.Uid;

        string nextId = OpenSilver.Interop.ExecuteJavaScriptString(
            $"(function(){{ var root=document.getElementById('{rootId}');" +
            $"var children=root.querySelectorAll('.opensilver-window-overlay');" +
            $"for(var i=children.length-1;i>=0;i--){{" +
            $"  var c=children[i]; if(c.id!=='{currentOverlayId}' && c.style.display!=='none') return c.id;" +
            $"}} return ''; }})()");

        if (!string.IsNullOrEmpty(nextId))
        {
            foreach (Window w in app.Windows)
            {
                if (!w._isClosed && w._overlayDiv.Uid == nextId)
                {
                    w.Activate();
                    return;
                }
            }
        }
    }

    private void RestoreFromMinimized()
    {
        // Show the overlay again
        if (_overlayDiv.IsConnected)
        {
            _overlayDiv.SetCssStyleProperty(CssPropertyNames.Display, "flex");
        }

        // Restore owned windows
        foreach (var owned in _ownedWindows)
        {
            if (!owned._isClosed && owned.WindowState == WindowState.Minimized)
            {
                owned.RestoreFromTaskbar();
            }
        }

        EnsureWithinBoundaries();
    }

    private void MaximizeSecondaryWindow(WindowState previousState)
    {
        if (_windowHost is null || !_windowHost.OuterDiv.IsConnected) return;

        _windowHost.UpdateMaximizeRestoreButton(true);

        if (previousState == WindowState.Normal)
        {
            // Save current position, size, and constraints for later restoration
            _restoreLeft = double.IsNaN(Left) ? 0 : Left;
            _restoreTop = double.IsNaN(Top) ? 0 : Top;
            _restoreWidth = Width;
            _restoreHeight = Height;
            _restoreMinWidth = MinWidth;
            _restoreMinHeight = MinHeight;
            _restoreMaxWidth = MaxWidth;
            _restoreMaxHeight = MaxHeight;
        }

        // Clear all size constraints so the window fills the available space.
        Width = double.NaN;
        Height = double.NaN;
        MinWidth = 0;
        MinHeight = 0;
        MaxWidth = double.PositiveInfinity;
        MaxHeight = double.PositiveInfinity;

        // Fill the overlay: position at origin with full size
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Left, "0px");
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Top, "0px");
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Width, "100%");
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Height, "100%");

        _windowHost.VisualOffset = new Vector(0, 0);

        // Observe overlay resize so maximized windows follow viewport changes
        if (_overlayDiv.IsConnected)
        {
            _resizeObserver?.Dispose();
            _resizeObserver = ResizeObserver.Observe(_overlayDiv, this);
        }

        // Hide resize borders when maximized
        _windowHost.SetResizeBordersVisible(false);

        // Invalidate both the Window and WindowHost so the constraint is re-evaluated
        InvalidateMeasure();
        _windowHost.InvalidateMeasure();
        _windowHost.SetLayoutSize();
    }

    private void RestoreFromMaximized()
    {
        if (_windowHost is null || !_windowHost.OuterDiv.IsConnected) return;

        _windowHost.UpdateMaximizeRestoreButton(false);

        // Stop observing overlay resize (no longer maximized)
        if (!_isFullScreen)
        {
            _resizeObserver?.Dispose();
            _resizeObserver = null;
        }

        // Restore resize borders
        _windowHost.SetResizeBordersVisible(ResizeMode >= ResizeMode.CanResize);

        // Restore position, size, and constraints
        Left = _restoreLeft;
        Top = _restoreTop;
        Width = _restoreWidth;
        Height = _restoreHeight;
        MinWidth = _restoreMinWidth;
        MinHeight = _restoreMinHeight;
        MaxWidth = _restoreMaxWidth;
        MaxHeight = _restoreMaxHeight;

        // Remove the 100% override so the layout system can size to content
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Width, string.Empty);
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Height, string.Empty);
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Left, $"{_restoreLeft.ToInvariantString()}px");
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Top, $"{_restoreTop.ToInvariantString()}px");

        _windowHost.VisualOffset = new Vector(_restoreLeft, _restoreTop);

        // Re-layout: Width/Height are restored so the layout uses those, or infinity if NaN
        InvalidateMeasure();
        _windowHost.InvalidateMeasure();
        _windowHost.SetLayoutSize();

        EnsureWithinBoundaries();
    }

    #endregion

    #region WindowChrome and Drag Support

    private bool _isDragging;
    private Point _dragStartMousePosition;
    private double _dragStartLeft;
    private double _dragStartTop;
    private MouseEventHandler _dragMoveHandler;
    private MouseButtonEventHandler _dragUpHandler;

    private double _restoreLeft;
    private double _restoreTop;
    private double _restoreWidth;
    private double _restoreHeight;
    private double _restoreMinWidth;
    private double _restoreMinHeight;
    private double _restoreMaxWidth;
    private double _restoreMaxHeight;
    private WindowState _stateBeforeMinimize;

    internal void OnWindowChromeChanged(WindowChrome oldChrome, WindowChrome newChrome)
    {
        if (_windowHost is not null)
        {
            _windowHost.UpdateTitleBarVisibility(newChrome is not null && WindowStyle != WindowStyle.None);
            if (newChrome is not null)
            {
                _windowHost.UpdateTitleBarHeight(newChrome.CaptionHeight);
                _windowHost.UpdateResizeBorderThickness(newChrome.ResizeBorderThickness);
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
        _dragStartMousePosition = Mouse.GetPosition(null);
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

        Point currentPosition = e.GetPosition(null);
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

        EnsureWithinBoundaries();
    }

    #endregion

    #region DragResize Support

    private bool _isResizing;
    private WindowResizeEdge _resizeEdge;
    private Point _resizeStartMousePosition;
    private double _resizeStartLeft;
    private double _resizeStartTop;
    private double _resizeStartWidth;
    private double _resizeStartHeight;
    private MouseEventHandler _resizeMoveHandler;
    private MouseButtonEventHandler _resizeUpHandler;

    private void BeginResize(WindowResizeEdge edge)
    {
        if (_isResizing) return;

        _isResizing = true;
        _resizeEdge = edge;
        _resizeStartMousePosition = Mouse.GetPosition(null);
        _resizeStartLeft = double.IsNaN(Left) ? 0 : Left;
        _resizeStartTop = double.IsNaN(Top) ? 0 : Top;
        _resizeStartWidth = ActualWidth;
        _resizeStartHeight = ActualHeight;

        _resizeMoveHandler ??= new MouseEventHandler(Window_ResizeMouseMove);
        _resizeUpHandler ??= new MouseButtonEventHandler(Window_ResizeMouseUp);

        CaptureMouse();
        AddHandler(Mouse.MouseMoveEvent, _resizeMoveHandler, true);
        AddHandler(Mouse.MouseUpEvent, _resizeUpHandler, true);
    }

    private void Window_ResizeMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isResizing) return;

        Point currentPosition = e.GetPosition(null);
        double deltaX = currentPosition.X - _resizeStartMousePosition.X;
        double deltaY = currentPosition.Y - _resizeStartMousePosition.Y;

        double newLeft = _resizeStartLeft;
        double newTop = _resizeStartTop;
        double newWidth = _resizeStartWidth;
        double newHeight = _resizeStartHeight;

        bool resizeLeft = _resizeEdge == WindowResizeEdge.Left ||
                          _resizeEdge == WindowResizeEdge.TopLeft ||
                          _resizeEdge == WindowResizeEdge.BottomLeft;

        bool resizeRight = _resizeEdge == WindowResizeEdge.Right ||
                           _resizeEdge == WindowResizeEdge.TopRight ||
                           _resizeEdge == WindowResizeEdge.BottomRight;

        bool resizeTop = _resizeEdge == WindowResizeEdge.Top ||
                         _resizeEdge == WindowResizeEdge.TopLeft ||
                         _resizeEdge == WindowResizeEdge.TopRight;

        bool resizeBottom = _resizeEdge == WindowResizeEdge.Bottom ||
                            _resizeEdge == WindowResizeEdge.BottomLeft ||
                            _resizeEdge == WindowResizeEdge.BottomRight;

        if (resizeRight)
        {
            newWidth = _resizeStartWidth + deltaX;
        }
        else if (resizeLeft)
        {
            newWidth = _resizeStartWidth - deltaX;
            newLeft = _resizeStartLeft + deltaX;
        }

        if (resizeBottom)
        {
            newHeight = _resizeStartHeight + deltaY;
        }
        else if (resizeTop)
        {
            newHeight = _resizeStartHeight - deltaY;
            newTop = _resizeStartTop + deltaY;
        }

        // Clamp to Min/Max constraints
        double minW = MinWidth > 0 ? MinWidth : 0;
        double minH = MinHeight > 0 ? MinHeight : 0;
        double maxW = double.IsPositiveInfinity(MaxWidth) ? double.MaxValue : MaxWidth;
        double maxH = double.IsPositiveInfinity(MaxHeight) ? double.MaxValue : MaxHeight;

        if (newWidth < minW)
        {
            if (resizeLeft) newLeft -= (minW - newWidth);
            newWidth = minW;
        }
        else if (newWidth > maxW)
        {
            if (resizeLeft) newLeft -= (maxW - newWidth);
            newWidth = maxW;
        }

        if (newHeight < minH)
        {
            if (resizeTop) newTop -= (minH - newHeight);
            newHeight = minH;
        }
        else if (newHeight > maxH)
        {
            if (resizeTop) newTop -= (maxH - newHeight);
            newHeight = maxH;
        }

        Width = newWidth;
        Height = newHeight;
        Left = newLeft;
        Top = newTop;

        UpdateWindowPosition();
        _windowHost?.InvalidateMeasure();
        _windowHost?.SetLayoutSize();
    }

    private void Window_ResizeMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            EndResize();
        }
    }

    private void EndResize()
    {
        if (!_isResizing) return;

        _isResizing = false;
        ReleaseMouseCapture();
        RemoveHandler(Mouse.MouseMoveEvent, _resizeMoveHandler);
        RemoveHandler(Mouse.MouseUpEvent, _resizeUpHandler);

        EnsureWithinBoundaries();
    }

    #endregion

    #region Window Position

    private void UpdateWindowPosition()
    {
        if (_windowHost is null || !_windowHost.OuterDiv.IsConnected) return;

        double left = double.IsNaN(Left) ? 0 : Left;
        double top = double.IsNaN(Top) ? 0 : Top;

        _windowHost.VisualOffset = new Vector(left, top);

        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Left, $"{left.ToInvariantString()}px");
        _windowHost.OuterDiv.SetCssStyleProperty(CssPropertyNames.Top, $"{top.ToInvariantString()}px");
    }

    /// <summary>
    /// Ensures the window stays within the visible boundaries of the viewport.
    /// Clamps position so the chrome's draggable area remains accessible.
    /// Limits the window's maximum height to the viewport height.
    /// Does not resize the window content — only repositions and caps height.
    /// </summary>
    internal void EnsureWithinBoundaries()
    {
        if (_windowHost is null || WindowState == WindowState.Maximized) return;

        Rect viewport = Bounds;
        if (viewport.Width <= 0 || viewport.Height <= 0) return;

        double left = double.IsNaN(Left) ? 0 : Left;
        double top = double.IsNaN(Top) ? 0 : Top;
        double hostWidth = _windowHost.DesiredSize.Width;

        double titleBarHeight = _windowHost.GetTitleBarHeight();

        // Compute the minimum draggable margin: we need some area on each side
        // where the user can click and drag (not covered by buttons).
        double minDraggableMargin = _windowHost.GetMinDraggableMargin();

        // Clamp top: title bar must remain visible (can't go above viewport)
        if (top < 0)
        {
            top = 0;
        }
        if (top > viewport.Height - titleBarHeight)
        {
            top = Math.Max(0, viewport.Height - titleBarHeight);
        }

        // Clamp left: enough draggable area must remain visible
        if (left + hostWidth < minDraggableMargin)
        {
            left = minDraggableMargin - hostWidth;
        }
        if (left > viewport.Width - minDraggableMargin)
        {
            left = viewport.Width - minDraggableMargin;
        }

        // Limit maximum height to viewport (accounting for chrome and position)
        if (ResizeMode >= ResizeMode.CanResize)
        {
            double maxHeight = viewport.Height - titleBarHeight;
            if (!double.IsNaN(Height) && Height > maxHeight && maxHeight > 0)
            {
                Height = maxHeight;
            }
        }

        bool changed = false;
        if (Left != left) { Left = left; changed = true; }
        if (Top != top) { Top = top; changed = true; }

        if (changed)
        {
            UpdateWindowPosition();
        }
    }

    internal static void EnsureAllWindowsWithinBoundaries()
    {
        if (Application.Current is not Application app) return;

        foreach (Window w in app.Windows)
        {
            if (!w._isClosed && w._windowHost is not null)
            {
                w.EnsureWithinBoundaries();
            }
        }
    }

    #endregion

    #region Main Window Enforcement

    internal bool IsMainWindow => this == Application.Current?.MainWindow;

    /// <summary>
    /// Enters or exits full-screen mode based on whether this is the sole window
    /// and no explicit window properties have been set.
    /// </summary>
    internal void UpdateFullScreenMode()
    {
        bool shouldBeFullScreen = IsMainWindow
            && !HasExplicitWindowProps()
            && Application.Current?.Windows.Count <= 1;

        if (shouldBeFullScreen && !_isFullScreen)
        {
            EnterFullScreen();
        }
        else if (!shouldBeFullScreen && _isFullScreen)
        {
            ExitFullScreen();
        }
    }

    private void EnterFullScreen()
    {
        _isFullScreen = true;

        _windowHost?.UpdateTitleBarVisibility(false);

        SetValueInternal(WindowStateProperty, WindowState.Maximized);

        if (_overlayDiv.IsConnected)
        {
            _resizeObserver?.Dispose();
            _resizeObserver = ResizeObserver.Observe(_overlayDiv, this);
        }
    }

    private void ExitFullScreen()
    {
        _isFullScreen = false;

        // Keep the resize observer — window stays maximized, just with chrome visible.

        if (_windowHost is not null)
        {
            WindowChrome chrome = WindowChrome.GetWindowChrome(this);
            _windowHost.UpdateTitleBarVisibility(chrome is not null && WindowStyle != WindowStyle.None);
        }
    }

    private bool HasExplicitWindowProps()
    {
        if (_hasExplicitWindowProps) return true;

        _hasExplicitWindowProps =
            ReadLocalValue(WidthProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(HeightProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(LeftProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(TopProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(MinWidthProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(MinHeightProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(MaxWidthProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(MaxHeightProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(WindowStyleProperty) != DependencyProperty.UnsetValue ||
            ReadLocalValue(ResizeModeProperty) != DependencyProperty.UnsetValue ||
            WindowStartupLocation != WindowStartupLocation.Manual ||
            ReadLocalValue(WindowChrome.WindowChromeProperty) != DependencyProperty.UnsetValue;

        return _hasExplicitWindowProps;
    }

    #endregion
}
