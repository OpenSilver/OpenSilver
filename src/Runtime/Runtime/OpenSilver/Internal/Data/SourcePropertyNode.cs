
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
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

internal sealed class SourcePropertyNode : IPropertyPathNode
{
    private readonly BindingExpression _listener;
    private object _source;

    public SourcePropertyNode(BindingExpression listener)
    {
        _listener = listener;
    }

    object IPropertyPathNode.Value
    {
        get => _source;
        set => throw new NotSupportedException();
    }

    object IPropertyPathNode.Source => _source;

    void IPropertyPathNode.SetSource(object source, bool transferValue)
    {
        _source = source;
        if (transferValue)
        {
            _listener.TransferValue(source);
        }
    }

    bool IPropertyPathNode.IsBroken => false;

    Type IPropertyPathNode.Type => null;

    string IPropertyPathNode.PropertyName => string.Empty;

    IPropertyPathNode IPropertyPathNode.Next
    {
        get => null;
        set => throw new NotSupportedException();
    }

    void IPropertyPathNode.SetValue(object value) { }
}
