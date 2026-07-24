
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
using System.Windows.Markup;

namespace OpenSilver.Internal;

internal static class Helper
{
    public const string ObsoleteMemberMessage = "Deprecated. It will be removed in a future release.";

    /// <summary>
    /// Check whether xxxTemplate property is set on the given element.
    /// Only explicit local values or resource references count;  data-bound or templated values don't count.
    /// </summary>
    internal static bool IsTemplateDefined(DependencyProperty templateProperty, DependencyObject d)
    {
        // Check whether xxxTemplate property is set on the given element.
        object template = d.ReadLocalValue(templateProperty);
        // the checks for UnsetValue and null are for perf:
        // they're redundant to the type checks, but they're cheaper
        return (template != DependencyProperty.UnsetValue &&
                template is not null &&
                (template is FrameworkTemplate || template is ResourceReferenceExpression));
    }

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
