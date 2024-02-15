
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
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using OpenSilver.Internal;

namespace System.Windows
{
    /// <summary>
    /// Represents a trigger that applies a set of actions (animation storyboards) in
    /// response to an event.
    /// </summary>
    [ContentProperty(nameof(Actions))]
    public sealed class EventTrigger : TriggerBase
    {
        // This is the listener that we hook up to the SourceId element.
        private RoutedEventHandler _routedEventHandler = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTrigger"/> class.
        /// </summary>
        public EventTrigger() { }

        /// <summary>
        /// Identifies the <see cref="Actions"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register(
                nameof(Actions),
                typeof(TriggerActionCollection),
                typeof(EventTrigger),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<TriggerAction>(
                        static () => new TriggerActionCollection(),
                        static (d, dp) => new TriggerActionCollection()),
                    null,
                    CoerceActions));

        private static object CoerceActions(DependencyObject d, object baseValue)
        {
            return baseValue ?? new TriggerActionCollection();
        }

        /// <summary>
        /// Gets the collection of <see cref="BeginStoryboard"/> objects
        /// that this <see cref="EventTrigger"/> maintains.
        /// </summary>
        /// <returns>
        /// The existing <see cref="TriggerActionCollection"/>.
        /// </returns>
        public TriggerActionCollection Actions => (TriggerActionCollection)GetValue(ActionsProperty);

        /// <summary>
        /// Identifies the <see cref="RoutedEvent"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty RoutedEventProperty =
            DependencyProperty.Register(
                nameof(RoutedEvent),
                typeof(RoutedEvent),
                typeof(EventTrigger),
                null);

        /// <summary>
        /// Gets or sets the name of the event that initiates the trigger.
        /// </summary>
        /// <returns>
        /// The name or identifier of the event. See Remarks.
        /// </returns>
        public RoutedEvent RoutedEvent
        {
            get { return (RoutedEvent)GetValue(RoutedEventProperty); }
            set { SetValueInternal(RoutedEventProperty, value); }
        }

        //
        // Internal static methods to process event trigger information stored
        //  in attached storage of other objects.
        //
        // Called when the FrameworkElement and the tree structure underneath it has been
        //  built up.  This is the earliest point we can resolve all the child 
        //  node identification that may exist in a Trigger object.
        // This should be moved to base class if PropertyTrigger support is added.
        internal static void ProcessTriggerCollection(IInternalFrameworkElement triggersHost)
        {
            if (triggersHost.GetValue(FrameworkElement.TriggersProperty) is TriggerCollection triggerCollection)
            {
                // Don't seal the collection, because we allow it to change.  We will,
                // however, seal each of the triggers.

                List<TriggerBase> internalTriggerCollection = triggerCollection.InternalItems;
                for (int i = 0; i < internalTriggerCollection.Count; i++)
                {
                    ProcessOneTrigger(triggersHost, internalTriggerCollection[i]);
                }
            }
        }

        //
        // ProcessOneTrigger
        //
        // Find the target element for this trigger, and set a listener for 
        // the event into (pointing back to the trigger).
        internal static void ProcessOneTrigger(IInternalFrameworkElement triggersHost, TriggerBase triggerBase)
        {
            // This code path is used in the element trigger case.  We don't actually
            //  need these guys to be usable cross-thread, so we don't really need
            //  to freeze/seal these objects.  The only one expected to cause problems
            //  is a change to the RoutedEvent.  At the same time we remove this
            //  Seal(), the RoutedEvent setter will check to see if the handler has
            //  already been created and refuse an update if so.
            // triggerBase.Seal();

            EventTrigger eventTrigger = triggerBase as EventTrigger;
            if (eventTrigger != null)
            {
                Debug.Assert(eventTrigger._routedEventHandler == null);

                // Create a statefull event delegate (which keeps a ref to the FE).
                EventTriggerSourceListener listener = new EventTriggerSourceListener(eventTrigger, triggersHost);

                // Store the RoutedEventHandler & target for use in DisconnectOneTrigger
                eventTrigger._routedEventHandler = new RoutedEventHandler(listener.Handler);
                AddHandler(triggersHost, eventTrigger.RoutedEvent, eventTrigger._routedEventHandler);
            }
            else
            {
                throw new InvalidOperationException("Triggers collection members must be of type EventTrigger.");
            }
        }

        //
        // DisconnectAllTriggers
        //
        // Call DisconnectOneTrigger for each trigger in the Triggers collection.
        internal static void DisconnectAllTriggers(IInternalFrameworkElement triggersHost)
        {
            if (triggersHost.GetValue(FrameworkElement.TriggersProperty) is TriggerCollection triggerCollection)
            {
                List<TriggerBase> internalTriggerCollection = triggerCollection.InternalItems;
                for (int i = 0; i < internalTriggerCollection.Count; i++)
                {
                    DisconnectOneTrigger(triggersHost, internalTriggerCollection[i]);
                }
            }
        }

        //
        // DisconnectOneTrigger
        //
        // In ProcessOneTrigger, we connect an event trigger to the element
        // which it targets.  Here, we remove the event listener to clean up.
        internal static void DisconnectOneTrigger(IInternalFrameworkElement triggersHost, TriggerBase triggerBase)
        {
            EventTrigger eventTrigger = triggerBase as EventTrigger;

            if (eventTrigger != null)
            {
                RemoveHandler(triggersHost, eventTrigger.RoutedEvent, eventTrigger._routedEventHandler);
                eventTrigger._routedEventHandler = null;
            }
            else
            {
                throw new InvalidOperationException("Triggers collection members must be of type EventTrigger.");
            }
        }

        private static void AddHandler(IInternalFrameworkElement fe, RoutedEvent routedEvent, RoutedEventHandler handler)
        {
            if (routedEvent == fe.LoadedEvent)
            {
                fe.Loaded += handler;
            }
            else
            {
                fe.AddHandler(routedEvent, handler, false);
            }
        }

        private static void RemoveHandler(IInternalFrameworkElement fe, RoutedEvent routedEvent, RoutedEventHandler handler)
        {
            if (routedEvent == fe.LoadedEvent)
            {
                fe.Loaded -= handler;
            }
            else
            {
                fe.RemoveHandler(routedEvent, handler);
            }
        }

        internal sealed class EventTriggerSourceListener
        {
            internal EventTriggerSourceListener(EventTrigger trigger, IInternalFrameworkElement host)
            {
                _owningTrigger = trigger;
                _owningTriggerHost = host;
            }

            internal void Handler(object sender, RoutedEventArgs e)
            {
                // Invoke all actions of the associated EventTrigger object.
                List<TriggerAction> actions = _owningTrigger.Actions.InternalItems;
                for (int j = 0; j < actions.Count; j++)
                {
                    actions[j].Invoke(_owningTriggerHost);
                }
            }

            private readonly EventTrigger _owningTrigger;
            private readonly IInternalFrameworkElement _owningTriggerHost;
        }
    }
}
