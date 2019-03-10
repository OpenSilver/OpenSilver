
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    }
}