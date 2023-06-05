
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSHTML5.Internal;
using Microsoft.JSInterop;

namespace OpenSilver;

/// <summary>
/// Helps to get the current state (values of properties) of the provided object.
/// </summary>
public static class DumpHelper
{
    /// <summary>
    /// Retrieves a dictionary of properties and corresponding values of the provided object.
    /// </summary>
    /// <param name="obj">Object to get properties.</param>
    /// <param name="names">
    /// Names of properties, provided as string of names separated by comma (,).
    /// Null or empty value means all public properties are return.
    /// </param>
    /// <exception cref="ArgumentNullException">Provided object is null</exception>
    public static IDictionary<string, object> GetPropertiesAndValues(object obj, params string[] names)
    {
        AssertDEBUG();

        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        Type type = obj.GetType();
        var properties = new List<PropertyInfo>();

        if (names == null || names.Length == 0)
        {
            properties.AddRange(type.GetProperties());
        }
        else
        {
            foreach (var name in names)
            {
                try
                {
                    var property = type.GetProperty(name);
                    if (property != null)
                    {
                        properties.Add(property);
                    }
                }
                catch (AmbiguousMatchException) { }
            }
        }

        var d = new SortedDictionary<string, object>();
        foreach (PropertyInfo prop in properties)
        {
            if (d.ContainsKey(prop.Name) || prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            d[prop.Name] = prop.GetValue(obj);
        }

        return d;
    }

    /// <summary>
    /// Retrieves a dictionary of properties and corresponding values of the provided element by it's id.
    /// </summary>
    /// <param name="elementId">Html id of an element.</param>
    /// <param name="names">
    /// Names of properties, provided as string of names separated by comma (,).
    /// Null or empty value means all public properties are return.
    /// </param>
    [JSInvokable]
    public static IDictionary<string, string> DumpProperties(string elementId, params string[] names)
    {
        AssertDEBUG();

        if (INTERNAL_HtmlDomManager.GetElementById(elementId) is { } element)
        {
            return GetPropertiesAndValues(element, names)
                .ToDictionary(x => x.Key, x => x.Value?.ToString());
        }

        return null;
    }

    /// <summary>
    /// Prints properties and corresponding values to console.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="names">
    /// Names of properties, provided as string of names separated by comma (,).
    /// Null or empty value means all public properties are return.
    /// </param>
    public static void PrintProperties(object element, params string[] names)
    {
        AssertDEBUG();

        Console.WriteLine($"=== {element} Properties ===");

        foreach (var item in GetPropertiesAndValues(element, names))
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
    }

    private static void AssertDEBUG()
    {
#if !DEBUG
        throw new InvalidOperationException();
#endif
    }
}
