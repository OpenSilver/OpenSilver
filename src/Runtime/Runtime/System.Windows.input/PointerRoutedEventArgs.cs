
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
using System.Globalization;
using CSHTML5;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using System.Windows.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Input;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Provides event data for pointer message events related to specific user interface
    /// elements, such as PointerPressed.
    /// </summary>
#if MIGRATION
    public class MouseEventArgs : RoutedEventArgs
#else
    public class PointerRoutedEventArgs : RoutedEventArgs
#endif
    {
        internal override void InvokeHandler(Delegate handler, object target)
        {
#if MIGRATION
            ((MouseEventHandler)handler)(target, this);
#else
            ((PointerEventHandler)handler)(target, this);
#endif
        }

        internal double _pointerAbsoluteX = 0d; // Note: they are actually "relative" to the XAML Window root.
        internal double _pointerAbsoluteY = 0d; // Note: they are actually "relative" to the XAML Window root.

        /// <summary>
        /// Gets or sets a value that marks the routed event as handled, and prevents
        /// most handlers along the event route from handling the same event again.
        /// </summary>
        public bool Handled
        {
            get => HandledImpl;
            set => HandledImpl = value;
        }

        /// <summary>
        /// Gets a value that indicates which key modifiers were active at the time that
        /// the pointer event was initiated.
        /// </summary>
        public ModifierKeys KeyModifiers
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets an object that reports stylus device information, such as the collection
        /// of stylus points associated with the input.
        /// </summary>
        /// <returns>
        /// The stylus device information object.
        /// </returns>
        public StylusDevice StylusDevice => new StylusDevice(this);

        internal void FillEventArgs(UIElement element, object jsEventArg)
        {
            KeyModifiers = Keyboard.Modifiers;
            SetPointerAbsolutePosition(jsEventArg, element.INTERNAL_ParentWindow);
        }

        protected internal void SetPointerAbsolutePosition(object jsEventArg, Window window)
        {
            {
                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string sEvent = INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg);
                string type = OpenSilver.Interop.ExecuteJavaScriptString($"{sEvent}.type");
                string concatenated = type.StartsWith("touch") ? OpenSilver.Interop.ExecuteJavaScriptString($"{sEvent}.changedTouches[0].pageX + '|' + {sEvent}.changedTouches[0].pageY")
                                                               : OpenSilver.Interop.ExecuteJavaScriptString($"{sEvent}.pageX + '|' + {sEvent}.pageY");
                int sepIndex = concatenated.IndexOf('|');
                string pointerAbsoluteXAsString = concatenated.Substring(0, sepIndex);
                string pointerAbsoluteYAsString = concatenated.Substring(sepIndex + 1);
                _pointerAbsoluteX = double.Parse(pointerAbsoluteXAsString, CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                _pointerAbsoluteY = double.Parse(pointerAbsoluteYAsString, CultureInfo.InvariantCulture); //todo: read note above
            }

            //---------------------------------------
            // Adjust the absolute coordinates to take into account the fact that the XAML Window is not necessary un the top-left corner of the HTML page:
            //---------------------------------------
            if (window != null)
            {
                // Get the XAML Window root position relative to the page:
                object windowRootDomElement = window.INTERNAL_OuterDomElement;
                string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(windowRootDomElement);

                double windowRootLeft;
                double windowRootTop;

                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string concatenated = OpenSilver.Interop.ExecuteJavaScriptString(
                    $"({sElement}.getBoundingClientRect().left - document.body.getBoundingClientRect().left) + '|' + ({sElement}.getBoundingClientRect().top - document.body.getBoundingClientRect().top)");
                int sepIndex = concatenated.IndexOf('|');
                if (sepIndex > -1)
                {
                    string windowRootLeftAsString = concatenated.Substring(0, sepIndex);
                    string windowRootTopAsString = concatenated.Substring(sepIndex + 1);
                    windowRootLeft = double.Parse(windowRootLeftAsString, CultureInfo.InvariantCulture);
                    windowRootTop = double.Parse(windowRootTopAsString, CultureInfo.InvariantCulture);
                }
                else
                {
                    windowRootLeft = Double.NaN;
                    windowRootTop = Double.NaN;
                }

                // Substract the XAML Window position, to get the pointer position relative to the XAML Window root:
                _pointerAbsoluteX -= windowRootLeft;
                _pointerAbsoluteY -= windowRootTop;
            }
        }

        /// <summary>
        /// Gets a reference to a pointer token.
        /// </summary>
        public Pointer Pointer { get; internal set; }

        /// <summary>
        /// Gets the number of times the button was clicked.
        /// </summary>
        public int ClickCount { get; internal set; }

        /// <summary>
        /// Returns the pointer position for this event occurrence, optionally evaluated
        /// against a coordinate origin of a supplied UIElement.
        /// </summary>
        /// <param name="relativeTo">
        /// Any UIElement-derived object that is connected to the same object tree. To
        /// specify the object relative to the overall coordinate system, use a relativeTo value
        /// of null.
        /// </param>
        /// <returns>
        /// A PointerPoint value that represents the pointer point associated with this
        /// event. If null was passed as relativeTo, the coordinates are in the frame
        /// of reference of the overall window. If a non-null relativeTo was passed,
        /// the coordinates are relative to the object referenced by relativeTo.
        /// </returns>
#if MIGRATION
        public Point GetPosition(UIElement relativeTo)
            => GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);
#else
        public PointerPoint GetCurrentPoint(UIElement relativeTo)
        {
            PointerPoint pointerPoint = new PointerPoint();
            pointerPoint.Properties.MouseWheelDelta = PointerPointProperties.GetPointerWheelDelta(UIEventArg);
            pointerPoint.Position = GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);

            return pointerPoint;
        }

        internal Point GetPosition(UIElement relativeTo)
            => GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);

        internal int Delta => GetCurrentPoint(null).Properties.MouseWheelDelta;
#endif

        internal static Point GetPosition(Point origin, UIElement relativeTo)
        {
            if (relativeTo is Popup popup)
            {
                relativeTo = popup.IsOpen ? popup.Child : null;
            }

            if (relativeTo == null)
            {
                //-----------------------------------
                // Return the absolute pointer coordinates:
                //-----------------------------------
                return origin;
            }
            else if (INTERNAL_VisualTreeManager.IsElementInVisualTree(relativeTo))
            {
                //-----------------------------------
                // Returns the pointer coordinates relative to the "relativeTo" element:
                //-----------------------------------

                UIElement rootVisual = Application.Current?.RootVisual;
                if (rootVisual != null)
                {
                    return rootVisual.TransformToVisual(relativeTo).Transform(origin);
                }
            }

            return new Point(0.0, 0.0);
        }

#if MIGRATION
        [OpenSilver.NotImplemented]
        public int Delta { get; private set; }
#endif
    }
}