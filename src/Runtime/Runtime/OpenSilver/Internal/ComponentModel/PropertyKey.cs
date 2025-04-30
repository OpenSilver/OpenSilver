
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

// This structure is used as a key in a dictionary of property key -> property descriptor
// The key is unique based on the type the property is attached to, and the property
// itself.
internal readonly struct PropertyKey : IEquatable<PropertyKey>
{
    internal PropertyKey(Type attachedType, DependencyProperty prop)
    {
        DependencyProperty = prop;
        AttachedType = attachedType;
        _hashCode = AttachedType.GetHashCode() ^ DependencyProperty.GetHashCode();
    }

    public override int GetHashCode() => _hashCode;

    public override bool Equals(object obj) => Equals((PropertyKey)obj);

    public bool Equals(PropertyKey key) => key.AttachedType == AttachedType && key.DependencyProperty == DependencyProperty;

    public static bool operator ==(PropertyKey key1, PropertyKey key2) => key1.Equals(key2);

    public static bool operator !=(PropertyKey key1, PropertyKey key2) => !key1.Equals(key2);

    internal readonly DependencyProperty DependencyProperty;
    internal readonly Type AttachedType;
    private readonly int _hashCode;
}
