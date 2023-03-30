
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

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input;
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

#if MIGRATION
        ModifierKeys _keyModifiers;
#else
        VirtualKeyModifiers _keyModifiers;
#endif

        /// <summary>
        /// Gets a value that indicates which key modifiers were active at the time that
        /// the pointer event was initiated.
        /// </summary>
#if MIGRATION
        public ModifierKeys KeyModifiers
#else
        public VirtualKeyModifiers KeyModifiers
#endif
        {
            get { return _keyModifiers; }
            internal set { _keyModifiers = value; }
        }

        /// <summary>
        /// Gets an object that reports stylus device information, such as the collection
        /// of stylus points associated with the input.
        /// </summary>
        /// <returns>
        /// The stylus device information object.
        /// </returns>
        public StylusDevice StylusDevice => new StylusDevice(this);

        internal bool CheckIfEventShouldBeTreated(UIElement element, object jsEventArg)
        {
            //todo: this method is called by "OnMouseEvent", "OnMouseLeave", etc., but there appears to be other events where this method is not called: shouldn't we call it in those methods too? todo: verify that the handler is not called twice when an element has captured the pointer and registered the PointerPressed event for example.
            string sEvent = INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg);
            if (Pointer.INTERNAL_captured == null)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid($"{sEvent}.doNotReroute = true"); //this is just in case the handler of this event starts a capture of the Pointer: we do not want to reroute it right away.
                //todo: check if the comment above is always right (questionnable if capturing element is another one than the one that threw this event).
                return true;
            }
            else
            {
                if (Pointer.INTERNAL_captured == element)
                {
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"{sEvent}.doNotReroute = true");
                    return true;
                }
                else
                {
                    return OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEvent}.doNotReroute == true");
                }
            }
        }

        internal void FillEventArgs(UIElement element, object jsEventArg)
        {
            // If the element has captured the pointer, we do not want the Document to reroute the event to the element because it would result in the event being handled twice.
            if (Pointer.INTERNAL_captured == null || Pointer.INTERNAL_captured == element)
            {
                string sEvent = INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg);
                OpenSilver.Interop.ExecuteJavaScriptVoid($"{sEvent}.doNotReroute = true");
            }

            AddKeyModifiers(jsEventArg);
            SetPointerAbsolutePosition(jsEventArg, element.INTERNAL_ParentWindow);
        }

        private void AddKeyModifiers(object jsEventArg)
        {
            string sEvent = INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg);
#if MIGRATION
            ModifierKeys keyModifiers = ModifierKeys.None;
#else
            VirtualKeyModifiers keyModifiers = VirtualKeyModifiers.None;
#endif
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEvent}.shiftKey || false"))
            {
#if MIGRATION
                keyModifiers |= ModifierKeys.Shift;
#else
                keyModifiers |= VirtualKeyModifiers.Shift;
#endif
            }
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEvent}.altKey || false"))
            {
#if MIGRATION
                keyModifiers |= ModifierKeys.Alt;
#else
                keyModifiers |= VirtualKeyModifiers.Menu;
#endif
            }
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEvent}.ctrlKey || false"))
            {
#if MIGRATION
                keyModifiers |= ModifierKeys.Control;
#else
                keyModifiers |= VirtualKeyModifiers.Control;
#endif
            }
            KeyModifiers = keyModifiers;
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

        Pointer _pointer;

        /// <summary>
        /// Gets a reference to a pointer token.
        /// </summary>
        public Pointer Pointer
        {
            get { return _pointer; }
            internal set { _pointer = value; }
        }


        int _clickCount = 1;
        /// <summary>
        /// Gets the number of times the button was clicked.
        /// </summary>
        public int ClickCount { get { return _clickCount; } }

        internal void RefreshClickCount(UIElement sender)
        {
            int currentDate = Environment.TickCount;
            if (currentDate - sender.INTERNAL_lastClickDate > 400) //Note: the duration is apparently dependent on the system's double click but there is apparently no way to get it. mine defaulted to 500ms but I feel it's too long so 400 it is.
            {
                sender.INTERNAL_clickCount = 1;
            }
            else
            {
                ++sender.INTERNAL_clickCount;
            }
            sender.INTERNAL_lastClickDate = currentDate;
            _clickCount = sender.INTERNAL_clickCount;
        }

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
            else if (relativeTo.IsConnectedToLiveTree)
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