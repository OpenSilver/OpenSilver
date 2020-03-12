

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


using CSHTML5;
using CSHTML5.Internal;
#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
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
    public partial class Window : FrameworkElement, INameScope
#endif
    {
        /// <summary>
        /// Gets the currently activated window for an application.
        /// </summary>
        public static Window Current { get; set; }

        internal PositionsWatcher INTERNAL_PositionsWatcher = new PositionsWatcher(); //Note: this is to handle the changes of position of elements (for example for when we want a popup to stick to a given UIElement - see Popup.PlacementTarget).
        internal object INTERNAL_RootDomElement;

        /// <summary>
        /// Set the DOM element that will host the window. This can be set only to new windows. The MainWindow looks for a DIV that has the ID "cshtml5-root".
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
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.innerHTML = ''", rootDomElement);

            // In case of XAML view hosted inside an HTML app, we usually set the "position" of the window root to "relative" rather than "absolute" (via external JavaScript code) in order to display it inside a specific DIV. However, in this case, the layers that contain the Popups are placed under the window DIV instead of over it. To work around this issue, we set the root element display to "grid". See the sample app "IntegratingACshtml5AppInAnSPA".
            if (Grid_InternalHelpers.isCSSGridSupported()) //todo: what about the old browsers where "CSS Grid" is not supported?
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.style.display = 'grid'", rootDomElement);
            }

            // Create the DIV that will correspond to the root of the window visual tree:
            object windowRootDiv;
#if CSHTML5NETSTANDARD
            INTERNAL_HtmlDomStyleReference
#else
            dynamic
#endif
            windowRootDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", rootDomElement, this, out windowRootDiv);

            windowRootDivStyle.position = "absolute";
            windowRootDivStyle.width = "100%";
            windowRootDivStyle.height = "100%";
            windowRootDivStyle.overflowX = "hidden";
            windowRootDivStyle.overflowY = "hidden";

            this.INTERNAL_OuterDomElement = windowRootDiv;
            this.INTERNAL_InnerDomElement = windowRootDiv;

            // Set the window as "loaded":
            this._isLoaded = true;

            // Attach the window content, if any:
            object content = this.Content;
            if (content != null)
            {
                OnContentChanged(null, content);
            }

            // Raise the "Loaded" event:
            this.INTERNAL_RaiseLoadedEvent();
        }

        #region Bounds and SizeChanged event

        INTERNAL_EventManager<WindowSizeChangedEventHandler, WindowSizeChangedEventArgs> _windowSizeChangedEventManager;
        /// <summary>
        /// Occurs when the window has rendered or changed its rendering size.
        /// </summary>
        public new event WindowSizeChangedEventHandler SizeChanged
        {
            add
            {
                if (_windowSizeChangedEventManager == null)
                    _windowSizeChangedEventManager = new INTERNAL_EventManager<WindowSizeChangedEventHandler, WindowSizeChangedEventArgs>(() => INTERNAL_HtmlDomManager.GetHtmlWindow(), "resize", ProcessOnWindowSizeChanged);
                _windowSizeChangedEventManager.Add(value);
            }
            remove
            {
                if (_windowSizeChangedEventManager != null)
                    _windowSizeChangedEventManager.Remove(value);
            }
        }

        void ProcessOnWindowSizeChanged(object jsEventArg)
        {
            double width;
            double height;
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string concatenated = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.offsetWidth + '|' + $0.offsetHeight", this.INTERNAL_OuterDomElement));
                int sepIndex = concatenated.IndexOf('|');
                string widthAsString = concatenated.Substring(0, sepIndex);
                string heightAsString = concatenated.Substring(sepIndex + 1);
                width = double.Parse(widthAsString, CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                height = double.Parse(heightAsString, CultureInfo.InvariantCulture); //todo: read note above
            }
            else
            {
                width = Convert.ToDouble(CSHTML5.Interop.ExecuteJavaScript("$0.offsetWidth", this.INTERNAL_OuterDomElement)); //(double)INTERNAL_HtmlDomManager.GetRawHtmlBody().clientWidth;
                height = Convert.ToDouble(CSHTML5.Interop.ExecuteJavaScript("$0.offsetHeight", this.INTERNAL_OuterDomElement)); //(double)INTERNAL_HtmlDomManager.GetRawHtmlBody().clientHeight;
            }

            var eventArgs = new WindowSizeChangedEventArgs()
            {
                Size = new Size(width, height)
            };
            OnWindowSizeChanged(eventArgs);
        }

        void OnWindowSizeChanged(WindowSizeChangedEventArgs eventArgs)
        {
            foreach (WindowSizeChangedEventHandler handler in _windowSizeChangedEventManager.Handlers.ToList<WindowSizeChangedEventHandler>())
            {
                handler(this, eventArgs);
            }
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

                if (Application.Current.Resources.INTERNAL_HasImplicitStyles)
                {
                    this.INTERNAL_InheritedImplicitStyles = new List<ResourceDictionary>();
                    this.INTERNAL_InheritedImplicitStyles.Add(Application.Current.Resources);
                }

                // Attach the child UI element:
#if RECURSIVE_CONSTRUCTION_FIXED
                base.OnContentChanged(oldContent, newContent);
#else
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldContent as UIElement, this);
#if REWORKLOADED
                this.AddVisualChild(newContent as UIElement);
#else
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newContent as UIElement, this);
#endif
#endif
                // We can now revert the "ParentWindow" to null (cf. comment above):
                this.INTERNAL_ParentWindow = null;


                // Reset the print area so that it becomes the window root control:
                if (!CSHTML5.Interop.IsRunningInTheSimulator)
                {
                    if (CSHTML5.Native.Html.Printing.PrintManager.IsDefaultPrintArea)
                    {
                        CSHTML5.Native.Html.Printing.PrintManager.ResetPrintArea();
                    }
                }
            }
        }


#if RECURSIVE_CONSTRUCTION_FIXED
#else
#if WORKINPROGRESS
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
#else
        /// <summary>
        /// Gets or sets the content of the Window.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(Window), new PropertyMetadata(null, Content_Changed)
        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static internal void Content_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (Window)d;
            window.OnContentChanged(e.OldValue, e.NewValue);
        }
#endif
#endif

#if RECURSIVE_CONSTRUCTION_FIXED
#else
        #region ---------- INameScope implementation ----------

        Dictionary<string, object> _nameScopeDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Finds the UIElement with the specified name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The object with the specified name if any; otherwise null.</returns>
        public new object FindName(string name)
        {
            if (_nameScopeDictionary.ContainsKey(name))
                return _nameScopeDictionary[name];
            else
                return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            if (_nameScopeDictionary.ContainsKey(name) && _nameScopeDictionary[name] != scopedElement)
                throw new ArgumentException(string.Format("Cannot register duplicate name '{0}' in this scope.", name));

            _nameScopeDictionary[name] = scopedElement;
        }

        public void UnregisterName(string name)
        {
            if (!_nameScopeDictionary.ContainsKey(name))
                throw new ArgumentException(string.Format("Name '{0}' was not found.", name));

            _nameScopeDictionary.Remove(name);
        }

        #endregion
#endif

#if !BRIDGE
        [JSIL.Meta.JSReplacement("true")]
#else
        [Template("true")]
#endif
        private static bool IsRunningInJavaScript()
        {
            return false;
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


        #region Closing event

        INTERNAL_EventManager<EventHandler<ClosingEventArgs>, ClosingEventArgs> _closingEventManager;
        INTERNAL_EventManager<EventHandler<ClosingEventArgs>, ClosingEventArgs> ClosingEventManager
        {
            get
            {
                if (_closingEventManager == null)
                {
                    _closingEventManager = new INTERNAL_EventManager<EventHandler<ClosingEventArgs>, ClosingEventArgs>(Application.Current.GetWindow, "beforeunload", ProcessOnClosing);
                }
                return _closingEventManager;
            }
        }

        // Exceptions:
        //   System.NotSupportedException:
        //     When adding or removing an event handler, the application is not running
        //     outside the browser.
        /// <summary>
        /// Occurs when the window is about to close.
        /// </summary>
        public event EventHandler<ClosingEventArgs> Closing
        {
            add
            {
                //CSHTML5.Interop.ExecuteJavaScript(@"window.addEventListener(""beforeunload"", function (e) { e.returnValue=""test"";})");
                //CSHTML5.Interop.ExecuteJavaScript(@"window.onbeforeunload = function (e) { return ""test"";}");
                ClosingEventManager.Add(value);
            }
            remove
            {
                ClosingEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the Closing event
        /// </summary>
        void ProcessOnClosing(object jsEventArg)
        {
            //CSHTML5.Interop.ExecuteJavaScript(@"$0.returnValue = ""The modifications you made may not be saved.""", jsEventArg); //todo: find a way to change this message at will (changing this only has an impact on IE and maybe edge).
            //CSHTML5.Interop.ExecuteJavaScript(@"return ""The modifications you made may not be saved.""", jsEventArg); //todo: find a way to change this message at will (changing this only has an impact on IE and maybe edge).

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
            foreach (EventHandler<ClosingEventArgs> handler in _closingEventManager.Handlers)//.ToList<EventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        #endregion

#if WORKINPROGRESS
        public bool IsActive { get; private set; }
        public bool IsVisible { get; private set; }

        public WindowStyle WindowStyle { get; set; }

        public static Window GetWindow(DependencyObject dependencyObject)
        {
            return null;
        }

        public WindowState WindowState { get; set; }

        public void Close()
        {

        }

        public void DragMove()
        {

        }

        public void DragResize(WindowResizeEdge resizeEdge)
        {

        }
#endif
    }
}
