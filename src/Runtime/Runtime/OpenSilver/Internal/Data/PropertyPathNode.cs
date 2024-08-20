
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
using System.Windows;
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

internal abstract class PropertyPathNode : IPropertyPathNode
{
    private ICollectionView _icv;

    protected PropertyPathNode(BindingExpression listener)
    {
        Listener = listener;
    }

    public BindingExpression Listener { get; }

    public object Source { get; private set; }

    public object Value { get; set; } = DependencyProperty.UnsetValue;

    public bool IsBroken { get; private set; } = true;

    public IPropertyPathNode Next { get; set; }

    public abstract Type Type { get; }

    public abstract string PropertyName { get; }

    public abstract bool IsBound { get; }

    void IPropertyPathNode.SetSource(object source, bool transferValue) => SetSource(source, false, transferValue);

    void IPropertyPathNode.SetValue(object value) => SetValue(value);

    private void SetSource(object source, bool sourceIsCurrentItem, bool transferValue)
    {
        UpdateSource(source, sourceIsCurrentItem);
        UpdateValue(transferValue);
    }

    private void UpdateSource(object source, bool sourceIsCurrentItem)
    {
        object oldSource = Source;
        Source = source;

        if (oldSource != Source)
        {
            OnSourceChanged(oldSource, source, sourceIsCurrentItem);
        }
    }

    private void OnSourceChanged(object oldSource, object newSource, bool sourceIsCurrentItem)
    {
        if (!sourceIsCurrentItem && _icv != null)
        {
            if (Listener.IsDynamic)
            {
                _icv.CurrentChanged -= new EventHandler(OnCurrentChanged);
            }
            _icv = null;
        }

        OnSourceChanged(oldSource, Source);

        if (!sourceIsCurrentItem && !IsBound && newSource is ICollectionView icv)
        {
            if (Listener.IsDynamic)
            {
                icv.CurrentChanged += new EventHandler(OnCurrentChanged);
            }
            _icv = icv;
            UpdateSource(icv.CurrentItem, true);
        }
    }

    private void OnCurrentChanged(object sender, EventArgs e)
    {
        if (_icv == null)
        {
            return;
        }

        SetSource(_icv.CurrentItem, true, true);
    }

    internal void UpdateValue(bool transferValue)
    {
        OnUpdateValue();

        if (Next is IPropertyPathNode next)
        {
            next.SetSource(Value == DependencyProperty.UnsetValue ? null : Value, transferValue);
        }
        else if (transferValue)
        {
            Listener.TransferValue(Value);
        }
    }

    internal void UpdateValueAndIsBroken(object newValue, bool isBroken)
    {
        IsBroken = isBroken;
        Value = newValue;
    }

    internal abstract void OnSourceChanged(object oldSource, object newSource);

    internal abstract void OnUpdateValue();

    internal abstract void SetValue(object value);
}
