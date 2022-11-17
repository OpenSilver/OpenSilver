
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
using System.Collections.Specialized;

namespace OpenSilver.Internal;

internal interface ICollectionChangedListener
{
    void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
}

internal sealed class WeakCollectionChangedListener : IDisposable
{
    private WeakReference<ICollectionChangedListener> _weakRef;
    private INotifyCollectionChanged _source;

    public WeakCollectionChangedListener(ICollectionChangedListener listener, INotifyCollectionChanged source)
    {
        if (listener is null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _weakRef = new WeakReference<ICollectionChangedListener>(listener);
        _source = source;
        source.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_weakRef.TryGetTarget(out ICollectionChangedListener listener))
        {
            Dispose();
            return;
        }

        listener.OnCollectionChanged(sender, e);
    }

    public void Dispose()
    {
        if (_source is null)
        {
            return;
        }

        _source.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        _source = null;
        _weakRef = null;
    }
}
