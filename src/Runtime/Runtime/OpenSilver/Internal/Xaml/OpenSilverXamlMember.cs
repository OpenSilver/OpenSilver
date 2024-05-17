
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
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;

namespace OpenSilver.Internal.Xaml;

internal class OpenSilverXamlMember : XamlMember, IProvideValueTarget
{
    public OpenSilverXamlMember(DependencyProperty dp, XamlType declaringType, bool isAttachable)
        : base(dp.Name, declaringType, isAttachable)
    {
        DependencyProperty = dp;
    }

    public OpenSilverXamlMember(DependencyProperty dp, MethodInfo getter, MethodInfo setter, XamlSchemaContext schemaContext)
        : base(dp.Name, getter, setter, schemaContext)
    {
        DependencyProperty = dp;
    }

    public OpenSilverXamlMember(DependencyProperty dp, PropertyInfo property, XamlSchemaContext schemaContext)
        : base(property, schemaContext)
    {
        DependencyProperty = dp;
    }

    public DependencyProperty DependencyProperty { get; }

    protected override XamlMemberInvoker LookupInvoker() => new OpenSilverMemberInvoker(this);

    object IProvideValueTarget.TargetProperty => DependencyProperty ?? (object)UnderlyingMember;

    object IProvideValueTarget.TargetObject => throw new NotSupportedException();
}
