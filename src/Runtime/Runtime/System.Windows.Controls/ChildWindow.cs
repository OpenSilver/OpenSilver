// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using MouseEventHandler = Windows.UI.Xaml.Input.PointerEventHandler;
using MouseButtonEventHandler = Windows.UI.Xaml.Input.PointerEventHandler;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Key = Windows.System.VirtualKey;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides a window that can be displayed over a parent window and blocks
    /// interaction with the parent window.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = PART_Chrome, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ContentRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Overlay, Type = typeof(Panel))]
    [TemplatePart(Name = PART_Root, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VSMSTATE_StateClosed, GroupName = VSMGROUP_Window)]
    [TemplateVisualState(Name = VSMSTATE_StateOpen, GroupName = VSMGROUP_Window)]
    public class ChildWindow : ContentControl
    {
        /// <summary>
        /// The name of the Chrome template part.
        /// </summary>
        private const string PART_Chrome = "Chrome";

        /// <summary>
        /// The name of the CloseButton template part.
        /// </summary>
        private const string PART_CloseButton = "CloseButton";

        /// <summary>
        /// The name of the ContentPresenter template part.
        /// </summary>
        private const string PART_ContentPresenter = "ContentPresenter";

        /// <summary>
        /// The name of the ContentRoot template part.
        /// </summary>
        private const string PART_ContentRoot = "ContentRoot";

        /// <summary>
        /// The name of the Overlay template part.
        /// </summary>
        private const string PART_Overlay = "Overlay";

        /// <summary>
        /// The name of the Root template part.
        /// </summary>
        private const string PART_Root = "Root";

        /// <summary>
        /// The name of the WindowStates VSM group.
        /// </summary>
        private const string VSMGROUP_Window = "WindowStates";

        /// <summary>
        /// The name of the Closing VSM state.
        /// </summary>
        private const string VSMSTATE_StateClosed = "Closed";

        /// <summary>
        /// The name of the Opening VSM state.
        /// </summary>
        private const string VSMSTATE_StateOpen = "Open";

        /// <summary>
        /// The name of the Modal VSM state.
        /// </summary>
        private const string VSMState_StateModal = "Modal";

        /// <summary>
        /// The name of the Not Modal VSM state.
        /// </summary>
        private const string VSMState_StateNotModal = "NotModal";

        /// <summary>
        /// Stores the previous value of RootVisual.IsEnabled.
        /// </summary>
        private static bool RootVisual_PrevEnabledState = true;

        /// <summary>
        /// Stores a count of the number of open ChildWindow instances.
        /// </summary>
        private static int OpenChildWindowCount = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChildWindow" /> has a close
        /// button.
        /// </summary>
        /// <value>
        /// True if the child window has a close button; otherwise, false. The
        /// default is true.
        /// </value>
        public bool HasCloseButton
        {
            get { return (bool)GetValue(HasCloseButtonProperty); }
            set { SetValue(HasCloseButtonProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HasCloseButton" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="HasCloseButton" /> dependency property.
        /// </value>
        public static readonly DependencyProperty HasCloseButtonProperty =
            DependencyProperty.Register(
            nameof(HasCloseButton),
            typeof(bool),
            typeof(ChildWindow),
            new PropertyMetadata(true, OnHasCloseButtonPropertyChanged));

        /// <summary>
        /// HasCloseButtonProperty PropertyChangedCallback call back static function.
        /// </summary>
        /// <param name="d">ChildWindow object whose HasCloseButton property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnHasCloseButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow cw = (ChildWindow)d;

            if (cw.CloseButton != null)
            {
                if ((bool)e.NewValue)
                {
                    cw.CloseButton.Visibility = Visibility.Visible;
                }
                else
                {
                    cw.CloseButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets the visual brush that is used to cover the parent
        /// window when the child window is open.
        /// </summary>
        /// <value>
        /// The visual brush that is used to cover the parent window when the
        /// <see cref="ChildWindow" /> is open. The default is null.
        /// </value>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OverlayBrush" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="OverlayBrush" /> dependency property.
        /// </value>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register(
            nameof(OverlayBrush),
            typeof(Brush),
            typeof(ChildWindow),
            new PropertyMetadata(OnOverlayBrushPropertyChanged));

        /// <summary>
        /// OverlayBrushProperty PropertyChangedCallback call back static function.
        /// </summary>
        /// <param name="d">ChildWindow object whose OverlayBrush property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnOverlayBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow cw = (ChildWindow)d;

            if (cw.Overlay != null)
            {
                cw.Overlay.Background = (Brush)e.NewValue;
            }
        }

        /// <summary>
        /// Gets or sets the opacity of the overlay brush that is used to cover
        /// the parent window when the child window is open.
        /// </summary>
        /// <value>
        /// The opacity of the overlay brush that is used to cover the parent
        /// window when the <see cref="ChildWindow" /> is open. The default is 1.0.
        /// </value>
        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OverlayOpacity" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="OverlayOpacity" /> dependency property.
        /// </value>
        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register(
            nameof(OverlayOpacity),
            typeof(double),
            typeof(ChildWindow),
            new PropertyMetadata(OnOverlayOpacityPropertyChanged));

        /// <summary>
        /// OverlayOpacityProperty PropertyChangedCallback call back static function.
        /// </summary>
        /// <param name="d">ChildWindow object whose OverlayOpacity property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnOverlayOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow cw = (ChildWindow)d;

            if (cw.Overlay != null)
            {
                cw.Overlay.Opacity = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Gets the root visual element.
        /// </summary>
        private static Control RootVisual
        {
            get
            {
                return Application.Current == null ? null : (Application.Current.RootVisual as Control);
            }
        }

        /// <summary>
        /// Gets or sets the title that is displayed in the frame of the <see cref="ChildWindow" />.
        /// </summary>
        /// <value>
        /// The title displayed at the top of the window. The default is null.
        /// </value>
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Title" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="Title" /> dependency property.
        /// </value>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
            nameof(Title),
            typeof(object),
            typeof(ChildWindow),
            null);

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChildWindow"/> is modal.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="ChildWindow"/> was open when attempting the modify IsModal.
        /// </exception>
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsModal" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="IsModal" /> dependency property.
        /// </value>
        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register(
                nameof(IsModal),
                typeof(bool),
                typeof(ChildWindow),
                new PropertyMetadata(true, OnIsModalPropertyChanged));

        private static void OnIsModalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow cw = (ChildWindow)d;
            
            if (cw._ignoreIsModalChanged)
            {
                return;
            }

            if (cw.IsOpen)
            {
                cw._ignoreIsModalChanged = true;
                cw.IsModal = (bool)e.OldValue;
                cw._ignoreIsModalChanged = false;

                throw new InvalidOperationException("Cannot change IsModal when ChildWindow is open.");
            }

            cw.UpdateIsModalVisualState();
        }

        /// <summary>
        /// Private accessor for the Chrome.
        /// </summary>
        private FrameworkElement _chrome;

        /// <summary>
        /// Private accessor for the click point on the chrome.
        /// </summary>
        private Point _clickPoint;

        /// <summary>
        /// Private accessor for the Closing storyboard.
        /// </summary>
        private Storyboard _closed;

        /// <summary>
        /// Private accessor for the ContentPresenter.
        /// </summary>
        private FrameworkElement _contentPresenter;

        /// <summary>
        /// Private accessor for the translate transform that needs to be applied on to the ContentRoot.
        /// </summary>
        private TranslateTransform _contentRootTransform;

        /// <summary>
        /// Content area desired width.
        /// </summary>
        private double _desiredContentWidth;

        /// <summary>
        /// Content area desired height.
        /// </summary>
        private double _desiredContentHeight;

        /// <summary>
        /// Desired margin for the window.
        /// </summary>
        private Thickness _desiredMargin;

        /// <summary>
        /// Private accessor for the Dialog Result property.
        /// </summary>
        private bool? _dialogresult;

        /// <summary>
        /// Private accessor for the ChildWindow InteractionState.
        /// </summary>
        private WindowInteractionState _interactionState;

        /// <summary>
        /// Boolean value that specifies whether the application is exit or not.
        /// </summary>
        private bool _isAppExit;

        /// <summary>
        /// Boolean value that specifies whether the window is in closing state or not.
        /// </summary>
        private bool _isClosing;

        /// <summary>
        /// Boolean value that specifies whether the window is opened.
        /// </summary>
        private bool _isOpen;

        /// <summary>
        /// Private accessor for the Opening storyboard.
        /// </summary>
        private Storyboard _opened;

        /// <summary>
        /// Boolean value that specifies whether the mouse is captured or not.
        /// </summary>
        private bool _isMouseCaptured;

        /// <summary>
        /// Boolean value that specifies whether we are listening to RootVisual.GotFocus.
        /// </summary>
        private bool _attachedRootVisualListener;

        /// <summary>
        /// Private accessor for the Root of the window.
        /// </summary>
        private FrameworkElement _root;

        /// <summary>
        /// Private accessor for the position of the window with respect to RootVisual.
        /// </summary>
        private Point _windowPosition;

        /// <summary>
        /// Boolean value specifying if IsModal callback should be ignored or nor.
        /// </summary>
        private bool _ignoreIsModalChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildWindow" /> class.
        /// </summary>
        public ChildWindow()
        {
            this.DefaultStyleKey = typeof(ChildWindow);
            this.InteractionState = WindowInteractionState.NotResponding;
        }

        /// <summary>
        /// Occurs when the <see cref="ChildWindow" /> is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when the <see cref="ChildWindow" /> is closing.
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        /// <summary>
        /// Gets the internal accessor for the ContentRoot of the window.
        /// </summary>
        internal FrameworkElement ContentRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChildWindow" /> was 
        /// accepted or canceled.
        /// </summary>
        /// <value>
        /// True if the child window was accepted; false if the child window was
        /// canceled. The default is null.
        /// </value>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? DialogResult
        {
            get
            {
                return this._dialogresult;
            }

            set
            {
                if (this._dialogresult != value)
                {
                    this._dialogresult = value;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Gets the internal accessor for the PopUp of the window.
        /// </summary>
        internal Popup ChildWindowPopup
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the internal accessor for the close button of the window.
        /// </summary>
        internal ButtonBase CloseButton
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the InteractionState for the ChildWindow.
        /// </summary>
        internal WindowInteractionState InteractionState
        {
            get
            {
                return this._interactionState;
            }
            private set
            {
                if (this._interactionState != value)
                {
                    WindowInteractionState oldValue = this._interactionState;
                    this._interactionState = value;
                    ChildWindowAutomationPeer peer = ChildWindowAutomationPeer.FromElement(this) as ChildWindowAutomationPeer;

                    if (peer != null)
                    {
                        peer.RaiseInteractionStatePropertyChangedEvent(oldValue, this._interactionState);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the PopUp is open or not.
        /// </summary>
        private bool IsOpen
        {
            get
            {
                return (this.ChildWindowPopup != null && this.ChildWindowPopup.IsOpen);
            }
        }

        /// <summary>
        /// Gets the internal accessor for the overlay of the window.
        /// </summary>
        internal Panel Overlay
        {
            get;
            private set;
        }

        /// <summary>
        /// Inverts the input matrix.
        /// </summary>
        /// <param name="matrix">The matrix values that is to be inverted.</param>
        /// <returns>Returns a value indicating whether the inversion was successful or not.</returns>
        private static bool InvertMatrix(ref Matrix matrix)
        {
            double determinant = (matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21);

            if (determinant == 0.0)
            {
                return false;
            }

            Matrix matCopy = matrix;
            matrix.M11 = matCopy.M22 / determinant;
            matrix.M12 = -1 * matCopy.M12 / determinant;
            matrix.M21 = -1 * matCopy.M21 / determinant;
            matrix.M22 = matCopy.M11 / determinant;
            matrix.OffsetX = ((matCopy.OffsetY * matCopy.M21) - (matCopy.OffsetX * matCopy.M22)) / determinant;
            matrix.OffsetY = ((matCopy.OffsetX * matCopy.M12) - (matCopy.OffsetY * matCopy.M11)) / determinant;

            return true;
        }

        /// <summary>
        /// Executed when the application is exited.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event args.</param>
        internal void Application_Exit(object sender, EventArgs e)
        {
            if (this.IsOpen)
            {
                this._isAppExit = true;
                try
                {
                    this.Close();
                }
                finally
                {
                    this._isAppExit = false;
                }
            }
        }

        /// <summary>
        /// Changes the visual state of the ChildWindow.
        /// </summary>
        private void ChangeVisualState()
        {
            if (this._isClosing)
            {
                VisualStateManager.GoToState(this, VSMSTATE_StateClosed, true);
            }
            else
            {
                VisualStateManager.GoToState(this, VSMSTATE_StateOpen, true);
            }
        }

        /// <summary>
        /// Change visual state of the ModalStates group.
        /// </summary>
        private void UpdateIsModalVisualState()
        {
            if (IsModal)
            {
                VisualStateManager.GoToState(this, VSMState_StateModal, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VSMState_StateNotModal, false);
            }
        }

        /// <summary>
        /// Executed when ChildWindow size is changed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Size changed event args.</param>
        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Overlay != null)
            {
                if (e.NewSize.Height != this.Overlay.ActualHeight)
                {
                    this._desiredContentHeight = e.NewSize.Height;
                }

                if (e.NewSize.Width != this.Overlay.ActualWidth)
                {
                    this._desiredContentWidth = e.NewSize.Width;
                }
            }

            if (this.IsOpen)
            {
                this.UpdateOverlaySize();
            }
        }

        /// <summary>
        /// Closes a <see cref="ChildWindow" />.
        /// </summary>
        public void Close()
        {
            // AutomationPeer returns "Closing" when Close() is called
            // but the window is not closed completely:
            this.InteractionState = WindowInteractionState.Closing;
            CancelEventArgs e = new CancelEventArgs();
            this.OnClosing(e);

            // On ApplicationExit, close() cannot be cancelled
            if (!e.Cancel || this._isAppExit)
            {
                if (RootVisual != null && this.IsOpen && this.IsModal)
                {
                    --OpenChildWindowCount;
                    if (OpenChildWindowCount == 0)
                    {
                        // Restore the value saved when the first window was opened
                        RootVisual.IsEnabled = RootVisual_PrevEnabledState;
                    }
                }

                // Close Popup
                if (this.IsOpen)
                {
                    if (this._closed != null)
                    {
                        // Popup will be closed when the storyboard ends
                        this._isClosing = true;
                        try
                        {
                            this.ChangeVisualState();
                        }
                        finally
                        {
                            this._isClosing = false;
                        }
                    }
                    else
                    {
                        // If no closing storyboard is defined, close the Popup
                        this.ChildWindowPopup.IsOpen = false;
                    }

                    if (!this._dialogresult.HasValue)
                    {
                        // If close action is not happening because of DialogResult property change action,
                        // Dialogresult is always false:
                        this._dialogresult = false;
                    }

                    this.OnClosed(EventArgs.Empty);
                    this.UnSubscribeFromEvents();
                    this.UnsubscribeFromTemplatePartEvents();

                    if (Application.Current.RootVisual != null)
                    {
                        Application.Current.RootVisual.GotFocus -= new RoutedEventHandler(this.RootVisual_GotFocus);
                        _attachedRootVisualListener = false;
                    }
                }
            }
            else
            {
                // If the Close is cancelled, DialogResult should always be NULL:
                this._dialogresult = null;
                this.InteractionState = WindowInteractionState.Running;
            }
        }

        /// <summary>
        /// Executed when the CloseButton is clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        internal void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Executed when the Closing storyboard ends.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Closing_Completed(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                this.ChildWindowPopup.IsOpen = false;
            }

            // AutomationPeer returns "NotResponding" when the ChildWindow is closed:
            this.InteractionState = WindowInteractionState.NotResponding;

            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }
        }

        /// <summary>
        /// Executed when the a key is presses when the window is open.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Key event args.</param>
        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            ChildWindow ew = sender as ChildWindow;
            Debug.Assert(ew != null, "ChildWindow instance is null.");

            // Ctrl+Shift+F4 closes the ChildWindow
            if (e != null && !e.Handled && e.Key == Key.F4 &&
                ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) &&
                ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
            {
                ew.Close();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Executed when the window loses focus.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        private void ChildWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            // If the ChildWindow loses focus but the popup is still open,
            // it means another popup is opened. To get the focus back when the
            // popup is closed, we handle GotFocus on the RootVisual
            // TODO: Something else could get focus and handle the GotFocus event right.  
            // Try listening to routed events that were Handled (new SL 3 feature)
            // Blocked by Jolt bug #29419
            if (this.IsOpen && Application.Current != null && Application.Current.RootVisual != null)
            {
                this.InteractionState = WindowInteractionState.BlockedByModalWindow;
                if (!_attachedRootVisualListener)
                {
                    Application.Current.RootVisual.GotFocus += new RoutedEventHandler(this.RootVisual_GotFocus);
                    _attachedRootVisualListener = true;
                }
            }
        }

        /// <summary>
        /// Executed when mouse left button is down on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse button event args.</param>
        private void Chrome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this._chrome != null)
            {
                e.Handled = true;

                if (this.CloseButton != null && !this.CloseButton.IsTabStop)
                {
                    this.CloseButton.IsTabStop = true;
                    try
                    {
                        this.Focus();
                    }
                    finally
                    {
                        this.CloseButton.IsTabStop = false;
                    }
                }
                else
                {
                    this.Focus();
                }
#if MIGRATION
                this._chrome.CaptureMouse();
#else
                this._chrome.CapturePointer();
#endif
                this._isMouseCaptured = true;
                this._clickPoint = e.GetPosition(sender as UIElement);
            }
        }

        /// <summary>
        /// Executed when mouse left button is up on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse button event args.</param>
        private void Chrome_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this._chrome != null)
            {
                e.Handled = true;
#if MIGRATION
                this._chrome.ReleaseMouseCapture();
#else
                this._chrome.ReleasePointerCapture();
#endif
                this._isMouseCaptured = false;
            }
        }

        /// <summary>
        /// Executed when mouse moves on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse event args.</param>
        private void Chrome_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._isMouseCaptured && this.ContentRoot != null && Application.Current != null && Application.Current.RootVisual != null)
            {
                Point position = e.GetPosition(Application.Current.RootVisual);

                GeneralTransform gt = this.ContentRoot.TransformToVisual(Application.Current.RootVisual);

                if (gt != null)
                {
                    Point p = gt.Transform(this._clickPoint);
                    this._windowPosition = gt.Transform(new Point(0, 0));

                    if (position.X < 0)
                    {
                        double Y = FindPositionY(p, position, 0);
                        position = new Point(0, Y);
                    }

                    if (position.X > this.Width)
                    {
                        double Y = FindPositionY(p, position, this.Width);
                        position = new Point(this.Width, Y);
                    }

                    if (position.Y < 0)
                    {
                        double X = FindPositionX(p, position, 0);
                        position = new Point(X, 0);
                    }

                    if (position.Y > this.Height)
                    {
                        double X = FindPositionX(p, position, this.Height);
                        position = new Point(X, this.Height);
                    }

                    double x = position.X - p.X;
                    double y = position.Y - p.Y;

                    // Take potential RightToLeft layout into account
                    FrameworkElement fe = Application.Current.RootVisual as FrameworkElement;
                    if (fe != null && fe.FlowDirection == FlowDirection.RightToLeft)
                    {
                        x = -x;
                    }

                    UpdateContentRootTransform(x, y);
                }
            }
        }

        /// <summary>
        /// Executed when the ContentPresenter size changes.
        /// </summary>
        /// <param name="sender">Content Presenter object.</param>
        /// <param name="e">SizeChanged event args.</param>
        private void ContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ContentRoot != null && Application.Current != null && Application.Current.RootVisual != null && _isOpen)
            {
                GeneralTransform gt = this.ContentRoot.TransformToVisual(Application.Current.RootVisual);

                if (gt != null)
                {
                    Point p = gt.Transform(new Point(0, 0));

                    double x = this._windowPosition.X - p.X;
                    double y = this._windowPosition.Y - p.Y;
                    UpdateContentRootTransform(x, y);
                }
            }

            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(0, 0, this._contentPresenter.ActualWidth, this._contentPresenter.ActualHeight);
            this._contentPresenter.Clip = rg;
            this.UpdatePosition();
        }

        /// <summary>
        /// Finds the X coordinate of a point that is defined by a line.
        /// </summary>
        /// <param name="p1">Starting point of the line.</param>
        /// <param name="p2">Ending point of the line.</param>
        /// <param name="y">Y coordinate of the point.</param>
        /// <returns>X coordinate of the point.</returns>
        private static double FindPositionX(Point p1, Point p2, double y)
        {
            if (y == p1.Y || p1.X == p2.X)
            {
                return p2.X;
            }

            Debug.Assert(p1.Y != p2.Y, "Unexpected equal Y coordinates");

            return (((y - p1.Y) * (p1.X - p2.X)) / (p1.Y - p2.Y)) + p1.X;
        }

        /// <summary>
        /// Finds the Y coordinate of a point that is defined by a line.
        /// </summary>
        /// <param name="p1">Starting point of the line.</param>
        /// <param name="p2">Ending point of the line.</param>
        /// <param name="x">X coordinate of the point.</param>
        /// <returns>Y coordinate of the point.</returns>
        private static double FindPositionY(Point p1, Point p2, double x)
        {
            if (p1.Y == p2.Y || x == p1.X)
            {
                return p2.Y;
            }

            Debug.Assert(p1.X != p2.X, "Unexpected equal X coordinates");

            return (((p1.Y - p2.Y) * (x - p1.X)) / (p1.X - p2.X)) + p1.Y;
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="ChildWindow" /> control when a
        /// new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            this.UnsubscribeFromTemplatePartEvents();

            base.OnApplyTemplate();

            this.CloseButton = GetTemplateChild(PART_CloseButton) as ButtonBase;

            if (this.CloseButton != null)
            {
                if (this.HasCloseButton)
                {
                    this.CloseButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.CloseButton.Visibility = Visibility.Collapsed;
                }
            }

            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }

            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }

            this._root = GetTemplateChild(PART_Root) as FrameworkElement;

            if (this._root != null)
            {
                IList groups = VisualStateManager.GetVisualStateGroups(this._root);

                if (groups != null)
                {
                    IList states = null;

                    foreach (VisualStateGroup vsg in groups)
                    {
                        if (vsg.Name == ChildWindow.VSMGROUP_Window)
                        {
                            states = vsg.States;
                            break;
                        }
                    }

                    if (states != null)
                    {
                        foreach (VisualState state in states)
                        {
                            if (state.Name == ChildWindow.VSMSTATE_StateClosed)
                            {
                                this._closed = state.Storyboard;
                            }

                            if (state.Name == ChildWindow.VSMSTATE_StateOpen)
                            {
                                this._opened = state.Storyboard;
                            }
                        }
                    }
                }
            }

            this.ContentRoot = GetTemplateChild(PART_ContentRoot) as FrameworkElement;

            this._chrome = GetTemplateChild(PART_Chrome) as FrameworkElement;

            this.Overlay = GetTemplateChild(PART_Overlay) as Panel;

            this._contentPresenter = GetTemplateChild(PART_ContentPresenter) as FrameworkElement;

            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();
            this._desiredMargin = this.Margin;
            this.Margin = new Thickness(0);

            // Update overlay size
            if (this.IsOpen)
            {
                this._desiredContentHeight = this.Height;
                this._desiredContentWidth = this.Width;
                this.UpdateOverlaySize();
                this.UpdateRenderTransform();
                this.ChangeVisualState();
                this.UpdateIsModalVisualState();
            }
        }

        /// <summary>
        /// Raises the <see cref="Closed" /> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnClosed(EventArgs e)
        {
            EventHandler handler = this.Closed;

            if (null != handler)
            {
                handler(this, e);
            }

            this._isOpen = false;
        }

        /// <summary>
        /// Raises the <see cref="Closing" /> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.Closing;

            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Returns a <see cref="ChildWindowAutomationPeer" /> for use by the 
        /// Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// <see cref="ChildWindowAutomationPeer" /> for the <see cref="ChildWindow" /> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChildWindowAutomationPeer(this);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (!this.IsModal && this.ChildWindowPopup != null)
            {
                this.ChildWindowPopup.PutPopupInFront();
            }
        }

        /// <summary>
        /// This method is called every time a <see cref="ChildWindow" /> is displayed.
        /// </summary>
        protected virtual void OnOpened()
        {
            this.UpdatePosition();
            this._isOpen = true;

            if (this.Overlay != null)
            {
                this.Overlay.Opacity = this.OverlayOpacity;
                this.Overlay.Background = this.OverlayBrush;
            }

            if (!this.Focus())
            {
                // If the Focus() fails it means there is no focusable element in the 
                // ChildWindow. In this case we set IsTabStop to true to have the keyboard functionality
                this.IsTabStop = true;
                this.Focus();
            }
        }

        /// <summary>
        /// Executed when the opening storyboard finishes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Opening_Completed(object sender, EventArgs e)
        {
            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }
            // AutomationPeer returns "ReadyForUserInteraction" when the ChildWindow 
            // is open and all animations have been completed.
            this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
            this.OnOpened();
        }

        /// <summary>
        /// Executed when the page resizes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Page_Resized(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                this.UpdateOverlaySize();
            }
        }

        /// <summary>
        /// Executed when the root visual gets focus.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        private void RootVisual_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
        }

        /// <summary>
        /// Opens a <see cref="T:System.Windows.Controls.ChildWindow" /> and
        /// returns without waiting for the
        /// <see cref="T:System.Windows.Controls.ChildWindow" /> to close.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The child window is already in the visual tree.
        /// </exception>
        public void Show()
        {
            // AutomationPeer returns "Running" when Show() is called
            // but the ChildWindow is not ready for user interaction:
            this.InteractionState = WindowInteractionState.Running;

            this.SubscribeToEvents();
            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();

            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = new Popup();

                try
                {
                    this.ChildWindowPopup.Child = this;
                }
                catch (ArgumentException)
                {
                    // If the ChildWindow is already in the visualtree, we cannot set it to be the child of the popup
                    // we are throwing a friendlier exception for this case:
                    this.InteractionState = WindowInteractionState.NotResponding;
                    throw new InvalidOperationException("Cannot call Show() on a ChildWindow that is in the visual tree. ChildWindow should be the top-most element in the .xaml file.");
                }
            }

            // MaxHeight and MinHeight properties should not be overwritten:
            this.MaxHeight = double.PositiveInfinity;
            this.MaxWidth = double.PositiveInfinity;

            // disable the underlying UI
            if (RootVisual != null && !this.IsOpen && this.IsModal)
            {
                if (OpenChildWindowCount == 0)
                {
                    // Save current value to restore it upon closing the last window
                    RootVisual_PrevEnabledState = RootVisual.IsEnabled;
                }
                ++OpenChildWindowCount;
                RootVisual.IsEnabled = false;
            }

            if (this.ChildWindowPopup != null && Application.Current.RootVisual != null)
            {
                this.ChildWindowPopup.IsOpen = true;

                // while the ChildWindow is open, the DialogResult is always NULL:
                this._dialogresult = null;
            }

            // if the template is already loaded, display loading visuals animation
            if (this.ContentRoot != null)
            {
                this.ChangeVisualState();
            }
        }

        /// <summary>
        /// Opens a ChildWindow and waits for the ChildWindow to close.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The child window is already in the visual tree.
        /// </exception>
        public Task ShowAndWait()
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            this.Closed += (s, e) => // We do not unsubscribe this event, but we set "taskCompletionSource" to null on closed.
            {
                if (taskCompletionSource != null)
                {
                    taskCompletionSource.SetResult(null);
                    taskCompletionSource = null;
                }
            };
            this.Show();
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Subscribes to events when the ChildWindow is opened.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit += new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized += new EventHandler(this.Page_Resized);
            }

            this.KeyDown += new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus += new RoutedEventHandler(this.ChildWindow_LostFocus);
            this.SizeChanged += new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
        }

        /// <summary>
        /// Subscribes to events that are on the storyboards. 
        /// Unsubscribing from these events happen in the event handlers individually.
        /// </summary>
        private void SubscribeToStoryBoardEvents()
        {
            if (this._closed != null)
            {
                this._closed.Completed += new EventHandler(this.Closing_Completed);
            }

            if (this._opened != null)
            {
                this._opened.Completed += new EventHandler(this.Opening_Completed);
            }
        }

        /// <summary>
        /// Subscribes to events on the template parts.
        /// </summary>
        private void SubscribeToTemplatePartEvents()
        {
            if (this.CloseButton != null)
            {
                this.CloseButton.Click += new RoutedEventHandler(this.CloseButton_Click);
            }

            if (this._chrome != null)
            {
#if MIGRATION
                this._chrome.MouseLeftButtonDown += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.MouseLeftButtonUp += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.MouseMove += new MouseEventHandler(this.Chrome_MouseMove);
#else
                this._chrome.PointerPressed += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.PointerReleased += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.PointerMoved += new MouseEventHandler(this.Chrome_MouseMove);
#endif
            }

            if (this._contentPresenter != null)
            {
                this._contentPresenter.SizeChanged += new SizeChangedEventHandler(this.ContentPresenter_SizeChanged);
            }
        }

        /// <summary>
        /// Unsubscribe from events when the ChildWindow is closed.
        /// </summary>
        private void UnSubscribeFromEvents()
        {
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit -= new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized -= new EventHandler(this.Page_Resized);
            }

            this.KeyDown -= new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus -= new RoutedEventHandler(this.ChildWindow_LostFocus);
            this.SizeChanged -= new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
        }

        /// <summary>
        /// Unsubscribe from the events that are subscribed on the template part elements.
        /// </summary>
        private void UnsubscribeFromTemplatePartEvents()
        {
            if (this.CloseButton != null)
            {
                this.CloseButton.Click -= new RoutedEventHandler(this.CloseButton_Click);
            }

            if (this._chrome != null)
            {
#if MIGRATION
                this._chrome.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.MouseMove -= new MouseEventHandler(this.Chrome_MouseMove);
#else
                this._chrome.PointerPressed -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.PointerReleased -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.PointerMoved -= new MouseEventHandler(this.Chrome_MouseMove);
#endif
            }

            if (this._contentPresenter != null)
            {
                this._contentPresenter.SizeChanged -= new SizeChangedEventHandler(this.ContentPresenter_SizeChanged);
            }
        }

        /// <summary>
        /// Updates the size of the overlay of the window.
        /// </summary>
        private void UpdateOverlaySize()
        {
            if (this.Overlay != null && Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                this.Height = Application.Current.Host.Content.ActualHeight;
                this.Width = Application.Current.Host.Content.ActualWidth;

                this.Overlay.Height = this.Height;
                this.Overlay.Width = this.Width;

                if (this.ContentRoot != null)
                {
                    this.ContentRoot.Width = this._desiredContentWidth;
                    this.ContentRoot.Height = this._desiredContentHeight;
                    this.ContentRoot.Margin = this._desiredMargin;
                }
            }
        }

        /// <summary>
        /// Updates the position of the window in case the size of the content changes.
        /// This allows ChildWindow only scale from right and bottom.
        /// </summary>
        private void UpdatePosition()
        {
            if (this.ContentRoot != null && Application.Current != null && Application.Current.RootVisual != null)
            {
                GeneralTransform gt = this.ContentRoot.TransformToVisual(Application.Current.RootVisual);

                if (gt != null)
                {
                    this._windowPosition = gt.Transform(new Point(0, 0));
                }
            }
        }

        /// <summary>
        /// Updates the render transform applied on the overlay.
        /// </summary>
        private void UpdateRenderTransform()
        {
            if (this._root != null && this.ContentRoot != null)
            {
                // The Overlay part should not be affected by the render transform applied on the
                // ChildWindow. In order to achieve this, we adjust an identity matrix to represent
                // the _root's transformation, invert it, apply the inverted matrix on the _root, so that 
                // nothing is affected by the rendertransform, and apply the original transform only on the Content
                GeneralTransform gt = this._root.TransformToVisual(null);
                if (gt != null)
                {
                    Point p10 = new Point(1, 0);
                    Point p01 = new Point(0, 1);
                    Point transform10 = gt.Transform(p10);
                    Point transform01 = gt.Transform(p01);

                    Matrix transformToRootMatrix = Matrix.Identity;
                    transformToRootMatrix.M11 = transform10.X;
                    transformToRootMatrix.M12 = transform10.Y;
                    transformToRootMatrix.M21 = transform01.X;
                    transformToRootMatrix.M22 = transform01.Y;

                    MatrixTransform original = new MatrixTransform();
                    original.Matrix = transformToRootMatrix;

                    InvertMatrix(ref transformToRootMatrix);
                    MatrixTransform mt = new MatrixTransform();
                    mt.Matrix = transformToRootMatrix;

                    TransformGroup tg = this._root.RenderTransform as TransformGroup;

                    if (tg != null)
                    {
                        tg.Children.Add(mt);
                    }
                    else
                    {
                        this._root.RenderTransform = mt;
                    }

                    tg = this.ContentRoot.RenderTransform as TransformGroup;

                    if (tg != null)
                    {
                        tg.Children.Add(original);
                    }
                    else
                    {
                        this.ContentRoot.RenderTransform = original;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the ContentRootTranslateTransform.
        /// </summary>
        /// <param name="X">X coordinate of the transform.</param>
        /// <param name="Y">Y coordinate of the transform.</param>
        private void UpdateContentRootTransform(double X, double Y)
        {
            if (this._contentRootTransform == null)
            {
                this._contentRootTransform = new TranslateTransform();
                this._contentRootTransform.X = X;
                this._contentRootTransform.Y = Y;

                TransformGroup transformGroup = this.ContentRoot.RenderTransform as TransformGroup;

                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(this.ContentRoot.RenderTransform);
                }
                transformGroup.Children.Add(this._contentRootTransform);
                this.ContentRoot.RenderTransform = transformGroup;
            }
            else
            {
                this._contentRootTransform.X += X;
                this._contentRootTransform.Y += Y;
            }
        }
    }
}