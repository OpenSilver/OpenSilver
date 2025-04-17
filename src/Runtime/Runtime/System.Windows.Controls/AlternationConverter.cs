
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

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows.Controls;

/// <summary>
/// Converts an integer to and from an object by applying the integer as an index to a list of objects.
/// </summary>
[ContentProperty(nameof(Values))]
public class AlternationConverter : IValueConverter
{
    private readonly List<object> _values = [];

    /// <summary>
    /// Gets a list of objects that the <see cref="AlternationConverter"/> returns when an integer is passed to 
    /// the <see cref="Convert(object, Type, object, CultureInfo)"/> method.
    /// </summary>
    /// <returns>
    /// A list of objects that the <see cref="AlternationConverter"/> returns when an integer is passed to the 
    /// <see cref="Convert(object, Type, object, CultureInfo)"/> method.
    /// </returns>
    public IList Values => _values;

    /// <summary>
    /// Converts an integer to an object in the <see cref="Values"/> list.
    /// </summary>
    /// <param name="o">
    /// The integer to use to find an object in the <see cref="Values"/> property.
    /// </param>
    /// <param name="targetType">
    /// The type of the binding target property.
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter.
    /// </param>
    /// <returns>
    /// The object that is in the position of o modulo the number of items in <see cref="Values"/>.
    /// </returns>
    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
        if (_values.Count > 0 && o is int index)
        {
            index %= _values.Count;

            // Adjust for incorrect definition of the %-operator for negative arguments.
            if (index < 0)
            {
                index += _values.Count;
            }

            return _values[index];
        }

        return DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Converts an object in the <see cref="Values"/> list to an integer.
    /// </summary>
    /// <param name="o">
    /// The object to find in the <see cref="Values"/> property.
    /// </param>
    /// <param name="targetType">
    /// The type of the binding target property.
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter.
    /// </param>
    /// <returns>
    /// The index of o if it is in <see cref="Values"/>, or -1 if o does not exist in <see cref="Values"/>.
    /// </returns>
    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture) => _values.IndexOf(o);
}
