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

using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    public static Dictionary<string, object> GetPropertiesAndValues(object obj, string names = null)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        Type type = obj.GetType();
        var properties = new List<PropertyInfo>();

        if (string.IsNullOrEmpty(names))
        {
            properties.AddRange(type.GetProperties());
        }
        else
        {
            var propertyNames = names.Split(',');
            foreach (var name in propertyNames)
            {
                var property = type.GetProperty(name);
                if (property != null)
                {
                    properties.Add(property);
                }
            }
        }

        return properties.OrderBy(x => x.Name).ToDictionary(x => x.Name, x => x.GetValue(obj));
    }

    /// <summary>
    /// Retrieves a dictionary of properties and corresponding values of the provided element by it's id.
    /// </summary>
    /// <param name="elementId">Html id of an element.</param>
    /// <param name="names">
    /// Names of properties, provided as string of names separated by comma (,).
    /// Null or empty value means all public properties are return.
    /// </param>
    public static Dictionary<string, string> DumpProperties(string elementId, string names = null)
    {
        var element = INTERNAL_HtmlDomManager.GetElementById(elementId);
        var dictionary = GetPropertiesAndValues(element, names);

        return dictionary.ToDictionary(x => x.Key, x => x.Value?.ToString());
    }

    /// <summary>
    /// Prints properties and corresponding values to console.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="names">
    /// Names of properties, provided as string of names separated by comma (,).
    /// Null or empty value means all public properties are return.
    /// </param>
    public static void PrintProperties(object element, string names = null)
    {
        var dictionary = GetPropertiesAndValues(element, names);

        Console.WriteLine($"=== {element} Properties ===");
        foreach (var item in dictionary)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
    }
}
