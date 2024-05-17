
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
using System.Xaml;

namespace OpenSilver.Internal.Xaml;

internal class OpenSilverXamlType : XamlType
{
    public OpenSilverXamlType(Type underlyingType, XamlSchemaContext schema)
        : base(underlyingType, schema)
    {
    }

    protected override XamlMember LookupMember(string name, bool skipReadOnlyCheck) => FindMember(name, false, skipReadOnlyCheck);

    protected override XamlMember LookupAttachableMember(string name) => FindMember(name, true, false);

    private XamlMember FindMember(string name, bool isAttached, bool skipReadOnlyCheck)
    {
        // Look for members backed by DPs
        if (FindDependencyPropertyBackedProperty(name, isAttached, skipReadOnlyCheck) is XamlMember member)
        {
            return member;
        }

        // Ask the base class (XamlType) to find the member
        if (isAttached)
        {
            member = base.LookupAttachableMember(name);
        }
        else
        {
            member = base.LookupMember(name, skipReadOnlyCheck);
        }

        return member;
    }

    private XamlMember FindDependencyPropertyBackedProperty(string name, bool isAttachable, bool skipReadOnlyCheck)
    {
        XamlMember xamlMember = null;

        // If it's a dependency property, return a wrapping XamlMember
        if (DependencyProperty.FromName(name, UnderlyingType) is DependencyProperty property)
        {
            // Try to find the MemberInfo so we can use that directly.  There's no direct way to do this
            // with XamlType so we'll just get the XamlMember and get the underlying member
            if (isAttachable)
            {
                xamlMember = GetAttachedDependencyProperty(name, property);
                if (xamlMember is null)
                {
                    return null;
                }
            }
            else
            {
                xamlMember = GetRegularDependencyProperty(name, property, skipReadOnlyCheck);
                xamlMember ??= GetAttachedDependencyProperty(name, property);
                xamlMember ??= new OpenSilverXamlMember(property, SchemaContext.GetXamlType(property.OwnerType), false);
            }
        }
        return xamlMember;
    }

    private XamlMember GetAttachedDependencyProperty(string name, DependencyProperty property)
    {
        if (base.LookupAttachableMember(name) is XamlMember memberFromBase)
        {
            return new OpenSilverXamlMember(
                property,
                memberFromBase.Invoker.UnderlyingGetter,
                memberFromBase.Invoker.UnderlyingSetter,
                SchemaContext);
        }
        return null;
    }

    private XamlMember GetRegularDependencyProperty(string name, DependencyProperty property, bool skipReadOnlyCheck)
    {
        if (base.LookupMember(name, skipReadOnlyCheck) is XamlMember memberFromBase)
        {
            if (memberFromBase.UnderlyingMember is PropertyInfo propertyInfo)
            {
                return new OpenSilverXamlMember(property, propertyInfo, SchemaContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        return null;
    }
}
