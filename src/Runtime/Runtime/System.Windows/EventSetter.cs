
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

using System.ComponentModel;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Represents an event setter in a style. Event setters invoke the specified event handlers in response to events.
/// </summary>
public class EventSetter : SetterBase
{
    private RoutedEvent _event;
    private Delegate _handler;
    private bool _handledEventsToo;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSetter"/> class.
    /// </summary>
    public EventSetter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSetter"/> class, using the provided event and handler parameters.
    /// </summary>
    /// <param name="routedEvent">
    /// The particular routed event that the <see cref="EventSetter"/> responds to.
    /// </param>
    /// <param name="handler">
    /// The handler to assign in this setter.
    /// </param>
    public EventSetter(RoutedEvent routedEvent, Delegate handler)
    {
        if (routedEvent is null)
        {
            throw new ArgumentNullException(nameof(routedEvent));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        _event = routedEvent;
        _handler = handler;
    }

    /// <summary>
    /// Gets or sets the particular routed event that this <see cref="EventSetter"/> responds to.
    /// </summary>
    /// <returns>
    /// The identifier field of the routed event.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Attempted to set this property on a sealed <see cref="EventSetter"/>.
    /// </exception>
    public RoutedEvent Event
    {
        get { return _event; }
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            CheckSealed();
            _event = value;
        }
    }

    /// <summary>
    /// Gets or sets the reference to a handler for a routed event in the setter.
    /// </summary>
    /// <returns>
    /// Reference to the handler that is attached by this <see cref="EventSetter"/>.
    /// </returns>
    public Delegate Handler
    {
        get { return _handler; }
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            CheckSealed();
            _handler = value;
        }
    }

    /// <summary>
    /// Gets or sets a value that determines whether the handler assigned to the setter should still be invoked, 
    /// even if the event is marked handled in its event data.
    /// </summary>
    /// <returns>
    /// true if the handler should still be invoked; otherwise, false.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HandledEventsToo
    {
        get { return _handledEventsToo; }
        set
        {
            CheckSealed();
            _handledEventsToo = value;
        }
    }

    //
    //  Do the error checking that we can only do once all of the properties have been
    //  set, then call up to base.
    //
    internal override void Seal()
    {
        if (_event is null)
        {
            throw new ArgumentException(string.Format(Strings.NullPropertyIllegal, "EventSetter.Event"));
        }
        if (_handler is null)
        {
            throw new ArgumentException(string.Format(Strings.NullPropertyIllegal, "EventSetter.Handler"));
        }
        if (_handler.GetType() != _event.HandlerType)
        {
            throw new ArgumentException(Strings.HandlerTypeIllegal);
        }

        base.Seal();
    }
}
