
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

internal interface IPropertyChangedListener
{
    void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
}

internal sealed class WeakPropertyChangedListener : IDisposable
{
    private WeakReference<IPropertyChangedListener> _weakRef;
    private INotifyPropertyChanged _source;

    public WeakPropertyChangedListener(IPropertyChangedListener listener, INotifyPropertyChanged source)
    {
        if (listener is null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _weakRef = new WeakReference<IPropertyChangedListener>(listener);
        _source = source;
        source.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (!_weakRef.TryGetTarget(out IPropertyChangedListener listener))
        {
            Dispose();
            return;
        }

        listener.OnPropertyChanged(sender, e);
    }

    public void Dispose()
    {
        if (_source is null)
        {
            return;
        }

        _source.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
        _source = null;
        _weakRef = null;
    }
}
