

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
using System.Windows.Threading;
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

        static bool ignoreMouseEvents = false; // This boolean is useful because we want to ignore mouse events when touch events have happened so the same user inputs are not handled twice. (Note: when using touch events, the browsers fire the touch events at the moment of the action, then throw the mouse events once touchend is fired)
        private static DispatcherTimer _ignoreMouseEventsTimer = null;
        private void _ignoreMouseEventsTimer_Tick(object sender, object e)
        {
            ignoreMouseEvents = false;
            _ignoreMouseEventsTimer.Stop();
        }

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
                    string[] eventsNames = new string[] { "mousemove", "touchmove" };
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
            if (_pointerMovedEventManager == null)
                return;
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
            if (_pointerMovedEventManager == null)
                return;
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
                    string[] eventsNames = new string[] { "mousedown", "touchstart" };
#if MIGRATION
                    _pointerPressedEventManager = new INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                        {
                            /*
                            We shouldn't trigger OnMouseLeftButtonDown if only right mouse button has been triggered, continue as before otherwise

                            Javascript Mouse events have a buttons property that can be a bitmask of:

                            0 : No button or un-initialized
                            1 : Primary button (usually the left button)
                            2 : Secondary button (usually the right button)
                            4 : Auxiliary button (usually the mouse wheel button or middle button)
                            8 : 4th button (typically the "Browser Back" button)
                            16 : 5th button (typically the "Browser Forward" button)*/
                            int mouseBtn = 0;
                            int.TryParse((CSHTML5.Interop.ExecuteJavaScript("$0.buttons", jsEventArg) ?? 0).ToString(), out mouseBtn);
                            if (mouseBtn != 2)
                            {
                                ProcessPointerEvent(jsEventArg, (Action<MouseButtonEventArgs>)OnMouseLeftButtonDown, (Action<MouseButtonEventArgs>)OnMouseLeftButtonDown_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true, checkForDivsThatAbsorbEvents: true, refreshClickCount: true);
                            }
                        });
#else
                    _pointerPressedEventManager = new INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                    {
                        int mouseBtn = 0;
                        int.TryParse((CSHTML5.Interop.ExecuteJavaScript("$0.buttons", jsEventArg) ?? 0).ToString(), out mouseBtn);
                        if (mouseBtn != 2)
                        {
                            ProcessPointerEvent(jsEventArg, (Action<PointerRoutedEventArgs>)OnPointerPressed, (Action<PointerRoutedEventArgs>)OnPointerPressed_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true, checkForDivsThatAbsorbEvents: true, refreshClickCount: true);
                        }
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
            if (_pointerPressedEventManager == null)
                return;
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
            if (_pointerPressedEventManager == null)
                return;
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

        #region MouseRightButtonDown (no equivalent in UWP)

#if MIGRATION

        /// <summary>
        /// Identifies the System.Windows.UIElement.MouseRightButtonDown routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent MouseRightButtonDownEvent = new RoutedEvent("MouseRightButtonDownEvent");

        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> _mouseRightButtonDownEventManager;
        INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs> MouseRightButtonDownEventManager
        {
            get
            {
                if (_mouseRightButtonDownEventManager == null)
                {
                    string[] eventsNames = new string[] { "mousedown", "touchstart" };
                    _mouseRightButtonDownEventManager = new INTERNAL_EventManager<MouseButtonEventHandler, MouseButtonEventArgs>(() => this.INTERNAL_OuterDomElement, eventsNames, (jsEventArg) =>
                    {
                        /*
                        We trigger OnMouseRightButtonDown only if right mouse button has been triggered.

                        Javascript Mouse events have a buttons property that can be a bitmask of:

                        0 : No button or un-initialized
                        1 : Primary button (usually the left button)
                        2 : Secondary button (usually the right button)
                        4 : Auxiliary button (usually the mouse wheel button or middle button)
                        8 : 4th button (typically the "Browser Back" button)
                        16 : 5th button (typically the "Browser Forward" button)*/
                        int mouseBtn = 0;
                        int.TryParse((CSHTML5.Interop.ExecuteJavaScript("$0.buttons", jsEventArg) ?? 0).ToString(), out mouseBtn);
                        if (mouseBtn == 2)
                        {
                            ProcessPointerEvent(jsEventArg, (Action<MouseButtonEventArgs>)OnMouseRightButtonDown, (Action<MouseButtonEventArgs>)OnMouseRightButtonDown_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: true, checkForDivsThatAbsorbEvents: true, refreshClickCount: true);
                        }
                    });
                }
                return _mouseRightButtonDownEventManager;
            }
        }


        public event MouseButtonEventHandler MouseRightButtonDown
        {
            add
            {
                MouseRightButtonDownEventManager.Add(value);
            }
            remove
            {
                MouseRightButtonDownEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the MouseRightButtonDown event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs eventArgs)
        {
            if (_mouseRightButtonDownEventManager == null)
                return;
            foreach (MouseButtonEventHandler handler in _mouseRightButtonDownEventManager.Handlers.ToList<MouseButtonEventHandler>())
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

        void OnMouseRightButtonDown_ForHandledEventsToo(MouseButtonEventArgs eventArgs)
        {
            if (_mouseRightButtonDownEventManager == null)
                return;
            foreach (MouseButtonEventHandler handler in _mouseRightButtonDownEventManager.HandlersForHandledEventsToo.ToList<MouseButtonEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

#endif

        #endregion

        #region PointerWheelChanged event (or MouseWheel)

#if MIGRATION
        /// <summary>
        /// Identifies the <see cref="UIElement.MouseWheel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseWheelEvent = new RoutedEvent("MouseWheelEvent");
#else
        /// <summary>
        /// Identifies the <see cref="UIElement.PointerWheelChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PointerWheelChangedEvent = new RoutedEvent("PointerWheelChangedEvent");
#endif


#if MIGRATION
        INTERNAL_EventManager<MouseWheelEventHandler, MouseWheelEventArgs> _pointerWheelChangedEventManager;
        INTERNAL_EventManager<MouseWheelEventHandler, MouseWheelEventArgs> PointerWheelChangedEventManager
#else
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> _pointerWheelChangedEventManager;
        INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs> PointerWheelChangedEventManager
#endif
        {
            get
            {
                if (_pointerWheelChangedEventManager == null)
                {
#if MIGRATION
                    _pointerWheelChangedEventManager = new INTERNAL_EventManager<MouseWheelEventHandler, MouseWheelEventArgs>(() => this.INTERNAL_OuterDomElement, "wheel", ProcessOnPointerWheelChangedEvent);
#else
                    _pointerWheelChangedEventManager = new INTERNAL_EventManager<PointerEventHandler, PointerRoutedEventArgs>(() => this.INTERNAL_OuterDomElement, "wheel", (jsEventArg) =>
                    {
                        ProcessPointerEvent(jsEventArg, (Action<PointerRoutedEventArgs>)OnPointerWheelChanged, (Action<PointerRoutedEventArgs>)OnPointerWheelChanged_ForHandledEventsToo, preventTextSelectionWhenPointerIsCaptured: false, checkForDivsThatAbsorbEvents: false, refreshClickCount: false);
                    });

#endif
                }
                return _pointerWheelChangedEventManager;

            }
        }

#if MIGRATION
        /// <summary>
        /// Occurs when the user rotates the mouse wheel while the mouse pointer is over
        /// a <see cref="UIElement"/>, or the <see cref="UIElement"/> has focus.
        /// </summary>
        public event MouseWheelEventHandler MouseWheel
#else
        /// <summary>
        /// Occurs when the user rotates the mouse wheel while the mouse pointer is over
        /// a <see cref="UIElement"/>, or the <see cref="UIElement"/> has focus.
        /// </summary>
        public event PointerEventHandler PointerWheelChanged
#endif
        {
            add
            {
                PointerWheelChangedEventManager.Add(value);
            }
            remove
            {
                PointerWheelChangedEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the PointerWheelChanged event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseWheel(MouseWheelEventArgs eventArgs)
#else
        protected virtual void OnPointerWheelChanged(PointerRoutedEventArgs eventArgs)
#endif
        {
            if (_pointerWheelChangedEventManager == null)
                return;
#if MIGRATION
            foreach (MouseWheelEventHandler handler in _pointerWheelChangedEventManager.Handlers.ToList<MouseWheelEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerWheelChangedEventManager.Handlers.ToList<PointerEventHandler>())
#endif
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        void OnMouseWheel_ForHandledEventsToo(MouseWheelEventArgs eventArgs)
#else
        void OnPointerWheelChanged_ForHandledEventsToo(PointerRoutedEventArgs eventArgs)
#endif
        {
            if (_pointerWheelChangedEventManager == null)
                return;
#if MIGRATION
            foreach (MouseWheelEventHandler handler in _pointerWheelChangedEventManager.HandlersForHandledEventsToo.ToList<MouseWheelEventHandler>())
#else
            foreach (PointerEventHandler handler in _pointerWheelChangedEventManager.HandlersForHandledEventsToo.ToList<PointerEventHandler>())
#endif
            {
                handler(this, eventArgs);
            }
        }

#if MIGRATION
        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        void ProcessOnPointerWheelChangedEvent(object jsEventArg)
        {
            var eventArgs = new MouseWheelEventArgs()
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Handled = ((CSHTML5.Interop.ExecuteJavaScript("$0.data", jsEventArg) ?? "").ToString() == "handled")
            };

            if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                eventArgs.FillEventArgs(this, jsEventArg);

                //fill the Mouse Wheel delta:
                eventArgs.Delta = MouseWheelEventArgs.GetPointerWheelDelta(jsEventArg);

                // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                if (!eventArgs.Handled)
                {
                    OnMouseWheel(eventArgs);
                }
                OnMouseWheel_ForHandledEventsToo(eventArgs);

                if (eventArgs.Handled)
                {
                    CSHTML5.Interop.ExecuteJavaScript("$0.data = 'handled'", jsEventArg);
                }
            }
        }
#endif

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
                    string[] eventsNames = new string[] { "mouseup", "touchend" };

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
            if (_pointerReleasedEventManager == null)
                return;
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
            if (_pointerReleasedEventManager == null)
                return;
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
            if (_pointerEnteredEventManager == null)
                return;
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
            if (_pointerEnteredEventManager == null)
                return;
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
            if (_pointerExitedEventManager == null)
                return;
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
            if (_pointerExitedEventManager == null)
                return;
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


        #region Text events

        /// <summary>
        /// Identifies the <see cref="TextInput"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent TextInputEvent = new RoutedEvent("TextInputEvent");

        /// <summary>
        /// Identifies the <see cref="TextInputStart"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent TextInputStartEvent = new RoutedEvent("TextInputStartEvent");

        /// <summary>
        /// Identifies the <see cref="TextInputUpdate"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent TextInputUpdateEvent = new RoutedEvent("TextInputUpdateEvent");

        /// <summary>
        /// Occurs when a UI element gets text in a device-independent manner.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event TextCompositionEventHandler TextInput;

        /// <summary>
        /// Occurs when a UI element initially gets text in a device-independent manner.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event TextCompositionEventHandler TextInputStart;

        /// <summary>
        /// Occurs when text continues to be composed via an input method editor (IME).
        /// </summary>
        [OpenSilver.NotImplemented]
        public event TextCompositionEventHandler TextInputUpdate;

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
            if (_tappedEventManager == null)
                return;
            foreach (TappedEventHandler handler in _tappedEventManager.Handlers.ToList<TappedEventHandler>())
            {
                if (eventArgs.Handled)
                    break;
                handler(this, eventArgs);
            }
        }

        void OnTapped_ForHandledEventsToo(TappedRoutedEventArgs eventArgs)
        {
            if (_tappedEventManager == null)
                return;
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
            if (_rightTappedEventManager == null)
                return;
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
            if (_rightTappedEventManager == null)
                return;
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
                Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
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
                CSHTML5.Interop.ExecuteJavaScript("$0.preventDefault()", jsEventArg);
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
            if (_keyDownEventManager == null)
                return;
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
            if (_keyDownEventManager == null)
                return;
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
                Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
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
            if (_keyUpEventManager == null)
                return;
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
            if (_keyUpEventManager == null)
                return;
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

            FocusManager.SetFocusedElement(this.INTERNAL_ParentWindow, this);
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnGotFocus(RoutedEventArgs eventArgs)
        {
            if (_gotFocusEventManager == null)
                return;
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

            FocusManager.SetFocusedElement(this.INTERNAL_ParentWindow, null);
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnLostFocus(RoutedEventArgs eventArgs)
        {
            if (_lostFocusEventManager == null)
                return;
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
            if (_gotFocusForIsTabStopEventManager == null)
                return;
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
            Type[] methodParameters = { typeof(RoutedEventArgs) };
            
            if (_gotFocusEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnGotFocus", methodParameters))
            {
                var v = GotFocusEventManager; //forces the creation of the event manager.
            }
            if (_gotFocusEventManager != null)
            {
                _gotFocusEventManager.AttachToDomEvents(this, typeof(UIElement), "OnGotFocus", methodParameters);
            }
            if (_lostFocusEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnLostFocus", methodParameters))
            {
                var v = LostFocusEventManager; //forces the creation of the event manager.
            }
            if (_lostFocusEventManager != null)
            {
                _lostFocusEventManager.AttachToDomEvents(this, typeof(UIElement), "OnLostFocus", methodParameters);
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
            bool isMouseEvent = Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0.type.startsWith('mouse')", jsEventArg));
            if (!(ignoreMouseEvents && isMouseEvent)) //Ignore mousedown, mousemove and mouseup if the touch equivalents have been handled.
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

                //Prevent text selection when the pointer is captured:
                if (preventTextSelectionWhenPointerIsCaptured && Pointer.INTERNAL_captured != null)
                {
                    CSHTML5.Interop.ExecuteJavaScript(@"window.getSelection().removeAllRanges()");
                }
                bool isTouchEndEvent = Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0.type == 'touchend'", jsEventArg));
                if(isTouchEndEvent) //prepare to ignore the mouse events since they were already handled as touch events
                {
                    ignoreMouseEvents = true;
                    if(_ignoreMouseEventsTimer == null)
                    {
                        _ignoreMouseEventsTimer = new DispatcherTimer() { Interval = new TimeSpan(0,0,0,0,100) }; //I arbitrarily picked 100ms because at 30ms with throttling x6, it didn't work every time (but sometimes did so it should be alright, also, I tested with 100ms and it worked everytime)
                        _ignoreMouseEventsTimer.Tick += _ignoreMouseEventsTimer_Tick;
                        
                    }
                    _ignoreMouseEventsTimer.Stop();
                    _ignoreMouseEventsTimer.Start();
                }
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

#if MIGRATION
            Type[] methodParameters = new Type[] { typeof(MouseEventArgs) };
            //
            if (_pointerMovedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseMove", methodParameters))
            {
                var v = PointerMovedEventManager; //forces the creation of the event manager.
            }
            if (_pointerMovedEventManager != null)
            {
                _pointerMovedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseMove", methodParameters);
            }
            //
            if (_pointerEnteredEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseEnter", methodParameters))
            {
                var v = PointerEnteredEventManager; //forces the creation of the event manager.
            }
            if (_pointerEnteredEventManager != null)
            {
                _pointerEnteredEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseEnter", methodParameters);
            }
            //
            if (_pointerExitedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseLeave", methodParameters))
            {
                var v = PointerExitedEventManager; //forces the creation of the event manager.
            }
            if (_pointerExitedEventManager != null)
            {
                _pointerExitedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseLeave", methodParameters);
            }
            //
            methodParameters = new Type[] { typeof(TappedRoutedEventArgs) };
            if (_tappedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnTapped", methodParameters))
            {
                var v = TappedEventManager; //forces the creation of the event manager.
            }
            if (_tappedEventManager != null)
            {
                _tappedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnTapped", methodParameters);
            }
            //
            methodParameters = new Type[] { typeof(MouseButtonEventArgs) };
            if (_pointerPressedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseLeftButtonDown", methodParameters))
            {
                var v = PointerPressedEventManager; //forces the creation of the event manager.
            }
            if (_pointerPressedEventManager != null)
            {
                _pointerPressedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseLeftButtonDown", methodParameters);
            }
            //
            if (_pointerReleasedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseLeftButtonUp", methodParameters))
            {
                var v = PointerReleasedEventManager; //forces the creation of the event manager.
            }
            if (_pointerReleasedEventManager != null)
            {
                _pointerReleasedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseLeftButtonUp", methodParameters);
            }
            //
            if (_mouseRightButtonDownEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseRightButtonDown", methodParameters))
            {
                var v = MouseRightButtonDownEventManager; //forces the creation of the event manager.
            }
            if (_mouseRightButtonDownEventManager != null)
            {
                _mouseRightButtonDownEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseRightButtonDown", methodParameters);
            }
            //
            if (_rightTappedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseRightButtonUp", methodParameters))
            {
                var v = RightTappedEventManager; //forces the creation of the event manager.
            }
            if (_rightTappedEventManager != null)
            {
                _rightTappedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseRightButtonUp", methodParameters);
            }
            //
            methodParameters = new Type[] { typeof(MouseWheelEventArgs) };
            if (_pointerWheelChangedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnMouseWheel", methodParameters))
            {
                var v = PointerWheelChangedEventManager; //forces the creation of the event manager.
            }
            if (_pointerWheelChangedEventManager != null)
            {
                _pointerWheelChangedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnMouseWheel", methodParameters);
            }
            //
            methodParameters = new Type[] { typeof(KeyEventArgs) };
            if (_keyDownEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnKeyDown", methodParameters))
            {
                var v = KeyDownEventManager; //forces the creation of the event manager.
            }
            if (_keyDownEventManager != null)
            {
                _keyDownEventManager.AttachToDomEvents(this, typeof(UIElement), "OnKeyDown", methodParameters);
            }
            //
            if (_keyUpEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnKeyUp", methodParameters))
            {
                var v = KeyUpEventManager; //forces the creation of the event manager.
            }
            if (_keyUpEventManager != null)
            {
                _keyUpEventManager.AttachToDomEvents(this, typeof(UIElement), "OnKeyUp", methodParameters);
            }
            //
            methodParameters = new Type[] { typeof(RoutedEventArgs) };
            if (_gotFocusEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnGotFocus", methodParameters))
            {
                var v = GotFocusEventManager; //forces the creation of the event manager.
            }
            if (_gotFocusEventManager != null)
            {
                _gotFocusEventManager.AttachToDomEvents(this, typeof(UIElement), "OnGotFocus", methodParameters);
            }
            //
            if (_lostFocusEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnLostFocus", methodParameters))
            {
                var v = LostFocusEventManager; //forces the creation of the event manager.
            }
            if (_lostFocusEventManager != null)
            {
                _lostFocusEventManager.AttachToDomEvents(this, typeof(UIElement), "OnLostFocus", methodParameters);
            }
#else
            Type[] methodParameters = new Type[] { typeof(PointerRoutedEventArgs) };
            if (_pointerMovedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnPointerMoved", methodParameters))
            {
                var v = PointerMovedEventManager; //forces the creation of the event manager.
            }
            if (_pointerMovedEventManager != null)
            {
                _pointerMovedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnPointerMoved", methodParameters);
            }
            if(_pointerPressedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnPointerPressed", methodParameters))
            {
                var v = PointerPressedEventManager; //forces the creation of the event manager.
            }
            if (_pointerPressedEventManager != null)
            {
                _pointerPressedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnPointerPressed", methodParameters);
            }
            if (_pointerReleasedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnPointerReleased", methodParameters))
            {
                var v = PointerReleasedEventManager; //forces the creation of the event manager.
            }
            if (_pointerReleasedEventManager != null)
            {
                _pointerReleasedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnPointerReleased", methodParameters);
            }
            if (_pointerEnteredEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnPointerEntered", methodParameters))
            {
                var v = PointerEnteredEventManager; //forces the creation of the event manager.
            }
            if (_pointerEnteredEventManager != null)
            {
                _pointerEnteredEventManager.AttachToDomEvents(this, typeof(UIElement), "OnPointerEntered", methodParameters);
            }
            if (_pointerExitedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnPointerExited", methodParameters))
            {
                var v = PointerExitedEventManager; //forces the creation of the event manager.
            }
            if (_pointerExitedEventManager != null)
            {
                _pointerExitedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnPointerExited", methodParameters);
            }
            if (_pointerWheelChangedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnPointerExited", methodParameters))
            {
                var v = PointerWheelChangedEventManager; //forces the creation of the event manager.
            }
            if (_pointerWheelChangedEventManager != null)
            {
                _pointerWheelChangedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnPointerWheelChanged", methodParameters);
            }
            methodParameters = new Type[] { typeof(TappedRoutedEventArgs) };
            if (_tappedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnTapped", methodParameters))
            {
                var v = TappedEventManager; //forces the creation of the event manager.
            }
            if (_tappedEventManager != null)
            {
                _tappedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnTapped", methodParameters);
            }
            methodParameters = new Type[] { typeof(RightTappedRoutedEventArgs) };
            if (_rightTappedEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnRightTapped", methodParameters))
            {
                var v = RightTappedEventManager; //forces the creation of the event manager.
            }
            if (_rightTappedEventManager != null)
            {
                _rightTappedEventManager.AttachToDomEvents(this, typeof(UIElement), "OnRightTapped", methodParameters);
            }
            methodParameters = new Type[] { typeof(KeyRoutedEventArgs) };
            if (_keyDownEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnKeyDown", methodParameters))
            {
                var v = KeyDownEventManager; //forces the creation of the event manager.
            }
            if (_keyDownEventManager != null)
            {
                _keyDownEventManager.AttachToDomEvents(this, typeof(UIElement), "OnKeyDown", methodParameters);
            }
            if (_keyUpEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnKeyUp", methodParameters))
            {
                var v = KeyUpEventManager; //forces the creation of the event manager.
            }
            if (_keyUpEventManager != null)
            {
                _keyUpEventManager.AttachToDomEvents(this, typeof(UIElement), "OnKeyUp", methodParameters);
            }
            methodParameters = new Type[] { typeof(RoutedEventArgs) };
            if (_gotFocusEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnGotFocus", methodParameters))
            {
                var v = GotFocusEventManager; //forces the creation of the event manager.
            }
            if (_gotFocusEventManager != null)
            {
                _gotFocusEventManager.AttachToDomEvents(this, typeof(UIElement), "OnGotFocus", methodParameters);
            }
            if (_lostFocusEventManager == null && INTERNAL_EventsHelper.IsEventCallbackOverridden(this, typeof(UIElement), "OnLostFocus", methodParameters))
            {
                var v = LostFocusEventManager; //forces the creation of the event manager.
            }
            if (_lostFocusEventManager != null)
            {
                _lostFocusEventManager.AttachToDomEvents(this, typeof(UIElement), "OnLostFocus", methodParameters);
            }
#endif
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
            if (_pointerWheelChangedEventManager != null)
            {
                _pointerWheelChangedEventManager.DetachFromDomEvents();
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
            else if (routedEvent == UIElement.MouseRightButtonDownEvent)
            {
                MouseRightButtonDownEventManager.Add((MouseButtonEventHandler)handler, handledEventsToo: handledEventsToo);
            }
#endif
#if MIGRATION
            else if (routedEvent == UIElement.MouseWheelEvent)
#else
            else if (routedEvent == UIElement.PointerWheelChangedEvent)
#endif
            {
#if MIGRATION
                PointerWheelChangedEventManager.Add((MouseWheelEventHandler)handler, handledEventsToo: handledEventsToo);
#else
                PointerWheelChangedEventManager.Add((PointerEventHandler)handler, handledEventsToo: handledEventsToo);
#endif
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
            else if (routedEvent == UIElement.TextInputUpdateEvent ||
                     routedEvent == UIElement.TextInputEvent ||
                     routedEvent == UIElement.TextInputStartEvent)
            {
            }
            else
            {
                throw new NotSupportedException(string.Format("The following routed event cannot be used in the AddHandler method: {0} - Please contact support.", routedEvent));
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
            else if (routedEvent == UIElement.MouseRightButtonDownEvent)
            {
                MouseRightButtonDownEventManager.Remove((MouseButtonEventHandler)handler);
                
            }
#endif
#if MIGRATION
            else if (routedEvent == UIElement.MouseWheelEvent)
#else
            else if (routedEvent == UIElement.PointerWheelChangedEvent)
#endif
            {
#if MIGRATION
                PointerWheelChangedEventManager.Remove((MouseWheelEventHandler)handler);
#else
                PointerWheelChangedEventManager.Remove((PointerEventHandler)handler);
#endif
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