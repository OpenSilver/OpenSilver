

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
using System.Globalization;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Exposes methods that allow the data to be modified as it passes through the binding engine.
    /// </summary>
    public partial interface IValueConverter
    {
       
        
#if MIGRATION
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The System.Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        object Convert(object value, Type targetType, object parameter, CultureInfo culture);
#else
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        object Convert(object value, Type targetType, object parameter, string language);
#endif


#if MIGRATION
        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in System.Windows.Data.BindingMode.TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The System.Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
#else
        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion</param>
        /// <returns>The value to be passed to the source object.</returns>
        object ConvertBack(object value, Type targetType, object parameter, string language);
#endif
    }
}
