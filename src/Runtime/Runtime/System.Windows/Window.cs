﻿
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
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using CSHTML5.Internal;
using CSHTML5;

#if MIGRATION
using System.Windows.Input;
#else
using Windows.UI.Core;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents an application window.
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class Window : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window() : this(false) { }

        internal Window(bool hookUpEvents)
        {
            BypassLayoutPolicies = true;

            if (hookUpEvents)
            {
                new DOMEventManager(
                    Application.Current.GetWindow, 
                    "beforeunload", 
                    ProcessOnClosing)
                .AttachToDomEvents();

                new DOMEventManager(
                    () => INTERNAL_HtmlDomManager.GetHtmlWindow(), 
                    "resize", 
                    ProcessOnWindowSizeChanged)
                .AttachToDomEvents();
            }

            GotFocus += new RoutedEventHandler(OnGotFocus);
        }

        internal ITextMeasurementService TextMeasurementService { get; private set; }

        internal override int VisualChildrenCount
        {
            get
            {
                return Content == null ? 0 : 1;
            }
        }

        internal override UIElement GetVisualChild(int index)
        {
            UIElement content = Content;
            if (content == null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return content;
        }

        /// <summary>
        /// Gets the currently activated window for an application.
        /// </summary>
        public static Window Current { get; set; }

        #region Out-of browser properties
        /// <summary>
        /// Gets or sets the position of the left edge of the application window; see Remarks
        /// for restrictions on setting this property at run time.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double Left { get; set; }

        /// <summary>
        /// Gets or sets the position of the top edge of the application window; see Remarks
        /// for restrictions on setting this property at run time.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double Top { get; set; }

        /// <summary>
        /// Gets or sets the window title bar text.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string Title { get; set; }
        #endregion

        internal object INTERNAL_RootDomElement;

        /// <summary>
        /// Set the DOM element that will host the window. This can be set only to new windows. The MainWindow looks for a DIV that has the ID "cshtml5-root" or "opensilver-root".
        /// </summary>
        /// <param name="rootDomElement">The DOM element that will host the window</param>
        public void AttachToDomElement(object rootDomElement)
        {
            if (INTERNAL_OuterDomElement != null || INTERNAL_RootDomElement != null)
            {
                throw new InvalidOperationException("The method 'Window.AttachToDomElement' can be called only once.");
            }

            //Note: The "rootDomElement" will contain one DIV for the root of the window visual tree, and other DIVs to host the popups.
            INTERNAL_RootDomElement = rootDomElement ?? throw new ArgumentNullException(nameof(rootDomElement));

            // In case of XAML view hosted inside an HTML app, we usually set the "position" of the window root to "relative" rather than "absolute" (via external JavaScript code) in order to display it inside a specific DIV. However, in this case, the layers that contain the Popups are placed under the window DIV instead of over it. To work around this issue, we set the root element display to "grid". See the sample app "IntegratingACshtml5AppInAnSPA".
            string sRootElement = INTERNAL_InteropImplementation.GetVariableStringForJS(rootDomElement);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sRootElement}.style.display = 'grid'");
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sRootElement}.style.overflow = 'clip'");

            // Create the DIV that will correspond to the root of the window visual tree:
            var windowRootDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", rootDomElement, this, out object windowRootDiv);

            windowRootDivStyle.width = "100%";
            windowRootDivStyle.height = "100%";
            windowRootDivStyle.overflowX = "hidden";
            windowRootDivStyle.overflowY = "hidden";

            INTERNAL_OuterDomElement = windowRootDiv;
            INTERNAL_InnerDomElement = windowRootDiv;

            INTERNAL_AttachToDomEvents();

            // Set the window as "loaded":
            _isLoaded = true;
            IsConnectedToLiveTree = true;
            UpdateIsVisible();

            TextMeasurementService = new TextMeasurementService(this);

            // Attach the window content, if any:
            object content = Content;
            if (content != null)
            {
                OnContentChanged(null, content);
            }

            // Raise the "Loaded" event:
            RaiseLoadedEvent();
            
            SizeChanged += WindowSizeChangedEventHandler;
        }

        private void WindowSizeChangedEventHandler(object sender, WindowSizeChangedEventArgs e)
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e) => Current = this;

        #region Bounds and SizeChanged event

        /// <summary>
        /// Occurs when the window has rendered or changed its rendering size.
        /// </summary>
        public new event WindowSizeChangedEventHandler SizeChanged;

        void ProcessOnWindowSizeChanged(object jsEventArg)
        {
            double width;
            double height;
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(this.INTERNAL_OuterDomElement);
            // Hack to improve the Simulator performance by making only one interop call rather than two:
            string concatenated = OpenSilver.Interop.ExecuteJavaScriptString($"{sElement}.offsetWidth + '|' + {sElement}.offsetHeight");
            int sepIndex = concatenated.IndexOf('|');
            string widthAsString = concatenated.Substring(0, sepIndex);
            string heightAsString = concatenated.Substring(sepIndex + 1);
            width = double.Parse(widthAsString, CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
            height = double.Parse(heightAsString, CultureInfo.InvariantCulture); //todo: read note above

            var eventArgs = new WindowSizeChangedEventArgs()
            {
                Size = new Size(width, height)
            };
            OnWindowSizeChanged(eventArgs);
        }

        void OnWindowSizeChanged(WindowSizeChangedEventArgs eventArgs)
        {
            SizeChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Gets the height and width of the application window, as a Rect value.
        /// </summary>
        public Rect Bounds
        {
            get
            {
                if (INTERNAL_OuterDomElement is not null)
                {
                    string sDiv = INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    double width = OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDiv}.offsetWidth");
                    double height = OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDiv}.offsetHeight");
                    return new Rect(0, 0, width, height);
                }

                return new Rect(0, 0, 0, 0);
            }
        }

        #endregion

        protected void OnContentChanged(object oldContent, object newContent)
        {
            if (_isLoaded)
            {
                // Due to the fact that the children fill their "ParentWindow"
                // property by copying the value of the "ParentWindow" property
                // of their parent, we need to temporarily set it to be equal
                // to "this" before calling "base.OnContentChanged", so that it
                // then gets passed to the children recursively:
                this.INTERNAL_ParentWindow = this;

                // Attach the child UI element:
                UIElement newChild = newContent as UIElement;
                UIElement oldChild = oldContent as UIElement;

                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, this);
                RemoveVisualChild(oldChild);
                AddVisualChild(newChild);
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, this);

                // We can now revert the "ParentWindow" to null (cf. comment above):
                this.INTERNAL_ParentWindow = null;
            }
        }

        /// <summary>
        /// Gets or sets the content of the Window.
        /// </summary>
        public FrameworkElement Content
        {
            get { return (FrameworkElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(FrameworkElement),
                typeof(Window),
                new PropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (Window)d;
            window.OnContentChanged(e.OldValue, e.NewValue);
            window.SetLayoutSize();
        }

        private void SetLayoutSize()
        {
            Rect bounds = Bounds;
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

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            throw new InvalidOperationException("\"CreateDomElement\" should not be called for the Window object.");
        }

        public override void INTERNAL_AttachToDomEvents()
        {
            InputManager.Current.RegisterRoot(INTERNAL_RootDomElement);
            base.INTERNAL_AttachToDomEvents();
        }

        #region Closing event

        /// <summary>
        /// Occurs when the window is about to close.
        /// </summary>
        public event EventHandler<ClosingEventArgs> Closing;

        /// <summary>
        /// Raises the Closing event
        /// </summary>
        void ProcessOnClosing(object jsEventArg)
        {
            var eventArgs = new ClosingEventArgs(true);
            eventArgs.INTERNAL_JSArgs = jsEventArg;
            OnClosing(eventArgs);
        }

        /// <summary>
        /// Raises the Closing event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected void OnClosing(ClosingEventArgs eventArgs)
        {
            Closing?.Invoke(this, eventArgs);
        }

        #endregion

        [OpenSilver.NotImplemented]
        public bool IsActive { get; private set; }
        [OpenSilver.NotImplemented]
        public bool IsVisible { get; private set; }

        [OpenSilver.NotImplemented]
        public WindowStyle WindowStyle { get; set; }

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
                throw new InvalidOperationException("Reference is not a valid visual DependencyObject.");
            }

            return GetWindow(uie);
        }

        internal static Window GetWindow(UIElement uie)
        {
            Debug.Assert(uie is not null);
            return uie.INTERNAL_ParentWindow;
        }

        [OpenSilver.NotImplemented]
        public WindowState WindowState { get; set; }

        /// <summary>
        ///     Show the window
        /// </summary>
        [OpenSilver.NotImplemented]
        public void Show()
        {
        
        }

        [OpenSilver.NotImplemented]
        public void Close()
        {

        }

        [OpenSilver.NotImplemented]
        public void DragMove()
        {

        }

        [OpenSilver.NotImplemented]
        public void DragResize(WindowResizeEdge resizeEdge)
        {

        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = Bounds.Size;
            if (Content is not null)
            {
                Content.Measure(availableSize);
                return Content.DesiredSize;
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = Bounds.Size;
            if (Content is not null)
            {
                Content.Arrange(new Rect(new Point(), finalSize));
            }
            return finalSize;
        }
    }
}
