
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

internal sealed class DependencyPropertyAccessor : PropertyAccessor
{
    private readonly DependencyProperty _dp;

    internal DependencyPropertyAccessor(DependencyProperty dp)
    {
        Debug.Assert(dp is not null);
        _dp = dp;
    }

    public override Type PropertyType => _dp.PropertyType;

    public override string PropertyName => _dp.Name;

    public override object GetValue(object component) => ((DependencyObject)component).GetValue(_dp);

    public override void SetValue(object component, object value) => ((DependencyObject)component).SetValue(_dp, value);
}
