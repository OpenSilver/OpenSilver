
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
using System.Diagnostics;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseEventHandler = Windows.UI.Xaml.Input.PointerEventHandler;
using MouseButtonEventHandler = Windows.UI.Xaml.Input.PointerEventHandler;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Key = Windows.System.VirtualKey;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a service that provides static methods to display a tooltip.
    /// </summary>
    public static class ToolTipService
    {
        private const int TOOLTIPSERVICE_betweenShowDelay = 100; // 100 milliseconds
        private const int TOOLTIPSERVICE_initialShowDelay = 400; // 400 milliseconds
        private const int TOOLTIPSERVICE_showDuration = 5000;    // 5000 milliseconds 

        private static readonly DispatcherTimer _openTimer;
        private static readonly DispatcherTimer _closeTimer;
        private static ToolTip _currentToolTip;
        private static object _lastEnterSource;
        private static DateTime _lastToolTipOpenedTime = DateTime.MinValue;
        private static UIElement _owner;
        private static FrameworkElement _rootVisual;

        static ToolTipService()
        {
            _openTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TOOLTIPSERVICE_initialShowDelay)
            };
            _openTimer.Tick += OpenAutomaticToolTip;

            _closeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TOOLTIPSERVICE_showDuration)
            };
            _closeTimer.Tick += CloseAutomaticToolTip;
        }

        private static readonly DependencyProperty ToolTipInternalProperty =
            DependencyProperty.RegisterAttached(
                "ToolTipInternal",
                typeof(ToolTip),
                typeof(ToolTipService),
                null);

        /// <summary>
        /// Identifies the ToolTipService.Placement XAML attached property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached(
                "Placement",
                typeof(PlacementMode),
                typeof(ToolTipService),
                new PropertyMetadata(PlacementMode.Mouse));

        /// <summary>
        /// Gets the ToolTipService.Placement XAML attached property value for the specified target element.
        /// </summary>
        /// <param name="element">
        /// The target element for the attached property value.
        /// </param>
        /// <returns>
        /// The relative position of the specified tooltip.
        /// </returns>
        public static PlacementMode GetPlacement(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (PlacementMode)element.GetValue(PlacementProperty);
        }

        /// <summary>
        /// Sets the ToolTipService.Placement XAML attached property value for the specified target element.
        /// </summary>
        /// <param name="element">
        /// The target element for the attached property value.
        /// </param>
        /// <param name="value">
        /// One of the PlacementMode values, which specifies where the tooltip should appear relative 
        /// to the control that is the placement target.
        /// </param>
        public static void SetPlacement(DependencyObject element, PlacementMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(PlacementProperty, value);
        }

        /// <summary>
        /// Identifies the ToolTipService.PlacementTarget XAML attached property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.RegisterAttached(
                "PlacementTarget",
                typeof(UIElement),
                typeof(ToolTipService),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets the visual element that the tooltip is positioned relative to.
        /// </summary>
        /// <param name="element">
        /// The tooltip to retrieve the visual element of.
        /// </param>
        /// <returns>
        /// The visual element that the tooltip is positioned relative to.
        /// </returns>
        public static UIElement GetPlacementTarget(DependencyObject element)
        {
            return (UIElement)element.GetValue(PlacementTargetProperty);
        }

        /// <summary>
        /// Sets the position of the specified ToolTipService.ToolTip relative to the 
        /// specified value element.
        /// </summary>
        /// <param name="element">
        /// The tooltip to set the position of.
        /// </param>
        /// <param name="value">
        /// The visual element to set the tooltip for.
        /// </param>
        public static void SetPlacementTarget(DependencyObject element, UIElement value)
        {
            element.SetValue(PlacementTargetProperty, value);
        }

        /// <summary>
        /// Identifies the ToolTipService.ToolTip XAML attached property.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.RegisterAttached(
                "ToolTip",
                typeof(object),
                typeof(ToolTipService),
                new PropertyMetadata(null, OnToolTipChanged));

        /// <summary>
        /// Gets the value of the ToolTipService.ToolTip XAML attached property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's tooltip content.</returns>
        public static object GetToolTip(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetValue(ToolTipProperty);
        }

        /// <summary>
        /// Sets the value of the ToolTipService.ToolTip XAML attached property.
        /// </summary>
        /// <param name="element">The object to set tooltip content on.</param>
        /// <param name="value">The value to set for tooltip content.</param>
        public static void SetToolTip(DependencyObject element, object value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(ToolTipProperty, value);
        }

        private static void OnToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement owner = (UIElement)d;

            object toolTip = e.NewValue;
            if (e.OldValue != null)
            {
                UnregisterToolTip(owner);
            }
            if (toolTip == null)
            {
                return;
            }

            RegisterToolTip(owner, toolTip);

            SetRootVisual();
        }

        /// <summary>
        /// Place the ToolTip relative to this point 
        /// </summary>
        internal static Point MousePosition { get; set; }

        /// <summary>
        /// VisualRoot - the main page
        /// </summary> 
        internal static FrameworkElement RootVisual
        {
            get
            {
                SetRootVisual();
                return _rootVisual;
            }
        }

        /// <summary> 
        /// Access the toolTip which is currenly open by mouse movements
        /// </summary> 
        internal static ToolTip CurrentToolTip
        {
            get { return _currentToolTip; }
        }

        internal static void OnOwnerMouseEnterInternal(object sender, object source)
        {
            if (_lastEnterSource != null && ReferenceEquals(_lastEnterSource, source))
            {
                // ToolTipService had processed this event once before, when it fired on the child
                // skip it now 
                return;
            }

            UIElement senderElement = (UIElement)sender;
            if (_currentToolTip != null)
            {
                if (senderElement.GetValue(ToolTipInternalProperty) != _currentToolTip)
                {
                    // first close the previous ToolTip if entering nested elements with tooltips 
                    CloseAutomaticToolTip(null, EventArgs.Empty);
                }
                else
                {
                    // reentering the same element
                    return;
                }
            }

            _owner = senderElement;
            _lastEnterSource = source;

            Debug.Assert(_currentToolTip == null);

            SetRootVisual();

            TimeSpan sinceLastOpen = DateTime.Now - _lastToolTipOpenedTime;
            if (TimeSpan.Compare(sinceLastOpen, TimeSpan.FromMilliseconds(TOOLTIPSERVICE_betweenShowDelay)) <= 0)
            {
                // open the ToolTip immediately 
                OpenAutomaticToolTip(null, EventArgs.Empty);
            }
            else
            {
                // open the ToolTip after the InitialShowDelay interval expires
                _openTimer.Start();
            }
        }

        internal static void OnOwnerMouseLeave(object sender, MouseEventArgs e)
        {
            if (_currentToolTip == null)
            {
                // ToolTip had not been opened yet 
                _openTimer.Stop();
                _owner = null;
                _lastEnterSource = null;
                return;
            }
            CloseAutomaticToolTip(null, EventArgs.Empty);
        }

        // This method should be executed on the UI thread 
#if MIGRATION
        private static void CloseAutomaticToolTip(object sender, EventArgs e)
#else
        private static void CloseAutomaticToolTip(object sender, object e)
#endif
        {
            _closeTimer.Stop();

            _currentToolTip.IsOpen = false;
            _currentToolTip = null;
            _owner = null;
            _lastEnterSource = null;

            // set last opened timestamp only if the ToolTip is opened by a mouse movement 
            _lastToolTipOpenedTime = DateTime.Now;
        }

        internal static ToolTip ConvertToToolTip(object o)
        {
            ToolTip toolTip = o as ToolTip;
            if (toolTip == null && o is FrameworkElement fe)
            {
                toolTip = fe.Parent as ToolTip;
            }

            toolTip ??= new ToolTip { Content = o };

            return toolTip;
        }

        private static void OnOwnerMouseEnter(object sender, MouseEventArgs e)
        {
#if MIGRATION
            MousePosition = e.GetPosition(null);
#else
            MousePosition = e.GetCurrentPoint(null).Position;
#endif

            OnOwnerMouseEnterInternal(sender, e.OriginalSource);
        }

        private static bool IsSpecialKey(Key key)
        {
            switch (key)
            {
#if MIGRATION
                case Key.Alt:
#else
                case Key.Menu:
#endif
                case Key.Back:
                case Key.Delete:
                case Key.Down:
                case Key.End:
                case Key.Home:
                case Key.Insert:
                case Key.Left:
                case Key.PageDown:
                case Key.PageUp:
                case Key.Right:
                case Key.Space:
                case Key.Up:
                    return true;

                default:
                    return false;
            }
        }

        internal static void OnKeyDown(KeyEventArgs e)
        {
            // close the opened ToolTip or cancel mouse hover 
            if (_currentToolTip == null)
            {
                _openTimer.Stop();
                _owner = null;
                _lastEnterSource = null;
                return;
            }

            if (IsSpecialKey(e.Key))
            {
                return;
            }

            CloseAutomaticToolTip(null, EventArgs.Empty);
        }

        internal static void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            // close the opened ToolTip or cancel mouse hover
            if (_currentToolTip == null)
            {
                _openTimer.Stop();
                _owner = null;
                _lastEnterSource = null;
                return;
            }

            CloseAutomaticToolTip(null, EventArgs.Empty);
        }

        private static void OnRootMouseMove(object sender, MouseEventArgs e)
        {
#if MIGRATION
            MousePosition = e.GetPosition(null);
#else
            MousePosition = e.GetCurrentPoint(null).Position;
#endif
        }

#if MIGRATION
        private static void OpenAutomaticToolTip(object sender, EventArgs e)
#else
        private static void OpenAutomaticToolTip(object sender, object e)
#endif
        {
            _openTimer.Stop();

            Debug.Assert(_owner != null, "ToolTip owner was not set prior to starting the open timer");

            _currentToolTip = (ToolTip)_owner.GetValue(ToolTipInternalProperty);

            if (_currentToolTip != null)
            {
                _currentToolTip.IsOpen = true;

                // start the timer which closes the ToolTip
                _closeTimer.Start();
            }
        }

        private static void RegisterToolTip(UIElement owner, object toolTip)
        {
            Debug.Assert(owner != null, "ToolTip must have an owner");
            Debug.Assert(toolTip != null, "ToolTip can not be null");

#if MIGRATION
            owner.MouseEnter += new MouseEventHandler(OnOwnerMouseEnter);
            owner.MouseLeave += new MouseEventHandler(OnOwnerMouseLeave);
#else
            owner.PointerEntered += new MouseEventHandler(OnOwnerMouseEnter);
            owner.PointerExited += new MouseEventHandler(OnOwnerMouseLeave);
#endif
            var converted = ConvertToToolTip(toolTip);
            owner.SetValue(ToolTipInternalProperty, converted);
            converted.SetOwner(owner);
        }

        private static void SetRootVisual()
        {
            if (_rootVisual == null && Application.Current != null)
            {
                _rootVisual = Application.Current.RootVisual as FrameworkElement;
                if (_rootVisual != null)
                {
                    // keep caching mouse position because we can't query it from Silverlight 
#if MIGRATION
                    _rootVisual.MouseMove += new MouseEventHandler(OnRootMouseMove);
#else
                    _rootVisual.PointerMoved += new MouseEventHandler(OnRootMouseMove);
#endif
                }
            }
        }

        private static void UnregisterToolTip(UIElement owner)
        {
            Debug.Assert(owner != null, "owner element is required");

            if (owner.GetValue(ToolTipInternalProperty) == null)
            {
                return;
            }

#if MIGRATION
            owner.MouseEnter -= new MouseEventHandler(OnOwnerMouseEnter);
            owner.MouseLeave -= new MouseEventHandler(OnOwnerMouseLeave);
#else
            owner.PointerEntered -= new MouseEventHandler(OnOwnerMouseEnter);
            owner.PointerExited -= new MouseEventHandler(OnOwnerMouseLeave);
#endif

            ToolTip toolTip = (ToolTip)owner.GetValue(ToolTipInternalProperty);
            toolTip.SetOwner(null);
            if (toolTip.IsOpen)
            {
                if (toolTip == _currentToolTip)
                {
                    // unregistering a currently open automatic toltip
                    // thus need to stop the timer 
                    _closeTimer.Stop();
                    _currentToolTip = null;
                    _owner = null;
                    _lastEnterSource = null;
                }

                toolTip.IsOpen = false;
            }

            owner.ClearValue(ToolTipInternalProperty);
        }

        // Only used by HtmlCanvasElement
        internal static void OpenToolTipAt(ToolTip toolTip, Point position)
        {
            if (_currentToolTip == null)
            { 
                _openTimer.Stop();

                _currentToolTip = toolTip;

                if (_currentToolTip != null)
                {
                    _currentToolTip.IsOpen = true;

                    // start the timer which closes the ToolTip
                    _closeTimer.Start();
                }
            }
        }

        // Only used by HtmlCanvasElement
        internal static void CloseToolTip(ToolTip toolTip)
        {
            if (_currentToolTip == toolTip)
            {
                CloseAutomaticToolTip(null, EventArgs.Empty);
            }
        }
    }
}
