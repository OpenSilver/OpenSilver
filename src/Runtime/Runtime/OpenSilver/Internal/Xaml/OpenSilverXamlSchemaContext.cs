
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
using System.Collections.Concurrent;
using System.Xaml;

namespace OpenSilver.Internal.Xaml;

internal sealed class OpenSilverXamlSchemaContext : XamlSchemaContext
{
    private readonly ConcurrentDictionary<Type, XamlType> _runtimeTypes = new();

    public override XamlType GetXamlType(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return _runtimeTypes.GetOrAdd(type, CreateXamlType);
    }

    private XamlType CreateXamlType(Type type) => new OpenSilverXamlType(type, this);
}
