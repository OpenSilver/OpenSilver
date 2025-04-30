
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
/// Provides a base class for .NET attributes that report the use scope of attached properties.
/// </summary>
public abstract class AttachedPropertyBrowsableAttribute : Attribute
{
    /// <summary>
    ///     Used to determine the browsable algorithm.  Normally, all 
    ///     AttachedPropertyBrowsable attributes must return true from 
    ///     IsBrowsable in order for the property to be considered browsable 
    ///     for the given dependency object.  If UnionResults is true, the 
    ///     IsBrowsable result from all AttachedPropertyBrowsable attributes 
    ///     of the same type will be logically or-ed together, and the result 
    ///     will be used to test for browsability.  UnionResults only applies 
    ///     to attributes of the same type.
    /// </summary>
    internal virtual bool UnionResults => false;

    /// <summary>
    ///     Returns true if the object allows the given dependency property 
    ///     should be visible on the given dependency object.
    /// </summary>
    internal abstract bool IsBrowsable(DependencyObject d, DependencyProperty dp);
}