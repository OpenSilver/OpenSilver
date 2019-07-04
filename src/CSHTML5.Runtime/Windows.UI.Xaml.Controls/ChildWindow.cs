
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



// Style and Template: https://msdn.microsoft.com/fr-fr/library/dd833070(v=vs.95).aspx


using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
#else
using Windows.UI.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Input;
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
#if SILVERLIGHT
    [TemplatePart(Name = PART_Chrome, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ContentRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Overlay, Type = typeof(Panel))]
    [TemplatePart(Name = PART_Root, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VSMSTATE_StateClosed, GroupName = VSMGROUP_Window)]
    [TemplateVisualState(Name = VSMSTATE_StateOpen, GroupName = VSMGROUP_Window)]
#endif
    public class ChildWindow : ContentControl
    {
        #region Static Fields and Constants

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

        #region public bool HasCloseButton

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// ChildWindow has a close
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
        /// Identifies the HasCloseButton
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the HasCloseButton
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty HasCloseButtonProperty =
            DependencyProperty.Register(
            "HasCloseButton",
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

        #endregion public bool HasCloseButton

        #region public Brush OverlayBrush

        /// <summary>
        /// Gets or sets the visual brush that is used to cover the parent
        /// window when the child window is open.
        /// </summary>
        /// <value>
        /// The visual brush that is used to cover the parent window when the
        /// ChildWindow is open. The
        /// default is null.
        /// </value>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the OverlayBrush
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OverlayBrush
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register(
            "OverlayBrush",
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

        #endregion public Brush OverlayBrush

        #region public double OverlayOpacity

        /// <summary>
        /// Gets or sets the opacity of the overlay brush that is used to cover
        /// the parent window when the child window is open.
        /// </summary>
        /// <value>
        /// The opacity of the overlay brush that is used to cover the parent
        /// window when the ChildWindow
        /// is open. The default is 1.0.
        /// </value>
        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        /// <summary>
        /// Identifies the OverlayOpacity
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OverlayOpacity
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register(
            "OverlayOpacity",
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

        #endregion public double OverlayOpacity

        #region private static Control RootVisual

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

        #endregion private static Control RootVisual

        #region public object Title

        /// <summary>
        /// Gets or sets the title that is displayed in the frame of the
        /// ChildWindow.
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
        /// Identifies the ChildWindow.Title" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the ChildWindow
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
            "Title",
            typeof(object),
            typeof(ChildWindow),
            null);

        #endregion public object Title

        #endregion Static Fields and Constants

        #region Member Fields

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

#if !SILVERLIGHT
        /// <summary>
        /// The width that was specified when the ChildWindow was created.
        /// </summary>
        private double _widthThatWasInitiallySpecified;

        /// <summary>
        /// The height that was specified when the ChildWindow was created.
        /// </summary>
        private double _heightThatWasInitiallySpecified;
#endif

        /// <summary>
        /// Desired margin for the window.
        /// </summary>
        private Thickness _desiredMargin;

        /// <summary>
        /// Private accessor for the Dialog Result property.
        /// </summary>
        private bool? _dialogresult;

#if SILVERLIGHT
        /// <summary>
        /// Private accessor for the ChildWindow InteractionState.
        /// </summary>
        private WindowInteractionState _interactionState;
#endif

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
        /// Private accessor for the Root of the window.
        /// </summary>
        private FrameworkElement _root;

        /// <summary>
        /// Private accessor for the position of the window with respect to RootVisual.
        /// </summary>
        private Point _windowPosition;

        #endregion Member Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// ChildWindow class.
        /// </summary>
        public ChildWindow()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(ChildWindow);
            this.InteractionState = WindowInteractionState.NotResponding;
#else
            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(ChildWindow);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultChildWindowStyle.GetDefaultStyle());
#endif
#endif
        }

#endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the ChildWindow
        /// is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when the ChildWindow
        /// is closing.
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the internal accessor for the ContentRoot of the window.
        /// </summary>
        internal FrameworkElement ContentRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// ChildWindow was accepted or
        /// canceled.
        /// </summary>
        /// <value>
        /// True if the child window was accepted; false if the child window was
        /// canceled. The default is null.
        /// </value>
#if SILVERLIGHT
        [TypeConverter(typeof(NullableBoolConverter))]
#endif
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

#if SILVERLIGHT
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
#endif

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

        #endregion Properties

        #region Static Methods

#if SILVERLIGHT
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
#endif

        #endregion Static Methods

        #region Methods

        /// <summary>
        /// Executed when mouse moves on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse event args.</param>
#if MIGRATION
        private void Chrome_MouseMove(object sender, MouseEventArgs e)
#else
        private void Chrome_MouseMove(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (this._isMouseCaptured)
            {
                Point posNow = new Point(e._pointerAbsoluteX, e._pointerAbsoluteY);

                Point delta = new Point(posNow.X - this._clickPoint.X, posNow.Y - this._clickPoint.Y);

                if (this._contentRootTransform == null)
                {
                    this._contentRootTransform = new TranslateTransform();
                    this._contentRootTransform.X = delta.X;
                    this._contentRootTransform.Y = delta.Y;
                }
                else
                {
                    this._contentRootTransform.X += delta.X;
                    this._contentRootTransform.Y += delta.Y;
                }

                this.ContentRoot.RenderTransform = this._contentRootTransform;

                this._clickPoint = posNow;
            }
        }

#if MIGRATION
        private void Chrome_PointerPressed(object sender, MouseEventArgs e)
#else
        private void Chrome_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            this._clickPoint = new Point(e._pointerAbsoluteX, e._pointerAbsoluteY);

            //this.Focus();

#if MIGRATION
            this._chrome.CaptureMouse();
#else
            this._chrome.CapturePointer();
#endif

            this._isMouseCaptured = true;
        }

#if MIGRATION
        private void Chrome_PointerReleased(object sender, MouseEventArgs e)
#else
        private void Chrome_PointerReleased(object sender, PointerRoutedEventArgs e)
#endif
        {
            this._clickPoint = new Point(0, 0);

#if MIGRATION
            this._chrome.ReleaseMouseCapture();
#else
            this._chrome.ReleasePointerCapture();
#endif

            this._isMouseCaptured = false;
        }

        private void RegisterEvents()
        {
            if (_chrome != null)
            {
#if MIGRATION
                _chrome.MouseMove -= Chrome_MouseMove;
                _chrome.MouseMove += Chrome_MouseMove;
                _chrome.MouseLeftButtonUp -= Chrome_PointerReleased;
                _chrome.MouseLeftButtonUp += Chrome_PointerReleased;
                _chrome.MouseLeftButtonDown -= Chrome_PointerPressed;
                _chrome.MouseLeftButtonDown += Chrome_PointerPressed;
#else
                _chrome.PointerMoved -= Chrome_MouseMove;
                _chrome.PointerMoved += Chrome_MouseMove;
                _chrome.PointerReleased -= Chrome_PointerReleased;
                _chrome.PointerReleased += Chrome_PointerReleased;
                _chrome.PointerPressed -= Chrome_PointerPressed;
                _chrome.PointerPressed += Chrome_PointerPressed;
#endif
            }
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
        /// Executed when ChildWindow size is changed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Size changed event args.</param>
        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Overlay != null)
            {
                if (e.NewSize.Height != this.Overlay.Height)
                {
                    this._desiredContentHeight = e.NewSize.Height;
                }

                if (e.NewSize.Width != this.Overlay.Width)
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
        /// Closes a ChildWindow.
        /// </summary>
        public void Close()
        {
#if SILVERLIGHT
            // AutomationPeer returns "Closing" when Close() is called
            // but the window is not closed completely:
            this.InteractionState = WindowInteractionState.Closing;
#endif
            CancelEventArgs e = new CancelEventArgs();
            this.OnClosing(e);

            // On ApplicationExit, close() cannot be cancelled
            if (!e.Cancel || this._isAppExit)
            {
                if (RootVisual != null)
                {
                    RootVisual.IsEnabled = true;
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
                    }
                }
            }
            else
            {
                // If the Close is cancelled, DialogResult should always be NULL:
                this._dialogresult = null;
#if SILVERLIGHT
                this.InteractionState = WindowInteractionState.Running;
#endif
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

#if SILVERLIGHT
            // AutomationPeer returns "NotResponding" when the ChildWindow is closed:
            this.InteractionState = WindowInteractionState.NotResponding;
#endif

            if (this._closed != null)
            {
#if SILVERLIGHT
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
#endif
            }
        }

#if SILVERLIGHT
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
#endif

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
#if SILVERLIGHT
                this.InteractionState = WindowInteractionState.BlockedByModalWindow;
#endif
                Application.Current.RootVisual.GotFocus += new RoutedEventHandler(this.RootVisual_GotFocus);
            }
        }

#if SILVERLIGHT
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
                this._chrome.CaptureMouse();
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
                this._chrome.ReleaseMouseCapture();
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
#endif

        /// <summary>
        /// Builds the visual tree for the
        /// ChildWindow control when a
        /// new template is applied.
        /// </summary>
#if SILVERLIGHT
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No need to split the code into two parts.")]
#endif
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
#if SILVERLIGHT
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
#endif
            }

            if (this._opened != null)
            {
#if SILVERLIGHT
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
#endif
            }

            this._root = GetTemplateChild(PART_Root) as FrameworkElement;

            if (this._root != null)
            {
#if SILVERLIGHT
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
#endif
            }

            this.ContentRoot = GetTemplateChild(PART_ContentRoot) as FrameworkElement;

            this._chrome = GetTemplateChild(PART_Chrome) as FrameworkElement;

            this.Overlay = GetTemplateChild(PART_Overlay) as Panel;

            this._contentPresenter = GetTemplateChild(PART_ContentPresenter) as FrameworkElement;

            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();
            this._desiredMargin = this.Margin;
            this.Margin = new Thickness(0);

            //this.ContentRoot.MaxHeight = 600;// like in chrome and firefox
            //this.ContentRoot.MaxWidth = 800;

            // Update overlay size
            if (this.IsOpen)
            {
                this._desiredContentHeight = this.Height;
                this._desiredContentWidth = this.Width;
#if !SILVERLIGHT
                this._heightThatWasInitiallySpecified = this.Height;
                this._widthThatWasInitiallySpecified = this.Width;
#endif
                this.UpdateOverlaySize();
                this.UpdateRenderTransform();
                this.ChangeVisualState();
            }

            RegisterEvents();
        }

        /// <summary>
        /// Raises the ChildWindow.Closed event.
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
        /// Raises the ChildWindow.Closing event.
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

#if SILVERLIGHT
        /// <summary>
        /// Returns a ChildWindowAutomationPeer
        /// for use by the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// Peers.ChildWindowAutomationPeer
        /// for the ChildWindow object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChildWindowAutomationPeer(this);
        }
#endif

        /// <summary>
        /// This method is called every time a
        /// ChildWindow is displayed.
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

#if SILVERLIGHT
            if (!this.Focus())
            {
                // If the Focus() fails it means there is no focusable element in the 
                // ChildWindow. In this case we set IsTabStop to true to have the keyboard functionality
                this.IsTabStop = true;
                this.Focus();
            }
#endif
        }

#if SILVERLIGHT
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
#endif

        /// <summary>
        /// Executed when the page resizes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Page_Resized(object sender, WindowSizeChangedEventArgs e)
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
#if SILVERLIGHT
            this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
#endif
        }

        /// <summary>
        /// Opens a ChildWindow and
        /// returns without waiting for the
        /// ChildWindow to close.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The child window is already in the visual tree.
        /// </exception>
        public void Show()
        {
#if SILVERLIGHT
            // AutomationPeer returns "Running" when Show() is called
            // but the ChildWindow is not ready for user interaction:
            this.InteractionState = WindowInteractionState.Running;
#endif

            this.SubscribeToEvents();
            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();

            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = new Popup();

#if !SILVERLIGHT
                // CSHTML5 makes it easier to have a full-screen popup by setting the following CSHTML5-specific properties to "Stretch":
                this.ChildWindowPopup.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                this.ChildWindowPopup.VerticalContentAlignment = VerticalAlignment.Stretch;
#endif
                try
                {
                    this.ChildWindowPopup.Child = this;
                }
                catch (ArgumentException)
                {
                    // If the ChildWindow is already in the visualtree, we cannot set it to be the child of the popup
                    // we are throwing a friendlier exception for this case:
#if SILVERLIGHT
                    this.InteractionState = WindowInteractionState.NotResponding;
                    throw new InvalidOperationException(Properties.Resources.ChildWindow_InvalidOperation);
#else
                    throw;
#endif
                }
            }

#if SILVERLIGHT
            // MaxHeight and MinHeight properties should not be overwritten:
            this.MaxHeight = double.PositiveInfinity;
            this.MaxWidth = double.PositiveInfinity;
#endif

            if (this.ChildWindowPopup != null)
            {
                this.ChildWindowPopup.IsOpen = true;

                // while the ChildWindow is open, the DialogResult is always NULL:
                this._dialogresult = null;
            }

            // disable the underlying UI
            if (RootVisual != null)
            {
                RootVisual.IsEnabled = false;
            }

            // if the template is already loaded, display loading visuals animation
            if (this.ContentRoot != null)
            {
                this.ChangeVisualState();
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Opens a ChildWindow and waits for the ChildWindow to close.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
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
#endif

        /// <summary>
        /// Subscribes to events when the ChildWindow is opened.
        /// </summary>
        private void SubscribeToEvents()
        {
#if SILVERLIGHT
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit += new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized += new EventHandler(this.Page_Resized);
            }

            this.KeyDown += new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus += new RoutedEventHandler(this.ChildWindow_LostFocus);
            this.SizeChanged += new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
#else
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit += new EventHandler(this.Application_Exit);
                Window.Current.SizeChanged += new WindowSizeChangedEventHandler(this.Page_Resized);
            }

            this.SizeChanged += new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
#endif
        }

        /// <summary>
        /// Subscribes to events that are on the storyboards. 
        /// Unsubscribing from these events happen in the event handlers individually.
        /// </summary>
        private void SubscribeToStoryBoardEvents()
        {
            if (this._closed != null)
            {
#if SILVERLIGHT
                this._closed.Completed += new EventHandler(this.Closing_Completed);
#endif
            }

            if (this._opened != null)
            {
#if SILVERLIGHT
                this._opened.Completed += new EventHandler(this.Opening_Completed);
#endif
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
#if SILVERLIGHT
                this._chrome.MouseLeftButtonDown += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.MouseLeftButtonUp += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.MouseMove += new MouseEventHandler(this.Chrome_MouseMove);
#endif
            }

            if (this._contentPresenter != null)
            {
#if SILVERLIGHT
                this._contentPresenter.SizeChanged += new SizeChangedEventHandler(this.ContentPresenter_SizeChanged);
#endif
            }
        }

        /// <summary>
        /// Unsubscribe from events when the ChildWindow is closed.
        /// </summary>
        private void UnSubscribeFromEvents()
        {
#if SILVERLIGHT
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit -= new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized -= new EventHandler(this.Page_Resized);
            }

            this.KeyDown -= new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus -= new RoutedEventHandler(this.ChildWindow_LostFocus);
            this.SizeChanged -= new SizeChangedEventHandler(this.ChildWindow_SizeChanged);

#else
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit -= new EventHandler(this.Application_Exit);
                Window.Current.SizeChanged -= new WindowSizeChangedEventHandler(this.Page_Resized);
            }

            this.SizeChanged -= new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
#endif
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
#if SILVERLIGHT
                this._chrome.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.MouseMove -= new MouseEventHandler(this.Chrome_MouseMove);
#endif
            }

            if (this._contentPresenter != null)
            {
#if SILVERLIGHT
                this._contentPresenter.SizeChanged -= new SizeChangedEventHandler(this.ContentPresenter_SizeChanged);
#endif
            }
        }

        /// <summary>
        /// Updates the size of the overlay of the window.
        /// </summary>
        private void UpdateOverlaySize()
        {
            if (this.Overlay != null && Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
#if !SILVERLIGHT
                Rect windowBounds = Window.Current.Bounds;
                double windowHeight = windowBounds.Height;
                double windowWidth = windowBounds.Width;
                this.Height = windowBounds.Height;
                this.Width = windowBounds.Width;
#else
                this.Height = Application.Current.Host.Content.ActualHeight;
                this.Width = Application.Current.Host.Content.ActualWidth;
#endif
                this.Overlay.Height = this.Height;
                this.Overlay.Width = this.Width;

                if (this.ContentRoot != null)
                {
#if !SILVERLIGHT
                    // We set the size of the ContentRoot to be the same size that the user specified for the ChildWindow itself (this is due to the fact that the size of the ChildWindow is automatically changed to fit the screen so that the overlay fits the screen):
                    this.ContentRoot.Width = this._widthThatWasInitiallySpecified;
                    this.ContentRoot.Height = this._heightThatWasInitiallySpecified;

                    // Get the content of the ContentPresenter if any. This is useful so that the Grid that is at the root of the ChildWindow user code is able to enforce its size on its children (to work around an issue with the CSS grid that is unable to properly enforce its size on its children if the size comes from a parent element) (cf. zendesk ticket #1178 where scrollbars inside the ChildWindow did not function properly)
                    FrameworkElement childWindowContentIfAny = (this._contentPresenter is ContentPresenter ? ((ContentPresenter)this._contentPresenter).Content as FrameworkElement : null);

                    // Determine the chrome size (ie. size of the header bar and margins, that is the difference between the size of the ContentPresenter and the ContentRoot):
                    double chromeHeight;
                    if (TryCalculateChromeHeight(out chromeHeight))
                    {
                        // If the size is "Auto", we set a limit (max height and max width) so that scroll bars work as expected:
                        bool isHeightAuto = double.IsNaN(this._heightThatWasInitiallySpecified); // Means "Auto"
                        if (isHeightAuto)
                        {
                            double maxHeight = windowHeight - chromeHeight;
                            this._contentPresenter.MaxHeight = maxHeight; // Note: "this._contentPresenter" is not null because the "TryCalculateChromeHeight" method verified it.
                            if (childWindowContentIfAny != null)
                                childWindowContentIfAny.MaxHeight = maxHeight;
                        }
                        else
                        {
                            double maxHeight = this._heightThatWasInitiallySpecified - chromeHeight;
                            this._contentPresenter.MaxHeight = maxHeight;
                            if (childWindowContentIfAny != null)
                                childWindowContentIfAny.MaxHeight = maxHeight;
                        }
                    }
                    // Determine the chrome size (ie. size of the header bar and margins, that is the difference between the size of the ContentPresenter and the ContentRoot):
                    double chromeWidth;
                    if (TryCalculateChromeWidth(out chromeWidth))
                    {
                        // If the size is "Auto", we set a limit (max height and max width) so that scroll bars work as expected:
                        bool isWidthAuto = double.IsNaN(this._widthThatWasInitiallySpecified); // Means "Auto"
                        if (isWidthAuto)
                        {
                            double maxWidth = windowWidth - chromeWidth;
                            this._contentPresenter.MaxWidth = maxWidth; // Note: "this._contentPresenter" is not null because the "TryCalculateChromeWidth" method verified it.
                            if (childWindowContentIfAny != null)
                                childWindowContentIfAny.MaxWidth = maxWidth;
                        }
                        else
                        {
                            double maxWidth = this._widthThatWasInitiallySpecified - chromeWidth;
                            this._contentPresenter.MaxWidth = maxWidth;
                            if (childWindowContentIfAny != null)
                                childWindowContentIfAny.MaxWidth = maxWidth;
                        }
                    }
#else
                    this.ContentRoot.Width = this._desiredContentWidth;
                    this.ContentRoot.Height = this._desiredContentHeight;
#endif
                    this.ContentRoot.Margin = this._desiredMargin;
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Determines the chrome size (ie. size of the header bar and margins, that is the difference between the size of the ContentPresenter and the ContentRoot)
        /// </summary>
        bool TryCalculateChromeHeight(out double chromeHeight)
        {
            if (this.ContentRoot != null && this._contentPresenter != null)
            {
                double actualContentRootHeight = this.ContentRoot.ActualHeight;
                double actualContentPresenterHeight = this._contentPresenter.ActualHeight;
                if (!double.IsNaN(actualContentRootHeight)
                    && !double.IsNaN(actualContentPresenterHeight))
                {
                    chromeHeight = actualContentRootHeight - actualContentPresenterHeight;
                    return true;
                }
            }
            chromeHeight = 0d;
            return false;
        }

        /// <summary>
        /// Determines the chrome size (ie. size of the header bar and margins, that is the difference between the size of the ContentPresenter and the ContentRoot)
        /// </summary>
        bool TryCalculateChromeWidth(out double chromeWidth)
        {
            if (this.ContentRoot != null && this._contentPresenter != null)
            {
                double actualContentRootWidth = this.ContentRoot.ActualWidth;
                double actualContentPresenterWidth = this._contentPresenter.ActualWidth;
                if (!double.IsNaN(actualContentRootWidth)
                    && !double.IsNaN(actualContentPresenterWidth))
                {
                    chromeWidth = actualContentRootWidth - actualContentPresenterWidth;
                    return true;
                }
            }
            chromeWidth = 0d;
            return false;
        }
#endif

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
#if SILVERLIGHT
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
#endif
        }

        /// <summary>
        /// Updates the ContentRootTranslateTransform.
        /// </summary>
        /// <param name="X">X coordinate of the transform.</param>
        /// <param name="Y">Y coordinate of the transform.</param>
        private void UpdateContentRootTransform(double X, double Y)
        {
#if SILVERLIGHT
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
#endif
        }

        #endregion Methods
    }
}