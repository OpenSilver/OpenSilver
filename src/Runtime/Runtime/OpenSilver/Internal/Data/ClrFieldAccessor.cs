
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

namespace OpenSilver.Internal.Data;

internal sealed class ClrFieldAccessor : PropertyAccessor
{
    private readonly FieldInfo _fieldInfo;

    internal ClrFieldAccessor(FieldInfo fieldInfo)
    {
        Debug.Assert(fieldInfo is not null);
        _fieldInfo = fieldInfo;
    }

    public override Type PropertyType => _fieldInfo.FieldType;

    public override string PropertyName => _fieldInfo.Name;

    public override object GetValue(object component) => _fieldInfo.GetValue(component);

    public override void SetValue(object component, object value) => _fieldInfo.SetValue(component, value);
}
