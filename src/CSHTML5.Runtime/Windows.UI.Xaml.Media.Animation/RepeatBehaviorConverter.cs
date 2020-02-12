
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

    public sealed partial class RepeatBehaviorConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object can be converted from a given type to an instance
        /// of a System.Windows.Media.Animation.RepeatBehavior.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
        /// <returns>
        /// true if the type can be converted to a System.Windows.Media.Animation.RepeatBehavior; otherwise,
        /// false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>System.Windows.Media.Animation.RepeatBehavior
        /// Determines whether an instance of a System.Windows.Media.Animation.RepeatBehavior can be converted
        /// to a different type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The desired type this System.Windows.Media.Animation.RepeatBehavior is being evaluated for conversion.</param>
        /// <returns>
        /// true if this System.Windows.Media.Animation.RepeatBehavior can be converted to destinationType;
        /// otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// Attempts to convert the specified object to a System.Windows.Media.Animation.RepeatBehavior.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Cultural information to respect during conversion.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.Media.Animation.RepeatBehavior created from converting value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return RepeatBehavior.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Attempts to convert a System.Windows.Media.Animation.RepeatBehavior to a specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The System.Windows.Media.Animation.RepeatBehavior to convert.</param>
        /// <param name="destinationType">The type to convert this System.Windows.Media.Animation.RepeatBehavior to.</param>
        /// <returns>The object created from converting this System.Windows.Media.Animation.RepeatBehavior.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}