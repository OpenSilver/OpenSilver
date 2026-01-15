
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

internal sealed class ClrPropertyAccessor : PropertyAccessor
{
    private readonly PropertyInfo _propertyInfo;

    internal ClrPropertyAccessor(PropertyInfo propertyInfo)
    {
        Debug.Assert(propertyInfo is not null);
        _propertyInfo = propertyInfo;
    }

    public override Type PropertyType => _propertyInfo.PropertyType;

    public override string PropertyName => _propertyInfo.Name;

    public override object GetValue(object component) => _propertyInfo.GetValue(component);

    public override void SetValue(object component, object value) => _propertyInfo.SetValue(component, value);
}
