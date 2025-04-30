
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

using System.Windows.Media;

namespace System.Windows;

/// <summary>
/// Specifies that an attached property has a browsable scope that extends to child elements in the logical tree.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class AttachedPropertyBrowsableForChildrenAttribute : AttachedPropertyBrowsableAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttachedPropertyBrowsableForChildrenAttribute"/> class.
    /// </summary>
    public AttachedPropertyBrowsableForChildrenAttribute() { }

    /// <summary>
    /// Gets or sets a value that declares whether to use the deep mode for detection of parent elements on the 
    /// attached property where this .NET attribute is applied.
    /// </summary>
    /// <returns>
    /// true if the attached property is browsable for all child elements in the logical tree of the parent element 
    /// that owns the attached property. false if the attached property is only browsable for immediate child elements 
    /// of a parent element that owns the attached property. The default is false.
    /// </returns>
    public bool IncludeDescendants { get; set; }

    /// <summary>
    /// Determines whether the current <see cref="AttachedPropertyBrowsableForChildrenAttribute"/> .NET attribute is 
    /// equal to a specified object.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="AttachedPropertyBrowsableForChildrenAttribute"/> to compare to the current 
    /// <see cref="AttachedPropertyBrowsableForChildrenAttribute"/>.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="AttachedPropertyBrowsableForChildrenAttribute"/> is equal to the current 
    /// <see cref="AttachedPropertyBrowsableForChildrenAttribute"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object obj) =>
        obj is AttachedPropertyBrowsableForChildrenAttribute other && IncludeDescendants == other.IncludeDescendants;

    /// <summary>
    /// Returns the hash code for this <see cref="AttachedPropertyBrowsableForChildrenAttribute"/> .NET attribute.
    /// </summary>
    /// <returns>
    /// An unsigned 32-bit integer value.
    /// </returns>
    public override int GetHashCode() => IncludeDescendants.GetHashCode();

    /// <summary>
    ///     Returns true if the object provided is the immediate logical 
    ///     child (if IncludeDescendants is false) or any logical child 
    ///     (if IncludeDescendants is true).
    /// </summary>
    internal override bool IsBrowsable(DependencyObject d, DependencyProperty dp)
    {
        if (d is null)
        {
            throw new ArgumentNullException(nameof(d));
        }
        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }

        DependencyObject walk = d;
        Type ownerType = dp.OwnerType;

        do
        {
            walk = GetFrameworkParent(walk);

            if (walk != null && ownerType.IsInstanceOfType(walk))
            {
                return true;
            }
        }
        while (IncludeDescendants && walk != null);

        return false;
    }

    private static DependencyObject GetFrameworkParent(DependencyObject element)
    {
        return element switch
        {
            FrameworkElement fe => fe.Parent ?? VisualTreeHelper.GetParent(fe),
            UIElement uie => VisualTreeHelper.GetParent(uie),
            _ => null,
        };
    }
}