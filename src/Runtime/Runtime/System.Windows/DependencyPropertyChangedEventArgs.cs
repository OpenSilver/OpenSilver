
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

namespace System.Windows;

/// <summary>
/// Provides data for a <see cref="PropertyChangedCallback"/> implementation.
/// </summary>
public readonly struct DependencyPropertyChangedEventArgs
{
    internal DependencyPropertyChangedEventArgs(
        object oldValue,
        object newValue,
        DependencyProperty property,
        PropertyMetadata metadata)
    {
        OldValue = oldValue;
        OldValueSource = BaseValueSourceInternal.Unknown;
        NewValue = newValue;
        NewValueSource = BaseValueSourceInternal.Unknown;
        Property = property;
        Metadata = metadata;
        OperationType = OperationType.Unknown;
    }

    internal DependencyPropertyChangedEventArgs(
        object oldValue,
        BaseValueSourceInternal oldValueSource,
        object newValue,
        BaseValueSourceInternal newValueSource,
        DependencyProperty property,
        PropertyMetadata metadata,
        OperationType operationType)
    {
        OldValue = oldValue;
        OldValueSource = oldValueSource;
        NewValue = newValue;
        NewValueSource = newValueSource;
        Property = property;
        Metadata = metadata;
        OperationType = operationType;
    }

    /// <summary>
    /// Gets the value of the property before the change.
    /// </summary>
    /// <returns>
    /// The property value before the change.
    /// </returns>
    public object OldValue { get; }

    /// <summary>
    /// Gets the value of the property after the change.
    /// </summary>
    /// <returns>
    /// The property value after the change.
    /// </returns>
    public object NewValue { get; }

    /// <summary>
    /// Gets the identifier for the dependency property where the value change occurred.
    /// </summary>
    /// <returns>
    /// The identifier field of the dependency property where the value change occurred.
    /// </returns>
    public DependencyProperty Property { get; }

    /// <summary>
    /// Metadata for the property
    /// </summary>
    internal PropertyMetadata Metadata { get; }

    /// <summary>
    /// The source of the old value
    /// </summary>
    internal BaseValueSourceInternal OldValueSource { get; }

    /// <summary>
    /// The source of the new value
    /// </summary>
    internal BaseValueSourceInternal NewValueSource { get; }

    internal OperationType OperationType { get; }
}

internal enum OperationType : byte
{
    Unknown = 0,
    Inherit = 1,
    ChangeMutableDefaultValue = 2,
}
