
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

using System.Globalization;

namespace System.Windows.Data;

/// <summary>
/// Provides a way to apply custom logic in a <see cref="MultiBinding"/>.
/// </summary>
public interface IMultiValueConverter
{
    /// <summary>
    /// Converts source values to a value for the binding target. The data binding engine calls this 
    /// method when it propagates the values from source bindings to the binding target.
    /// </summary>
    /// <param name="values">
    /// The array of values that the source bindings in the <see cref="MultiBinding"/> produces. The 
    /// value <see cref="DependencyProperty.UnsetValue"/> indicates that the source binding has no 
    /// value to provide for conversion.
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
    /// A converted value. If the method returns null, the valid null value is used. A return value 
    /// of <see cref="DependencyProperty.UnsetValue"/> indicates that the converter did not produce 
    /// a value, and that the binding will use the <see cref="BindingBase.FallbackValue"/> if it is 
    /// available, or else will use the default value.
    /// </returns>
    object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

    /// <summary>
    /// Converts a binding target value to the source binding values.
    /// </summary>
    /// <param name="value">
    /// The value that the binding target produces.
    /// </param>
    /// <param name="targetTypes">
    /// The array of types to convert to. The array length indicates the number and types of values 
    /// that are suggested for the method to return.
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter.
    /// </param>
    /// <returns>
    /// An array of values that have been converted from the target value back to the source values.
    /// </returns>
    object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
}
