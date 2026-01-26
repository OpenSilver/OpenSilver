
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

namespace System.Windows;

/// <summary>
/// Specifies that an attached property is browsable only for elements that derive from a specified type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AttachedPropertyBrowsableForTypeAttribute : AttachedPropertyBrowsableAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttachedPropertyBrowsableForTypeAttribute"/> class,
    /// using the provided targetType.
    /// </summary>
    /// <param name="targetType">
    /// The intended type that scopes the use of the attached property where this .NET attribute applies.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="targetType"/> is null.
    /// </exception>
    public AttachedPropertyBrowsableForTypeAttribute(Type targetType)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        TargetType = targetType;
    }

    /// <summary>
    /// Gets the base type that scopes the use of the attached property where this .NET attribute applies.
    /// </summary>
    /// <returns>
    /// The requested <see cref="Type"/>.
    /// </returns>
    public Type TargetType { get; }

    /// <summary>
    /// Gets a unique type identifier for this <see cref="AttachedPropertyBrowsableForTypeAttribute"/> .NET attribute.
    /// </summary>
    /// <returns>
    /// An object that is a unique identifier for the <see cref="AttachedPropertyBrowsableForTypeAttribute"/>.
    /// </returns>
    public override object TypeId => this;

    /// <summary>
    /// Determines whether the current <see cref="AttachedPropertyBrowsableForTypeAttribute"/> .NET attribute 
    /// is equal to a specified object.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="AttachedPropertyBrowsableForTypeAttribute"/> to compare to the current 
    /// <see cref="AttachedPropertyBrowsableForTypeAttribute"/>.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="AttachedPropertyBrowsableForTypeAttribute"/> is equal to the current 
    /// <see cref="AttachedPropertyBrowsableForTypeAttribute"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object obj) => obj is AttachedPropertyBrowsableForTypeAttribute other && TargetType == other.TargetType;

    /// <summary>
    /// Returns the hash code for this <see cref="AttachedPropertyBrowsableForTypeAttribute"/> .NET attribute.
    /// </summary>
    /// <returns>
    /// An unsigned 32-bit integer value.
    /// </returns>
    public override int GetHashCode() => TargetType.GetHashCode();

    /// <summary>
    ///     Returns true if the dependency object passed to the method is a type, 
    ///     subtype or implememts the interface of any of the the types contained 
    ///     in this object.
    /// </summary>
    internal override bool IsBrowsable(DependencyObject d, DependencyProperty dp)
    {
        ArgumentNullException.ThrowIfNull(d);
        ArgumentNullException.ThrowIfNull(dp);

        // Get the dependency object type for our target type.
        // We cannot assume the user didn't do something wrong and
        // feed us a type that is not a dependency object, but that is
        // rare enough that it is worth the try/catch here rather than
        // a double IsAssignableFrom (one here, and one in DependencyObjectType).
        // We still use a flag here rather than checking for a null
        // _dTargetType so that a bad property that throws won't consistently
        // slow the system down with ArgumentExceptions.

        if (!_dTargetTypeChecked)
        {
            try
            {
                _dTargetType = DependencyObjectType.FromSystemType(TargetType);
            }
            catch (ArgumentException)
            {
            }

            _dTargetTypeChecked = true;
        }


        if (_dTargetType != null && _dTargetType.IsInstanceOfType(d))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Returns true if a browsable match is true if any one of multiple
    ///     instances of the same type return true for IsBrowsable.  We override
    ///     this to return true because any one of a successfull match for 
    ///     IsBrowsable is accepted.
    /// </summary>
    internal override bool UnionResults => true;

    private DependencyObjectType _dTargetType;
    private bool _dTargetTypeChecked;
}