
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

namespace OpenSilver.Internal.ComponentModel;

/// <summary>
///     This attribute is synthesized by our DependencyObjectProvider
///     to relate a property descriptor back to a dependency property.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class DependencyPropertyAttribute : Attribute
{
    /// <summary>
    ///     Creates a new DependencyPropertyAttribute for the given dependency property.
    /// </summary>
    internal DependencyPropertyAttribute(DependencyProperty dependencyProperty, bool isAttached)
    {
        if (dependencyProperty is null)
        {
            throw new ArgumentNullException(nameof(dependencyProperty));
        }

        DependencyProperty = dependencyProperty;
        IsAttached = isAttached;
    }

    /// <summary>
    ///     Override of Object.Equals that returns true when the dependency
    ///     property contained within each attribute is the same.
    /// </summary>
    public override bool Equals(object value)
    {
        if (value is DependencyPropertyAttribute da &&
            ReferenceEquals(da.DependencyProperty, DependencyProperty) &&
            da.IsAttached == IsAttached)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Override of Object.GetHashCode();
    /// </summary>
    public override int GetHashCode() => DependencyProperty.GetHashCode();

    /// <summary>
    ///     Overrides Attribute.TypeId to be unique with respect to
    ///     other dependency property attributes.c
    /// </summary>
    public override object TypeId => typeof(DependencyPropertyAttribute);

    /// <summary>
    ///     Returns whether the dependency property is an attached
    ///     property.
    /// </summary>
    internal bool IsAttached { get; }

    /// <summary>
    ///     Returns the dependency property instance this attribute is
    ///     associated with.
    /// </summary>
    internal DependencyProperty DependencyProperty { get; }
}