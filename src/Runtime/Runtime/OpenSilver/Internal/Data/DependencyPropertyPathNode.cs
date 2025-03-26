
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
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

internal sealed class DependencyPropertyPathNode : PropertyPathNode
{
    private readonly DependencyProperty _dp;
    private PropertyChangeListener _dpListener;

    public DependencyPropertyPathNode(BindingExpression listener, DependencyProperty dp)
        : base(listener)
    {
        Debug.Assert(dp is not null);
        _dp = dp;
    }

    public override Type Type => _dp.PropertyType;

    public override string PropertyName => _dp.Name;

    public override bool IsBound => Source is not null;

    internal override void SetValue(object value)
    {
        if (Source is DependencyObject source)
        {
            source.SetValue(_dp, value);
        }
    }

    internal override void OnUpdateValue()
    {
        if (Source is DependencyObject source)
        {
            UpdateValueAndIsBroken(source.GetValue(_dp), false);
        }
        else
        {
            UpdateValueAndIsBroken(DependencyProperty.UnsetValue, true);
        }
    }

    internal override void OnSourceChanged(object oldSource, object newSource)
    {
        if (_dpListener is PropertyChangeListener listener)
        {
            _dpListener = null;
            listener.Dispose();
        }

        if (Listener.IsDynamic && newSource is DependencyObject source)
        {
            _dpListener = PropertyChangeListener.CreateListener(source, _dp, OnPropertyChanged);
        }
    }

    private void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) => UpdateValue(true);
}
