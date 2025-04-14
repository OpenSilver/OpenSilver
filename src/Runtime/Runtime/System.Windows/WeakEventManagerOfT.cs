
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

using System.Reflection;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Provides a type-safe <see cref="WeakEventManager"/> that enables you to specify the event handler to use 
/// for the "weak event listener" pattern. This class defines a type parameter for the source of the event 
/// and a type parameter for the event data that is used.
/// </summary>
/// <typeparam name="TEventSource">
/// The type that raises the event.
/// </typeparam>
/// <typeparam name="TEventArgs">
/// The type that holds the event data.
/// </typeparam>
public class WeakEventManager<TEventSource, TEventArgs> : WeakEventManager
    where TEventArgs : EventArgs
{
    private WeakEventManager(string eventName)
    {
        _eventName = eventName;
        _eventInfo = typeof(TEventSource).GetEvent(_eventName);

        if (_eventInfo is null)
        {
            throw new ArgumentException(string.Format(Strings.EventNotFound, typeof(TEventSource).FullName, eventName));
        }

        _handler = Delegate.CreateDelegate(_eventInfo.EventHandlerType, this, DeliverEventMethodInfo);
    }

    /// <summary>
    /// Adds the specified event handler to the specified event.
    /// </summary>
    /// <param name="source">
    /// The source object that raises the specified event.
    /// </param>
    /// <param name="eventName">
    /// The name of the event to subscribe to.
    /// </param>
    /// <param name="handler">
    /// The delegate that handles the event.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="handler"/> is null.
    /// </exception>
    public static void AddHandler(TEventSource source, string eventName, EventHandler<TEventArgs> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        CurrentManager(eventName).ProtectedAddHandler(source, handler);
    }

    /// <summary>
    /// Removes the specified event handler from the specified event.
    /// </summary>
    /// <param name="source">
    /// The source object that raises the specified event.
    /// </param>
    /// <param name="eventName">
    /// The name of the event to remove the handler from.
    /// </param>
    /// <param name="handler">
    /// The delegate to remove
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="handler"/> is null.
    /// </exception>
    public static void RemoveHandler(TEventSource source, string eventName, EventHandler<TEventArgs> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        CurrentManager(eventName).ProtectedRemoveHandler(source, handler);
    }

    /// <summary>
    /// Returns a new object to contain listeners to an event.
    /// </summary>
    /// <returns>
    /// A new object to contain listeners to an event.
    /// </returns>
    protected override ListenerList NewListenerList() => new ListenerList<TEventArgs>();

    /// <summary>
    /// Starts listening for the event on the specified object.
    /// </summary>
    /// <param name="source">
    /// The object to that raises the event.
    /// </param>
    protected override void StartListening(object source) => _eventInfo.AddEventHandler(source, _handler);

    /// <summary>
    /// Stops listening for the event on the specified object.
    /// </summary>
    /// <returns>
    /// The object to that raises the event.
    /// </returns>
    protected override void StopListening(object source) => _eventInfo.RemoveEventHandler(source, _handler);

    // get the event manager for the current thread
    private static WeakEventManager<TEventSource, TEventArgs> CurrentManager(string eventName)
    {
        // at first use, create and register a new manager
        if (GetCurrentManager(typeof(TEventSource), eventName) is not WeakEventManager<TEventSource, TEventArgs> manager)
        {
            manager = new WeakEventManager<TEventSource, TEventArgs>(eventName);
            SetCurrentManager(typeof(TEventSource), eventName, manager);
        }

        return manager;
    }

    private readonly Delegate _handler;
    private readonly string _eventName;
    private readonly EventInfo _eventInfo;
}
