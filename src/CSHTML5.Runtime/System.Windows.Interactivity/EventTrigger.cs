using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endif

namespace System.Windows.Interactivity //Windows.UI.Interactivity
{
    /// <summary>
    /// A trigger that listens for a specified event on its source and fires when that event is fired.
    /// 
    /// </summary>
    public class EventTrigger : TriggerBase //EventTriggerBase<object> For simplicity's sake, we inherited directly from TriggerBase and will currently only support the properties EventName and Actions directly here.
    {
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(EventTrigger), new PropertyMetadata("Loaded", new PropertyChangedCallback(EventTrigger.OnEventNameChanged)));

        /// <summary>
        /// Gets or sets the name of the event to listen for. This is a dependency property.
        /// </summary>
        /// 
        /// <value>
        /// The name of the event.
        /// </value>
        public string EventName
        {
            get
            {
                return (string)this.GetValue(EventTrigger.EventNameProperty);
            }
            set
            {
                this.SetValue(EventTrigger.EventNameProperty, (object)value);
            }
        }

        static EventTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Interactivity.EventTrigger"/> class.
        /// </summary>
        public EventTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Interactivity.EventTrigger"/> class.
        /// 
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public EventTrigger(string eventName)
        {
            this.EventName = eventName;
        }

        protected string GetEventName() //should be an override but we changed the heritage
        {
            return this.EventName;
        }

        private static void OnEventNameChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            //Note: this method originally only had the following line but we removed the EventTriggerBase type.
            //((EventTriggerBase)sender).OnEventNameChanged((string)args.OldValue, (string)args.NewValue);
            //todo: unregister from the former event and register to the new one.
            EventTrigger eventTrigger = (EventTrigger)sender;
            if (eventTrigger.AssociatedObject != null) //Making sure the source of the event is set. Note: The source can apparently be something other than the AssociatedObject but we ignore this case for now.
            {
                string oldEventName = (string)args.OldValue;
                string newEventName = (string)args.NewValue;
                if (!string.IsNullOrWhiteSpace(oldEventName))
                {
                    eventTrigger.UnregisterEvent(eventTrigger.AssociatedObject, oldEventName);
                }
                if (!string.IsNullOrWhiteSpace(newEventName))
                {
                    eventTrigger.RegisterEvent(eventTrigger.AssociatedObject, newEventName);
                }
            }
        }


#region added because we changed heritage

        private void UnregisterEvent(DependencyObject associatedObject, string eventName)
        {
            Type type = associatedObject.GetType();
            EventInfo eventInfo = type.GetEvent(eventName);
            if (eventInfo != null)
            {
                EventHandler eventHandler = OnEventImpl;
                eventInfo.RemoveEventHandler(associatedObject, eventHandler);
            }
        }

        private void RegisterEvent(DependencyObject associatedObject, string eventName)
        {
            if (Interop.IsRunningInTheSimulator)
            {
                RegisterEvent_SimulatorOnly(associatedObject, eventName);
            }
            else
            {
                Type type = associatedObject.GetType();
                EventInfo eventInfo = type.GetEvent(eventName);
                if (eventInfo != null)
                {
                    EventHandler eventHandler = OnEventImpl; //Note: Bridge doesn't care about the type of the Delegate so we can do this. In the Simulator, it causes an exception on the lines below.
                    eventInfo.RemoveEventHandler(associatedObject, eventHandler);
                    eventInfo.AddEventHandler(associatedObject, eventHandler);
                }
            }
        }

        private void RegisterEvent_SimulatorOnly(DependencyObject associatedObject, string eventName)
        {
            Type type = associatedObject.GetType();
            EventInfo eventInfo = type.GetEvent(eventName);
            if (eventInfo != null)
            {
                //We get the expected Event handler type. Note: we have to do it through Reflection because Bridge does not implement the EventHandlerType property so we cannot directly call it.
                var eventInfoType = eventInfo.GetType();
                PropertyInfo eventhandlerTypeProperty = eventInfoType.GetProperty("EventHandlerType", BindingFlags.Public | BindingFlags.Instance);
                Type eventHandlerType = (Type)eventhandlerTypeProperty.GetValue(eventInfo);

                if(eventHandlerType == typeof(EventHandler)) //todo: find a better way to do this.
                {
                        EventHandler eventHandler = OnEventImpl; //Note: Bridge doesn't care about the type of the Delegate so we can do this. In the Simulator, it causes an exception on the lines below.
                        eventInfo.RemoveEventHandler(associatedObject, eventHandler);
                        eventInfo.AddEventHandler(associatedObject, eventHandler);
                }
                else if(eventHandlerType == typeof(RoutedEventHandler))
                {
                    RoutedEventHandler eventHandler = OnEventImpl; //Note: Bridge doesn't care about the type of the Delegate so we can do this. In the Simulator, it causes an exception on the lines below.
                    eventInfo.RemoveEventHandler(associatedObject, eventHandler);
                    eventInfo.AddEventHandler(associatedObject, eventHandler);
                }
#if MIGRATION
                else if (eventHandlerType == typeof(MouseEventHandler))
#else
                else if (eventHandlerType == typeof(PointerEventHandler))
#endif
                {
                    RoutedEventHandler eventHandler = OnEventImpl; //Note: Bridge doesn't care about the type of the Delegate so we can do this. In the Simulator, it causes an exception on the lines below.
                    eventInfo.RemoveEventHandler(associatedObject, eventHandler);
                    eventInfo.AddEventHandler(associatedObject, eventHandler);
                }

            }
        }

        private void OnEventImpl(object sender, object eventArgs)
        {
            OnEvent(eventArgs);
        }

        protected virtual void OnEvent(object eventArgs)
        {
            this.InvokeActions(eventArgs);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            //Note: the following line might be useless since right now, AssociatedObject is set in the same method as the one that calls OnAttached. We keep it just in case.
            if (AssociatedObject != null) //Making sure the source of the event is set. Note: The source can apparently be something other than the AssociatedObject but we ignore this case for now.
            {
                string eventName = EventName;
                if (!string.IsNullOrWhiteSpace(eventName))
                {
                    this.RegisterEvent(AssociatedObject, eventName);
                }
            }
        }
#endregion
    }
}