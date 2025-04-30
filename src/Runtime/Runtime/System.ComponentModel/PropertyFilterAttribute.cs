
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

namespace System.ComponentModel;

/// <summary>
/// Specifies which properties should be reported by type descriptors, specifically the 
/// <see cref="TypeDescriptor.GetProperties(object)"/> method.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public sealed class PropertyFilterAttribute : Attribute
{
    /// <summary>
    /// This member supports the .NET infrastructure and is not intended to be used directly from your code.
    /// </summary>
    public static readonly PropertyFilterAttribute Default = new(PropertyFilterOptions.All);

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyFilterAttribute"/> class.
    /// </summary>
    /// <param name="filter">
    /// The options that describe this property filter.
    /// </param>
    public PropertyFilterAttribute(PropertyFilterOptions filter)
    {
        Filter = filter;
    }

    /// <summary>
    /// Gets the filter options for this <see cref="PropertyFilterAttribute"/> .NET attribute.
    /// </summary>
    /// <returns>
    /// The property filter options.
    /// </returns>
    public PropertyFilterOptions Filter { get; }

    /// <summary>
    /// Returns a value that indicates whether the current <see cref="PropertyFilterAttribute"/> 
    /// .NET attribute is equal to a specified object.
    /// </summary>
    /// <param name="value">
    /// The object to compare to this <see cref="PropertyFilterAttribute"/>.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="PropertyFilterAttribute"/> is equal to the current 
    /// <see cref="PropertyFilterAttribute"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object value) => value is PropertyFilterAttribute a && Filter == a.Filter;

    /// <summary>
    /// Returns the hash code for the current <see cref="PropertyFilterAttribute"/> .NET attribute.
    /// </summary>
    /// <returns>
    /// A signed 32-bit integer value.
    /// </returns>
    public override int GetHashCode() => Filter.GetHashCode();

    /// <summary>
    /// Returns a value that indicates whether the property filter options of the current 
    /// <see cref="PropertyFilterAttribute"/> .NET attribute match the property filter options 
    /// of the provided object.
    /// </summary>
    /// <param name="value">
    /// The object to compare. This object is expected to be a <see cref="PropertyFilterAttribute"/>.
    /// </param>
    /// <returns>
    /// true if a match exists; otherwise, false.
    /// </returns>
    public override bool Match(object value) => value is PropertyFilterAttribute a && (Filter & a.Filter) == Filter;
}