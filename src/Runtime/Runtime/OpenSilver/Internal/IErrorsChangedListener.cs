
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
using System.ComponentModel;

namespace OpenSilver.Internal;

internal interface IErrorsChangedListener
{
    void OnErrorsChanged(object sender, DataErrorsChangedEventArgs args);
}

internal sealed class WeakErrorsChangedListener
{
    private WeakReference<IErrorsChangedListener> _weakRef;
    private INotifyDataErrorInfo _source;

    public WeakErrorsChangedListener(IErrorsChangedListener listener, INotifyDataErrorInfo source)
    {
        if (listener is null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _weakRef = new WeakReference<IErrorsChangedListener>(listener);
        _source = source;
        source.ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(OnErrorsChanged);
    }

    private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
        if (!_weakRef.TryGetTarget(out IErrorsChangedListener listener))
        {
            Dispose();
            return;
        }

        listener.OnErrorsChanged(sender, e);
    }

    public void Dispose()
    {
        if (_source is null)
        {
            return;
        }

        _source.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(OnErrorsChanged);
        _source = null;
        _weakRef = null;
    }
}