
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
using System.Windows;
using System.Xaml.Schema;

namespace OpenSilver.Internal.Xaml;

internal sealed class OpenSilverMemberInvoker : XamlMemberInvoker
{
    private readonly OpenSilverXamlMember _member;

    public OpenSilverMemberInvoker(OpenSilverXamlMember member)
        : base(member)
    {
        _member = member ?? throw new ArgumentNullException(nameof(member));
    }

    public override object GetValue(object instance)
    {
        if (instance is DependencyObject d && _member.DependencyProperty is DependencyProperty dp)
        {
            return d.GetValue(dp);
        }

        return base.GetValue(instance);
    }

    public override void SetValue(object instance, object value)
    {
        if (instance is DependencyObject d && _member.DependencyProperty is DependencyProperty dp)
        {
            d.SetValue(dp, value);
            return;
        }

        base.SetValue(instance, value);
    }
}
