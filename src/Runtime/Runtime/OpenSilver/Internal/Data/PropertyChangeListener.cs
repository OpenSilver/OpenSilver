
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
using System.Windows;

namespace OpenSilver.Internal.Data;

internal sealed class PropertyChangeListener : IDisposable
{
    private readonly DependencyProperty _dp;
    private DependencyObject _source;
    private PropertyChangedCallback _sourceCallBack;

    public static PropertyChangeListener CreateListener(
        DependencyObject source,
        DependencyProperty dp,
        PropertyChangedCallback sourceCallBack)
    {
        Debug.Assert(source is not null);
        Debug.Assert(dp is not null);
        Debug.Assert(sourceCallBack is not null);

        // A Sealed DependencyObject does not have a Dependents list so don't bother updating it.
        if (source.IsSealed)
        {
            return null;
        }

        var listener = new PropertyChangeListener(source, dp, sourceCallBack);
        source.AddDependent(dp, listener);
        return listener;
    }

    private PropertyChangeListener(
        DependencyObject source,
        DependencyProperty dp,
        PropertyChangedCallback sourceCallBack)
    {
        _source = source;
        _dp = dp;
        _sourceCallBack = sourceCallBack;
    }

    public void Dispose()
    {
        if (_source is null)
        {
            return;
        }

        _source.RemoveDependent(_dp, this);
        _source = null;
        _sourceCallBack = null;
    }

    public void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) => _sourceCallBack?.Invoke(sender, args);
}
