
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

namespace OpenSilver.Internal;

/// <summary>
/// Implements a weak event listener that allows the owner to be garbage
/// collected if its only remaining link is an event handler.
/// </summary>
/// <typeparam name="TInstance">Type of instance listening for the event.</typeparam>
/// <typeparam name="TSource">Type of source for the event.</typeparam>
/// <typeparam name="TEventArgs">Type of event arguments for the event.</typeparam>
internal sealed class WeakEventListener<TInstance, TSource, TEventArgs>
    where TInstance : class
    where TSource : class
{
    private WeakReference<TInstance> _weakInstance;
    private TSource _source;
    private Action<TInstance, object, TEventArgs> _onEventAction;
    private Action<WeakEventListener<TInstance, TSource, TEventArgs>, TSource> _onDetachAction;

    /// <summary>
    /// Initializes a new instances of the WeakEventListener class.
    /// </summary>
    /// <param name="instance">Instance subscribing to the event.</param>
    /// <param name="source">Source of the event</param>
    public WeakEventListener(TInstance instance, TSource source)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _weakInstance = new WeakReference<TInstance>(instance);
        _source = source;
    }

    /// <summary>
    /// Gets or sets the method to call when the event fires.
    /// </summary>
    public Action<TInstance, object, TEventArgs> OnEventAction
    {
        get => _onEventAction;
        init => _onEventAction = value;
    }

    /// <summary>
    /// Gets or sets the method to call when detaching from the event.
    /// </summary>
    public Action<WeakEventListener<TInstance, TSource, TEventArgs>, TSource> OnDetachAction
    {
        get => _onDetachAction;
        init => _onDetachAction = value;
    }

    /// <summary>
    /// Handler for the subscribed event calls OnEventAction to handle it.
    /// </summary>
    /// <param name="source">Event source.</param>
    /// <param name="eventArgs">Event arguments.</param>
    public void OnEvent(object source, TEventArgs eventArgs)
    {
        if (_weakInstance is null)
        {
            return;
        }

        if (_weakInstance.TryGetTarget(out TInstance target))
        {
            // Call the registered action.
            _onEventAction?.Invoke(target, source, eventArgs);
        }
        else
        {
            // Detach from the event.
            Detach();
        }
    }

    /// <summary>
    /// Detaches from the subscribed event.
    /// </summary>
    public void Detach()
    {
        var onDetachAction = _onDetachAction;
        if (onDetachAction != null)
        {
            _onDetachAction = null;
            onDetachAction(this, _source);
        }

        _onEventAction = null;
        _weakInstance = null;
        _source = null;
    }
}