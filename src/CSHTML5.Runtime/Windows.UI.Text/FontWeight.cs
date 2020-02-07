
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



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Markup;
using DotNetForHtml5.Core;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Text
#endif
{

#if FOR_DESIGN_TIME
    [TypeConverter(typeof(FontWeightConverter))]
#endif
    /// <summary>
    /// Refers to the density of a typeface, in terms of the lightness or heaviness
    /// of the strokes.
    /// </summary>
    [SupportsDirectContentViaTypeFromStringConverters]
    public struct FontWeight
    {
        /// <summary>
        /// The font weight expressed as a numeric value. See Remarks.
        /// </summary>
        public ushort Weight;

        static FontWeight()
        {
            TypeFromStringConverters.RegisterConverter(typeof(FontWeight), INTERNAL_ConvertFromString);
        }

        internal string INTERNAL_ToHtmlString()
        {
            return Weight.ToString();
        }

        internal static object INTERNAL_ConvertFromString(string fontCode)
        {
            try
            {
                // Check if the font is a named font:
                ushort result;
                if (!ushort.TryParse(fontCode, out result))
                {
                    FontWeights.INTERNAL_FontweightsEnum namedFont = (FontWeights.INTERNAL_FontweightsEnum)Enum.Parse(typeof(FontWeights.INTERNAL_FontweightsEnum), fontCode); // Note: "TryParse" does not seem to work in JSIL.
                    result = (ushort)namedFont;
                }
                return INTERNAL_ConvertFromUshort(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid font: " + fontCode, ex);
            }
        }

        internal static FontWeight INTERNAL_ConvertFromUshort(ushort fontWeightAsUshort)
        {
            return new FontWeight()
            {
                Weight = fontWeightAsUshort
            };
        }

        public override string ToString()
        {
            return Weight.ToString();
        }

        /// <summary>
        /// Compares two font weight values and returns an indication of their relative values.
        /// </summary>
        /// <param name="left">First object to compare.</param>
        /// <param name="right">Second object to compare.</param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// When the return value is less than zero this means that left is less than right.
        /// When the return value is zero this means that left is equal to right.
        /// When the return value is greater than zero this means that left is greater than right.
        /// </returns>
        public static int Compare(FontWeight left, FontWeight right)
        {
            return left.Weight - right.Weight;
        }

        public bool Equals(FontWeight fontWeight)
        {
            return this == fontWeight;
        }

        public override bool Equals(object o)
        {
            if (o is FontWeight fw)
            {
                return this == fw;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Weight;
        }

        public static bool operator ==(FontWeight left, FontWeight right)
        {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(FontWeight left, FontWeight right)
        {
            return Compare(left, right) != 0;
        }

        public static bool operator >(FontWeight left, FontWeight right)
        {
            return Compare(left, right) > 0;
        }

        public static bool operator <(FontWeight left, FontWeight right)
        {
            return Compare(left, right) < 0;
        }

        public static bool operator >=(FontWeight left, FontWeight right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator <=(FontWeight left, FontWeight right)
        {
            return Compare(left, right) <= 0;
        }
    }
}