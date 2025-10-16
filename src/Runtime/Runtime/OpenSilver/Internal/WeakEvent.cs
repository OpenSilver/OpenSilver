
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
using System.Diagnostics;

namespace OpenSilver.Internal;

internal static class WeakEvent
{
    public static WeakEventToken Subscribe<TInstance, TSource, TEventArgs>(
        TInstance instance,
        TSource source,
        Action<TInstance, object, TEventArgs> onEvent,
        Action<Action<object, TEventArgs>, TSource> unsubscribe,
        Action<Action<object, TEventArgs>, TSource> subscribe)
        where TInstance : class
        where TSource : class
    {
        Debug.Assert(instance is not null);
        Debug.Assert(source is not null);
        Debug.Assert(onEvent is not null);
        Debug.Assert(unsubscribe is not null);
        Debug.Assert(subscribe is not null);

        var listener = new WeakEventListener<TInstance, TSource, TEventArgs>(instance, source, onEvent, unsubscribe);
        subscribe(listener.OnEvent, source);

        return new WeakEventToken(listener);
    }

    private sealed class WeakEventListener<TInstance, TSource, TEventArgs> : IDisposable
        where TInstance : class
        where TSource : class
    {
        private WeakReference<TInstance> _weakInstance;
        private TSource _source;
        private Action<TInstance, object, TEventArgs> _onEventAction;
        private Action<Action<object, TEventArgs>, TSource> _onDetachAction;

        public WeakEventListener(
            TInstance instance,
            TSource source,
            Action<TInstance, object, TEventArgs> onEvent,
            Action<Action<object, TEventArgs>, TSource> onDetach)
        {
            _weakInstance = new WeakReference<TInstance>(instance);
            _source = source;
            _onEventAction = onEvent;
            _onDetachAction = onDetach;
        }

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
                Dispose();
            }
        }

        public void Dispose()
        {
            var onDetachAction = _onDetachAction;
            if (onDetachAction != null)
            {
                _onDetachAction = null;
                onDetachAction(OnEvent, _source);
            }

            _onEventAction = null;
            _weakInstance = null;
            _source = null;
        }
    }
}

internal sealed class WeakEventToken : IDisposable
{
    private readonly IDisposable _listener;

    internal WeakEventToken(IDisposable listener)
    {
        _listener = listener;
    }

    ~WeakEventToken() => Dispose(false);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    private void Dispose(bool disposing) => _listener.Dispose();
}
