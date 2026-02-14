
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
using System.Windows;
using CSHTML5.Internal;

namespace OpenSilver.Internal;

internal interface IResizeObserverListener
{
    void OnSizeChanged(Size size);
}

internal static class ResizeObserver
{
    private static readonly Dictionary<string, WeakListenerList> _listeners = [];

    static ResizeObserver()
    {
        var jsCallback = Interop.GetVariableStringForJS(JavaScriptCallback.Create(OnSizeChangedCallback));
        Interop.ExecuteJavaScriptVoidAsync($"document.createResizeManager({jsCallback})");
    }

    private static void OnSizeChangedCallback(string id, double width, double height)
    {
        if (!_listeners.TryGetValue(id, out WeakListenerList listeners))
        {
            return;
        }

        LinkedListNode<WeakListener> weakListener = listeners.First;

        while (weakListener is not null)
        {
            if (weakListener.Value.TryGetListener(out IResizeObserverListener listener))
            {
                listener.OnSizeChanged(new Size(width, height));
            }

            weakListener = weakListener.Next;
        }
    }

    public static IDisposable Observe(HtmlElementReference element, IResizeObserverListener listener)
    {
        Debug.Assert(element.IsConnected && !string.IsNullOrEmpty(element.Uid));

        lock (_listeners)
        {
            string id = element.Uid;

            if (!_listeners.TryGetValue(id, out WeakListenerList listeners))
            {
                listeners = new(id);
                _listeners[id] = listeners;
                Interop.ExecuteJavaScriptVoidAsync($"document.resizeManager.observe('{element.Uid}')");
            }

            LinkedListNode<WeakListener> weakListener = listeners.AddLast(new WeakListener(listener));
            return new Disposable(weakListener);
        }
    }

    private sealed class WeakListenerList : LinkedList<WeakListener>
    {
        public WeakListenerList(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }

    private sealed class WeakListener
    {
        private readonly WeakReference<IResizeObserverListener> _listener;

        public WeakListener(IResizeObserverListener listener)
        {
            _listener = new(listener);
        }

        public bool TryGetListener(out IResizeObserverListener listener) => _listener.TryGetTarget(out listener);
    }

    private static void Unobserve(LinkedListNode<WeakListener> listener)
    {
        var list = (WeakListenerList)listener.List;

        lock (_listeners)
        {
            list.Remove(listener);
            if (list.Count == 0)
            {
                _listeners.Remove(list.Id);
                Interop.ExecuteJavaScriptVoidAsync($"document.resizeManager.unobserve('{list.Id}')");
            }
        }
    }

    private sealed class Disposable : IDisposable
    {
        private readonly LinkedListNode<WeakListener> _weakListener;
        private bool _disposed;

        public Disposable(LinkedListNode<WeakListener> weakListener)
        {
            _weakListener = weakListener;
        }

        ~Disposable() => Dispose(false);

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            Unobserve(_weakListener);
        }
    }
}
