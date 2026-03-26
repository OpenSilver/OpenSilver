
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
using System.Xaml.Markup;

namespace OpenSilver.Internal;

internal static class Helper
{
    public const string ObsoleteMemberMessage = "Deprecated. It will be removed in a future release.";

    internal static EventHandler<XamlSetMarkupExtensionEventArgs> LookupSetMarkupExtensionHandler(Type type)
    {
        if (typeof(Setter) == type)
        {
            return Setter.ReceiveMarkupExtension;
        }
        else if (typeof(DataTrigger) == type)
        {
            return DataTrigger.ReceiveMarkupExtension;
        }
        else if (typeof(Condition) == type)
        {
            return Condition.ReceiveMarkupExtension;
        }
        return null;
    }

    internal static EventHandler<XamlSetTypeConverterEventArgs> LookupSetTypeConverterHandler(Type type)
    {
        if (typeof(Setter).IsAssignableFrom(type))
        {
            return Setter.ReceiveTypeConverter;
        }
        else if (typeof(Trigger).IsAssignableFrom(type))
        {
            return Trigger.ReceiveTypeConverter;
        }
        else if (typeof(Condition).IsAssignableFrom(type))
        {
            return Condition.ReceiveTypeConverter;
        }
        return null;
    }
}
