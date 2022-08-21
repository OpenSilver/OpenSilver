
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.Foundation;
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
    [global::System.Windows.Markup.ContentProperty("Content")]
#if RECURSIVE_CONSTRUCTION_FIXED
    public partial class Window : ContentControl
#else
    public partial class Window : FrameworkElement
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window() : this(false) { }

        internal Window(bool hookUpEvents)
        {
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
        }

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

        internal PositionsWatcher INTERNAL_PositionsWatcher = new PositionsWatcher(); //Note: this is to handle the changes of position of elements (for example for when we want a popup to stick to a given UIElement - see Popup.PlacementTarget).
        internal object INTERNAL_RootDomElement;

        /// <summary>
        /// Set the DOM element that will host the window. This can be set only to new windows. The MainWindow looks for a DIV that has the ID "cshtml5-root" or "opensilver-root".
        /// </summary>
        /// <param name="rootDomElement">The DOM element that will host the window</param>
        public void AttachToDomElement(object rootDomElement)
        {
            if (this.INTERNAL_OuterDomElement != null
                || this.INTERNAL_RootDomElement != null)
                throw new InvalidOperationException("The method 'Window.AttachToDomElement' can be called only once.");

            if (rootDomElement == null)
                throw new ArgumentNullException("rootDomElement");

            //Note: The "rootDomElement" will contain one DIV for the root of the window visual tree, and other DIVs to host the popups.
            this.INTERNAL_RootDomElement = rootDomElement;

            // Reset the content of the root DIV:
            CSHTML5.Interop.ExecuteJavaScript("document.clearXamlRoot()");

            // In case of XAML view hosted inside an HTML app, we usually set the "position" of the window root to "relative" rather than "absolute" (via external JavaScript code) in order to display it inside a specific DIV. However, in this case, the layers that contain the Popups are placed under the window DIV instead of over it. To work around this issue, we set the root element display to "grid". See the sample app "IntegratingACshtml5AppInAnSPA".
            if (Grid_InternalHelpers.isCSSGridSupported()) //todo: what about the old browsers where "CSS Grid" is not supported?
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.style.display = 'grid'", rootDomElement);
            }

            // Create the DIV that will correspond to the root of the window visual tree:
            object windowRootDiv;

            var windowRootDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", rootDomElement, this, out windowRootDiv);

            windowRootDivStyle.position = "absolute";
            windowRootDivStyle.width = "100%";
            windowRootDivStyle.height = "100%";
            windowRootDivStyle.overflowX = "hidden";
            windowRootDivStyle.overflowY = "hidden";

            this.INTERNAL_OuterDomElement = windowRootDiv;
            this.INTERNAL_InnerDomElement = windowRootDiv;

            // Set the window as "loaded":
            this._isLoaded = true;
            this.UpdateIsVisible();

            // Attach the window content, if any:
            object content = this.Content;
            if (content != null)
            {
                OnContentChanged(null, content);
            }

            // Raise the "Loaded" event:
            this.RaiseLoadedEvent();
            this.InvalidateMeasure();
            
            this.SizeChanged += WindowSizeChangedEventHandler;
        }

        private void WindowSizeChangedEventHandler(object sender, WindowSizeChangedEventArgs e)
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        #region Bounds and SizeChanged event

        /// <summary>
        /// Occurs when the window has rendered or changed its rendering size.
        /// </summary>
        public new event WindowSizeChangedEventHandler SizeChanged;

        void ProcessOnWindowSizeChanged(object jsEventArg)
        {
            double width;
            double height;
#if OPENSILVER
            if (true)
#elif BRIDGE
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string concatenated = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("$0.offsetWidth + '|' + $0.offsetHeight", this.INTERNAL_OuterDomElement));
                int sepIndex = concatenated.IndexOf('|');
                string widthAsString = concatenated.Substring(0, sepIndex);
                string heightAsString = concatenated.Substring(sepIndex + 1);
                width = double.Parse(widthAsString, CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                height = double.Parse(heightAsString, CultureInfo.InvariantCulture); //todo: read note above
            }
            else
            {
                width = Convert.ToDouble(OpenSilver.Interop.ExecuteJavaScript("$0.offsetWidth", this.INTERNAL_OuterDomElement)); //(double)INTERNAL_HtmlDomManager.GetRawHtmlBody().clientWidth;
                height = Convert.ToDouble(OpenSilver.Interop.ExecuteJavaScript("$0.offsetHeight", this.INTERNAL_OuterDomElement)); //(double)INTERNAL_HtmlDomManager.GetRawHtmlBody().clientHeight;
            }

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
                var actualSize = this.INTERNAL_GetActualWidthAndHeight();
                double width = actualSize.Width; //(double)INTERNAL_HtmlDomManager.GetRawHtmlBody().clientWidth; // Commented because it didn't work properly in the Simulator.
                double height = actualSize.Height; //(double)INTERNAL_HtmlDomManager.GetRawHtmlBody().clientHeight; // Commented because it didn't work properly in the Simulator.
                if (!double.IsNaN(width) && !double.IsNaN(height))
                    return new Rect(0, 0, width, height);
                else
                    return new Rect(0, 0, 0, 0);
            }
        }

        #endregion

#if RECURSIVE_CONSTRUCTION_FIXED
        protected override void OnContentChanged(object oldContent, object newContent)
#else
        protected void OnContentChanged(object oldContent, object newContent)
#endif
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
#if RECURSIVE_CONSTRUCTION_FIXED
                base.OnContentChanged(oldContent, newContent);
#else
                UIElement newChild = newContent as UIElement;
                UIElement oldChild = oldContent as UIElement;

                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, this);
                RemoveVisualChild(oldChild);
                AddVisualChild(newChild);
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, this);
#endif
                // We can now revert the "ParentWindow" to null (cf. comment above):
                this.INTERNAL_ParentWindow = null;


                // Reset the print area so that it becomes the window root control:
#if OPENSILVER
                if (false)
#elif BRIDGE
                if (!CSHTML5.Interop.IsRunningInTheSimulator)
#endif
                {
                    if (CSHTML5.Native.Html.Printing.PrintManager.IsDefaultPrintArea)
                    {
                        CSHTML5.Native.Html.Printing.PrintManager.ResetPrintArea();
                    }
                }

                Application.Current.TextMeasurementService.CreateMeasurementText(this);

                /*
                // Invalidate when content changed
                InvalidateMeasure();
                InvalidateArrange();

                // At the first contentChanged, InvaliateMeasure/Arrange does not work because IsArrangeValid and IsMeasureValid is false.
                if (CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
                {
                    Debug.WriteLine("Delayed CalculateWindowLayout");
                    // On the simulator, Window Bounds height is zero at the startup.
                    Task.Delay(500).ContinueWith(t => CalculateWindowLayout());
                }
                else
                    CalculateWindowLayout();*/
                // Disabled for CustomLayout
            }
        }


#if RECURSIVE_CONSTRUCTION_FIXED
#else
        /// <summary>
        /// Gets or sets the content of the Window.
        /// </summary>
        public FrameworkElement Content
        {
            get { return (FrameworkElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(Window), new PropertyMetadata(null, Content_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static internal void Content_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (Window)d;
            window.OnContentChanged(e.OldValue, e.NewValue);
        }
#endif

#if BRIDGE
        [Bridge.Template("true")]
        private static bool IsRunningInJavaScript()
        {
            return false;
        }
#endif

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

        [OpenSilver.NotImplemented]
        public static Window GetWindow(DependencyObject dependencyObject)
        {
            return null;
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

        private void CalculateWindowLayout()
        {
            if (Current.INTERNAL_VisualChildrenInformation == null)
                return;

            Rect windowBounds = this.Bounds;
            double width = windowBounds.Width;
            double height = windowBounds.Height;
            Debug.WriteLine($"CalculateWindowLayout {width}, {height}");
            Current.Measure(new Size(width, height));
            Current.Arrange(windowBounds);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Content as FrameworkElement == null)
                return new Size();

            FrameworkElement _content = this.Content as FrameworkElement;
            Rect windowBounds = this.Bounds;
            availableSize = new Size(windowBounds.Width, windowBounds.Height);
            _content.Measure(availableSize);
            return _content.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect windowBounds = this.Bounds;
            finalSize = new Size(windowBounds.Width, windowBounds.Height);
            return base.ArrangeOverride(finalSize);
        }
    }
}
