
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

using System.Windows;
using System.Diagnostics;

namespace OpenSilver.Internal;

internal sealed class UncommonField<T>
{
    private readonly T _defaultValue;
    private bool _hasBeenSet;

    /// <summary>
    ///     Create a new UncommonField.
    /// </summary>
    public UncommonField()
        : this(default(T))
    {
    }

    /// <summary>
    ///     Create a new UncommonField.
    /// </summary>
    /// <param name="defaultValue">The default value of the field.</param>
    public UncommonField(T defaultValue)
    {
        _defaultValue = defaultValue;
        _hasBeenSet = false;

        lock (DependencyProperty.Synchronized)
        {
            GlobalIndex = DependencyProperty.GetUniqueGlobalIndex();

            DependencyProperty.RegisteredPropertyList.Add(null);
        }
    }

    internal int GlobalIndex { get; }

    /// <summary>
    ///     Write the given value onto a DependencyObject instance.
    /// </summary>
    /// <param name="instance">The DependencyObject on which to set the value.</param>
    /// <param name="value">The value to set.</param>
    public void SetValue(DependencyObject instance, T value)
    {
        Debug.Assert(instance is not null);

        // Set the value if it's not the default, otherwise remove the value.
        if (!ReferenceEquals(value, _defaultValue))
        {
            instance.GetUncommonStorage(GlobalIndex).LocalValue = value;
            _hasBeenSet = true;
        }
        else
        {
            ClearValue(instance);
        }
    }

    /// <summary>
    ///     Read the value of this field on a DependencyObject instance.
    /// </summary>
    /// <param name="instance">The DependencyObject from which to get the value.</param>
    /// <returns></returns>
    public T GetValue(DependencyObject instance)
    {
        Debug.Assert(instance is not null);

        if (_hasBeenSet)
        {
            if (instance.GetStorage(GlobalIndex) is Storage storage)
            {
                object value = storage.LocalValue;

                if (value != DependencyProperty.UnsetValue)
                {
                    return (T)value;
                }
            }
        }

        return _defaultValue;
    }


    /// <summary>
    ///     Clear this field from the given DependencyObject instance.
    /// </summary>
    /// <param name="instance"></param>
    public void ClearValue(DependencyObject instance)
    {
        Debug.Assert(instance is not null);

        instance.RemoveUncommonStorage(GlobalIndex);
    }
}
