
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Represents a trigger that applies a set of actions (animation storyboards) in
/// response to an event.
/// </summary>
[ContentProperty(nameof(Actions))]
public sealed class EventTrigger : TriggerBase
{
    // Event that will fire this trigger
    private RoutedEvent _routedEvent;

    // Name of the element in the visual tree whose event to listen to.
    // May remain the default value of null, which  means the object being 
    // Styled is the target instead of something within the visual tree.
    private string _sourceName;

    // Actions to invoke when this trigger is fired
    private TriggerActionCollection _actions;

    // This is the listener that we hook up to the SourceId element.
    private RoutedEventHandler _routedEventHandler;

    // This is the SourceId-ed element.
    private IInternalFrameworkElement _source;

    internal static readonly UncommonField<TriggerCollection> TriggerCollectionField = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EventTrigger"/> class.
    /// </summary>
    public EventTrigger() { }

    /// <summary>
    /// Gets the collection of <see cref="BeginStoryboard"/> objects
    /// that this <see cref="EventTrigger"/> maintains.
    /// </summary>
    /// <returns>
    /// The existing <see cref="TriggerActionCollection"/>.
    /// </returns>
    public TriggerActionCollection Actions => _actions ??= new TriggerActionCollection(this);

    /// <summary>
    /// Gets or sets the name of the event that initiates the trigger.
    /// </summary>
    /// <returns>
    /// The name or identifier of the event. See Remarks.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <see cref="RoutedEvent"/> property cannot be null.
    /// </exception>
    public RoutedEvent RoutedEvent
    {
        get { return _routedEvent; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(EventTrigger)));
            }

            // If we are fixed on listening to an event already, don't allow this change.
            if (_routedEventHandler is not null)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(EventTrigger)));
            }

            _routedEvent = value;
        }
    }

    /// <summary>
    /// Gets or sets the name of the object with the event that activates this trigger.
    /// This is only used by element triggers or template triggers.
    /// </summary>
    /// <returns>
    /// The default value is null. If this property value is null, then the element being
    /// monitored for the raising of the event is the templated parent or the logical tree root.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// After an <see cref="EventTrigger"/> is in use, it cannot be modified.
    /// </exception>
    public string SourceName
    {
        get { return _sourceName; }
        set
        {
            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(EventTrigger)));
            }

            _sourceName = value;
        }
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
        Debug.Assert(triggersHost is not null);

        if (TriggerCollectionField.GetValue((DependencyObject)triggersHost) is TriggerCollection triggerCollection)
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

        if (triggerBase is not EventTrigger eventTrigger)
        {
            throw new InvalidOperationException(Strings.TriggersSupportsEventTriggersOnly);
        }

        Debug.Assert(eventTrigger._routedEventHandler is null && eventTrigger._source is null);

        // PERF: Cache this result if it turns out we're doing a lot of lookups on the same name.
        eventTrigger._source = FindNamedFrameworkElement(triggersHost, eventTrigger.SourceName);

        // Create a statefull event delegate (which keeps a ref to the FE).
        var listener = new EventTriggerSourceListener(eventTrigger, triggersHost);

        // Store the RoutedEventHandler & target for use in DisconnectOneTrigger
        eventTrigger._routedEventHandler = new RoutedEventHandler(listener.Handler);
        eventTrigger._source.AddHandler(eventTrigger.RoutedEvent, eventTrigger._routedEventHandler, false);
    }

    //
    // DisconnectAllTriggers
    //
    // Call DisconnectOneTrigger for each trigger in the Triggers collection.
    internal static void DisconnectAllTriggers(IInternalFrameworkElement triggersHost)
    {
        if (TriggerCollectionField.GetValue((DependencyObject)triggersHost) is TriggerCollection triggerCollection)
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
        if (triggerBase is not EventTrigger eventTrigger)
        {
            throw new InvalidOperationException(Strings.TriggersSupportsEventTriggersOnly);
        }

        eventTrigger._source.RemoveHandler(eventTrigger.RoutedEvent, eventTrigger._routedEventHandler);
        eventTrigger._routedEventHandler = null;
    }

    private static IInternalFrameworkElement FindNamedFrameworkElement(IInternalFrameworkElement startElement, string targetName)
    {
        if (string.IsNullOrEmpty(targetName))
        {
            return startElement;
        }

        DependencyObject targetObject = LogicalTreeHelper.FindLogicalNode(startElement.AsDependencyObject(), targetName)
            ?? throw new ArgumentException(string.Format(Strings.TargetNameNotFound, targetName));

        if (targetObject is not IInternalFrameworkElement targetFE)
        {
            throw new InvalidOperationException(string.Format(Strings.NamedObjectMustBeFrameworkElement, targetName));
        }

        return targetFE;
    }

    private sealed class EventTriggerSourceListener
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
