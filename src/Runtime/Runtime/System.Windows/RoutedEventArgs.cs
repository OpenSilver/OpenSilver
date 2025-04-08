
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

using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Contains state information and event data associated with a routed event.
/// </summary>
/// <remarks>
/// Different <see cref="RoutedEventArgs"/> can be used with a single <see cref="RoutedEvent"/>. 
/// This class is responsible for packaging the event data for a <see cref="RoutedEvent"/>, providing 
/// extra event state information, and is used by the event system for invoking the handler associated 
/// with the routed event.
/// </remarks>
public class RoutedEventArgs : EventArgs
{
    private RoutedEvent _routedEvent;
    private object _source;
    private object _originalSource;
    private bool _invokingHandler;
    private bool _userInitiated;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedEventArgs"/> class.
    /// </summary>
    public RoutedEventArgs() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedEventArgs"/> class, using the supplied 
    /// routed event identifier.
    /// </summary>
    /// <param name="routedEvent">
    /// The routed event identifier for this instance of the <see cref="RoutedEventArgs"/> class.
    /// </param>
    public RoutedEventArgs(RoutedEvent routedEvent)
        : this(routedEvent, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedEventArgs"/> class, using the supplied 
    /// routed event identifier, and providing the opportunity to declare a different source for the 
    /// event.
    /// </summary>
    /// <param name="routedEvent">
    /// The routed event identifier for this instance of the <see cref="RoutedEventArgs"/> class.
    /// </param>
    /// <param name="source">
    /// An alternate source that will be reported when the event is handled. This pre-populates the 
    /// <see cref="Source"/> property.
    /// </param>
    public RoutedEventArgs(RoutedEvent routedEvent, object source)
    {
        _routedEvent = routedEvent;
        _source = _originalSource = source;
    }

    /// <summary>
    /// Gets or sets the <see cref="Windows.RoutedEvent"/> associated with this <see cref="RoutedEventArgs"/> 
    /// instance.
    /// </summary>
    /// <returns>
    /// The identifier for the event that has been invoked.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Attempted to change the <see cref="RoutedEvent"/> value while the event is being routed.
    /// </exception>
    /// <remarks>
    /// You cannot set this value on a <see cref="RoutedEventArgs"/> that has already been routed (for 
    /// instance, if you obtained the arguments through a handler). Attempting to do so will generate an 
    /// exception. You can only set it on an instance that has not yet been used to generate an invocation 
    /// of the event.
    /// The value of <see cref="RoutedEvent"/> cannot be null at any time.
    /// </remarks>
    public RoutedEvent RoutedEvent
    {
        get { return _routedEvent; }
        set
        {
            if (_userInitiated && _invokingHandler)
            {
                throw new InvalidOperationException(Strings.RoutedEventCannotChangeWhileRouting);
            }

            _routedEvent = value;
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates the present state of the event handling for a routed event 
    /// as it travels the route.
    /// </summary>
    /// <returns>
    /// If setting, set to true if the event is to be marked handled; otherwise false. If reading this 
    /// value, true indicates that either a class handler, or some instance handler along the route, 
    /// has already marked this event handled. False indicates that no such handler has marked the event 
    /// handled. The default value is false.
    /// </returns>
    /// <remarks>
    /// Marking the event handled will limit the visibility of the routed event to listeners along the 
    /// event route. The event does still travel the remainder of the route, but only handlers specifically 
    /// added with HandledEventsToo true in the <see cref="UIElement.AddHandler(RoutedEvent, Delegate, bool)"/> 
    /// method call will be invoked in response. Default handlers on instance listeners (such as those 
    /// expressed in Extensible Application Markup Language (XAML)) will not be invoked. Handling events that 
    /// are marked handled is not a common scenario.
    /// If you are a control author defining your own events, the decisions you make regarding event handling 
    /// at the class level will impact users of your control as well as any users of derived controls, and 
    /// potentially other elements that are either contained by your control or that contain your control.
    /// In very rare circumstances it is appropriate to handle events where <see cref="Handled"/> is marked 
    /// true, and modify the event arguments by changing <see cref="Handled"/> to false. This can be necessary 
    /// in certain areas of input events of controls, such as key handling of <see cref="UIElement.KeyDown"/> 
    /// versus <see cref="UIElement.TextInput"/> where low level and high level input events compete for the 
    /// handling, and each is attempting to work with a different routing strategy.
    /// </remarks>
    public bool Handled { get; set; }

    /// <summary>
    /// Gets the original reporting source as determined by pure hit testing, before any possible 
    /// <see cref="Source"/> adjustment by a parent class.
    /// </summary>
    /// <returns>
    /// The original reporting source, before any possible <see cref="Source"/> adjustment made by class 
    /// handling, which may have been done to flatten composited element trees.
    /// </returns>
    /// <remarks>
    /// This property acquires its value once, before the class event handlers or any instance handlers 
    /// are invoked, and is never adjusted past this point. The original source information is read-only 
    /// to class handlers or class implementations, just as it is reported in the event data.
    /// Common cases where the source may be adjusted include content elements inside a content model 
    /// for a control (the contents of a list item, for instance, will report the list item element as 
    /// the <see cref="Source"/> and the actual element within the list item will be the <see cref="OriginalSource"/>.
    /// Source adjustment by various elements and content models varies from class to class. Each class 
    /// that adjusts event sources attempts to anticipate which source is the most useful to report for 
    /// most input scenarios and the scenarios for which the class is intended, and then sets that source 
    /// as the <see cref="Source"/>. If this source is not the one that has relevance to your handling of 
    /// the event, try checking <see cref="OriginalSource"/> instead to see if it reports a different source
    /// that is more suitable.
    /// </remarks>
    public object OriginalSource => _originalSource;

    /// <summary>
    /// Gets or sets a reference to the object that raised the event.
    /// </summary>
    /// <returns>
    /// The object that raised the event.
    /// </returns>
    /// <remarks>
    /// For any bubbling routed event that has actually traveled the route beyond the element that raised 
    /// it, and for any tunneling routed event that has not yet tunneled down to the element that raised 
    /// it, the value of <see cref="Source"/> will be different than the value of the sender parameter of 
    /// the event arguments class. Which of the two elements involved in the event is of the most importance 
    /// in any given handler (<see cref="Source"/>, the element that raised it, or sender, the element that 
    /// is currently handling it) is dependent on the application logic that your handler is addressing.
    /// Setting this property is typically only done when overriding or implementing other APIs that adjust 
    /// event sources, such as when class handling an event. Resetting apparent event sources from instance 
    /// handlers is not recommended, particularly when the handler does not mark the event as handled.
    /// If you do reset <see cref="Source"/> to report a different event source, <see cref="OriginalSource"/> 
    /// will continue to report the source as first raised by the originating <see cref="UIElement.RaiseEvent"/> 
    /// call.
    /// </remarks>
    public object Source
    {
        get { return _source; }
        set
        {
            if (_invokingHandler && _userInitiated)
            {
                throw new InvalidOperationException(Strings.RoutedEventCannotChangeWhileRouting);
            }

            object source = value;
            if (_source is null && _originalSource is null)
            {
                // Gets here when it is the first time that the source is set.
                // This implies that this is also the original source of the event
                _source = _originalSource = source;
                OnSetSource(source);
            }
            else if (_source != source)
            {
                // This is the actiaon taken at all other times when the
                // source is being set to a different value from what it was
                _source = source;
                OnSetSource(source);
            }
        }
    }

    /// <summary>
    /// When overridden in a derived class, provides a notification callback entry point whenever the 
    /// value of the <see cref="Source"/> property of an instance changes.
    /// </summary>
    /// <param name="source">
    /// The new value that <see cref="Source"/> is being set to.
    /// </param>
    /// <remarks>
    /// Changing the reported source of an event programmatically can potentially require updating the 
    /// type-specific data within the event. For this reason, the <see cref="OnSetSource(object)"/> method 
    /// is protected virtual and is intended to be overridden by subclasses of <see cref="RoutedEventArgs"/>.
    /// This method has no default implementation.
    /// </remarks>
    protected virtual void OnSetSource(object source) { }

    /// <summary>
    /// When overridden in a derived class, provides a way to invoke event handlers in a type-specific
    /// way, which can increase efficiency over the base implementation.
    /// </summary>
    /// <param name="genericHandler">
    /// The generic handler / delegate implementation to be invoked.
    /// </param>
    /// <param name="genericTarget">
    /// The target on which the provided handler should be invoked.
    /// </param>
    protected virtual void InvokeEventHandler(Delegate genericHandler, object genericTarget)
    {
        if (genericHandler is null)
        {
            throw new ArgumentNullException(nameof(genericHandler));
        }

        if (genericTarget is null)
        {
            throw new ArgumentNullException(nameof(genericTarget));
        }

        if (genericHandler is RoutedEventHandler routedEventHandler)
        {
            routedEventHandler(genericTarget, this);
        }
        else
        {
            // Restricted Action - reflection permission required
            genericHandler.DynamicInvoke(new object[] { genericTarget, this });
        }
    }

    internal void InvokeHandler(Delegate handler, object target)
    {
        _invokingHandler = true;

        try
        {
            InvokeEventHandler(handler, target);
        }
        finally
        {
            _invokingHandler = false;
        }
    }

    /// <summary>
    ///     Changes the RoutedEvent assocatied with these RoutedEventArgs
    /// </summary>
    /// <remarks>
    ///     Only used internally.  Added to support cracking generic MouseButtonDown/Up events
    ///     into MouseLeft/RightButtonDown/Up events.
    /// </remarks>
    /// <param name="newRoutedEvent">
    ///     The new RoutedEvent to associate with these RoutedEventArgs
    /// </param>
    internal void OverrideRoutedEvent(RoutedEvent newRoutedEvent) => _routedEvent = newRoutedEvent;

    /// <summary>
    ///     Changes the Source assocatied with these RoutedEventArgs
    /// </summary>
    /// <remarks>
    ///     Only used internally.  Added to support cracking generic MouseButtonDown/Up events
    ///     into MouseLeft/RightButtonDown/Up events.
    /// </remarks>
    /// <param name="source">
    ///     The new object to associate as the source of these RoutedEventArgs
    /// </param>
    internal void OverrideSource(object source) => _source = source;

    internal void MarkAsUserInitiated() => _userInitiated = true;

    internal void ClearUserInitiated() => _userInitiated = false;

    internal object UIEventArg { get; set; }

    internal bool Cancellable { get; set; } = true;

    internal void PreventDefault()
    {
        if (!Cancellable)
        {
            return;
        }

        if (UIEventArg != null)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid(
                $"{OpenSilver.Interop.GetVariableStringForJS(UIEventArg)}.preventDefault()");
        }
    }
}
