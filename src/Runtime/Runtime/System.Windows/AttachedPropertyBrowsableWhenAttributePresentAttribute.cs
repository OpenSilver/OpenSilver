
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

using System.ComponentModel;

namespace System.Windows;

/// <summary>
/// Specifies that an attached property is only browsable on an element that also has another specific .NET attribute 
/// applied to its class definition.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class AttachedPropertyBrowsableWhenAttributePresentAttribute : AttachedPropertyBrowsableAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/> class.
    /// </summary>
    /// <param name="attributeType">
    /// The <see cref="Type"/> of the .NET attribute that must also be applied on a class in order for the attached property 
    /// to be browsable on the class where <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/> is applied.
    /// </param>
    public AttachedPropertyBrowsableWhenAttributePresentAttribute(Type attributeType)
    {
        ArgumentNullException.ThrowIfNull(attributeType);

        AttributeType = attributeType;
    }

    /// <summary>
    /// Gets the type of the .NET attribute that must also be applied on a class.
    /// </summary>
    /// <returns>
    /// The .NET attribute type.
    /// </returns>
    public Type AttributeType { get; }

    /// <summary>
    /// Determines whether the current <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/> .NET attribute 
    /// is equal to a specified object.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/> to compare to the current
    /// <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/>.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/> is equal to the current 
    /// <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object obj) =>
        obj is AttachedPropertyBrowsableWhenAttributePresentAttribute other && AttributeType == other.AttributeType;

    /// <summary>
    /// Returns the hash code for this <see cref="AttachedPropertyBrowsableWhenAttributePresentAttribute"/> .NET attribute.
    /// </summary>
    /// <returns>
    /// An unsigned 32-bit integer value.
    /// </returns>
    public override int GetHashCode() => AttributeType.GetHashCode();

    /// <summary>
    ///     Returns true if the dependency object class defines an attribute 
    ///     of the same type contained in this attribute.  The attribute must 
    ///     differ from the "default" state of the attribute.
    /// </summary>
    internal override bool IsBrowsable(DependencyObject d, DependencyProperty dp)
    {
        ArgumentNullException.ThrowIfNull(d);
        ArgumentNullException.ThrowIfNull(dp);

        Attribute a = TypeDescriptor.GetAttributes(d)[AttributeType];
        return a != null && !a.IsDefaultAttribute();
    }
}