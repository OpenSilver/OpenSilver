﻿
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



using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
//using Windows.System;
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
        internal double _pointerAbsoluteX = 0d; // Note: they are actually "relative" to the XAML Window root.
        internal double _pointerAbsoluteY = 0d; // Note: they are actually "relative" to the XAML Window root.

        bool _handled = false;
        /// <summary>
        /// Gets or sets a value that marks the routed event as handled, and prevents
        /// most handlers along the event route from handling the same event again.
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
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

        internal bool CheckIfEventShouldBeTreated(UIElement element, object jsEventArg)
        {
            //todo: this method is called by "OnMouseEvent", "OnMouseLeave", etc., but there appears to be other events where this method is not called: shouldn't we call it in those methods too? todo: verify that the handler is not called twice when an element has captured the pointer and registered the PointerPressed event for example.

            if (Pointer.INTERNAL_captured == null)
            {
                Interop.ExecuteJavaScript("$0.doNotReroute = true", jsEventArg); //this is just in case the handler of this event starts a capture of the Pointer: we do not want to reroute it right away.
                //todo: check if the comment above is always right (questionnable if capturing element is another one than the one that threw this event).
                return true;
            }
            else
            {
                if (Pointer.INTERNAL_captured == element)
                {
                    Interop.ExecuteJavaScript("$0.doNotReroute = true", jsEventArg);
                    return true;
                }
                else
                {
                    if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.doNotReroute == true", jsEventArg)))
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        internal void FillEventArgs(UIElement element, object jsEventArg)
        {
            // If the element has captured the pointer, we do not want the Document to reroute the event to the element because it would result in the event being handled twice.
            if (Pointer.INTERNAL_captured == null || Pointer.INTERNAL_captured == element)
            {
                Interop.ExecuteJavaScript("$0.doNotReroute = true", jsEventArg);
            }

            AddKeyModifiers(jsEventArg);
            SetPointerAbsolutePosition(jsEventArg, element.INTERNAL_ParentWindow);
        }

        private void AddKeyModifiers(object jsEventArg)
        {
#if MIGRATION
            ModifierKeys keyModifiers = ModifierKeys.None;
#else
            VirtualKeyModifiers keyModifiers = VirtualKeyModifiers.None;
#endif
            if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.shiftKey || false", jsEventArg))) //Note: we use "||" because the value "shiftKey" may be null or undefined. For more information on "||", read: https://stackoverflow.com/questions/476436/is-there-a-null-coalescing-operator-in-javascript
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Shift;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Shift;
#endif
            }
            if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.altKey || false", jsEventArg)))
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Alt;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Menu;
#endif
            }
            if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.ctrlKey || false", jsEventArg)))
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Control;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Control;
#endif
            }
            KeyModifiers = keyModifiers;
        }

        protected internal void SetPointerAbsolutePosition(object jsEventArg, Window window)
        {
            if (Interop.IsRunningInTheSimulator)
            {
                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string concatenated = Convert.ToString(Interop.ExecuteJavaScript("$0.pageX + '|' + $0.pageY", jsEventArg));
                int sepIndex = concatenated.IndexOf('|');
                string pointerAbsoluteXAsString = concatenated.Substring(0, sepIndex);
                string pointerAbsoluteYAsString = concatenated.Substring(sepIndex + 1);
                _pointerAbsoluteX = double.Parse(pointerAbsoluteXAsString, CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                _pointerAbsoluteY = double.Parse(pointerAbsoluteYAsString, CultureInfo.InvariantCulture); //todo: read note above
            }
            else
            {
                dynamic jsEventArgDynamic = (dynamic)jsEventArg;
                //todo - removeJSIL: once we stop supporting the JSIL version, remove the bools like the following and put the thing directly in the if (3x in this method for now).
                bool isArgsPageXDefined = INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(jsEventArgDynamic.pageX); // Using a temporary variable to conatin the tests's result (this line and the following) because in JSIL, trying to do both tests in the if results in an UntranslatableMethod for some reason.
                bool isArgsPageXNotNull = isArgsPageXDefined;
                if(isArgsPageXDefined) //Note: apparently, JSIL is really bad at translating things like "bool isArgsPageXNotNull = isArgsPageXDefined && (jsEventArgDynamic.pageX != 0) and ends up commiting seppuku by trying to cast 0 to a boolean.
                {
                    isArgsPageXNotNull = jsEventArgDynamic.pageX != 0;
                }
                if (isArgsPageXNotNull)
                {
                    _pointerAbsoluteX = (double)jsEventArgDynamic.pageX;
                    _pointerAbsoluteY = (double)jsEventArgDynamic.pageY;
                }
                else
                {
                    bool isArgsTouchesDefined = INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(jsEventArgDynamic.touches); // Using a temporary variable to conatin the tests's result (this line and the following) because in JSIL, trying to do both tests in the if results in an UntranslatableMethod for some reason.
                    bool isArgsTouchesNotEmpty = isArgsTouchesDefined;
                    if(isArgsTouchesDefined)
                    {
                        isArgsTouchesNotEmpty = jsEventArgDynamic.touches.length != 0;
                    }
                    if (isArgsTouchesNotEmpty) //Chrome for Android uses different ways to access the pointer's position.
                    {
                        _pointerAbsoluteX = (double)jsEventArgDynamic.touches[0].pageX;
                        _pointerAbsoluteY = (double)jsEventArgDynamic.touches[0].pageY;
                    }
                    else
                    {
                        bool isArgsChangedTouchesDefined = INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(jsEventArgDynamic.changedTouches); // Using a temporary variable to conatin the tests's result (this line and the following) because in JSIL, trying to do both tests in the if results in an UntranslatableMethod for some reason.
                        bool isArgsChangedTouchesNotEmpty = isArgsChangedTouchesDefined;
                        if (isArgsChangedTouchesDefined)
                        {
                            isArgsChangedTouchesNotEmpty = jsEventArgDynamic.changedTouches.length != 0;
                        }
                        if (isArgsChangedTouchesNotEmpty) //this is for the PointerRelease event on Chrome for Android
                        {
                            _pointerAbsoluteX = (double)jsEventArgDynamic.changedTouches[0].pageX;
                            _pointerAbsoluteY = (double)jsEventArgDynamic.changedTouches[0].pageY;
                        }
                        else
                        {
                            _pointerAbsoluteX = 0d;
                            _pointerAbsoluteY = 0d;
                        }
                    }
                }
            }

            //---------------------------------------
            // Adjust the absolute coordinates to take into account the fact that the XAML Window is not necessary un the top-left corner of the HTML page:
            //---------------------------------------
            if (window != null)
            {
                // Get the XAML Window root position relative to the page:
                object windowRootDomElement = window.INTERNAL_OuterDomElement;
                object windowBoundingClientRect = Interop.ExecuteJavaScript("$0.getBoundingClientRect()", windowRootDomElement);
                object pageBodyBoundingClientRect = Interop.ExecuteJavaScript("document.body.getBoundingClientRect()"); // This is to take into account the scrolling.

                double windowRootLeft;
                double windowRootTop;

                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string concatenated = CSHTML5.Interop.ExecuteJavaScript("($0.left - $1.left) + '|' + ($0.top - $1.top)", windowBoundingClientRect, pageBodyBoundingClientRect).ToString();
                int sepIndex = concatenated.IndexOf('|');
                if (sepIndex > -1)
                {
                    string windowRootLeftAsString = concatenated.Substring(0, sepIndex);
                    string windowRootTopAsString = concatenated.Substring(sepIndex + 1);
#if BRIDGE
                    windowRootLeft = double.Parse(windowRootLeftAsString, global::System.Globalization.CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                    windowRootTop = double.Parse(windowRootTopAsString, global::System.Globalization.CultureInfo.InvariantCulture); //todo: read note above
#else
                    //JSIL doesn't have a double.Parse with localization:
                    windowRootLeft = double.Parse(windowRootLeftAsString); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                    windowRootTop = double.Parse(windowRootTopAsString); //todo: read note above
#endif
                }
                else
                {
                    windowRootLeft = Double.NaN;
                    windowRootTop = Double.NaN;
                }

                // Substract the XAML Window position, to get the pointer position relative to the XAML Window root:
                _pointerAbsoluteX = _pointerAbsoluteX - windowRootLeft;
                _pointerAbsoluteY = _pointerAbsoluteY - windowRootTop;
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
        {
            if (relativeTo == null)
            {
                //-----------------------------------
                // Return the absolute pointer coordinates:
                //-----------------------------------
                return new Point(_pointerAbsoluteX, _pointerAbsoluteY);
            }
            else
            {
                //-----------------------------------
                // Returns the pointer coordinates relative to the "relativeTo" element:
                //-----------------------------------

                // Get the opposite of the absolute position of the "relativeTo" element:
                GeneralTransform generalTransform = Window.Current.TransformToVisual(relativeTo);

                // Get the pointer coordinates relative to "relativeTo" element: 
                return generalTransform.TransformPoint(new Point(_pointerAbsoluteX, _pointerAbsoluteY));
            }
        }
#else
        public PointerPoint GetCurrentPoint(UIElement relativeTo)
        {
            if (relativeTo == null)
            {
                //-----------------------------------
                // Return the absolute pointer coordinates:
                //-----------------------------------
                PointerPoint pointerPoint = new PointerPoint()
                {
                    Position = new Point(_pointerAbsoluteX, _pointerAbsoluteY)
                };
                return pointerPoint;
            }
            else
            {
                //-----------------------------------
                // Returns the pointer coordinates relative to the "relativeTo" element:
                //-----------------------------------

                // Get the opposite of the absolute position of the "relativeTo" element:
                GeneralTransform generalTransform = Window.Current.TransformToVisual(relativeTo);

                // Get the pointer coordinates relative to "relativeTo" element: 
                PointerPoint pointerPoint = new PointerPoint()
                {
                    Position = generalTransform.TransformPoint(new Point(_pointerAbsoluteX, _pointerAbsoluteY))
                };
                return pointerPoint;
            }
        }
#endif

        ////
        //// Summary:
        ////     Returns the historical pointer positions for this event occurrence, optionally
        ////     evaluated against a coordinate origin of a supplied UIElement.
        ////
        //// Parameters:
        ////   relativeTo:
        ////     Any UIElement-derived object that is connected to the same object tree. To
        ////     specify the object relative to the overall coordinate system, use a relativeTo value
        ////     of null.
        ////
        //// Returns:
        ////     A list of PointerPoint values that represent the intermediate, historical
        ////     pointer points associated with this event. If null was passed as relativeTo,
        ////     the coordinates are in the frame of reference of the overall window. If a
        ////     non-null relativeTo was passed, the coordinates are relative to the object
        ////     referenced by relativeTo.
        //public IList<PointerPoint> GetIntermediatePoints(UIElement relativeTo)
        //{

        //}
    }
}