
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

using OpenSilver.Internal;
using System.Reflection;

namespace System.Windows;

/// <summary>
/// Defines or references resource keys based on class names in external assemblies, as well 
/// as an additional identifier.
/// </summary>
public class ComponentResourceKey : ResourceKey
{
    private Type _typeInTargetAssembly;
    private bool _typeInTargetAssemblyInitialized;
    private object _resourceId;
    private bool _resourceIdInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentResourceKey"/> class.
    /// </summary>
    public ComponentResourceKey() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentResourceKey"/> class, specifying the 
    /// <see cref="Type"/> that defines the key, and an object to use as an additional resource 
    /// identifier.
    /// </summary>
    /// <param name="typeInTargetAssembly">
    /// The type that defines the resource key.
    /// </param>
    /// <param name="resourceId">
    /// A unique identifier to differentiate this <see cref="ComponentResourceKey"/> from others 
    /// associated with the typeInTargetAssembly type.
    /// </param>
    public ComponentResourceKey(Type typeInTargetAssembly, object resourceId)
    {
        ArgumentNullException.ThrowIfNull(typeInTargetAssembly);
        ArgumentNullException.ThrowIfNull(resourceId);

        _typeInTargetAssembly = typeInTargetAssembly;
        _typeInTargetAssemblyInitialized = true;

        _resourceId = resourceId;
        _resourceIdInitialized = true;
    }

    /// <summary>
    /// Gets the assembly object that indicates which assembly's dictionary to look in for the 
    /// value associated with this key.
    /// </summary>
    /// <returns>
    /// The retrieved assembly, as a reflection class.
    /// </returns>
    public override Assembly Assembly => _typeInTargetAssembly?.Assembly;

    /// <summary>
    /// Gets or sets a unique identifier to differentiate this key from others associated with 
    /// this type.
    /// </summary>
    /// <returns>
    /// A unique identifier. Typically this is a string.
    /// </returns>
    public object ResourceId
    {
        get => _resourceId;
        set
        {
            if (_resourceIdInitialized)
            {
                throw new InvalidOperationException(Strings.ChangingIdNotAllowed);
            }

            _resourceId = value;
            _resourceIdInitialized = true;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="Type"/> that defines the resource key.
    /// </summary>
    /// <returns>
    /// The type that defines the resource key.
    /// </returns>
    public Type TypeInTargetAssembly
    {
        get => _typeInTargetAssembly;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (_typeInTargetAssemblyInitialized)
            {
                throw new InvalidOperationException(Strings.ChangingTypeNotAllowed);
            }

            _typeInTargetAssembly = value;
            _typeInTargetAssemblyInitialized = true;
        }
    }

    /// <summary>
    /// Determines whether the provided object is equal to the current <see cref="ComponentResourceKey"/>.
    /// </summary>
    /// <param name="o">
    /// Object to compare with the current <see cref="ComponentResourceKey"/>.
    /// </param>
    /// <returns>
    /// true if the objects are equal; otherwise, false.
    /// </returns>
    public override bool Equals(object o)
    {
        return o is ComponentResourceKey key &&
               _typeInTargetAssembly == key._typeInTargetAssembly &&
               Equals(_resourceId, key._resourceId);
    }

    /// <summary>
    /// Returns a hash code for this <see cref="ComponentResourceKey"/>.
    /// </summary>
    /// <returns>
    /// A signed 32-bit integer value.
    /// </returns>
    public override int GetHashCode()
        => (_typeInTargetAssembly?.GetHashCode() ?? 0) ^ (_resourceId?.GetHashCode() ?? 0);

    /// <summary>
    /// Gets the string representation of a <see cref="ComponentResourceKey"/>.
    /// </summary>
    /// <returns>
    /// The string representation.
    /// </returns>
    public override string ToString()
    {
        string targetType = _typeInTargetAssembly?.FullName ?? "null";
        string resourceId = _resourceId is null ? "null" : _resourceId.ToString();
        return $"TargetType={targetType} ID={resourceId}";
    }
}
