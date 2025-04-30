
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
/// <see cref="TypeDescriptor.GetProperties(object)"/> method. This enumeration is used 
/// to specify the value of the <see cref="PropertyFilterAttribute.Filter"/> property.
/// </summary>
[Flags]
public enum PropertyFilterOptions
{
    /// <summary>
    /// Return no properties.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Return only those properties that are not valid given the current context of the object.
    /// </summary>
    Invalid = 0x01,

    /// <summary>
    /// Return only those properties that have local values currently set.
    /// </summary>
    SetValues = 0x02,

    /// <summary>
    /// Return only those properties whose local values are not set, or do not have properties 
    /// set in an external expression store (such as binding or deferred resource).
    /// </summary>
    UnsetValues = 0x04,

    /// <summary>
    /// Return any property that is valid on the object in the current scope.
    /// </summary>
    Valid = 0x08,

    /// <summary>
    /// Return all properties.
    /// </summary>
    All = 0x0F,
}