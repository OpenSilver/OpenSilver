
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
using System.Reflection;
using System.Windows;

namespace OpenSilver.Internal.Data;

internal sealed class ClrIndexerAccessor : IndexerAccessor
{
    private readonly PropertyInfo _indexer;

    public ClrIndexerAccessor(PropertyInfo indexer)
    {
        Debug.Assert(indexer is not null);
        _indexer = indexer;
    }

    public override Type PropertyType => _indexer.PropertyType;

    public override string GetPropertyName(string index) => $"{_indexer.Name}[{index}]";

    public override object GetValue(object component, object[] args)
    {
        try
        {
            return _indexer.GetValue(component, args);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public override void SetValue(object component, object[] args, object value)
    {
        try
        {
            _indexer.SetValue(component, value, args);
        }
        catch { }
    }
}
