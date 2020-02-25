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



#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
#if MIGRATION
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    partial class UIElement
    {

        #region Pointer moved event

#if MIGRATION
        public static readonly RoutedEvent MouseMoveEvent = new RoutedEvent("MouseMoveEvent");
#else
        public static readonly RoutedEvent PointerMovedEvent = new RoutedEvent("PointerMovedEvent");
#endif

#if MIGRATION
        INTERNAL_EventManager<MouseEventHandler, MouseEventArgs> _pointerMovedEventManager;
        INTERNAL_EventManager<MouseEventHandler, MouseEventArgs> PointerMovedEventManager
#else
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> _pointerMovedEventManager;
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> PointerMovedEventManager
#endif
        {
            get
            {
                if (_pointerMovedEventManager == null)
                {
                    string[] eventsNames = { "mousemove", "touchmove" };
#if MIGRATION
                    _pointerMovedEventManager = new INTERNAL_EventManager<MouseEventHandler, MouseEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                        {
                            ProcessPointerEvent(jsEventArg, (Action<MouseButtonEventArgs>)OnMouseMove, (Action<MouseButtonEventArgs>)OnMouseMove_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true);
                        });
#else
                    _pointerMovedEventManager = new INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                    {
                        ProcessPointerEvent(jsEventArg, (Action<PointerRoutedEventArgs>)OnPointerMoved, (Action<PointerRoutedEventArgs>)OnPointerMoved_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true);
                    });
#endif
                }
                return _pointerMovedEventManager;
            }
        }

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// moved, while within this element.
        /// </summary>
#if MIGRATION
        public event MouseEventHandler MouseMove
#else
        public event PointerEventHandler PointerMoved
#endif
        {
            add
            {
                PointerMovedEventManager.Add(value);
            }
            remove
            {
                PointerMovedEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the PointerMoved event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>

#if MIGRATION
        protected virtual void OnMouseMove(MouseEventArgs eventArgs)
#else
        protected virtual void OnPointerMoved(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            foreach (MouseEventHandler handler in _pointerMovedEventManager.Handlers.ToList<MouseEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerMovedEventManager.Handlers.ToList<PointerEventHandler>())
#endif
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        void OnMouseMove_ForHandledEventsToo(MouseEventArgs eventArgs)
#else
        void OnPointerMoved_ForHandledEventsToo(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            foreach (MouseEventHandler handler in _pointerMovedEventManager.HandlersForHandledEventsToo.ToList<MouseEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerMovedEventManager.HandlersForHandledEventsToo.ToList<PointerEventHandler>())
#endif
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region Pointer pressed event

#if MIGRATION
        public static readonly RoutedEvent MouseLeftButtonDownEvent = new RoutedEvent("MouseLeftButtonDownEvent");
#else
        public static readonly RoutedEvent PointerPressedEvent = new RoutedEvent("PointerPressedEvent");
#endif

#if WORKINPROGRESS
#if MIGRATION
        //
        // Summary:
        //     Identifies the System.Windows.UIElement.MouseRightButtonDown routed event.
        //
        // Returns:
        //     The identifier for the System.Windows.UIElement.MouseRightButtonDown routed event.
        public static readonly RoutedEvent MouseRightButtonDownEvent;
#endif
#endif

#if MIGRATION
        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> _pointerPressedEventManager;
        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> PointerPressedEventManager
#else
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> _pointerPressedEventManager;
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> PointerPressedEventManager
#endif
        {
            get
            {
                if (_pointerPressedEventManager == null)
                {
                    string[] eventsNames = { "mousedown", "touchstart" };

#if MIGRATION
                    _pointerPressedEventManager = new INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                        {
                            ProcessPointerEvent(jsEventArg, (Action<MouseButtonEventArgs>)OnMouseLeftButtonDown, (Action<MouseButtonEventArgs>)OnMouseLeftButtonDown_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true, checkForDivsThatAbsorbEvents: true, refreshClickCount: true);
                        });
#else
                    _pointerPressedEventManager = new INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                    {
                        ProcessPointerEvent(jsEventArg, (Action<PointerRoutedEventArgs>)OnPointerPressed, (Action<PointerRoutedEventArgs>)OnPointerPressed_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true, checkForDivsThatAbsorbEvents: true, refreshClickCount: true);
                    });
#endif
                }
                return _pointerPressedEventManager;
            }
        }

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// </summary>
#if MIGRATION
        public event MouseButtonEventHandler MouseLeftButtonDown
#else
        public event PointerEventHandler PointerPressed
#endif
        {
            add
            {
                PointerPressedEventManager.Add(value);
            }
            remove
            {
                PointerPressedEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the PointerPressed event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
#else
        protected virtual void OnPointerPressed(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            foreach (MouseButtonEventHandler handler in _pointerPressedEventManager.Handlers.ToList<MouseButtonEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerPressedEventManager.Handlers.ToList<PointerEventHandler>())
#endif
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }

            //Workaround so that the elements that capture the pointer events still get the focus:
            //Note: we put this here instead of in OnPointerPressed_ForHandledEventsToo because this takes into consideration the checkForDivsThatAbsorbEvents thing (it broke the <Button><TextBox/></Button> case for example).
            //      It might be better in certain cases to have this in the other method and especially test for those event-absorbing divs at that moment but I'm not sure.
            //      todo: check if the position of this workaround is correct in the case of an Handled event without a div that absorbs the events.
            if (this is Control)
            {
                Control thisAsControl = (Control)this;
                if (thisAsControl.IsTabStop)
                {
                    thisAsControl.Focus();
                }
            }
        }

#if MIGRATION
        void OnMouseLeftButtonDown_ForHandledEventsToo(MouseButtonEventArgs eventArgs)
#else
        void OnPointerPressed_ForHandledEventsToo(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            foreach (MouseButtonEventHandler handler in _pointerPressedEventManager.HandlersForHandledEventsToo.ToList<MouseButtonEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerPressedEventManager.HandlersForHandledEventsToo.ToList<PointerEventHandler>())
#endif
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region Pointer released event

#if MIGRATION
        public static readonly RoutedEvent MouseLeftButtonUpEvent = new RoutedEvent("MouseLeftButtonUpEvent");
#else
        public static readonly RoutedEvent PointerReleasedEvent = new RoutedEvent("PointerReleasedEvent");
#endif

#if MIGRATION
        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> _pointerReleasedEventManager;
        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> PointerReleasedEventManager
#else
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> _pointerReleasedEventManager;
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> PointerReleasedEventManager
#endif
        {
            get
            {
                if (_pointerReleasedEventManager == null)
                {
                    string[] eventsNames = { "mouseup", "touchend" };

#if MIGRATION
                    _pointerReleasedEventManager = new INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                        {
                            ProcessPointerEvent(jsEventArg, (Action<MouseButtonEventArgs>)OnMouseLeftButtonUp, (Action<MouseButtonEventArgs>)OnMouseLeftButtonUp_ForHandledEventsToo, checkForDivsThatAbsorbEvents: true);
                        });
#else
                    _pointerReleasedEventManager = new INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                    {
                        ProcessPointerEvent(jsEventArg, (Action<PointerRoutedEventArgs>)OnPointerReleased, (Action<PointerRoutedEventArgs>)OnPointerReleased_ForHandledEventsToo, checkForDivsThatAbsorbEvents: true);
                    });
#endif
                }
                return _pointerReleasedEventManager;
            }
        }

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// released, while within this element.
        /// </summary>
#if MIGRATION
        public event MouseButtonEventHandler MouseLeftButtonUp
#else
        public event PointerEventHandler PointerReleased
#endif
        {
            add
            {
                PointerReleasedEventManager.Add(value);
            }
            remove
            {
                PointerReleasedEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the PointerReleased event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>

#if MIGRATION
        protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs eventArgs)
#else
        protected virtual void OnPointerReleased(PointerRoutedEventArgs eventArgs)
#endif
        {
            //todo-perf: make sure that this is efficient (we need to use a copy because the list might be changed by the handlers.
#if MIGRATION
            foreach (MouseButtonEventHandler handler in _pointerReleasedEventManager.Handlers.ToList<MouseButtonEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerReleasedEventManager.Handlers.ToList<PointerEventHandler>())
#endif
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        void OnMouseLeftButtonUp_ForHandledEventsToo(MouseButtonEventArgs eventArgs)
#else
        void OnPointerReleased_ForHandledEventsToo(PointerRoutedEventArgs eventArgs)
#endif
        {
            //todo-perf: make sure that this is efficient (we need to use a copy because the list might be changed by the handlers.
#if MIGRATION
            foreach (MouseButtonEventHandler handler in _pointerReleasedEventManager.HandlersForHandledEventsToo.ToList<MouseButtonEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerReleasedEventManager.HandlersForHandledEventsToo.ToList<PointerEventHandler>())
#endif
            {
                handler(this, eventArgs);
            }
        }



        #endregion


        #region Pointer entered event

#if MIGRATION
        public static readonly RoutedEvent MouseEnterEvent = new RoutedEvent("MouseEnterEvent");
#else
        public static readonly RoutedEvent PointerEnteredEvent = new RoutedEvent("PointerEnteredEvent");
#endif

#if MIGRATION
        internal INTERNAL_EventManager<MouseEventHandler, MouseEventArgs> _pointerEnteredEventManager;  //note: this is internal so that we can check if it is null in the Window class, to manage the Pointer exited event for the simulator.
        INTERNAL_EventManager<MouseEventHandler, MouseEventArgs> PointerEnteredEventManager
#else
        internal INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> _pointerEnteredEventManager;  //note: this is internal so that we can check if it is null in the Window class, to manage the Pointer exited event for the simulator.
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> PointerEnteredEventManager
#endif
        {
            get
            {
                if (_pointerEnteredEventManager == null)
                {
                    string[] eventsNames = { "mouseenter" };

#if MIGRATION
                    _pointerEnteredEventManager = new INTERNAL_EventManager<MouseEventHandler, MouseEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, ProcessOnMouseEnter);
#else
                    _pointerEnteredEventManager = new INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, ProcessOnPointerEntered);
#endif
                }
                return _pointerEnteredEventManager;
            }
        }

        /// <summary>
        /// Occurs when a pointer enters the hit test area of this element.
        /// </summary>
#if MIGRATION
        public event MouseEventHandler MouseEnter
#else
        public event PointerEventHandler PointerEntered
#endif
        {
            add
            {
                PointerEnteredEventManager.Add(value);

            }
            remove
            {
                PointerEnteredEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Creates the eventArgs from the infos in the javascript's version of the event then raises the PointerEntered event
        /// </summary>
#if MIGRATION
        private void ProcessOnMouseEnter(object jsEventArg)
#else
        private void ProcessOnPointerEntered(object jsEventArg)
#endif
        {

#if MIGRATION
                ProcessPointerEvent(jsEventArg, OnMouseEnter, OnMouseEnter_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true);
#else
            ProcessPointerEvent(jsEventArg, OnPointerEntered, OnPointerEntered_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true);
#endif
        }

        /// <summary>
        /// Raises the PointerEntered event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseEnter(MouseEventArgs eventArgs)
#else
        protected virtual void OnPointerEntered(PointerRoutedEventArgs eventArgs)
#endif
        {
            //todo-perf: make sure that this is efficient (we need to use a copy because the list might be changed by the handlers.
#if MIGRATION
            foreach (MouseEventHandler handler in _pointerEnteredEventManager.Handlers.ToList<MouseEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerEnteredEventManager.Handlers.ToList<PointerEventHandler>())
#endif
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        void OnMouseEnter_ForHandledEventsToo(MouseEventArgs eventArgs)
#else
        void OnPointerEntered_ForHandledEventsToo(PointerRoutedEventArgs eventArgs)
#endif
        {
            //todo-perf: make sure that this is efficient (we need to use a copy because the list might be changed by the handlers.
#if MIGRATION
            foreach (MouseEventHandler handler in _pointerEnteredEventManager.HandlersForHandledEventsToo.ToList<MouseEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerEnteredEventManager.HandlersForHandledEventsToo.ToList<PointerEventHandler>())
#endif
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region Pointer exited event

#if MIGRATION
        public static readonly RoutedEvent MouseLeaveEvent = new RoutedEvent("MouseLeaveEvent");
#else
        public static readonly RoutedEvent PointerExitedEvent = new RoutedEvent("PointerExitedEvent");
#endif

#if MIGRATION
        internal INTERNAL_EventManager<MouseEventHandler, MouseEventArgs> _pointerExitedEventManager; //note: this is internal so that we can check if it is null in the Window class, to manage the Pointer exited event for the simulator.
        INTERNAL_EventManager<MouseEventHandler, MouseEventArgs> PointerExitedEventManager
#else
        internal INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> _pointerExitedEventManager; //note: this is internal so that we can check if it is null in the Window class, to manage the Pointer exited event for the simulator.
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> PointerExitedEventManager
#endif
        {
            get
            {
                if (_pointerExitedEventManager == null)
                {
                    string[] eventsNames = { "mouseleave" };
#if MIGRATION
                    _pointerExitedEventManager = new INTERNAL_EventManager<MouseEventHandler, MouseEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, ProcessOnMouseLeave);
#else
                    _pointerExitedEventManager = new INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, ProcessOnPointerExited);
#endif
                }
                return _pointerExitedEventManager;
            }
        }


        /// <summary>
        /// Occurs when a pointer leaves the hit test area of this element.
        /// </summary>
#if MIGRATION
        public event MouseEventHandler MouseLeave
#else
        public event PointerEventHandler PointerExited
#endif
        {
            add
            {
                StartManagingPointerPositionForPointerExitedEvent();
                PointerExitedEventManager.Add(value);
            }
            remove
            {
                PointerExitedEventManager.Remove(value);
                if (PointerExitedEventManager.Handlers.Count == 0)
                {
                    StopManagingPointerPositionForPointerExitedEvent();
                }
            }
        }

        internal bool isAlreadySubscribedToMouseEnterAndLeave = false;
        /// <summary>
        /// IMPORTANT NOTE: This boolean is only updated in the case the PointerExited/MouseLeave event is used on this element.
        /// It states whether the pointer is currently inside the UIElement.
        /// </summary>
        internal bool INTERNAL_isPointerInside = false;
        void SetIsPointerInsideToTrue() { this.INTERNAL_isPointerInside = true; }
        void StartManagingPointerPositionForPointerExitedEvent()
        {
            if (!isAlreadySubscribedToMouseEnterAndLeave && this.INTERNAL_OuterDomElement != null)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"$0.addEventListener(""mouseenter"", $1, false);", this.INTERNAL_OuterDomElement, (Action)SetIsPointerInsideToTrue);


                //CSHTML5.Interop.ExecuteJavaScript("window.subscribeToPointerEnteredAndLeft($0);", this.INTERNAL_OuterDomElement); //todo: decide whether test is we already subscribed in c# or in the window.subscribeToPointerMovedEventOnWindow method.
                isAlreadySubscribedToMouseEnterAndLeave = true;
            }
        }
        void StopManagingPointerPositionForPointerExitedEvent()
        {
            if (isAlreadySubscribedToMouseEnterAndLeave && this.INTERNAL_OuterDomElement != null)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"$0.removeEventListener(""mouseenter"", $1);", this.INTERNAL_OuterDomElement, (Action)SetIsPointerInsideToTrue);

                INTERNAL_isPointerInside = false; //don't know if this is useful but just in case.
                isAlreadySubscribedToMouseEnterAndLeave = false;
            }
        }


        internal void StartManagingPointerPositionForPointerExitedEventIfNeeded()
        {
            if (_pointerExitedEventManager != null)
            {
                StartManagingPointerPositionForPointerExitedEvent();
            }
        }

        /// <summary>
        /// Creates the eventArgs from the infos in the javascript's version of the event then raises the PointerExited event
        /// </summary>
#if MIGRATION
        internal void ProcessOnMouseLeave(object jsEventArg)
#else
        internal void ProcessOnPointerExited(object jsEventArg) //todo: why is this a different name from the SL version? it's internal so I don't see the point.
#endif
        {
#if MIGRATION
            ProcessPointerEvent(jsEventArg, OnMouseLeave, OnMouseLeave_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true);
#else
            ProcessPointerEvent(jsEventArg, OnPointerExited, OnPointerExited_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true);
#endif
        }

        /// <summary>
        /// Raises the PointerExited event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected internal virtual void OnMouseLeave(MouseEventArgs eventArgs)
#else
        protected internal virtual void OnPointerExited(PointerRoutedEventArgs eventArgs)
#endif
        {
            this.INTERNAL_isPointerInside = false;
            //todo-perf: make sure that this is efficient (we need to use a copy because the list might be changed by the handlers.
#if MIGRATION
            foreach (MouseEventHandler handler in _pointerExitedEventManager.Handlers.ToList<MouseEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerExitedEventManager.Handlers.ToList<PointerEventHandler>())
#endif
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        internal void OnMouseLeave_ForHandledEventsToo(MouseEventArgs eventArgs)
#else
        internal void OnPointerExited_ForHandledEventsToo(PointerRoutedEventArgs eventArgs)
#endif
        {
            this.INTERNAL_isPointerInside = false;
            //todo-perf: make sure that this is efficient (we need to use a copy because the list might be changed by the handlers.
#if MIGRATION
            foreach (MouseEventHandler handler in _pointerExitedEventManager.HandlersForHandledEventsToo.ToList<MouseEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerExitedEventManager.HandlersForHandledEventsToo.ToList<PointerEventHandler>())
#endif
            {
                handler(this, eventArgs);
            }
        }


        #endregion


        #region Wheel event

#if WORKINPROGRESS
#if MIGRATION
        //
        // Summary:
        //     Identifies the System.Windows.UIElement.MouseWheel routed event.
        //
        // Returns:
        //     The identifier for the System.Windows.UIElement.MouseWheel routed event.
        public static readonly RoutedEvent MouseWheelEvent = new RoutedEvent("MouseWheelEvent");

        /// <summary>
        /// Occurs when a keyboard key is pressed while the UIElement has focus.
        /// </summary>
        public event MouseWheelEventHandler MouseWheel
        {
            add
            {

            }
            remove
            {

            }
        }
#endif
        public static readonly RoutedEvent TextInputEvent;
        public static readonly RoutedEvent TextInputStartEvent;
        public static readonly RoutedEvent TextInputUpdateEvent;

        public event TextCompositionEventHandler TextInput;
        public event TextCompositionEventHandler TextInputStart;
#if MIGRATION
        public event MouseButtonEventHandler MouseRightButtonDown;
#endif

#endif

        #endregion


        #region Tapped event

        public static readonly RoutedEvent TappedEvent = new RoutedEvent("TappedEvent");

        INTERNAL_EventManager<TappedEventHandler, TappedRoutedEventArgs> _tappedEventManager;
        INTERNAL_EventManager<TappedEventHandler, TappedRoutedEventArgs> TappedEventManager
        {
            get
            {
                if (_tappedEventManager == null)
                {
                    _tappedEventManager = new INTERNAL_EventManager<TappedEventHandler, TappedRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, "mouseup", ProcessOnTapped);
                }
                return _tappedEventManager;
            }
        }

        /// <summary>
        /// Occurs when an otherwise unhandled Tap interaction occurs over the hit test
        /// area of this element.
        /// </summary>
        public event TappedEventHandler Tapped
        {
            add
            {
                TappedEventManager.Add(value);
            }
            remove
            {
                TappedEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        void ProcessOnTapped(object jsEventArg)
        {
            var eventArgs = new TappedRoutedEventArgs()
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Handled = ((CSHTML5.Interop.ExecuteJavaScript("$0.data", jsEventArg) ?? "").ToString() == "handled")
            };

            if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                eventArgs.FillEventArgs(this, jsEventArg);

                // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                if (!eventArgs.Handled)
                {
                    OnTapped(eventArgs);
                }
                OnTapped_ForHandledEventsToo(eventArgs);

                if (eventArgs.Handled)
                {
                    CSHTML5.Interop.ExecuteJavaScript("$0.data = 'handled'", jsEventArg);
                }
            }
        }

        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnTapped(TappedRoutedEventArgs eventArgs)
        {
            foreach (TappedEventHandler handler in _tappedEventManager.Handlers.ToList<TappedEventHandler>())
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

        void OnTapped_ForHandledEventsToo(TappedRoutedEventArgs eventArgs)
        {
            foreach (TappedEventHandler handler in _tappedEventManager.HandlersForHandledEventsToo.ToList<TappedEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region RightTapped (aka MouseRightButtonUp) event

#if MIGRATION
        public static readonly RoutedEvent MouseRightButtonUpEvent = new RoutedEvent("MouseRightButtonUpEvent");
#else
        public static readonly RoutedEvent RightTappedEvent = new RoutedEvent("RightTappedEvent");
#endif

#if MIGRATION
        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> _rightTappedEventManager;
        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> RightTappedEventManager
#else
        INTERNAL_EventManager<RightTappedEventHandler, RightTappedRoutedEventArgs> _rightTappedEventManager;
        INTERNAL_EventManager<RightTappedEventHandler, RightTappedRoutedEventArgs> RightTappedEventManager
#endif
        {
            get
            {
                if (_rightTappedEventManager == null)
                {
#if MIGRATION
                    _rightTappedEventManager = new INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs>(() => this.INTERNAL_OuterDomElement, "contextmenu", ProcessOnMouseRightButtonUp);
#else
                    _rightTappedEventManager = new INTERNAL_EventManager<RightTappedEventHandler, RightTappedRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, "contextmenu", ProcessOnRightTapped);
#endif
                }
                return _rightTappedEventManager;
            }
        }

        /// <summary>
        /// Occurs when a right-tap input stimulus happens while the pointer is over
        /// the element.
        /// </summary>
#if MIGRATION
        public event MouseButtonEventHandler MouseRightButtonUp
#else
        public event RightTappedEventHandler RightTapped
#endif
        {
            add
            {
                RightTappedEventManager.Add(value);
            }
            remove
            {
                RightTappedEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the RightTapped event
        /// </summary>
#if MIGRATION
        void ProcessOnMouseRightButtonUp(object jsEventArg)
#else
        void ProcessOnRightTapped(object jsEventArg)
#endif
        {
#if MIGRATION
            var eventArgs = new MouseButtonEventArgs()
#else
            var eventArgs = new RightTappedRoutedEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Handled = ((CSHTML5.Interop.ExecuteJavaScript("$0.data", jsEventArg) ?? "").ToString() == "handled")
            };

            if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                eventArgs.FillEventArgs(this, jsEventArg);

                // Prevent the default behavior (which is to show the browser context menu):
                CSHTML5.Interop.ExecuteJavaScript(@"
                    if ($0.preventDefault)
                        $0.preventDefault();", jsEventArg);

                // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                if (!eventArgs.Handled)
                {
#if MIGRATION
                    OnMouseRightButtonUp(eventArgs);
#else
                    OnRightTapped(eventArgs);
#endif
                }
#if MIGRATION
                OnMouseRightButtonUp_ForHandledEventsToo(eventArgs);
#else
                OnRightTapped_ForHandledEventsToo(eventArgs);
#endif

                if (eventArgs.Handled)
                {
                    CSHTML5.Interop.ExecuteJavaScript("$0.data = 'handled'", jsEventArg);
                }
            }
        }

        /// <summary>
        /// Raises the RightTapped event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs eventArgs)
#else
        protected virtual void OnRightTapped(RightTappedRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            foreach (MouseButtonEventHandler handler in _rightTappedEventManager.Handlers.ToList<MouseButtonEventHandler>())
#else
            foreach (RightTappedEventHandler handler in _rightTappedEventManager.Handlers.ToList<RightTappedEventHandler>())
#endif
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        void OnMouseRightButtonUp_ForHandledEventsToo(MouseButtonEventArgs eventArgs)
#else
        void OnRightTapped_ForHandledEventsToo(RightTappedRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            foreach (MouseButtonEventHandler handler in _rightTappedEventManager.Handlers.ToList<MouseButtonEventHandler>())
#else
            foreach (RightTappedEventHandler handler in _rightTappedEventManager.HandlersForHandledEventsToo.ToList<RightTappedEventHandler>())
#endif
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region KeyDown event

        public static readonly RoutedEvent KeyDownEvent = new RoutedEvent("KeyDownEvent");

#if MIGRATION
        INTERNAL_EventManager<KeyEventHandler, KeyEventArgs> _keyDownEventManager;
        INTERNAL_EventManager<KeyEventHandler, KeyEventArgs> KeyDownEventManager
#else
        INTERNAL_EventManager<KeyEventHandler, KeyRoutedEventArgs> _keyDownEventManager;
        INTERNAL_EventManager<KeyEventHandler, KeyRoutedEventArgs> KeyDownEventManager
#endif
        {
            get
            {
                if (_keyDownEventManager == null)
                {
#if MIGRATION
                    _keyDownEventManager = new INTERNAL_EventManager<KeyEventHandler, KeyEventArgs>(() => this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement, "keydown", ProcessOnKeyDown);
#else
                    _keyDownEventManager = new INTERNAL_EventManager<KeyEventHandler, KeyRoutedEventArgs>(() => this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement, "keydown", ProcessOnKeyDown);
#endif
                }
                return _keyDownEventManager;
            }
        }

        /// <summary>
        /// Occurs when a keyboard key is pressed while the UIElement has focus.
        /// </summary>
        public event KeyEventHandler KeyDown
        {
            add
            {
                KeyDownEventManager.Add(value);
            }
            remove
            {
                KeyDownEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the OnKeyDown event
        /// </summary>
        void ProcessOnKeyDown(object jsEventArg)
        {
            int keyCode = Convert.ToInt32(CSHTML5.Interop.ExecuteJavaScript("$0.keyCode", jsEventArg));

#if MIGRATION
            keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
            var eventArgs = new KeyEventArgs()
#else
            var eventArgs = new KeyRoutedEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                PlatformKeyCode = keyCode,
#if MIGRATION

                Key = (INTERNAL_VirtualKeysHelpers.IsUnknownKey(keyCode) ? Key.Unknown : (Key)keyCode),
#else
                Key = (INTERNAL_VirtualKeysHelpers.IsUnknownKey(keyCode) ? VirtualKey.Unknown : (VirtualKey)keyCode),
#endif
                Handled = ((CSHTML5.Interop.ExecuteJavaScript("$0.data", jsEventArg) ?? "").ToString() == "handled")
            };

            // Add the key modifier to the eventArgs:
            eventArgs.AddKeyModifiersAndUpdateDocumentValue(jsEventArg);

            // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
            if (!eventArgs.Handled)
            {
                OnKeyDown(eventArgs);
            }
            OnKeyDown_ForHandledEventsToo(eventArgs);

            if (eventArgs.Handled)
            {
                CSHTML5.Interop.ExecuteJavaScript("$0.data = 'handled'", jsEventArg);
            }
        }

        /// <summary>
        /// Raises the KeyDown event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnKeyDown(KeyEventArgs eventArgs)
#else
        protected virtual void OnKeyDown(KeyRoutedEventArgs eventArgs)
#endif
        {
            foreach (KeyEventHandler handler in _keyDownEventManager.Handlers.ToList<KeyEventHandler>())
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        void OnKeyDown_ForHandledEventsToo(KeyEventArgs eventArgs)
#else
        void OnKeyDown_ForHandledEventsToo(KeyRoutedEventArgs eventArgs)
#endif
        {
            foreach (KeyEventHandler handler in _keyDownEventManager.HandlersForHandledEventsToo.ToList<KeyEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region KeyUp event

        public static readonly RoutedEvent KeyUpEvent = new RoutedEvent("KeyUpEvent");

#if MIGRATION
        INTERNAL_EventManager<KeyEventHandler, KeyEventArgs> _keyUpEventManager;
        INTERNAL_EventManager<KeyEventHandler, KeyEventArgs> KeyUpEventManager
#else
        INTERNAL_EventManager<KeyEventHandler, KeyRoutedEventArgs> _keyUpEventManager;
        INTERNAL_EventManager<KeyEventHandler, KeyRoutedEventArgs> KeyUpEventManager
#endif
        {
            get
            {
                if (_keyUpEventManager == null)
                {
#if MIGRATION
                    _keyUpEventManager = new INTERNAL_EventManager<KeyEventHandler, KeyEventArgs>(() => this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement, "keyup", ProcessOnKeyUp);
#else
                    _keyUpEventManager = new INTERNAL_EventManager<KeyEventHandler, KeyRoutedEventArgs>(() => this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement, "keyup", ProcessOnKeyUp);
#endif
                }
                return _keyUpEventManager;
            }
        }

        /// <summary>
        /// Occurs when a keyboard key is released while the UIElement has focus.
        /// </summary>
        public event KeyEventHandler KeyUp
        {
            add
            {
                KeyUpEventManager.Add(value);
            }
            remove
            {
                KeyUpEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the OnKeyUp event
        /// </summary>
        void ProcessOnKeyUp(object jsEventArg)
        {
            int keyCode = Convert.ToInt32(CSHTML5.Interop.ExecuteJavaScript("$0.keyCode", jsEventArg));

#if MIGRATION
            keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
            var eventArgs = new KeyEventArgs()
#else
            var eventArgs = new KeyRoutedEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                PlatformKeyCode = keyCode,
#if MIGRATION
                Key = (INTERNAL_VirtualKeysHelpers.IsUnknownKey(keyCode) ? Key.Unknown : (Key)keyCode),
#else
                Key = (INTERNAL_VirtualKeysHelpers.IsUnknownKey(keyCode) ? VirtualKey.Unknown : (VirtualKey)keyCode),
#endif
                Handled = ((CSHTML5.Interop.ExecuteJavaScript("$0.data", jsEventArg) ?? "").ToString() == "handled")
            };

            // Add the key modifier to the eventArgs:
            eventArgs.AddKeyModifiersAndUpdateDocumentValue(jsEventArg);

            // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
            if (!eventArgs.Handled)
            {
                OnKeyUp(eventArgs);
            }
            OnKeyUp_ForHandledEventsToo(eventArgs);

            if (eventArgs.Handled)
            {
                CSHTML5.Interop.ExecuteJavaScript("$0.data = 'handled'", jsEventArg);
            }
        }

        /// <summary>
        /// Raises the KeyUp event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnKeyUp(KeyEventArgs eventArgs)
#else
        protected virtual void OnKeyUp(KeyRoutedEventArgs eventArgs)
#endif
        {
            foreach (KeyEventHandler handler in _keyUpEventManager.Handlers.ToList<KeyEventHandler>())
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        void OnKeyUp_ForHandledEventsToo(KeyEventArgs eventArgs)
#else
        void OnKeyUp_ForHandledEventsToo(KeyRoutedEventArgs eventArgs)
#endif
        {
            foreach (KeyEventHandler handler in _keyUpEventManager.HandlersForHandledEventsToo.ToList<KeyEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region GotFocus event

        public static readonly RoutedEvent GotFocusEvent = new RoutedEvent("GotFocusEvent");

        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> _gotFocusEventManager;
        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> GotFocusEventManager
        {
            get
            {
                if (_gotFocusEventManager == null)
                {
                    _gotFocusEventManager = new INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs>(() => (this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement), "focusin", ProcessOnGotFocus);
                }
                return _gotFocusEventManager;
            }
        }

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// Note that ONLY sender's informations are currently filled (not pointer's)
        /// </summary>
        public event RoutedEventHandler GotFocus //todo: fill everything and remove the note above
        {
            add
            {
                GotFocusEventManager.Add(value);
            }
            remove
            {
                GotFocusEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        void ProcessOnGotFocus(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs() //todo: fill the rest
            {
                INTERNAL_OriginalJSEventArg = jsEventArg
            };
            OnGotFocus(eventArgs); //todo: should we skip this method if "handled" is true? (test by overriding "OnGotFocus" method below and see how it works in this case in WPF)
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnGotFocus(RoutedEventArgs eventArgs)
        {
            foreach (RoutedEventHandler handler in _gotFocusEventManager.Handlers.ToList<RoutedEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region Lostfocus event

        public static readonly RoutedEvent LostFocusEvent = new RoutedEvent("LostFocusEvent");

        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> _lostFocusEventManager;
        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> LostFocusEventManager
        {
            get
            {
                if (_lostFocusEventManager == null)
                {
                    _lostFocusEventManager = new INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs>(() => (this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement), "focusout", ProcessOnLostFocus);
                }
                return _lostFocusEventManager;
            }
        }

        /// <summary>
        /// Occurs when a UIElement loses focus.
        /// </summary>
        public event RoutedEventHandler LostFocus
        {
            add
            {
                LostFocusEventManager.Add(value);
            }
            remove
            {
                LostFocusEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        void ProcessOnLostFocus(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs() //todo: fill the rest
            {
                INTERNAL_OriginalJSEventArg = jsEventArg
            };
            OnLostFocus(eventArgs); //todo: should we skip this method if "handled" is true? (test by overriding "OnLostFocus" method below and see how it works in this case in WPF)
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnLostFocus(RoutedEventArgs eventArgs)
        {
            foreach (RoutedEventHandler handler in _lostFocusEventManager.Handlers.ToList<RoutedEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region Handle keyboard events when focused

        /// <summary>
        /// Set this boolean to true for the UIElements that have to react to keyboard events when they are focused.
        /// It activates the mechanic used to handle those events.
        /// It is used with the method OnKeyDownWhenFocused which is overriden and does the specific handling of the events.
        /// </summary>
        internal bool _reactsToKeyboardEventsWhenFocused = false;

        void StartListeningToKeyboardEvents(object sender, RoutedEventArgs e)
        {
            //the following test is for cases such as focusing a TextBox that is inside a Button for example.
            if (!Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("document.checkForDivsThatAbsorbEvents($0)", e.INTERNAL_OriginalJSEventArg)))
            {
                this.KeyDown -= OnKeyDownWhenFocused; //just in case but we shouldn't need it (if we need it here, it means that the keyboard events kept getting taken into consideration even though it didn't have the focus).
                this.KeyDown += OnKeyDownWhenFocused;
            }
        }

        void StopListeningToKeyboardEvents(object sender, RoutedEventArgs e)
        {
            this.KeyDown -= OnKeyDownWhenFocused; //just in case but we shouldn't need it (if we need it here, it means that the button kept getting triggered by keyboard events even though it didn't have the focus).
        }

        /// <summary>
        /// This method is to be overriden by UIElements that react to keyboard events when they are focused, to handle those events properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#if MIGRATION
        internal virtual void OnKeyDownWhenFocused(object sender, KeyEventArgs e)
#else
        internal virtual void OnKeyDownWhenFocused(object sender, KeyRoutedEventArgs e)
#endif
        {
            //if(e.Key == ...)
            //{
            //    Do something...
            //}
        }

        #endregion


        #region Allow or Prevent Focus events for IsTabStop

        //todo-perf: use the code that is in the "first try at this" region rather than creating a full-fledged event and find out why it doesn't work (going from IsTabStop = false to IsTabStop = true doesn't succeed to remove the listener that calls blur on the dom element and it is thus for the element to get the focus despite having IsTabStop to true).

        #region GotFocusForIsTabStop event

        static readonly RoutedEvent GotFocusForIsTabStopEvent = new RoutedEvent("GotFocusForIsTabStopEvent");

        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> _gotFocusForIsTabStopEventManager;
        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> GotFocusForIsTabStopEventManager
        {
            get
            {
                if (_gotFocusForIsTabStopEventManager == null)
                {
                    _gotFocusForIsTabStopEventManager = new INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs>(() => (this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement), "focusin", ProcessOnGotFocusForIsTabStop);
                }
                return _gotFocusForIsTabStopEventManager;
            }
        }

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// Note that ONLY sender's informations are currently filled (not pointer's)
        /// </summary>
        private event RoutedEventHandler GotFocusForIsTabStop 
        {
            add
            {
                GotFocusForIsTabStopEventManager.Add(value);
            }
            remove
            {
                GotFocusForIsTabStopEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        void ProcessOnGotFocusForIsTabStop(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs() //todo: fill the rest
            {
                INTERNAL_OriginalJSEventArg = jsEventArg
            };
            OnGotFocusForIsTabStop(eventArgs); //todo: should we skip this method if "handled" is true? (test by overriding "OnGotFocus" method below and see how it works in this case in WPF)
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        private void OnGotFocusForIsTabStop(RoutedEventArgs eventArgs)
        {
            foreach (RoutedEventHandler handler in _gotFocusForIsTabStopEventManager.Handlers.ToList<RoutedEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        #endregion

        private bool INTERNAL_AreFocusEventsAllowed = true;

        internal void PreventFocusEvents()
        {
            if (INTERNAL_AreFocusEventsAllowed)
            {
                INTERNAL_AreFocusEventsAllowed = false;

                INTERNAL_DetachFromFocusEvents();

                GotFocusForIsTabStop -= UIElement_GotFocusForIsTabStop; //just in case.
                GotFocusForIsTabStop += UIElement_GotFocusForIsTabStop;
                //var domElementConcernedByFocus = INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? INTERNAL_OuterDomElement;

                //_preventFocusProxy = INTERNAL_EventsHelper.AttachToDomEvents("focusin", domElementConcernedByFocus, (Action<object>)(jsEventArg =>
                //{
                //    PreventFocus(jsEventArg);
                //}));
            }
        }

        void UIElement_GotFocusForIsTabStop(object sender, RoutedEventArgs e)
        {
            UIElement element = (UIElement)sender; //jsEvent should be called "sender" but I kept the former implementation so I also kept the name.
            var elementToBlur = element.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? element.INTERNAL_OuterDomElement;
            if (elementToBlur != null)
                CSHTML5.Interop.ExecuteJavaScript(@"$0.blur()", elementToBlur);
        }

        internal void AllowFocusEvents()
        {
            if (!INTERNAL_AreFocusEventsAllowed)
            {
                 INTERNAL_AreFocusEventsAllowed = true;

                INTERNAL_AttachToFocusEvents();
                GotFocusForIsTabStop -= UIElement_GotFocusForIsTabStop; //just in case.


                //_preventFocusProxy.Dispose();
                //var domElementConcernedByFocus = INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? INTERNAL_OuterDomElement;

                //INTERNAL_EventsHelper.DetachEvent("focusin", domElementConcernedByFocus, _preventFocusProxy, (Action<object>)PreventFocus);
            }
        }

        /// <summary>
        /// DO NOT USE THIS, IT IS ONLY FOR THE ALLOWFOCUSEVENTS METHOD
        /// </summary>
        private void INTERNAL_AttachToFocusEvents()
        {
            if (_gotFocusEventManager != null)
            {
                _gotFocusEventManager.AttachToDomEvents();
            }
            if (_lostFocusEventManager != null)
            {
                _lostFocusEventManager.AttachToDomEvents();
            }
        }

        /// <summary>
        /// DO NOT USE THIS, IT IS ONLY FOR THE PREVENTFOCUSEVENTS METHOD
        /// </summary>
        private void INTERNAL_DetachFromFocusEvents()
        {
            if (_gotFocusEventManager != null)
            {
                _gotFocusEventManager.DetachFromDomEvents();
            }
            if (_lostFocusEventManager != null)
            {
                _lostFocusEventManager.DetachFromDomEvents();
            }
        }

        #region first try at this (would be better than the current one but It doesn't work for whatever reason).

//        private HtmlEventProxy _preventFocusProxy = null;

//        private static void PreventFocus(object jsEvent)
//        {
//            //Note: this is not ideal because TextBoxes still have a flicker of Focus in the simulator and it is possible to type something during that flicker (there is a frame where the textBox did get the focus).
//            //          fortunately, it seems that we cannot do the same in browsers (at least chrome)
//            //      and pressing tab afterwards brings to the element after the one that we prevented from being focused while in SL it goes back to the first of the page.
//            //          I consider this acceptable (at least for now) because it is more like I would have expected.
//            CSHTML5.Interop.ExecuteJavaScript(@"
//        var v = $0.target || $0.srcElement
//        v.blur()", jsEvent);
//        }

//        internal void PreventFocusEvents()
//        {
//            if (INTERNAL_AreFocusEventsAllowed)
//            {
//                INTERNAL_AreFocusEventsAllowed = false;

//                INTERNAL_DetachFromFocusEvents();


//                var domElementConcernedByFocus = INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? INTERNAL_OuterDomElement;

//                _preventFocusProxy = INTERNAL_EventsHelper.AttachToDomEvents("focusin", domElementConcernedByFocus, (Action<object>)(jsEventArg =>
//                {
//                    PreventFocus(jsEventArg);
//                }));
//            }
//        }

//        internal void AllowFocusEvents()
//        {
//            if (!INTERNAL_AreFocusEventsAllowed)
//            {
//                INTERNAL_AreFocusEventsAllowed = true;

//                INTERNAL_AttachToFocusEvents();
//                _preventFocusProxy.Dispose();
//            }
//        }

        #endregion

        #endregion

        void ProcessPointerEvent(
            object jsEventArg,
#if MIGRATION
            Action<MouseButtonEventArgs> onEvent,
            Action<MouseButtonEventArgs> onEvent_ForHandledEventsToo,
#else
 Action<PointerRoutedEventArgs> onEvent,
            Action<PointerRoutedEventArgs> onEvent_ForHandledEventsToo,
#endif
 bool preventTextSelectionWhenPointerIsCaptured = false,
            bool checkForDivsThatAbsorbEvents = false,  //Note: this is currently true only for PointerPressed and PointerReleased
            //because those are the events we previously attached ourselves to for TextBox
            //so that it would set the event to handled to prevent the click in a TextBox (to change the text) located
            //in a button or any other control that reacts to clicks from also triggering the click from that control
            bool refreshClickCount = false)
        {
#if MIGRATION
            var eventArgs = new MouseButtonEventArgs()
#else
            var eventArgs = new PointerRoutedEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Handled = ((CSHTML5.Interop.ExecuteJavaScript("$0.data", jsEventArg) ?? "").ToString() == "handled")
            };

            if (!eventArgs.Handled && checkForDivsThatAbsorbEvents)
            {
                eventArgs.Handled = Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("document.checkForDivsThatAbsorbEvents($0)", jsEventArg));
            }

            if (refreshClickCount)
            {
                eventArgs.RefreshClickCount(this);
            }

            if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                eventArgs.FillEventArgs(this, jsEventArg);

                // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                if (!eventArgs.Handled)
                {
                    onEvent(eventArgs);
                }
                onEvent_ForHandledEventsToo(eventArgs);

                if (eventArgs.Handled)
                {
                    CSHTML5.Interop.ExecuteJavaScript("$0.data = 'handled'", jsEventArg);
                }
            }

            // If the pointer is captured, prevent the text selection during a drag operation (cf https://stackoverflow.com/questions/5429827/how-can-i-prevent-text-element-selection-with-cursor-drag)
            if (preventTextSelectionWhenPointerIsCaptured && Pointer.INTERNAL_captured != null)
            {
#if !CSHTML5NETSTANDARD
                if (IsRunningInJavaScript())
#endif
                    CSHTML5.Interop.ExecuteJavaScript(@"
                    if ($0.preventDefault)
                        $0.preventDefault();", jsEventArg);
#if !CSHTML5NETSTANDARD
                else
                {
                    //The current version of the simulator browser requires to clear the selection manually //todo: Fix this when replacing the simulator browser with a newer version (at the time of writing current version was chrome 18)
                    CSHTML5.Interop.ExecuteJavaScript(@"window.getSelection().removeAllRanges()");
                }
#endif
            }
        }




        public virtual void INTERNAL_AttachToDomEvents()
        {
            if (_reactsToKeyboardEventsWhenFocused)
            {
                GotFocus -= StartListeningToKeyboardEvents;
                GotFocus += StartListeningToKeyboardEvents;
                LostFocus -= StopListeningToKeyboardEvents;
                LostFocus += StopListeningToKeyboardEvents;
            }

            if (_pointerMovedEventManager != null)
            {
                _pointerMovedEventManager.AttachToDomEvents();
            }
            if (_pointerPressedEventManager != null)
            {
                _pointerPressedEventManager.AttachToDomEvents();
            }
            if (_pointerReleasedEventManager != null)
            {
                _pointerReleasedEventManager.AttachToDomEvents();
            }
            if (_pointerEnteredEventManager != null)
            {
                _pointerEnteredEventManager.AttachToDomEvents();
            }
            if (_pointerExitedEventManager != null)
            {
                _pointerExitedEventManager.AttachToDomEvents();
            }
            if (_tappedEventManager != null)
            {
                _tappedEventManager.AttachToDomEvents();
            }
            if (_rightTappedEventManager != null)
            {
                _rightTappedEventManager.AttachToDomEvents();
            }
            if (_keyDownEventManager != null)
            {
                _keyDownEventManager.AttachToDomEvents();
            }
            if (_keyUpEventManager != null)
            {
                _keyUpEventManager.AttachToDomEvents();
            }
            if (_gotFocusEventManager != null)
            {
                _gotFocusEventManager.AttachToDomEvents();
            }
            if (_lostFocusEventManager != null)
            {
                _lostFocusEventManager.AttachToDomEvents();
            }
        }

        public virtual void INTERNAL_DetachFromDomEvents()
        {
            if (_pointerMovedEventManager != null)
            {
                _pointerMovedEventManager.DetachFromDomEvents();
            }
            if (_pointerPressedEventManager != null)
            {
                _pointerPressedEventManager.DetachFromDomEvents();
            }
            if (_pointerReleasedEventManager != null)
            {
                _pointerReleasedEventManager.DetachFromDomEvents();
            }
            if (_pointerEnteredEventManager != null)
            {
                _pointerEnteredEventManager.DetachFromDomEvents();
            }
            if (_pointerExitedEventManager != null)
            {
                _pointerExitedEventManager.DetachFromDomEvents();
            }
            if (_tappedEventManager != null)
            {
                _tappedEventManager.DetachFromDomEvents();
            }
            if (_rightTappedEventManager != null)
            {
                _rightTappedEventManager.DetachFromDomEvents();
            }
            if (_keyDownEventManager != null)
            {
                _keyDownEventManager.DetachFromDomEvents();
            }
            if (_keyUpEventManager != null)
            {
                _keyUpEventManager.DetachFromDomEvents();
            }
            if (_gotFocusEventManager != null)
            {
                _gotFocusEventManager.DetachFromDomEvents();
            }
            if (_lostFocusEventManager != null)
            {
                _lostFocusEventManager.DetachFromDomEvents();
            }
        }




        /// <summary>
        /// Adds a routed event handler for a specified routed event, adding the handler
        /// to the handler collection on the current element. Specify handledEventsToo
        /// as true to have the provided handler be invoked for a routed event case that
        /// had already been marked as handled by another element along the event route.
        /// </summary>
        /// <param name="routedEvent">An identifier for the routed event to be handled.</param>
        /// <param name="handler">A reference to the handler implementation.</param>
        /// <param name="handledEventsToo">
        /// True to register the handler such that it is invoked even when the routed
        /// event is marked handled in its event data. False to register the handler
        /// with the default condition that it will not be invoked if the routed event
        /// is already marked handled. The default is false.
        /// </param>
        public void AddHandler(RoutedEvent routedEvent, object handler, bool handledEventsToo)
        {
#if MIGRATION
            if (routedEvent == UIElement.MouseMoveEvent)
#else
            if (routedEvent == UIElement.PointerMovedEvent)
#endif
            {
#if MIGRATION
                PointerMovedEventManager.Add((MouseEventHandler)handler, handledEventsToo: handledEventsToo);
#else
                PointerMovedEventManager.Add((PointerEventHandler)handler, handledEventsToo: handledEventsToo);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseLeftButtonDownEvent)
#else
            else if (routedEvent == UIElement.PointerPressedEvent)
#endif
            {
#if MIGRATION
                PointerPressedEventManager.Add((MouseButtonEventHandler)handler, handledEventsToo: handledEventsToo);
#else
                PointerPressedEventManager.Add((PointerEventHandler)handler, handledEventsToo: handledEventsToo);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseLeftButtonUpEvent)
#else
            else if (routedEvent == UIElement.PointerReleasedEvent)
#endif
            {
#if MIGRATION
                PointerReleasedEventManager.Add((MouseButtonEventHandler)handler, handledEventsToo: handledEventsToo);
#else
                PointerReleasedEventManager.Add((PointerEventHandler)handler, handledEventsToo: handledEventsToo);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseEnterEvent)
#else
            else if (routedEvent == UIElement.PointerEnteredEvent)
#endif
            {
#if MIGRATION
                PointerEnteredEventManager.Add((MouseEventHandler)handler, handledEventsToo: handledEventsToo);
#else
                PointerEnteredEventManager.Add((PointerEventHandler)handler, handledEventsToo: handledEventsToo);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseLeaveEvent)
#else
            else if (routedEvent == UIElement.PointerExitedEvent)
#endif
            {
#if MIGRATION
                PointerExitedEventManager.Add((MouseEventHandler)handler, handledEventsToo: handledEventsToo);
#else
                PointerExitedEventManager.Add((PointerEventHandler)handler, handledEventsToo: handledEventsToo);
#endif
            }
            else if (routedEvent == UIElement.TappedEvent)
            {
                TappedEventManager.Add((TappedEventHandler)handler, handledEventsToo: handledEventsToo);
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseRightButtonUpEvent)
#else
            else if (routedEvent == UIElement.RightTappedEvent)
#endif
            {
#if MIGRATION
                RightTappedEventManager.Add((MouseButtonEventHandler)handler, handledEventsToo: handledEventsToo);
#else
                RightTappedEventManager.Add((RightTappedEventHandler)handler, handledEventsToo: handledEventsToo);
#endif
            }
            else if (routedEvent == UIElement.KeyDownEvent)
            {
                KeyDownEventManager.Add((KeyEventHandler)handler, handledEventsToo: handledEventsToo);
            }
            else if (routedEvent == UIElement.KeyUpEvent)
            {
                KeyUpEventManager.Add((KeyEventHandler)handler, handledEventsToo: handledEventsToo);
            }
            else if (routedEvent == UIElement.GotFocusEvent)
            {
                GotFocusEventManager.Add((RoutedEventHandler)handler, handledEventsToo: handledEventsToo);
            }
            else if (routedEvent == UIElement.LostFocusEvent)
            {
                LostFocusEventManager.Add((RoutedEventHandler)handler, handledEventsToo: handledEventsToo);
            }
            else
            {
                throw new NotSupportedException("The following routed event cannot be used in the AddHandler method: " + routedEvent.ToString() + " - Please contact support.");
            }
        }

        /// <summary>
        /// Removes the specified routed event handler from this UIElement. Typically
        /// the handler in question was added by AddHandler.
        /// </summary>
        /// <param name="routedEvent">The identifier of the routed event for which the handler is attached.</param>
        /// <param name="handler">The specific handler implementation to remove from the event handler collection
        /// on this UIElement.</param>
        public void RemoveHandler(RoutedEvent routedEvent, object handler)
        {
#if MIGRATION
            if (routedEvent == UIElement.MouseMoveEvent)
#else
            if (routedEvent == UIElement.PointerMovedEvent)
#endif
            {
#if MIGRATION
                PointerMovedEventManager.Remove((MouseEventHandler)handler);
#else
                PointerMovedEventManager.Remove((PointerEventHandler)handler);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseLeftButtonDownEvent)
#else
            else if (routedEvent == UIElement.PointerPressedEvent)
#endif
            {
#if MIGRATION
                PointerPressedEventManager.Remove((MouseButtonEventHandler)handler);
#else
                PointerPressedEventManager.Remove((PointerEventHandler)handler);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseLeftButtonUpEvent)
#else
            else if (routedEvent == UIElement.PointerReleasedEvent)
#endif
            {
#if MIGRATION
                PointerReleasedEventManager.Remove((MouseButtonEventHandler)handler);
#else
                PointerReleasedEventManager.Remove((PointerEventHandler)handler);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseEnterEvent)
#else
            else if (routedEvent == UIElement.PointerEnteredEvent)
#endif
            {
#if MIGRATION
                PointerEnteredEventManager.Remove((MouseEventHandler)handler);
#else
                PointerEnteredEventManager.Remove((PointerEventHandler)handler);
#endif
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseLeaveEvent)
#else
            else if (routedEvent == UIElement.PointerExitedEvent)
#endif
            {
#if MIGRATION
                PointerExitedEventManager.Remove((MouseEventHandler)handler);
#else
                PointerExitedEventManager.Remove((PointerEventHandler)handler);
#endif
            }
            else if (routedEvent == UIElement.TappedEvent)
            {
                TappedEventManager.Remove((TappedEventHandler)handler);
            }
#if MIGRATION
            else if (routedEvent == UIElement.MouseRightButtonUpEvent)
#else
            else if (routedEvent == UIElement.RightTappedEvent)
#endif
            {
#if MIGRATION
                RightTappedEventManager.Remove((MouseButtonEventHandler)handler);
#else
                RightTappedEventManager.Remove((RightTappedEventHandler)handler);
#endif
            }
            else if (routedEvent == UIElement.KeyDownEvent)
            {
                KeyDownEventManager.Remove((KeyEventHandler)handler);
            }
            else if (routedEvent == UIElement.KeyUpEvent)
            {
                KeyUpEventManager.Remove((KeyEventHandler)handler);
            }
            else if (routedEvent == UIElement.GotFocusEvent)
            {
                GotFocusEventManager.Remove((RoutedEventHandler)handler);
            }
            else if (routedEvent == UIElement.LostFocusEvent)
            {
                LostFocusEventManager.Remove((RoutedEventHandler)handler);
            }
            else
            {
                throw new NotSupportedException("The following routed event cannot be used in the RemoveHandler method: " + routedEvent.ToString() + " - Please contact support.");
            }
        }


    }
}