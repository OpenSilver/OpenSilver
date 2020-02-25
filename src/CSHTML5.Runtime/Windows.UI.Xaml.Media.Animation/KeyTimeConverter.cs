
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of System.Windows.Media.Animation.KeyTime to and from
    /// other types.
    /// </summary>
    public partial class KeyTimeConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object can be converted from a given type to an instance
        /// of a System.Windows.Media.Animation.KeyTime.
        /// </summary>
        /// <param name="typeDescriptorContext">Contextual information required for conversion.</param>
        /// <param name="type">Type being evaluated for conversion.</param>
        /// <returns>true if this type can be converted; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
  
        /// <summary>
        /// Determines if a given type can be converted to an instance of System.Windows.Media.Animation.KeyTime.
        /// </summary>
        /// <param name="typeDescriptorContext">Contextual information required for conversion.</param>
        /// <param name="type">Type being evaluated for conversion.</param>
        /// <returns>true if this type can be converted; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
      
        /// <summary>
        /// Attempts to convert a given object to an instance of System.Windows.Media.Animation.KeyTime.
        /// </summary>
        /// <param name="typeDescriptorContext">Context information required for conversion.</param>
        /// <param name="cultureInfo">Cultural information that is respected during conversion.</param>
        /// <param name="value">The object being converted to an instance of System.Windows.Media.Animation.KeyTime.</param>
        /// <returns>
        /// A new instance of System.Windows.Media.Animation.KeyTime, based on the supplied
        /// value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return KeyTime.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Attempts to convert an instance of System.Windows.Media.Animation.KeyTime
        /// to another type.
        /// </summary>
        /// <param name="typeDescriptorContext">Context information required for conversion.</param>
        /// <param name="cultureInfo">Cultural information that is respected during conversion.</param>
        /// <param name="value">System.Windows.Media.Animation.KeyTime value to convert from.</param>
        /// <param name="destinationType">Type being evaluated for conversion.</param>
        /// <returns>A new object, based on value.</returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}