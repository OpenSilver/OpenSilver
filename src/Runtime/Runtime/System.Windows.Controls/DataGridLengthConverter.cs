

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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of various types to and from instances of the System.Windows.Controls.DataGridLength
    /// class.
    /// </summary>
    public sealed partial class DataGridLengthConverter : TypeConverter
    {
        
        /// <summary>
        /// Determines whether an instance of the specified type can be converted to
        /// an instance of the System.Windows.Controls.DataGridLength class.
        /// </summary>
        /// <param name="context">An object that provides a format context.</param>
        /// <param name="sourceType">The type to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
        
        /// <summary>
        /// Determines whether an instance of the System.Windows.Controls.DataGridLength
        /// class can be converted to an instance of the specified type.
        /// </summary>
        /// <param name="context">An object that provides a format context.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        
        // Exceptions:
        //   System.ArgumentNullException:
        //     value is null.
        //
        //   System.ArgumentException:
        //     value is not a valid type that can be converted to type System.Windows.Controls.DataGridLength.
        /// <summary>
        /// Converts the specified object to an instance of the System.Windows.Controls.DataGridLength
        /// class.
        /// </summary>
        /// <param name="context">An object that provides a format context.</param>
        /// <param name="culture">The object to use as the current culture.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return DataGridLength.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     destinationType is null.
        //
        //   System.ArgumentException:
        //     value is not a System.Windows.Controls.DataGridLength or destinationType
        //     is not a valid conversion type.
        /// <summary>
        /// Converts an instance of the System.Windows.Controls.DataGridLength class
        /// to an instance of the specified type.
        /// </summary>
        /// <param name="context">An object that provides a format context.</param>
        /// <param name="culture">The object to use as the current culture.</param>
        /// <param name="value">The System.Windows.Controls.DataGridLength to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}