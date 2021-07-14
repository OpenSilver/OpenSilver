

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


#if MIGRATION
using DotNetForHtml5.Core;
using System.ComponentModel;

namespace System.Windows
{
    [TypeConverter(typeof(FontStyleTypeConverter))]
    public partial struct FontStyle
    {
        private readonly int Style;

        static FontStyle()
        {
            TypeFromStringConverters.RegisterConverter(typeof(FontStyle), INTERNAL_ConvertFromString);
        }

        internal FontStyle(int style)
        {
            Style = style;
        }

        public override bool Equals(object o)
        {
            if (o is FontStyle)
            {
                var fs = (FontStyle)o;
                return this == fs;
            }
            return false;
        }

        public bool Equals(FontStyle fontStyle)
        {
            return this == fontStyle;
        }

        public static bool operator ==(FontStyle left, FontStyle right)
        {
            return left.Style == right.Style;
        }

        public static bool operator !=(FontStyle left, FontStyle right)
        {
            return left.Style != right.Style;
        }

        public override string ToString()
        {
            switch (Style)
            {
                case 0:
                    return "Normal";
                case 1:
                    return "Oblique";
                case 2:
                    return "Italic";
                default:
                    return string.Empty; //should not be possible
            }
        }

        internal static object INTERNAL_ConvertFromString(string fontStyleAsString)
        {
            switch ((fontStyleAsString ?? string.Empty).ToLower())
            {
                case "normal":
                    return FontStyles.Normal;
                case "oblique":
                    return FontStyles.Oblique;
                case "italic":
                    return FontStyles.Italic;
                default:
                    throw new Exception(string.Format("Invalid FontStyle: '{0}'", fontStyleAsString));
            }
        }
    }
}
#endif